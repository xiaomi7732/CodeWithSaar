using CodeNameK.Contracts;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DAL.OneDrive;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeNameK.DAL
{
    public static class DataAccessRegister
    {
        public static IServiceCollection RegisterDataAccessModule(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<DataPointOperator>();
            services.AddScoped<IDataWriter<DataPoint>>(p => p.GetRequiredService<DataPointOperator>());
            services.AddScoped<IDataReader<DataPoint>>(p => p.GetRequiredService<DataPointOperator>());

            services.AddScoped<ILocalPathProvider, LocalPathProvider>();
            services.AddScoped<IRemotePathProvider, RemotePathProvider>();

            services.AddScoped<DataRepo>();
            services.AddScoped<ICategoryRepo>(p => p.GetRequiredService<DataRepo>());
            services.AddScoped<IDataPointRepo>(p => p.GetRequiredService<DataRepo>());

            services.AddOptions<MSALAppOptions<OneDriveSync>>().Bind(configuration.GetSection("GraphAPI"));
            services.AddSingleton<OneDriveTokenCredential>();
            services.AddSingleton<ITokenCredentialManager<OneDriveCredentialStatus>>(p => p.GetRequiredService<OneDriveTokenCredential>());
            services.AddScoped<IOneDriveSync, OneDriveSync>();

            services.AddSingleton<IUserPreferenceManager>(p => UserPreferenceManager.Instance);
            return services;
        }
    }
}