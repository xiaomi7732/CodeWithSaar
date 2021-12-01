using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts.CustomOptions;
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

        public async Task<int> SyncDown(IProgress<double>? progress, CancellationToken cancellationToken = default)
        {
            List<DataPointPathInfo> downloadTarget = new List<DataPointPathInfo>();
            await foreach (DataPointPathInfo pathInfo in _oneDriveSync.ListAllDataPointsAsync(cancellationToken).ConfigureAwait(false))
            {
                if (!_localPathProvider.PhysicalFileExists(pathInfo, _localStoreOptions.DataStorePath))
                {
                    downloadTarget.Add(pathInfo);
                }
            }

            int downloaded = 0;
            await foreach (DataPointPathInfo item in _oneDriveSync.DownSyncAsync(downloadTarget, progress: null, cancellationToken).ConfigureAwait(false))
            {
                downloaded++;
            }
            return downloaded;
        }

        public Task<int> SyncUp(IProgress<double> progress, CancellationToken cancellationToken)
        {
            return _oneDriveSync.UpSyncAsync(progress, cancellationToken);
        }
    }
}