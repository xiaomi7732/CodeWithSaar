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
            // This is preagreed pipename between the server and the client;
            const string PipeName = "hello_namedpipe_service";
            NamedPipeOptions namedPipeOptions = new NamedPipeOptions()
            {
                ConnectionTimeout = TimeSpan.FromSeconds(30),
            };

            using (INamedPipeServerService namedPipeServer = NamedPipeServerFactory.Instance.CreateNamedPipeService(namedPipeOptions, serializer: default, LoggerFactory.Create(builder => { })))
            {
                // Send messages back and forth
                Console.WriteLine("[SERVER] Waiting for connection.");
                await namedPipeServer.WaitForConnectionAsync(PipeName, cancellationToken: default).ConfigureAwait(false);
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
