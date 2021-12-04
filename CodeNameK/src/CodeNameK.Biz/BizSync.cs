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

        public async Task<OperationResult<SyncStatistic>> Sync(IProgress<double>? progress, CancellationToken cancellationToken = default)
        {
            SyncStatistic result = default;

            List<DataPointPathInfo> remoteDataPoints = new List<DataPointPathInfo>();
            List<DataPointPathInfo> downloadTarget = new List<DataPointPathInfo>();
            await foreach (DataPointPathInfo pathInfo in _oneDriveSync.ListAllDataPointsAsync(cancellationToken).ConfigureAwait(false))
            {
                remoteDataPoints.Add(pathInfo);
                if (!_localPathProvider.PhysicalFileExists(pathInfo, _localStoreOptions.DataStorePath))
                {
                    downloadTarget.Add(pathInfo);
                }
            }

            Progress<double> downloadProgress = new Progress<double>();
            await foreach (DataPointPathInfo item in _oneDriveSync.DownSyncAsync(downloadTarget, progress: downloadProgress, cancellationToken).ConfigureAwait(false))
            {
                if (item != null)
                {
                    result.Downloaded++;
                }
                _logger.LogTrace("Item downloaded.");
            }

            Progress<double> uploadProgress = new Progress<double>();
            IEnumerable<DataPointPathInfo> uploadTargets = _localPathProvider.ListAllDataPointPaths().Except(remoteDataPoints);
            await foreach (DataPointInfo uploaded in _oneDriveSync.UpSyncAsync(uploadTargets, uploadProgress, cancellationToken).ConfigureAwait(false))
            {
                if (uploaded != null)
                {
                    result.Uploaded++;
                }
            }

            return new OperationResult<SyncStatistic>() { 
                IsSuccess = true,
                Entity = result,
            };
        }
    }
}