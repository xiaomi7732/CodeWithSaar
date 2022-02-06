using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserSettingsDemo;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile(UserConfiguration.FilePath, optional: false, reloadOnChange: true)
    .Build();

IServiceCollection services = new ServiceCollection();
services.AddOptions<UserConfiguration>().Bind(configuration.GetSection("UserConfiguration"));
services.AddTransient<Consumer>();
services.AddTransient<ConfigurationWriter>();

using ServiceProvider provider = services.BuildServiceProvider();

// Reading
Consumer consumer = provider.GetRequiredService<Consumer>();
consumer.PrintSyncSettings();

// Write false
ConfigurationWriter writer = provider.GetRequiredService<ConfigurationWriter>();
UserConfiguration userConfiguration = consumer.GetOptions();
userConfiguration.IsSyncEnabled = false;
await writer.WriteConfigurationAsync(userConfiguration, default);
consumer.PrintSyncSettings();

// Write true
userConfiguration.IsSyncEnabled = true;
await writer.WriteConfigurationAsync(userConfiguration, default);
consumer.PrintSyncSettings();

consumer.UserConfigurationChanged += OnUserConfigurationChanged;

Console.WriteLine("Press any key to exit.");
Console.ReadKey(true);

void OnUserConfigurationChanged(object? sender, UserConfiguration e)
{
    Console.WriteLine($"New sync settings: {e.IsSyncEnabled}");
}
