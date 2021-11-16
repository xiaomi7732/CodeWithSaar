using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeNameK.ViewModels
{
    public static class ViewModelRegister
    {
        public static IServiceCollection RegisterViewModels(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<MainViewModel>();

            return services;
        }
    }
}