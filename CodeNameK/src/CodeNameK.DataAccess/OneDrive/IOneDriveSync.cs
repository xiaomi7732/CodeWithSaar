using CodeNameK.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.DAL.OneDrive;
public interface IOneDriveSync
{
    IAsyncEnumerable<DataPointPathInfo> ListAllDataPointsAsync(CancellationToken cancellationToken);
    IAsyncEnumerable<DataPointPathInfo> DownSyncAsync(IEnumerable<DataPointPathInfo> data, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
    Task<int> UpSyncAsync(IProgress<double> progress, CancellationToken cancellationToken);
}
