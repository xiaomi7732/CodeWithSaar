
using System;
using Azure.Identity;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DAL.OneDrive;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace CodeNameK.DAL
{
    public static class DataAccessRegister
    {
        public static IServiceCollection RegisterDataAccessModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<MSALAppOptions<GraphServiceClient>>().Bind(configuration.GetSection("GraphAPI"));
            services.AddScoped<GraphServiceClient>(p =>
            {
                MSALAppOptions<GraphServiceClient> appOptions = p.GetRequiredService<IOptions<MSALAppOptions<GraphServiceClient>>>().Value;
                InteractiveBrowserCredentialOptions options = new InteractiveBrowserCredentialOptions
                {
                    TenantId = appOptions.TenantId,
                    ClientId = appOptions.ClientId,
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                    RedirectUri = new Uri(appOptions.RedirectUri),
                };
                InteractiveBrowserCredential credential = new InteractiveBrowserCredential(options);
                GraphServiceClient graphClient = new GraphServiceClient(credential, appOptions.Scopes);

                return graphClient;
            });

            services.AddScoped<DataPointOperator>();
            services.AddScoped<IDataWriter<DataPoint>>(p => p.GetRequiredService<DataPointOperator>());
            services.AddScoped<IDataReader<DataPoint>>(p => p.GetRequiredService<DataPointOperator>());

            services.AddScoped<ILocalPathProvider, LocalPathProvider>();
            services.AddScoped<IRemotePathProvider, RemotePathProvider>();

            services.AddScoped<DataRepo>();
            services.AddScoped<ICategoryRepo>(p => p.GetRequiredService<DataRepo>());
            services.AddScoped<IDataPointRepo>(p => p.GetRequiredService<DataRepo>());

            services.AddScoped<IOneDriveSync, OneDriveSync>();

            return services;
        }
    }
}