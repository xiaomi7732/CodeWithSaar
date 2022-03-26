using System;
using System.Threading.Channels;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Contracts.CustomOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        services.AddSingleton<UpSyncBackgroundService>();
        services.AddSingleton<ISyncQueueRequestService<UpSyncRequest>>(p => p.GetRequiredService<UpSyncBackgroundService>());
        services.AddHostedService<UpSyncBackgroundService>(p => p.GetRequiredService<UpSyncBackgroundService>());
        services.AddSingleton<DownSyncBackgroundService>();
        services.AddSingleton<ISyncQueueRequestService<DownSyncRequest>>(p => p.GetRequiredService<DownSyncBackgroundService>());
        services.AddHostedService<DownSyncBackgroundService>(p => p.GetRequiredService<DownSyncBackgroundService>());

        services.AddSingleton<IBizUserPreferenceService, BizUserPreferenceService>();
        return services;
    }

    [Obsolete("This is for deubgging only", error: false)]
    public static IServiceCollection RegisterBizModuleForAndroid(this IServiceCollection services)
    { 
        services.TryAddScoped<ICategory, BizCategory>();

        return services;
    }
}