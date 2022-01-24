using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Core.Utilities;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeNameK.BIZ
{
    /// <summary>
    /// Handles data point download channel.
    /// </summary>
    public class DownSyncBackgroundService : BackgroundService
    {
        private readonly Channel<DownSyncRequest> _channel;
        private readonly ISync _syncService;
        private readonly IProgress<(string, int)> _progress;
        private readonly IBizUserPreferenceService _userPreferenceService;
        private readonly InternetAvailability _internetAvailability;
        private readonly ILogger _logger;
        private readonly string _sessionFilePath;

        public DownSyncBackgroundService(
            Channel<DownSyncRequest> channel,
            ISync syncService,
            BackgroundSyncProgress<DownSyncBackgroundService> progress,
            IBizUserPreferenceService userPreferenceService,
            InternetAvailability internetAvailability,
            ILogger<DownSyncBackgroundService> logger
        )
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _userPreferenceService = userPreferenceService ?? throw new ArgumentNullException(nameof(userPreferenceService));
            _internetAvailability = internetAvailability ?? throw new ArgumentNullException(nameof(internetAvailability));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _sessionFilePath = Path.Combine(DirectoryUtilities.GetExecutingAssemblyDirectory(), "down-sync.json");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Restore data from saved session.
            // await RestoreSessionAsync(stoppingToken).ConfigureAwait(false);
            LogAndReport("Check internet status...");
            while (!await _internetAvailability.IsInternetAvailableAsync())
            {
                LogAndReport("No internet access. Retry in 1 minute.");
                await Task.Delay(TimeSpan.FromMinutes(1)).ConfigureAwait(false);
                LogAndReport("Check internet status...");
            }
            LogAndReport("Internet Connected. Waiting for data.");

            while (await _channel.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false))
            {
                if (_userPreferenceService.UserPreference.EnableSync)
                {
                    LogAndReport("Sign in failed. Wait for signing in to success.");
                    await _syncService.WaitForSignInSuccessAsync(stoppingToken).ConfigureAwait(false);

                    DataPointPathInfo input = (await _channel.Reader.ReadAsync(stoppingToken).ConfigureAwait(false)).Payload;
                    try
                    {
                        string message;
                        if (await DownloadAsync(input, stoppingToken).ConfigureAwait(false))
                        {
                            message = $"Downloaded: {input}";
                        }
                        else
                        {
                            message = $"Download didn't happen: {input}";
                        }
                        LogAndReport(message);
                    }
                    catch (Exception ex)
                    {
                        // Put back:
                        bool putBack = !_channel.Writer.TryWrite(new DownSyncRequest(input));
                        _logger.LogError(ex, "Error downloading data: {data}. Data returned to queue: {putBack}", input, putBack);
                        ReportProgress("Data uploaded error.");
                    }
                }
                else
                {
                    // Try again in 30 seconds
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    _logger.LogInformation("Sync is not enabled. Try again in 30 seconds.");
                }
            }
        }

        private async Task RestoreSessionAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_sessionFilePath))
            {
                _logger.LogInformation("No session file found at {sessionFilePath}.", _sessionFilePath);
                return;
            }

            using (Stream inputStream = File.OpenRead(_sessionFilePath))
            {
                List<DataPointPathInfo>? messages = await JsonSerializer.DeserializeAsync<List<DataPointPathInfo>>(inputStream, cancellationToken: cancellationToken).ConfigureAwait(false);
                int count = 0;
                foreach (DataPointPathInfo item in messages.NullAsEmpty())
                {
                    await _channel.Writer.WriteAsync(new DownSyncRequest(item));
                    count++;
                }
                _logger.LogInformation("{count} item restored for downloading.", count);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Writer.Complete();
            _logger.LogInformation("{count} items are still in the channel.", _channel.Reader.Count);

            List<DataPointPathInfo> messages = new List<DataPointPathInfo>();
            await foreach (DownSyncRequest downSyncRequest in _channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
            {
                messages.Add(downSyncRequest.Payload);
            }

            string tempFileName = Path.GetTempFileName();
            using (Stream outputFileStream = File.OpenWrite(tempFileName))
            {
                await JsonSerializer.SerializeAsync(outputFileStream, messages).ConfigureAwait(false);
            }
            FileUtilities.Move(tempFileName, _sessionFilePath, overwrite: true);
            _logger.LogInformation("Session info persistent to: {destination}", _sessionFilePath);
        }

        private Task<bool> DownloadAsync(DataPointPathInfo input, CancellationToken stoppingToken)
            => _syncService.DownSyncAsync(input, stoppingToken);

        private void LogAndReport(string message, LogLevel logLevel = LogLevel.Information)
        {
            _logger.Log(logLevel, message);
            ReportProgress(message);
        }

        private void ReportProgress(string message)
        {
            _progress.Report((message, _channel.Reader.Count));
        }
    }
}