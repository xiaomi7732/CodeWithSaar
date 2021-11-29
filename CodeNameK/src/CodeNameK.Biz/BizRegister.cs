using CodeNameK.BIZ.Interfaces;
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
        return services;
    }
}