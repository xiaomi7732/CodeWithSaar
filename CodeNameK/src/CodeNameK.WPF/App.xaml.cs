﻿using System.Windows;
using CodeNameK.BIZ;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.Core.Utilities;
using CodeNameK.DAL;
using CodeNameK.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeNameK.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        public App()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.jsonc", optional: false, reloadOnChange: true)
#if DEBUG
                .AddJsonFile("appsettings.debug.jsonc", optional: false, reloadOnChange: true)
#endif
                                            .Build();
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configurationRoot)
        {
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configurationRoot.GetSection("Logging"));
                builder.AddDebug();
            });

            services.RegisterDataAccessModule(configurationRoot);
            services.AddOptions<LocalStoreOptions>().Bind(configurationRoot.GetSection(LocalStoreOptions.SectionName));
            services.RegisterBizModule(configurationRoot);
            services.RegisterViewModels(configurationRoot);

            services.AddHttpClient<InternetAvailability>();

            services.AddSingleton<MainWindow>();
        }

        protected void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
