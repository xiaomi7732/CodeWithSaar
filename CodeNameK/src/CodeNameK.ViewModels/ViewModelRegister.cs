using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeNameK.ViewModels
{
    public static class ViewModelRegister
    {
        public static IServiceCollection RegisterViewModels(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IErrorRevealerFactory, ErrorRevealerFactory>();
            services.TryAddSingleton<MainViewModel>();
            services.TryAddSingleton<DataPointViewModel>(); // DataPoint operator

            return services;
        }
    }
}