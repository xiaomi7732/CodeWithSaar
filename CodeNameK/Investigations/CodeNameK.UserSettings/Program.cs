using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserSettingsDemo;

// Reading
using ServiceProvider provider = Bootstrap().BuildServiceProvider();
Consumer consumer = provider.GetRequiredService<Consumer>();
consumer.PrintSyncSettings();

// Write false
ConfigurationWriter writer = provider.GetRequiredService<ConfigurationWriter>();
UserConfiguration configuration = consumer.GetOptions();
configuration.IsSyncEnabled = false;
await writer.WriteConfigurationAsync(configuration, default);
consumer.PrintSyncSettings();

// Write true
configuration.IsSyncEnabled = true;
await writer.WriteConfigurationAsync(configuration, default);
consumer.PrintSyncSettings();



IServiceCollection Bootstrap()
{
    IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(UserConfiguration.FileName, optional: false, reloadOnChange: false)
        .Build();
    IServiceCollection services = new ServiceCollection();
    services.AddOptions<UserConfiguration>().Bind(configuration.GetSection("UserConfiguration"));
    services.AddTransient<Consumer>();
    services.AddTransient<ConfigurationWriter>();
    return services;
}