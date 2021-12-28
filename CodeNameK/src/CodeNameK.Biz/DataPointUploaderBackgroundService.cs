using CodeNameK.Core.Utilities;
using CodeNameK.DAL.OneDrive;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
        private readonly IHostEnvironment _hostEnvironment;
        private readonly InternetAvailability _internetAvailability;
        private readonly ILogger<DataPointUploaderBackgroundService> _logger;

        public DataPointUploaderBackgroundService(
            Channel<DataPointPathInfo> channel,
            IOneDriveSync oneDrive,
            BackgroundSyncProgress progress,
            IHostEnvironment hostEnvironment,
            InternetAvailability internetAvailability,
            ILogger<DataPointUploaderBackgroundService> logger
            )
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _oneDrive = oneDrive ?? throw new ArgumentNullException(nameof(oneDrive));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _internetAvailability = internetAvailability ?? throw new ArgumentNullException(nameof(internetAvailability));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _channel.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false))
            {
                DataPointPathInfo input = await _channel.Reader.ReadAsync(stoppingToken).ConfigureAwait(false);

                LogAndReport("Check internet status...");
                while (!await _internetAvailability.IsInternetAvailableAsync())
                {
                    LogAndReport("No internet access. Retry in 1 minute.");
                    await Task.Delay(TimeSpan.FromMinutes(1)).ConfigureAwait(false);
                    LogAndReport("Check internet status...");
                }

                LogAndReport("Signing in for auto sync...");
                bool signedIn = await _oneDrive.SignInAsync(stoppingToken).ConfigureAwait(false);
                if (signedIn)
                {
                    LogAndReport("User signed in.");
                }
                else
                {
                    LogAndReport("User sign in failed. No auto sync.");
                    return;
                }

                try
                {
                    _progress.Report("Uploading data.");
                    await _oneDrive.SignInAsync(stoppingToken).ConfigureAwait(false);
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

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Writer.Complete();
            _logger.LogInformation("{count} items are still in the channel.", _channel.Reader.Count);

            List<DataPointPathInfo> messages = new List<DataPointPathInfo>();
            await foreach (DataPointPathInfo message in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                messages.Add(message);
            }

            string tempFileName = Path.GetTempFileName();
            using (Stream outputFileStream = File.OpenWrite(tempFileName))
            {
                await JsonSerializer.SerializeAsync(outputFileStream, messages).ConfigureAwait(false);
            }
            string targetFilePath = Path.Combine(_hostEnvironment.ContentRootPath, "job.json");
            FileUtilities.Move(tempFileName, targetFilePath, true);
            _logger.LogInformation("Session info persistent to: {destination}", targetFilePath);
        }

        private Task<DataPointPathInfo?> UploadAsync(DataPointPathInfo localPath, CancellationToken cancellationToken)
            => _oneDrive.UpSyncAsync(localPath, cancellationToken);

        private void LogAndReport(string message, LogLevel logLevel = LogLevel.Information)
        {
            _logger.Log(logLevel, message);
            _progress.Report(message);
        }
    }
}
