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
        static async Task Main(string[] args)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            IDataWriter<DataPoint> dataPointWriter = new DataPointWriter();
            IDataPointPathService pathService = new DataPointPathService();
            DataRepo dataRepo = new DataRepo(
                dataPointWriter,
                pathService,
                loggerFactory.CreateLogger<DataRepo>());

            Category category = new Category() { Id = "401KSaving" };
            Console.WriteLine($"Add a category: {category.Id}");
            await dataRepo.AddCategoryAsync(category);

            DataPoint dataPoint = new DataPoint()
            {
                Id = Guid.NewGuid(),
                WhenUTC = DateTime.UtcNow,
                Value = 20000,
                Category = category,
            };
            Guid newPointGuid = await dataRepo.AddPointAsync(dataPoint).ConfigureAwait(false);
            Console.WriteLine("Added a point: {0:D}", newPointGuid);

            // Load all data points
            List<DataPoint> dataPoints = new List<DataPoint>();
            await foreach (DataPoint dot in dataRepo.GetPointsAsync(category, null, null))
            {
                Console.WriteLine("Got data point: {0} Local DateTime: {1:yyyy-MM-dd HH:mm:ss}, Value: {2} Is Deleted: {3}", dot.Id, dot.WhenUTC.ToLocalTime(), dot.Value, dot.IsDeleted);
                dataPoints.Add(dot);
            }

            if (!dataPoints.Any())
            {
                return;
            }

            Console.WriteLine("Pick a dot to delete:");
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
    }
}
