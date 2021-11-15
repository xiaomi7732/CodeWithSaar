using CodeNameK.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeNameK.DataAccess
{
    public static class DataAccessRegister
    {
        public static IServiceCollection RegisterDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DataPointOperator>();
            services.AddScoped<IDataWriter<DataPoint>>(p => p.GetRequiredService<DataPointOperator>());
            services.AddScoped<IDataReader<DataPoint>>(p => p.GetRequiredService<DataPointOperator>());

            services.AddScoped<IDataPointPathService, DataPointPathService>();

            services.AddScoped<DataRepo>();
            services.AddScoped<ICategoryRepo>(p => p.GetRequiredService<DataRepo>());
            services.AddScoped<IDataPointRepo>(p => p.GetRequiredService<DataRepo>());

            return services;
        }
    }
}