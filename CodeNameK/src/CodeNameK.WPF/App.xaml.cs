using System;
using System.Windows;
using CodeNameK.BIZ;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.Core.Utilities;
using CodeNameK.DAL;
using CodeNameK.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeNameK.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IHost _host;
        // private ServiceProvider _serviceProvider;
        public App()
        {
            _host = new HostBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.jsonc", optional: false, reloadOnChange: true)
#if DEBUG
                    .AddJsonFile("appsettings.debug.jsonc", optional: false, reloadOnChange: true)
#endif
                    ;
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddDebug().AddConsole();

                })
                .ConfigureServices(ConfigureServices2).Build();
        }

        private void ConfigureServices2(HostBuilderContext context, IServiceCollection services)
        {
            IConfiguration configurationRoot = context.Configuration;
            services.RegisterDataAccessModule(configurationRoot);
            services.AddOptions<LocalStoreOptions>().Bind(configurationRoot.GetSection(LocalStoreOptions.SectionName));
            services.RegisterBizModule(configurationRoot);
            services.RegisterViewModels(configurationRoot);

            services.AddHttpClient<InternetAvailability>();

            services.AddSingleton<MainWindow>();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }

        protected async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
            MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
