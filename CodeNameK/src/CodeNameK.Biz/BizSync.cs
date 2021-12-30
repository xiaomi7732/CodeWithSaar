using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Contracts.DataContracts;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DAL.OneDrive;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;

namespace CodeNameK.BIZ
{
    internal class BizSync : ISync
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IOneDriveSync _oneDriveSync;
        private readonly ILocalPathProvider _localPathProvider;
        private readonly Channel<UpSyncRequest> _upSyncChannel;
        private readonly Channel<DownSyncRequest> _downSyncChannel;
        private readonly IProgress<(string, int)> _upSyncProgress;
        private readonly IProgress<(string, int)> _downSyncProgress;
        private readonly ILogger _logger;

        public BizSync(
            IOneDriveSync oneDriveSync,
            ILocalPathProvider localPathProvider,
            Channel<UpSyncRequest> upSyncChannel,
            Channel<DownSyncRequest> downSyncChannel,
            BackgroundSyncProgress<UpSyncBackgroundService> upSyncProgress,
            BackgroundSyncProgress<DownSyncBackgroundService> downSyncProgress,
            ILogger<BizSync> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _oneDriveSync = oneDriveSync ?? throw new ArgumentNullException(nameof(oneDriveSync));
            _localPathProvider = localPathProvider ?? throw new ArgumentNullException(nameof(localPathProvider));
            _upSyncChannel = upSyncChannel ?? throw new ArgumentNullException(nameof(upSyncChannel));
            _downSyncChannel = downSyncChannel ?? throw new ArgumentNullException(nameof(downSyncChannel));
            _upSyncProgress = upSyncProgress ?? throw new ArgumentNullException(nameof(upSyncProgress));
            _downSyncProgress = downSyncProgress ?? throw new ArgumentNullException(nameof(downSyncProgress));
        }

        public int UpSyncQueueLength => _upSyncChannel.Reader.Count;

        public int DownSyncQueueLength => _downSyncChannel.Reader.Count;

        public async ValueTask EnqueueDownSyncRequestAsync(Category forCategory, CancellationToken cancellationToken = default)
        {
            // Category can't be null
            if (string.IsNullOrEmpty(forCategory.Id))
            {
                throw new ArgumentNullException(nameof(forCategory));
            }

            await foreach (DataPointPathInfo dataInfo in _oneDriveSync.ListAllDataPointsAsync(forCategory, cancellationToken).ConfigureAwait(false))
            {
                if (!_localPathProvider.PhysicalFileExists(dataInfo))
                {
                    if (await _downSyncChannel.Writer.WaitToWriteAsync(cancellationToken))
                    {
                        await _downSyncChannel.Writer.WriteAsync(new DownSyncRequest(dataInfo), cancellationToken);
                        _downSyncProgress.Report(("Down sync added", _downSyncChannel.Reader.Count));
                    }
                }
            }
        }

        public async ValueTask EnqueueSyncRequestAsync(UpSyncRequest request, CancellationToken cancellationToken = default)
        {
            if (await _upSyncChannel.Writer.WaitToWriteAsync(cancellationToken))
            {
                await _upSyncChannel.Writer.WriteAsync(request, cancellationToken);
                _upSyncProgress.Report(("Up sync added.", _upSyncChannel.Reader.Count));
            }
        }

        public IAsyncEnumerable<Category> PeekRemoteCategoriesAsync(CancellationToken cancellationToken)
        {
            return _oneDriveSync.ListCategoriesAsync(cancellationToken);
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
                SyncProgress syncProgress = new SyncProgress() { DisplayText = "Signing in", Value = 0 };
                progress?.Report(syncProgress); // 0%
                await _oneDriveSync.SignInAsync(cancellationToken).ConfigureAwait(false);
                syncProgress = new SyncProgress() { DisplayText = "Signed in", Value = (double)5 / 100 };
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
                await foreach (DataPointPathInfo uploaded in _oneDriveSync.UpSyncAsync(uploadTargets, uploadProgress, cancellationToken).ConfigureAwait(false))
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