using System;
using System.Threading.Tasks;
using CodeWithSaar.IPC;

namespace Client.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage:");
                System.Console.WriteLine("Client.Console.exe <pipeName>");
                System.Console.WriteLine();
                System.Console.WriteLine("Get the pipename from the server log.");
                Environment.Exit(-1);
            }

            string pipeName = args[0];
            using (INamedPipeClientService client = NamedPipeClientFactory.Instance.CreateNamedPipeService())
            {
                System.Console.WriteLine("[CLIENT] Try to reach server at pipe: {0}", pipeName);
                await client.ConnectAsync(pipeName, default);
                string handshake = await client.ReadMessageAsync().ConfigureAwait(false);

                System.Console.WriteLine("[CLIENT] Got handshake message from the server: {0}", handshake);
            }
        }
    }
}
