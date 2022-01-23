using System.Threading.Channels;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Contracts.CustomOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeNameK.BIZ;
public static class BizRegister
{
    public static IServiceCollection RegisterBizModule(
        this IServiceCollection services,
        IConfiguration syncSection,
        IConfiguration userPreferenceSection)
    {
        services.AddOptions<SyncOptions>().Bind(syncSection);
        services.AddOptions<UserPreference>().Bind(userPreferenceSection);

        services.AddScoped<ICategory, BizCategory>();
        services.AddScoped<IDataPoint, BizDataPoint>();
        services.AddScoped<ISync, BizSync>();
        services.AddTransient<IChartAxisExpansion, BizChartAxisExpansion>();
        services.AddTransient<IDateRangeService, BizDateRangeService>();

        services.AddSingleton<Channel<UpSyncRequest>>(p => Channel.CreateUnbounded<UpSyncRequest>());
        services.AddSingleton<Channel<DownSyncRequest>>(p => Channel.CreateUnbounded<DownSyncRequest>());
        services.AddSingleton<BackgroundSyncProgress<UpSyncBackgroundService>>();
        services.AddSingleton<BackgroundSyncProgress<DownSyncBackgroundService>>();
        services.AddHostedService<UpSyncBackgroundService>();
        services.AddHostedService<DownSyncBackgroundService>();

        services.AddSingleton<IBizUserPreferenceService, BizUserPreferenceService>();
        return services;
    }
}