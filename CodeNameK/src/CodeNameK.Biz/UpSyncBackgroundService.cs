using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Core.Utilities;
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
        private readonly ISync _syncService;
        private readonly ILogger _logger;

        public UpSyncBackgroundService(
            Channel<UpSyncRequest> channel,
            ISync syncService,
            InternetAvailability internetAvailability,
            IBizUserPreferenceService userPreferenceService,
            ILogger<UpSyncBackgroundService> logger
            ) : base(channel, syncService, internetAvailability, userPreferenceService, "up-sync.json", logger
            )
        {
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override SyncDirection _syncDirection => SyncDirection.Up;

        protected override UpSyncRequest CreateRequestFromDataModel(DataPointPathInfo payload) => new UpSyncRequest(payload);

        protected override async ValueTask<bool> ExecuteSyncAsync(DataPointPathInfo input, CancellationToken cancellationToken)
            => await _syncService.UpSyncAsync(input, cancellationToken).ConfigureAwait(false);

    }
}
