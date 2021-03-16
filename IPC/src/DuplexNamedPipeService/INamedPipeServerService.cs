using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeWithSaar.IPC
{
    public interface INamedPipeServerService : INamedPipeOperations, IDisposable
    {
        Task WaitForConnectionAsync(CancellationToken cancellationToken);
    }
}
