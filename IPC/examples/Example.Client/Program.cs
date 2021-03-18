using System;
using System.Threading.Tasks;
using CodeWithSaar.IPC;
using Example.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeWithSaar.Example.Client
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

            using (INamedPipeClientService namedPipeClient = new DuplexNamedPipeService(options, logger))
            {
                Console.WriteLine("[CLIENT] Connecting to named pipe server...");
                await namedPipeClient.ConnectAsync(default).ConfigureAwait(false);
                Console.WriteLine("[CLIENT] Connected to named pipe server.");

                string whatTheServerSay = await namedPipeClient.ReadMessageAsync().ConfigureAwait(false);
                Console.WriteLine("[CLIENT] The server says: {0}", whatTheServerSay);
                whatTheServerSay = await namedPipeClient.ReadMessageAsync().ConfigureAwait(false);
                Console.WriteLine("[CLIENT] The server says(2): {0}", whatTheServerSay);

                string reply = "Hey from the client";
                Console.WriteLine("[CLIENT] Telling the server: {0}", reply);
                await namedPipeClient.SendMessageAsync(reply);

                // Send objects back and forth
                Customer serverCustomer = await namedPipeClient.ReadAsync<Customer>().ConfigureAwait(false);
                Console.WriteLine("[CLIENT] Server customer name: {0}", serverCustomer.Name);

                Customer clientCustomer = new Customer()
                {
                    Name = "Client Customer",
                };
                await namedPipeClient.SendAsync(clientCustomer).ConfigureAwait(false);
            }
        }
    }
}
