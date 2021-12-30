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
    IAsyncEnumerable<DataPointPathInfo> UpSyncAsync(IEnumerable<DataPointPathInfo> source, IProgress<double>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads an item from remote to local.
    /// </summary>
    /// <param name="data">The contract for data item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// True when the download happened and succeeded. Returns false when the target file already exists and download didn't really happen.
    /// Exceptions might throw on other cases.
    /// </returns>
    Task<bool> DownSyncAsync(DataPointPathInfo data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads an item from local to remote.
    /// </summary>
    /// <param name="source">Data point path info to the point on local.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// True when the upload happend and succeeded. Returns false when the target file already exists and upload didn't really happen.
    /// Exceptions might throw on other cases.
    /// </returns>
    Task<bool> UpSyncAsync(DataPointPathInfo source, CancellationToken cancellationToken = default);
    Task<bool> SignInAsync(CancellationToken cancellationToken = default);
}
