using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace CodeNameK.BIZ;

public abstract class SyncBackgroundServiceBase<TRequest> : BackgroundService
    where TRequest : SyncRequestBase<DataPointPathInfo>
{
    private bool _disposed = false;
    private readonly Channel<TRequest> _channel;
    private readonly ISync _syncService;
    private readonly InternetAvailability _internetAvailability;
    private readonly IBizUserPreferenceService _userPreferenceService;
    private readonly ILogger _logger;
    private IProgress<(int, string)>? _progressReporter;

    private ManualResetEventSlim? _syncEnabledFlag = new ManualResetEventSlim(false);

    private readonly string _persistentFileFullPath;

    protected enum SyncDirection
    {
        Up,
        Down,
    }

    /// <summary>
    /// Defines the sync direction of the current service.
    /// </summary>
    protected abstract SyncDirection _syncDirection { get; }

    public SyncBackgroundServiceBase(
        Channel<TRequest> channel,
        ISync syncService,
        InternetAvailability internetAvailability,
        IBizUserPreferenceService userPreferenceService,
        string persistentFileName,
        ILogger<SyncBackgroundServiceBase<TRequest>> logger)
    {
        if (string.IsNullOrEmpty(persistentFileName))
        {
            throw new ArgumentException($"'{nameof(persistentFileName)}' cannot be null or empty.", nameof(persistentFileName));
        }
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
        _internetAvailability = internetAvailability ?? throw new ArgumentNullException(nameof(internetAvailability));
        _userPreferenceService = userPreferenceService ?? throw new ArgumentNullException(nameof(userPreferenceService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _persistentFileFullPath = Path.Combine(DirectoryUtilities.GetExecutingAssemblyDirectory(), persistentFileName);

        _syncEnabledFlag = new ManualResetEventSlim(_userPreferenceService.UserPreference.EnableSync);
        _userPreferenceService.UserPreferenceChanged += UserPreferenceChanged;
    }

    private void UserPreferenceChanged(object sender, UserPreference e)
    {
        if (e.EnableSync)
        {
            _syncEnabledFlag?.Set();
        }
        else
        {
            _syncEnabledFlag?.Reset();
        }
    }

    /// <summary>
    /// Sets up a progress handler.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">Throws if progressChangedHandler is null.</exception>
    public void ReportProgressTo(Action<(int, string)> progressChangedHandler)
    {
        _progressReporter = new Progress<(int, string)>(progressChangedHandler);
    }

    /// <summary>
    /// Executes the core logic
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Restore data from saved session.
        await RestoreSessionAsync(stoppingToken).ConfigureAwait(false);

        while (true)
        {
            // Ensure sync is enabled;
            ReportProgress("Making sure sync is enabled.");
            _syncEnabledFlag?.Wait();

            // Sync cycle
            ReportProgress("Wait for data to sync...");
            await _channel.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false);

            // Ensure internet is there.
            ReportProgress("Check internet status ...");
            while (!await _internetAvailability.IsInternetAvailableAsync().ConfigureAwait(false))
            {
                await Task.Delay(TimeSpan.FromMinutes(1)).ConfigureAwait(false);
                ReportProgress("Recheck internet status ...");
            }

            // Ensure sign in
            ReportProgress("Check signIn status");
            await _syncService.WaitForSignInSuccessAsync(stoppingToken).ConfigureAwait(false);

            // Start sync
            DataPointPathInfo input = (await _channel.Reader.ReadAsync(stoppingToken).ConfigureAwait(false)).Payload;
            try
            {
                if (await ExecuteSyncAsync(input, stoppingToken).ConfigureAwait(false))
                {
                    ReportProgress($"{nameof(ExecuteSyncAsync)} completed successfully for {_syncDirection}");
                }
                else
                {
                    ReportProgress($"{nameof(ExecuteSyncAsync)} failed for {_syncDirection}");
                }
            }
            catch (Exception ex)
            {
                // Put back:
                bool putBack = !_channel.Writer.TryWrite(CreateRequestFromDataModel(input));
                _logger.LogError(ex, "Error uploading data: {data}. Data returned to queue: {putBack}", input, putBack);
                ReportProgress("Data uploaded error.");
            }
        }
    }

    /// <summary>
    /// Stop handler for sync service.
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Writer.Complete();
        _logger.LogInformation("{count} items are still in the channel.", _channel.Reader.Count);

        List<DataPointPathInfo> messages = new List<DataPointPathInfo>();
        await foreach (TRequest upSyncRequest in _channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            messages.Add(upSyncRequest.Payload);
        }

        using (Stream outputFileStream = new FileStream(_persistentFileFullPath, FileMode.Create, FileAccess.Write))
        {
            await JsonSerializer.SerializeAsync(outputFileStream, messages).ConfigureAwait(false);
        }
        _logger.LogInformation("Session info persistent to: {destination}", _persistentFileFullPath);
    }

    /// <summary>
    /// Execute the sync operation for a given data point.
    /// </summary>
    /// <return>
    /// Returns true when sync succeeded; Otherwise, false.
    /// </return>
    protected abstract ValueTask<bool> ExecuteSyncAsync(DataPointPathInfo input, CancellationToken cancellationToken);

    /// <summary>
    /// Restores data from saved file.
    /// </summary>
    private async ValueTask RestoreSessionAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_persistentFileFullPath))
        {
            _logger.LogInformation("No session file found at {sessionFilePath}.", _persistentFileFullPath);
            return;
        }

        using (Stream inputStream = File.OpenRead(_persistentFileFullPath))
        {
            List<DataPointPathInfo>? messages = await JsonSerializer.DeserializeAsync<List<DataPointPathInfo>>(inputStream, cancellationToken: cancellationToken).ConfigureAwait(false);
            int count = 0;
            foreach (DataPointPathInfo item in messages.NullAsEmpty())
            {
                await _channel.Writer.WriteAsync(CreateRequestFromDataModel(item), cancellationToken).ConfigureAwait(false);
                count++;
            }
            _logger.LogInformation("{count} item restored for uploading.", count);
        }
    }

    /// <summary>
    /// Creates a request from the data model.
    /// </summary>
    protected abstract TRequest CreateRequestFromDataModel(DataPointPathInfo payload);

    /// <summary>
    /// Report progress.
    /// </summary>
    private void ReportProgress(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
        }

        Debug.Assert(_channel.Reader.CanCount);
        int count = _channel.Reader.Count;
        _logger.LogInformation("Progress reported. Count: {readerValueCount}. Message: {message}", count, message);
        _progressReporter?.Report((count, message));
    }

    /// <summary>
    /// Dispose managed resources.
    /// </summary>
    public override void Dispose()
    {
        Dispose(true);
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _syncEnabledFlag?.Dispose();
            _syncEnabledFlag = null;
        }

        _disposed = true;
    }
}