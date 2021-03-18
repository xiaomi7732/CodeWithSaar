using System;
using System.Threading.Tasks;
using CodeWithSaar.IPC;
using Example.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeWithSaar.Example.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            NamedPipeOptions namedPipeOptions = new NamedPipeOptions()
            {
                PipeName = "hello_namedpipe_service",
                ConnectionTimeout = TimeSpan.FromSeconds(30),
            };
            IOptions<NamedPipeOptions> options = Options.Create<NamedPipeOptions>(namedPipeOptions);
            ILogger<DuplexNamedPipeService> logger = LoggerFactory.Create(config => { }).CreateLogger<DuplexNamedPipeService>();
            using (INamedPipeServerService namedPipeServer = new DuplexNamedPipeService(options, logger))
            {
                // Send messages back and forth
                Console.WriteLine("[SERVER] Waiting for connection.");
                await namedPipeServer.WaitForConnectionAsync(default).ConfigureAwait(false);
                Console.WriteLine("[SERVER] Connected.");

                Console.WriteLine("[SERVER] Sending greeting...");
                await namedPipeServer.SendMessageAsync("Hello~from server").ConfigureAwait(false);
                await namedPipeServer.SendMessageAsync("Hello again from server").ConfigureAwait(false);
                Console.WriteLine("[SERVER] Greeting sent.");

                Console.WriteLine("[SERVER] Guessing what will the client say...");
                string whatTheClientSay = await namedPipeServer.ReadMessageAsync().ConfigureAwait(false);
                Console.WriteLine("[SERVER] The client says: {0}", whatTheClientSay);

                // Send objects back and forth
                Customer serverCustomer = new Customer()
                {
                    Name = "Server Customer",
                };
                await namedPipeServer.SendAsync(serverCustomer).ConfigureAwait(false);
                Customer clientCustomer = await namedPipeServer.ReadAsync<Customer>().ConfigureAwait(false);
                Console.WriteLine("[SERVER] Client customer name: {0}", clientCustomer.Name);
            }
        }
    }
}
