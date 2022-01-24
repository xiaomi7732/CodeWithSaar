using System;
using System.Windows;
using CodeNameK.BIZ;
using CodeNameK.Contracts;
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
        private readonly IHost _host;

        public App()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            _host = new HostBuilder()
                .ConfigureDefaults(new string[] { })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.jsonc", optional: false, reloadOnChange: true)
                    .AddJsonFile(BizUserPreferenceService.FilePath, optional: true, reloadOnChange: true)
#if DEBUG
                    .AddJsonFile("appsettings.debug.jsonc", optional: false, reloadOnChange: true)
#endif
                    ;
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    builder
                        .AddDebug()
                        .AddSimpleConsole(
                            configure =>
                            {
                                configure.SingleLine = true;
                                configure.TimestampFormat = "[HH:mm:ss] ";
                            });
                })
                .ConfigureServices(ConfigureServices).Build();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            IConfiguration configurationRoot = context.Configuration;
            services.RegisterDataAccessModule(configurationRoot);
            services.AddOptions<LocalStoreOptions>().Bind(configurationRoot.GetSection(LocalStoreOptions.SectionName));
            services.RegisterBizModule(
                configurationRoot.GetSection(SyncOptions.SectionName),
                configurationRoot.GetSection(UserPreference.SectionName));
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
