using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeNameK.DataAccess;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;

namespace CodeNameK.Cli
{
    class Program
    {
        private static ILogger _logger;

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

            IDataWriter<DataPoint> dataPointWriter = new DataPointWriter();
            IDataPointPathService pathService = new DataPointPathService();
            DataRepo dataRepo = new DataRepo(
                dataPointWriter,
                pathService,
                loggerFactory.CreateLogger<DataRepo>());

            Console.WriteLine("List all categories:");
            List<Category> categories = dataRepo.GetAllCategories().OrderBy(c => c.Id, StringComparer.InvariantCultureIgnoreCase).ToList();
            foreach (Category c in categories)
            {
                Console.WriteLine("\t{0}", c.Id);
            }
            Console.WriteLine("Pick a category for the next operation:");
            string pickedCategoryName = Console.ReadLine();
            if (string.IsNullOrEmpty(pickedCategoryName))
            {
                Console.WriteLine("Must have a category name. Terminated.");
                return;
            }
            Category category = categories.FirstOrDefault(item => string.Equals(item.Id, pickedCategoryName, StringComparison.OrdinalIgnoreCase));
            if (category is null)
            {
                Console.WriteLine("No category matched the picked named of: {0}, creating one.", pickedCategoryName);
                category = new Category() { Id = pickedCategoryName };
                Console.WriteLine($"Adding a category: {category.Id}");
                await dataRepo.AddCategoryAsync(category);
                categories.Add(category);
            }

            Console.WriteLine("Current category: {0}", category.Id);

            Console.WriteLine("Listing all data points:");
            // Load all data points
            List<DataPoint> dataPoints = new List<DataPoint>();
            await foreach (DataPoint dot in dataRepo.GetPointsAsync(category, null, null))
            {
                dataPoints.Add(dot);
            }
            PrintDataPoints(dataPoints);

            double? dataValue = null;
            while (!dataValue.HasValue)
            {
                Console.WriteLine("Adding a data point. What will the value be?");
                string dataValueString = Console.ReadLine();

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
            Guid newPointGuid = await dataRepo.AddPointAsync(dataPoint).ConfigureAwait(false);
            dataPoints.Add(dataPoint);
            Console.WriteLine("Added a point: {0:D}", newPointGuid);

            if (!dataPoints.Any())
            {
                return;
            }
            PrintDataPoints(dataPoints);
            Console.WriteLine("There are {0} data points in the list. Pick a dot to delete:", dataPoints.Count);
            string dotId = Console.ReadLine();

            if (Guid.TryParse(dotId, out Guid dotGuid))
            {
                DataPoint target = dataPoints.FirstOrDefault(dot => dot.Id == dotGuid);
                target.Category = category;
                bool deleted = await dataRepo.DeletePointAsync(target).ConfigureAwait(false);
                Console.WriteLine("Delete result: {0}", deleted);
            }
            else
            {
                Console.WriteLine("Invalid guid.");
            }
        }

        private static void PrintDataPoints(IEnumerable<DataPoint> dots)
        {
            foreach (DataPoint dot in dots)
            {
                Console.WriteLine("- {0} Local DateTime: {1:yyyy-MM-dd HH:mm:ss} (UTC: {4:o}), Value: {2} Is Deleted: {3}", dot.Id, dot.WhenUTC.ToLocalTime(), dot.Value, dot.IsDeleted, dot.WhenUTC);
            }
            Console.WriteLine("===");
        }
    }
}
