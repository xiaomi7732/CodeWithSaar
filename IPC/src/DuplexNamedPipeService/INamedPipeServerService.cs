using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeWithSaar.IPC
{
    public interface INamedPipeServerService : INamedPipeOperations, IDisposable
    {
        void Disconnect();
        Task WaitForConnectionAsync(string pipeName, CancellationToken cancellationToken);
    }
}
