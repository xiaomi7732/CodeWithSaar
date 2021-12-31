using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
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
        private readonly ITokenCredentialManager<OneDriveCredentialStatus> _oneDriveTokenManager;
        private readonly ILocalPathProvider _localPathProvider;
        private readonly Channel<UpSyncRequest> _upSyncChannel;
        private readonly Channel<DownSyncRequest> _downSyncChannel;
        private readonly IProgress<(string, int)> _upSyncProgress;
        private readonly IProgress<(string, int)> _downSyncProgress;
        private readonly SyncOptions _options;
        private readonly ILogger _logger;

        public event EventHandler<OneDriveCredentialStatus>? SignInStatusChanged;

        public BizSync(
            IOneDriveSync oneDriveSync,
            ITokenCredentialManager<OneDriveCredentialStatus> oneDriveTokenManager,
            ILocalPathProvider localPathProvider,
            Channel<UpSyncRequest> upSyncChannel,
            Channel<DownSyncRequest> downSyncChannel,
            BackgroundSyncProgress<UpSyncBackgroundService> upSyncProgress,
            BackgroundSyncProgress<DownSyncBackgroundService> downSyncProgress,
            IOptions<SyncOptions> options,
            ILogger<BizSync> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _oneDriveSync = oneDriveSync ?? throw new ArgumentNullException(nameof(oneDriveSync));
            _oneDriveTokenManager = oneDriveTokenManager ?? throw new ArgumentNullException(nameof(oneDriveTokenManager));
            _oneDriveTokenManager.StatusChanged += (sender, newStatus) =>
            {
                SignInStatusChanged?.Invoke(this, newStatus);
            };
            _localPathProvider = localPathProvider ?? throw new ArgumentNullException(nameof(localPathProvider));
            _upSyncChannel = upSyncChannel ?? throw new ArgumentNullException(nameof(upSyncChannel));
            _downSyncChannel = downSyncChannel ?? throw new ArgumentNullException(nameof(downSyncChannel));
            _upSyncProgress = upSyncProgress ?? throw new ArgumentNullException(nameof(upSyncProgress));
            _downSyncProgress = downSyncProgress ?? throw new ArgumentNullException(nameof(downSyncProgress));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public int UpSyncQueueLength => _upSyncChannel.Reader.Count;

        public int DownSyncQueueLength => _downSyncChannel.Reader.Count;

        public async Task<bool> SignInAsync(CancellationToken cancellationToken)
        {
            return await _oneDriveTokenManager.SignInAsync(_options.SignInTimeout, cancellationToken).ConfigureAwait(false) == OneDriveCredentialStatus.SignedIn;
        }

        public void CancelSignIn()
        {
            // Do nothing if it is not currently signing in.
            if(_oneDriveTokenManager.CurrentStatus != OneDriveCredentialStatus.SigningIn)
            {
                return;
            }
            
            _oneDriveTokenManager.CancelSignIn();
        }

        public async Task<bool> DownSyncAsync(DataPointPathInfo item, CancellationToken cancellationToken = default)
        {
            // Must have category:
            if (string.IsNullOrEmpty(item?.Category?.Id))
            {
                throw new ArgumentNullException("item", "Category is required for downloading an item.");
            }

            // Must sign in first.
            if (!await SignInAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new UnauthorizedAccessException("Invalid sign in.");
            }

            // Down sync.
            return await _oneDriveSync.DownSyncAsync(item, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask EnqueueDownSyncAsync(Category forCategory, CancellationToken cancellationToken = default)
        {
            // Category can't be null
            if (string.IsNullOrEmpty(forCategory.Id))
            {
                throw new ArgumentNullException(nameof(forCategory));
            }

            // Must sign in first.
            if (!await SignInAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new UnauthorizedAccessException("Invalid sign in.");
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

        public async Task<bool> UpSyncAsync(DataPointPathInfo item, CancellationToken cancellationToken = default)
        {
            // Must have category:
            if (string.IsNullOrEmpty(item?.Category?.Id))
            {
                throw new ArgumentNullException("item", "Category is required for uploading an item.");
            }

            // Must sign in first.
            if (!await SignInAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new UnauthorizedAccessException("Invalid sign in.");
            }

            return await _oneDriveSync.UpSyncAsync(item, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask EnqueueUpSyncAsync(UpSyncRequest request, CancellationToken cancellationToken = default)
        {
            if (await _upSyncChannel.Writer.WaitToWriteAsync(cancellationToken))
            {
                await _upSyncChannel.Writer.WriteAsync(request, cancellationToken);
                _upSyncProgress.Report(("Up sync added.", _upSyncChannel.Reader.Count));
            }
        }

        public async IAsyncEnumerable<Category> PeekRemoteCategoriesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // Must sign in first.
            if (!await SignInAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new UnauthorizedAccessException("Invalid sign in.");
            }

            await foreach (Category category in _oneDriveSync.ListCategoriesAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return category;
            }
        }

        public async Task<OperationResult<SyncStatistic>> Sync(IProgress<SyncProgress>? progress, CancellationToken cancellationToken = default)
        {
            if (!await _semaphore.WaitAsync(timeout: TimeSpan.FromSeconds(1)))
            {
                return new OperationResult<SyncStatistic>()
                {
                    IsSuccess = false,
                    Reason = "Another synchronization is in progress.",
                };
            }
            try
            {
                SyncStatistic result = default;

                List<DataPointPathInfo> remoteDataPoints = new List<DataPointPathInfo>();
                List<DataPointPathInfo> downloadTarget = new List<DataPointPathInfo>();
                SyncProgress syncProgress = new SyncProgress() { DisplayText = "Signing in", Value = 0 };

                progress?.Report(syncProgress); // 0%
                if (!await SignInAsync(cancellationToken).ConfigureAwait(false))
                {
                    return new OperationResult<SyncStatistic>()
                    {
                        IsSuccess = false,
                        Reason = "Sign in failed",
                        Entity = result,
                    };
                }

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

        public Task WaitForSignInSuccessAsync(CancellationToken cancellationToken = default)
        {
            return _oneDriveTokenManager.SigningWaiter;
        }
    }
}