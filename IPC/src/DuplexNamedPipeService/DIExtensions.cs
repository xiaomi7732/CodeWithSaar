using System;
using CodeWithSaar.IPC;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddNamedPipeClient(this ServiceCollection services, Action<NamedPipeOptions> option = null)
        {
            services.RegisterServices(option);
            services.TryAddSingleton<INamedPipeClientService, DuplexNamedPipeService>();
            return services;
        }

        public static IServiceCollection AddNamedPipeServer(this ServiceCollection services, Action<NamedPipeOptions> option = null)
        {
            services.RegisterServices(option);
            services.TryAddSingleton<INamedPipeServerService, DuplexNamedPipeService>();
            return services;
        }

        private static void RegisterServices(this ServiceCollection services, Action<NamedPipeOptions> option = null)
        {
            option = option ?? (opt => { });
            services.AddLogging();
            services.AddOptions();
            services.AddOptions<NamedPipeOptions>().Configure(option);
            services.TryAddTransient<ISerializationProvider, DefaultSerializationProvider>();
        }
    }
}