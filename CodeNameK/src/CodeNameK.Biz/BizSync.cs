using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.Contracts.DataContracts;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DAL.OneDrive;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeNameK.BIZ
{
    internal class BizSync : ISync
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IOneDriveSync _oneDriveSync;
        private readonly ILocalPathProvider _localPathProvider;
        private readonly LocalStoreOptions _localStoreOptions;
        private readonly ILogger _logger;

        public BizSync(
            IOneDriveSync oneDriveSync,
            ILocalPathProvider localPathProvider,
            IOptions<LocalStoreOptions> localStoreOptions,
            ILogger<BizSync> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _oneDriveSync = oneDriveSync ?? throw new ArgumentNullException(nameof(oneDriveSync));
            _localPathProvider = localPathProvider ?? throw new ArgumentNullException(nameof(localPathProvider));
            _localStoreOptions = localStoreOptions?.Value ?? throw new ArgumentNullException(nameof(localStoreOptions));
        }

        public async Task<OperationResult<SyncStatistic>> Sync(IProgress<SyncProgress>? progress, CancellationToken cancellationToken = default)
        {
            if (!await _semaphore.WaitAsync(timeout: TimeSpan.FromSeconds(1)))
            {
                return new OperationResult<SyncStatistic>()
                {
                    IsSuccess = false,
                    Reason = "Another sychronization is in progress.",
                };
            }
            try
            {
                SyncStatistic result = default;

                List<DataPointPathInfo> remoteDataPoints = new List<DataPointPathInfo>();
                List<DataPointPathInfo> downloadTarget = new List<DataPointPathInfo>();
                SyncProgress syncProgress = new SyncProgress() { DisplayText = "Get ready", Value = (double)5 / 100 };
                progress?.Report(syncProgress); // 5%
                bool reported = false;
                await foreach (DataPointPathInfo pathInfo in _oneDriveSync.ListAllDataPointsAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (!reported)
                    {
                        reported = true;
                        syncProgress.DisplayText = "Discover remote data";
                        syncProgress.Value = (double)10 / 100;
                        progress?.Report(syncProgress);
                    }

                    remoteDataPoints.Add(pathInfo);
                    if (!_localPathProvider.PhysicalFileExists(pathInfo))
                    {
                        downloadTarget.Add(pathInfo);
                    }
                }

                syncProgress.DisplayText = "Downloading data";
                double progressOffset = (double)40 / 100; // 40%
                syncProgress.Value = progressOffset;
                progress?.Report(syncProgress);

                double progressAllocation = (double)30 / 100; // 30%
                Progress<double> downloadProgress = new Progress<double>(newValue =>
                {
                    syncProgress.Value = newValue * progressAllocation + progressOffset;
                    progress?.Report(syncProgress);
                });

                await foreach (DataPointPathInfo item in _oneDriveSync.DownSyncAsync(downloadTarget, progress: downloadProgress, cancellationToken).ConfigureAwait(false))
                {
                    if (item != null)
                    {
                        result.Downloaded++;
                    }
                    _logger.LogTrace("Item downloaded.");
                }

                syncProgress.DisplayText = "Uploading data";
                progressOffset += progressAllocation;
                Progress<double> uploadProgress = new Progress<double>(newValue =>
                {
                    syncProgress.Value = newValue * progressAllocation + progressOffset;
                    progress?.Report(syncProgress);
                });
                IEnumerable<DataPointPathInfo> uploadTargets = _localPathProvider.ListAllDataPointPaths().Except(remoteDataPoints);
                await foreach (DataPointInfo uploaded in _oneDriveSync.UpSyncAsync(uploadTargets, uploadProgress, cancellationToken).ConfigureAwait(false))
                {
                    if (uploaded != null)
                    {
                        result.Uploaded++;
                    }
                }

                syncProgress.DisplayText = "Done";
                syncProgress.Value = 1;
                progress?.Report(syncProgress);
                return new OperationResult<SyncStatistic>()
                {
                    IsSuccess = true,
                    Entity = result,
                };
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}