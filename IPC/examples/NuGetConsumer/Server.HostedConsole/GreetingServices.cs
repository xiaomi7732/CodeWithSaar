using System;
using System.Threading;
using System.Threading.Tasks;
using CodeWithSaar.IPC;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Server.HostedConsole
{
    public class GreetingServices : IHostedService
    {
        private readonly INamedPipeServerService _namedPipeServer;
        private readonly ILogger<GreetingServices> _logger;

        public GreetingServices(INamedPipeServerService namedPipeServer, ILogger<GreetingServices> logger)
        {
            _namedPipeServer = namedPipeServer ?? throw new System.ArgumentNullException(nameof(namedPipeServer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                string pipeName = Guid.NewGuid().ToString();
                while (true)
                {
                    try
                    {
                        _logger.LogInformation("Listening on named pipe: {pipeName}", pipeName);
                        await _namedPipeServer.WaitForConnectionAsync(pipeName, cancellationToken);
                        _logger.LogInformation("A connection is established. Sending a welcome message...");
                        await _namedPipeServer.SendMessageAsync("Hi, welcome!");
                        _logger.LogInformation("Welcome message sent...");
                        _namedPipeServer.Disconnect();
                    }
                    catch (NamedPipeTimeoutException ex) when (ex.Operation == nameof(_namedPipeServer.WaitForConnectionAsync))
                    {
                        // Move on to the next interation, keep waiting for input.
                        _logger.LogInformation("{opName} operation timedout. Move on to the next iteration...", ex.Operation);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Shutdown the pipeline server.");
                // Gracefully exit.
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}