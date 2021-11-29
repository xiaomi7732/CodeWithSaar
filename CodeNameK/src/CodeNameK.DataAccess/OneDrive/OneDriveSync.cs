using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace CodeNameK.DAL.OneDrive
{
    public class OneDriveSync : IOneDriveSync
    {

        private const string _remoteBasePath = "Data"; // root://AppRoot/Data
        private readonly GraphServiceClient _graphServiceClient;

        private readonly IRemotePathProvider _remotePathProvider;
        private readonly ILocalPathProvider _localPathProvider;
        private readonly LocalStoreOptions _localStoreOptions;

        public OneDriveSync(
            GraphServiceClient graphServiceClient,
            IRemotePathProvider remotePathProvider,
            ILocalPathProvider localPathProvider,
            IOptions<LocalStoreOptions> localStoreOptions
            )
        {
            _graphServiceClient = graphServiceClient ?? throw new System.ArgumentNullException(nameof(graphServiceClient));
            _remotePathProvider = remotePathProvider ?? throw new System.ArgumentNullException(nameof(remotePathProvider));
            _localPathProvider = localPathProvider ?? throw new System.ArgumentNullException(nameof(localPathProvider));
            _localStoreOptions = localStoreOptions?.Value ?? throw new System.ArgumentNullException(nameof(localStoreOptions));
        }

        public Task<int> DownSyncAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> UpSyncAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            string localPathSearchBase = Environment.ExpandEnvironmentVariables(_localStoreOptions.DataStorePath);
            if (!System.IO.Directory.Exists(localPathSearchBase))
            {
                return 0;
            }

            List<string> localFilePathList = System.IO.Directory.EnumerateFiles(localPathSearchBase, "*", new EnumerationOptions { RecurseSubdirectories = true }).ToList();
            int total = localFilePathList.Count;
            int processed = 0;
            int uploaded = 0;
            foreach (string localFilePath in localFilePathList)
            {
                if (!_localPathProvider.TryGetDataPointInfo(localFilePath, Path.GetFullPath(localPathSearchBase), out DataPointPathInfo? dataPointPath))
                {
                    UpdateProgress(progress, ++processed, total);
                    continue;
                }
                string remotePath = _remotePathProvider.GetRemotePath(dataPointPath!, _remoteBasePath);
                await UpSyncAsync(localFilePath, remotePath, cancellationToken).ConfigureAwait(false);
                UpdateProgress(progress, ++processed, total);
                uploaded++;
            }

            return uploaded;
        }

        private void UpdateProgress(IProgress<double> progress, int current, int total)
        {
            double rate = (double)current / total;
            progress.Report(rate);
        }

        private async Task UpSyncAsync(string localPath, string remotePath, CancellationToken cancellationToken)
        {
            using (Stream localFileContentStream = System.IO.File.OpenRead(localPath))
            {
                await _graphServiceClient.Me.Drive.Special.AppRoot.ItemWithPath(remotePath).Content.Request().PutAsync<DriveItem>(localFileContentStream, cancellationToken);
            }
        }

        private async Task DownSyncAsync(string remotePath, string localPath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            // _graphServiceClient.Me.Drive.Special.AppRoot.ItemWithPath(remotePath).
        }
    }
}