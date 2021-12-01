using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.BIZ.Interfaces
{
    public interface ISync
    {
        Task<int> SyncUp(IProgress<double> progress, CancellationToken cancellationToken);

        Task<int> SyncDown(IProgress<double>? progress, CancellationToken cancellationToken = default);
    }
}