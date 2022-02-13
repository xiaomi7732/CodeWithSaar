using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Core.Utilities;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DAL.OneDrive;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CodeNameK.BIZ
{
    public class UpSyncBackgroundService : SyncBackgroundServiceBase<UpSyncRequest>
    {
        private readonly IOneDriveSync _oneDriveSync;
        private readonly ILogger _logger;

        public UpSyncBackgroundService(
            Channel<UpSyncRequest> channel,
            IOneDriveSync oneDriveSync,
            ITokenCredentialManager<OneDriveCredentialStatus> oneDriveTokenManager,
            InternetAvailability internetAvailability,
            IBizUserPreferenceService userPreferenceService,
            ILogger<UpSyncBackgroundService> logger
            ) : base(channel, internetAvailability, userPreferenceService, "up-sync.json", oneDriveTokenManager, logger)
        {
            _oneDriveSync = oneDriveSync ?? throw new ArgumentNullException(nameof(_oneDriveSync));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override SyncDirection _syncDirection => SyncDirection.Up;

        protected override UpSyncRequest CreateRequestFromDataModel(DataPointPathInfo payload) => new UpSyncRequest(payload);

        protected override async ValueTask<bool> ExecuteSyncAsync(DataPointPathInfo input, CancellationToken cancellationToken)
            => await _oneDriveSync.UpSyncAsync(input, cancellationToken).ConfigureAwait(false);

    }
}
