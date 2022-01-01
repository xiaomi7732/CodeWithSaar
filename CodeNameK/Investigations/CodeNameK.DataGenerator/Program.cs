using CodeNameK.BIZ;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.DAL;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IConfiguration configuration = new ConfigurationBuilder().Build();

IServiceCollection services = new ServiceCollection();
services.AddLogging();

services.RegisterBizModule(configuration);
services.RegisterDataAccessModule(configuration);

IServiceProvider serviceProvider = services.BuildServiceProvider();

ICategory categoryBiz = serviceProvider.GetRequiredService<ICategory>();
IDataPoint dataPointBiz = serviceProvider.GetRequiredService<IDataPoint>();

Category newCategory = new Category()
{
    Id = "DataGen",
};
OperationResult<Category> newCategoryResult = await categoryBiz.AddCategoryAsync(newCategory, overwrite: true, default).ConfigureAwait(false);
if (!newCategoryResult.IsSuccess)
{
    Console.WriteLine(newCategoryResult.Reason);
    return;
}

DateTime initialDateTime = DateTime.UtcNow;
Random random = new Random((int)initialDateTime.Ticks);

for (int i = 0; i < 100; i++)
{
    double value = random.Next(500, 10000);
    int minutesGap = random.Next(30, 180);
    DateTime dataPointUTC = initialDateTime.AddMinutes(minutesGap);

    DataPoint newDataPoint = new DataPoint()
    {
        WhenUTC = dataPointUTC,
        Value = value,
        Category = newCategoryResult,
    };

    OperationResult<DataPoint> newDataPointResult = await dataPointBiz.AddAsync(newDataPoint, default).ConfigureAwait(false);
    if (!newDataPointResult.IsSuccess)
    {
        Console.WriteLine(newDataPointResult.Reason);
        break;
    }
}