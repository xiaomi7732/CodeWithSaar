using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Server.HostedConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddNamedPipeServer(option => option.ConnectionTimeout = TimeSpan.FromMinutes(5));
                    services.AddHostedService<GreetingServices>();
                });
    }
}
