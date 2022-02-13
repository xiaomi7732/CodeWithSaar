using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Core.Utilities;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;

namespace CodeNameK.BIZ
{
    /// <summary>
    /// /// Handles data point download channel.
    /// </summary>
    public class DownSyncBackgroundService : SyncBackgroundServiceBase<DownSyncRequest>
    {
        private readonly ISync _syncService;
        private readonly ILogger _logger;

        public DownSyncBackgroundService(
            Channel<DownSyncRequest> channel,
            ISync syncService,
            BackgroundSyncProgress<DownSyncBackgroundService> progress,
            IBizUserPreferenceService userPreferenceService,
            InternetAvailability internetAvailability,
            ILogger<DownSyncBackgroundService> logger
        ) : base(channel, syncService, internetAvailability, userPreferenceService, "down-sync.json", logger)
        {
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override SyncDirection _syncDirection => SyncDirection.Down;

        protected override DownSyncRequest CreateRequestFromDataModel(DataPointPathInfo payload)
            => new DownSyncRequest(payload);

        protected override async ValueTask<bool> ExecuteSyncAsync(DataPointPathInfo input, CancellationToken cancellationToken)
            => await _syncService.DownSyncAsync(input, cancellationToken).ConfigureAwait(false);
    }
}