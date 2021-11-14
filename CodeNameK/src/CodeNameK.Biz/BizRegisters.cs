using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeNameK.Biz;
public static class BizRegisters
{
    public static IServiceCollection RegisterBiz(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategory, BizCategory>();

        return services;
    }
}