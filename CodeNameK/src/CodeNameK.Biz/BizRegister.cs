using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeNameK.Biz;
public static class BizRegister
{
    public static IServiceCollection RegisterBiz(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategory, BizCategory>();
        services.AddScoped<IDataPoint, BizDataPoint>();
        return services;
    }
}