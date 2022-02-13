using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Core.Utilities;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DAL.OneDrive;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;

namespace CodeNameK.BIZ
{
    /// <summary>
    /// /// Handles data point download channel.
    /// </summary>
    public class DownSyncBackgroundService : SyncBackgroundServiceBase<DownSyncRequest>
    {
        private readonly IOneDriveSync _oneDriveSync;
        private readonly ILogger _logger;

        public DownSyncBackgroundService(
            Channel<DownSyncRequest> channel,
            IOneDriveSync oneDriveSync,
            ITokenCredentialManager<OneDriveCredentialStatus> oneDriveTokenManager,
            IBizUserPreferenceService userPreferenceService,
            InternetAvailability internetAvailability,
            ILogger<DownSyncBackgroundService> logger
        ) : base(channel, internetAvailability, userPreferenceService, "down-sync.json", oneDriveTokenManager, logger)
        {
            _oneDriveSync = oneDriveSync ?? throw new ArgumentNullException(nameof(oneDriveSync));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override SyncDirection _syncDirection => SyncDirection.Down;

        protected override DownSyncRequest CreateRequestFromDataModel(DataPointPathInfo payload)
            => new DownSyncRequest(payload);

        protected override async ValueTask<bool> ExecuteSyncAsync(DataPointPathInfo input, CancellationToken cancellationToken)
            => await _oneDriveSync.DownSyncAsync(input, cancellationToken).ConfigureAwait(false);
    }
}