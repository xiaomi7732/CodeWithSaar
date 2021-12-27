using CodeNameK.DAL.OneDrive;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CodeNameK.BIZ
{
    internal class DataPointUploaderBackgroundService : BackgroundService
    {
        private readonly Channel<DataPointPathInfo> _channel;
        private readonly IOneDriveSync _oneDrive;
        private readonly IProgress<string> _progress;
        private readonly ILogger<DataPointUploaderBackgroundService> _logger;

        public DataPointUploaderBackgroundService(
            Channel<DataPointPathInfo> channel,
            IOneDriveSync oneDrive,
            BackgroundSyncProgress progress,
            ILogger<DataPointUploaderBackgroundService> logger
            )
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _oneDrive = oneDrive ?? throw new ArgumentNullException(nameof(oneDrive));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!_channel.Reader.Completion.IsCompleted)
            {
                DataPointPathInfo input = await _channel.Reader.ReadAsync(stoppingToken).ConfigureAwait(false);
                try
                {
                    _progress.Report("Uploading data.");
                    DataPointPathInfo? result = await UploadAsync(input, stoppingToken).ConfigureAwait(false);
                    _logger.LogInformation("Uploaded: {0}", result);
                    _progress.Report("Data uploaded");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading data: {0}");
                    _progress.Report("Data uploaded error.");
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping background service gracefully.");
            return Task.CompletedTask;
        }

        private Task<DataPointPathInfo?> UploadAsync(DataPointPathInfo localPath, CancellationToken cancellationToken)
            => _oneDrive.UpSyncAsync(localPath, cancellationToken);
    }
}
