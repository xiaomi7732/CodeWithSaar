using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeWithSaar.IPC
{
    public interface INamedPipeClientService: INamedPipeOperations, IDisposable
    {
        Task ConnectAsync(CancellationToken cancellationToken);
    }
}
