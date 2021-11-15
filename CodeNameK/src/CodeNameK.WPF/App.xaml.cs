using System.Windows;
using CodeNameK.Biz;
using CodeNameK.DataAccess;
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
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.jsonc", optional: true, reloadOnChange: true)
                                            .Build();

            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configurationRoot)
        {
            services.AddLogging(builder => builder.AddConfiguration(configurationRoot.GetSection("Logging")));

            services.RegisterDataAccess(configurationRoot);
            services.RegisterBiz(configurationRoot);
            services.RegisterViewModels(configurationRoot);

            services.AddSingleton<MainWindow>();
        }

        protected void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
