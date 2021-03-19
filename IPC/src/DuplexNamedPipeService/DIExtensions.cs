using System;
using CodeWithSaar.IPC;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddNamedPipeClient(this IServiceCollection services, Action<NamedPipeOptions> option = null)
        {
            services.RegisterServices(option);
            services.TryAddSingleton<INamedPipeClientService>(p =>
            {
                IOptions<NamedPipeOptions> options = p.GetRequiredService<IOptions<NamedPipeOptions>>();
                return NamedPipeClientFactory.Instance.CreateNamedPipeService(options?.Value, p.GetRequiredService<ISerializationProvider>(), p.GetService<ILoggerFactory>());
            });
            return services;
        }

        public static IServiceCollection AddNamedPipeServer(this IServiceCollection services, Action<NamedPipeOptions> option = null)
        {
            services.RegisterServices(option);
            services.TryAddSingleton<INamedPipeServerService>(p =>
            {
                IOptions<NamedPipeOptions> options = p.GetRequiredService<IOptions<NamedPipeOptions>>();
                return NamedPipeServerFactory.Instance.CreateNamedPipeService(options?.Value, p.GetRequiredService<ISerializationProvider>(), p.GetService<ILoggerFactory>());
            });
            return services;
        }

        private static void RegisterServices(this IServiceCollection services, Action<NamedPipeOptions> option = null)
        {
            option = option ?? (opt => { });
            services.AddLogging();
            services.AddOptions();
            services.AddOptions<NamedPipeOptions>().Configure(option);
            services.TryAddTransient<ISerializationProvider, DefaultSerializationProvider>();
        }
    }
}