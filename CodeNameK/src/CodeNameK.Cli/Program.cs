using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.BIZ;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.Contracts.DataContracts;
using CodeNameK.DAL;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace CodeNameK.Cli
{
    sealed class Program
    {
        private readonly ICategory _categoryService;
        private readonly IDataPoint _datapointService;
        private readonly ISync _syncService;
        private readonly ILogger _logger;
        private static CancellationTokenSource? _cancellationTokenSource = new CancellationTokenSource();

        public Program(
            ICategory categoryService,
            IDataPoint datapointService,
            ISync syncService,
            ILogger<Program> logger)
        {
            _categoryService = categoryService;
            _datapointService = datapointService;
            _syncService = syncService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        static async Task Main(string[] args)
        {
            IServiceProvider serviceProvider = Bootstrap();
            Program program = serviceProvider.GetRequiredService<Program>();
            await program.RunAsync(_cancellationTokenSource?.Token ?? default);
        }

        private async Task RunAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Welcome to CodeNameK!");

            Console.Write("Do you want to start a sync with your OneDrive (Y/n)?");
            string? isSync;
            isSync = Console.ReadLine();
            if (string.Equals(isSync, "y", StringComparison.OrdinalIgnoreCase) || string.Equals(isSync, "yes", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    Progress<SyncProgress> progress = new Progress<SyncProgress>(DisplayProgress);
                    await _syncService.Sync(progress, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex);
                }
            }

            Console.WriteLine("List all categories:");
            List<Category> categories = _categoryService.GetAllCategories().OrderBy(c => c.Id, StringComparer.InvariantCultureIgnoreCase).ToList();
            PrintCategories(categories);
            Console.WriteLine("Pick a category for the next operation:");
            string? pickedCategoryName = Console.ReadLine();
            if (string.IsNullOrEmpty(pickedCategoryName))
            {
                Console.WriteLine("Must have a category name. Terminated.");
                return;
            }
            Category? category = categories.FirstOrDefault(item => string.Equals(item.Id, pickedCategoryName, StringComparison.OrdinalIgnoreCase));
            if (category is null)
            {
                Console.WriteLine("No category matched the picked named of: {0}, creating one.", pickedCategoryName);
                category = new Category() { Id = pickedCategoryName };
                Console.WriteLine($"Adding a category: {category.Id}");
                await _categoryService.AddCategoryAsync(category, cancellationToken: default);
                categories.Add(category);
            }

            Console.WriteLine("Current category: {0}", category.Id);

            Console.WriteLine("Listing all data points:");
            // Load all data points
            List<DataPoint> dataPoints = new List<DataPoint>();
            await foreach (DataPoint dot in _datapointService.GetDataPoints(category, cancellationToken: cancellationToken))
            {
                dataPoints.Add(dot);
            }
            PrintDataPoints(dataPoints);

            double? dataValue = null;
            while (!dataValue.HasValue)
            {
                Console.WriteLine("Adding a data point. What will the value be?");
                string? dataValueString = Console.ReadLine();

                if (double.TryParse(dataValueString, out double newValue))
                {
                    dataValue = newValue;
                }
            }
            Console.WriteLine("New value accepted: {0}", dataValue.Value);
            DataPoint? dataPoint = new DataPoint()
            {
                Id = Guid.NewGuid(),
                WhenUTC = DateTime.UtcNow,
                Value = dataValue.Value,
                Category = category,
            };
            dataPoint = await _datapointService.AddAsync(dataPoint, cancellationToken: default).ConfigureAwait(false);
            if (dataPoint is null)
            {
                Console.WriteLine("Data point creation failed.");
                return;
            }
            dataPoints.Add(dataPoint);
            Console.WriteLine("Added a point: {0:D}", (DataPointPathInfo)dataPoint);

            if (!dataPoints.Any())
            {
                return;
            }
            PrintDataPoints(dataPoints);
            Console.WriteLine("There are {0} data points in the list. Pick a dot to delete:", dataPoints.Count);
            string? dotId = Console.ReadLine();

            if (Guid.TryParse(dotId, out Guid dotGuid))
            {
                DataPoint? target = dataPoints.FirstOrDefault(dot => dot.Id == dotGuid);
                if (target != null)
                {
                    bool deleted = await _datapointService.DeleteAsync(target, cancellationToken: default).ConfigureAwait(false);
                    Console.WriteLine("Delete result: {0}", deleted);
                }
                else
                {
                    Console.WriteLine("No point found by guid: {0}", dotGuid);
                }
            }
            else
            {
                Console.WriteLine("No deletion. Invalid guid.");
            }

            Console.WriteLine("Do you want to update a data point? Input the guid if you want.");
            dotId = Console.ReadLine();
            if (Guid.TryParse(dotId, out dotGuid))
            {
                DataPoint? target = dataPoints.FirstOrDefault(dot => dot.Id == dotGuid);
                if (target is null)
                {
                    Console.WriteLine("No point found with Guid: {0}", dotGuid);
                }
                else
                {
                    Console.WriteLine("You picked the following point:");
                    PrintDataPoint(target);
                    Console.WriteLine("What's the new value:");
                    string? newValueString = Console.ReadLine();

                    Console.WriteLine("What's the new local time (yyyy-MM-dd HH:mm:ss):");
                    string? newLocalTimeString = Console.ReadLine();

                    Console.WriteLine("There are these categories:");
                    PrintCategories(categories);
                    Console.WriteLine("What's the new category: ");
                    string? newCategoryName = Console.ReadLine();

                    if (!double.TryParse(newValueString, out double newValue))
                    {
                        newValue = target.Value;
                    }

                    if (!DateTime.TryParse(newLocalTimeString, provider: null, styles: System.Globalization.DateTimeStyles.AssumeLocal, out DateTime newLocalDateTime))
                    {
                        newLocalDateTime = target.WhenUTC.ToLocalTime();
                    }

                    Category? newCategory = null;
                    if (!string.IsNullOrEmpty(newCategoryName))
                    {
                        newCategory = categories.FirstOrDefault(c => string.Equals(c.Id, newCategoryName));
                    }
                    newCategory = newCategory ?? target.Category;

                    DataPoint updateTo = target with
                    {
                        Id = Guid.NewGuid(),
                        Value = newValue,
                        WhenUTC = newLocalDateTime.ToUniversalTime(),
                        Category = newCategory,
                    };

                    await _datapointService.Update(target, updateTo, cancellationToken).ConfigureAwait(false);
                    Console.WriteLine("DataPoint updated: {0} => {1}", (DataPointPathInfo)target, (DataPointPathInfo)updateTo);
                }
            }
            else
            {
                Console.WriteLine("No update. Invalid guid.");
            }

            // Sync
            Console.WriteLine("Now it is time to sync the changes to remote. Press any key to continue ... or Ctrl + C to break");
            Console.ReadKey(intercept: true);
            try
            {
                Progress<SyncProgress> progress = new Progress<SyncProgress>(DisplayProgress);
                await _syncService.Sync(progress, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex);
            }
        }

        private void DisplayProgress(SyncProgress progress)
        {
            Console.WriteLine("[{0}]{1:p}", progress.DisplayText, progress.Value);
        }

        private static void PrintCategories(IEnumerable<Category> categories)
        {
            foreach (Category c in categories)
            {
                Console.WriteLine("\t{0}", c.Id);
            }
        }

        private static void PrintDataPoints(IEnumerable<DataPoint> dots)
        {
            int count = 0;
            foreach (DataPoint dot in dots)
            {
                PrintDataPoint(dot);
                count++;
            }

            if (count == 0)
            {
                Console.WriteLine("No data");
            }

            Console.WriteLine("===");
        }

        private static void PrintDataPoint(DataPoint dot)
        {
            Console.WriteLine("- " + dot.ToString());
            // Console.WriteLine("- {0} Local DateTime: {1:yyyy-MM-dd HH:mm:ss} (UTC: {4:o}), Value: {2} Is Deleted: {3}", dot.Id, dot.WhenUTC.ToLocalTime(), dot.Value, dot.IsDeleted, dot.WhenUTC);
        }

        private static IServiceProvider Bootstrap()
        {
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(Path.Combine(assemblyDirectory, "appsettings.jsonc"), optional: false).Build();

            IServiceCollection services = new ServiceCollection();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddSimpleConsole(opt =>
                {
                    opt.TimestampFormat = "[yyyyMMdd HH:mm:ss]";
                    opt.SingleLine = true;
                });
            });
            services.TryAddSingleton<IConfiguration>(p => configuration);

            ConfigureServices(services, configuration);

            return services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configurationRoot)
        {
            services.AddOptions<LocalStoreOptions>().Bind(configurationRoot.GetSection(LocalStoreOptions.SectionName));
            services.RegisterDataAccessModule(configurationRoot);
            services.RegisterBizModule(configurationRoot);

            services.AddSingleton<Program>();
        }
    }
}
