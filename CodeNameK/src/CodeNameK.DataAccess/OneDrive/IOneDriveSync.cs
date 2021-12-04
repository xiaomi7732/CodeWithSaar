using CodeNameK.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CodeNameK.DAL.OneDrive;
public interface IOneDriveSync
{
    IAsyncEnumerable<DataPointPathInfo> ListAllDataPointsAsync(CancellationToken cancellationToken);
    IAsyncEnumerable<DataPointPathInfo> DownSyncAsync(IEnumerable<DataPointPathInfo> data, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<DataPointPathInfo> UpSyncAsync(IEnumerable<DataPointPathInfo> source, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
}
