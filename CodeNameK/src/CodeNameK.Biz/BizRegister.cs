using System.Threading.Channels;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeNameK.BIZ;
public static class BizRegister
{
    public static IServiceCollection RegisterBizModule(this IServiceCollection services, IConfiguration localStoreSection)
    {
        services.AddScoped<ICategory, BizCategory>();
        services.AddScoped<IDataPoint, BizDataPoint>();
        services.AddScoped<ISync, BizSync>();
        services.AddTransient<IChartAxisExpansion, BizChartAxisExpansion>();
        services.AddTransient<IDateRangeService, BizDateRangeService>();

        services.AddSingleton<Channel<UpSyncRequest>>(p => Channel.CreateUnbounded<UpSyncRequest>());
        services.AddSingleton<BackgroundSyncProgress>();
        services.AddHostedService<DataPointUploaderBackgroundService>();

        return services;
    }
}