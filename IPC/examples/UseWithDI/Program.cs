using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeWithSaar.IPC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UseWithDI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Start DI container
            IServiceCollection serviceCollection = new ServiceCollection();
            IDictionary<string, string> initConfigure = new Dictionary<string, string>()
            {
                ["NamedPipe:ConnectionTimeout"] = "00:05:00"
            };

            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(initConfigure).Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole();
            });


            // Usually, only one of the following will be called.
            // Adding NamedPipe server
            serviceCollection.AddNamedPipeServer();
            // Adding NamedPipe client
            serviceCollection.AddNamedPipeClient();

            // Get the container ready
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            string pipeName = Guid.NewGuid().ToString("D");


            // Inject named pipe services;
            await Task.WhenAll(
                RunServerAsync(serviceProvider.GetRequiredService<INamedPipeServerService>(), pipeName),
                RunClientAsync(serviceProvider.GetRequiredService<INamedPipeClientService>(), pipeName)
            );
        }

        private static async Task RunServerAsync(INamedPipeServerService serverService, string pipeName)
        {
            await serverService.WaitForConnectionAsync(pipeName, cancellationToken: default).ConfigureAwait(false);
            await serverService.SendMessageAsync("Hello~").ConfigureAwait(false);
            string received = await serverService.ReadMessageAsync().ConfigureAwait(false);
            Console.WriteLine("[Server] Received: {0}", received);
        }

        private static async Task RunClientAsync(INamedPipeClientService clientService, string pipeName)
        {
            await clientService.ConnectAsync(pipeName, cancellationToken: default).ConfigureAwait(false);
            string received = await clientService.ReadMessageAsync();
            Console.WriteLine("[Client] Received: {0}", received);
            await clientService.SendMessageAsync("Hi, I am client.");
        }
    }
}
