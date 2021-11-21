using CodeNameK.BIZ.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeNameK.BIZ;
public static class BizRegister
{
    public static IServiceCollection RegisterBiz(this IServiceCollection services, IConfiguration localStoreSection)
    {
        services.AddScoped<ICategory, BizCategory>();
        services.AddScoped<IDataPoint, BizDataPoint>();
        return services;
    }
}