using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.DAL.OneDrive;
public interface IOneDriveSync
{
    Task<int> DownSyncAsync(IProgress<double> progress, CancellationToken cancellationToken);
    Task<int> UpSyncAsync(IProgress<double> progress, CancellationToken cancellationToken);
}
