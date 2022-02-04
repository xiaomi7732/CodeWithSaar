using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserSettingsDemo;

using ServiceProvider provider = Bootstrap().BuildServiceProvider();

Consumer consumer =  provider.GetRequiredService<Consumer>();
consumer.PrintSyncSettings();

IServiceCollection Bootstrap()
{
    IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.jsonc", optional: false, reloadOnChange: false)
        .Build();
    IServiceCollection services = new ServiceCollection();
    services.AddOptions<UserConfiguration>().Bind(configuration.GetSection("UserConfiguration"));
    services.AddTransient<Consumer>();
    return services;
}