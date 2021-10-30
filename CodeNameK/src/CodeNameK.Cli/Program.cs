using System;
using System.Threading.Tasks;
using CodeNameK.DataAccess;
using CodeNameK.DataContracts;

namespace CodeNameK.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DataRepo dataRepo = new DataRepo();

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
        }
    }
}
