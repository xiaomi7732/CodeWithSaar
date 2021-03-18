using System;
using System.Threading.Tasks;
using CodeWithSaar.IPC;

namespace GetStartedConsole
{
    // This is a in-process example. The same code will work when the client & the server belongs to different processes.
    class Program
    {
        static async Task Main(string[] args)
        {
            // Make an agreement on the pipeName for both the client and the server.
            string namedPipeName = Guid.NewGuid().ToString("D");
            
            // Start the server and the client at the same time for communication.
            await Task.WhenAll(
                RunServerAsync(namedPipeName),
                RunClientAsync(namedPipeName)
            ).ConfigureAwait(false);
        }

        private static async Task RunServerAsync(string pipeName)
        {
            // Create the server service
            INamedPipeServerService namedPipeServer = new DuplexNamedPipeService();

            // Wait for connection
            await namedPipeServer.WaitForConnectionAsync(pipeName, cancellationToken: default).ConfigureAwait(false);

            // Send a message once there's a client
            await namedPipeServer.SendMessageAsync("Hello from server!").ConfigureAwait(false);

            // Retrieve a message from the client
            string message = await namedPipeServer.ReadMessageAsync();
            Console.WriteLine("[SERVER] Message received from the client: {0}", message);
        }

        private static async Task RunClientAsync(string pipeName)
        {
            // Create a client service
            INamedPipeClientService namedPipeClient = new DuplexNamedPipeService();

            // Try to connect to the server using the pipeName
            await namedPipeClient.ConnectAsync(pipeName, cancellationToken: default).ConfigureAwait(false);

            // Retrieve the message the server will send once connected
            string messageReceived =await namedPipeClient.ReadMessageAsync().ConfigureAwait(false);
            Console.WriteLine("[CLIENT] Message from the server: {0}", messageReceived );

            // Send a message back to the server
            string messageFromClient = "Hello from client!";
            await namedPipeClient.SendMessageAsync(messageFromClient).ConfigureAwait(false);
        }
    }
}
