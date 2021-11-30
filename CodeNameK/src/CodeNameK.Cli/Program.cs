using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeNameK.Cli
{
    class Program
    {
        private static ILogger? _logger;

        static async Task Main(string[] args)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(opt =>
                {
                    opt.TimestampFormat = "[yyyyMMdd HH:mm:ss]";
                    opt.SingleLine = true;
                });
            });
            _logger = loggerFactory.CreateLogger<Program>();
            _logger.LogInformation("Logger is ready.");

            DataPointOperator dataPointOperator = new DataPointOperator();
            IDataWriter<DataPoint> dataPointWriter = dataPointOperator;
            IDataReader<DataPoint> dataPointReader = dataPointOperator;
            ILocalPathProvider pathService = new LocalPathProvider();
            LocalStoreOptions localStoreOptions = new LocalStoreOptions(){
                DataStorePath = "%userprofile%/.codeNameK/DebugData",
            };
            DataRepo dataRepo = new DataRepo(
                dataPointWriter,
                dataPointReader,
                pathService,
                Options.Create<LocalStoreOptions>(localStoreOptions),
                loggerFactory.CreateLogger<DataRepo>());

            Console.WriteLine("List all categories:");
            List<Category> categories = dataRepo.GetAllCategories().OrderBy(c => c.Id, StringComparer.InvariantCultureIgnoreCase).ToList();
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
                await dataRepo.AddCategoryAsync(category, cancellationToken: default);
                categories.Add(category);
            }

            Console.WriteLine("Current category: {0}", category.Id);

            Console.WriteLine("Listing all data points:");
            // Load all data points
            List<DataPoint> dataPoints = new List<DataPoint>();
            await foreach (DataPoint dot in dataRepo.GetPointsAsync(category, null, null, cancellationToken: default))
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
            DataPoint dataPoint = new DataPoint()
            {
                Id = Guid.NewGuid(),
                WhenUTC = DateTime.UtcNow,
                Value = dataValue.Value,
                Category = category,
            };
            DataPointInfo newPointHandle = await dataRepo.AddPointAsync(dataPoint, cancellationToken: default).ConfigureAwait(false);
            dataPoints.Add(dataPoint);
            Console.WriteLine("Added a point: {0:D}", newPointHandle);

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
                    bool deleted = await dataRepo.DeletePointAsync(target, cancellationToken: default).ConfigureAwait(false);
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

                    await dataRepo.UpdatePointAsync(target, updateTo, default).ConfigureAwait(false);
                    Console.WriteLine("DataPoint updated: {0} => {1}", pathService.GetLocalPath(target), pathService.GetLocalPath(updateTo));
                }
            }
            else
            {
                Console.WriteLine("No deletion. Invalid guid.");
            }
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
    }
}
