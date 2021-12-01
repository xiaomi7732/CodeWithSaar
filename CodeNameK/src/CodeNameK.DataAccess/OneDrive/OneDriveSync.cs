using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public async IAsyncEnumerable<DataPointPathInfo> DownSyncAsync(
            IEnumerable<DataPointPathInfo> data,
            IProgress<double>? progress = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            int processed = 0;
            List<DataPointPathInfo> dataList = data.ToList();
            int total = dataList.Count;

            foreach (DataPointPathInfo item in dataList)
            {
                if (await DownSyncAsync(
                    remotePath: _remotePathProvider.GetRemotePath(item, _remoteBasePath),
                    localPath: _localPathProvider.GetLocalPath(item, _localStoreOptions.DataStorePath),
                    cancellationToken).ConfigureAwait(false))
                {
                    yield return item;
                }
                processed++;
                TryUpdateProgress(progress, processed, total);
            }
        }

        public async Task<int> UpSyncAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            string localPathSearchBase = _localStoreOptions.DataStorePath;
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
                    TryUpdateProgress(progress, ++processed, total);
                    continue;
                }
                string remotePath = _remotePathProvider.GetRemotePath(dataPointPath!, _remoteBasePath);
                if (await UpSyncAsync(localFilePath, remotePath, cancellationToken).ConfigureAwait(false))
                {
                    uploaded++;
                }
                TryUpdateProgress(progress, ++processed, total);
            }

            return uploaded;
        }

        private void TryUpdateProgress(IProgress<double>? progress, int current, int total)
        {
            if (progress is null)
            {
                return;
            }

            if (total == 0)
            {
                return;
            }

            double rate = (double)current / total;
            progress.Report(rate);
        }

        private async Task<bool> UpSyncAsync(string localPath, string remotePath, CancellationToken cancellationToken)
        {
            try
            {
                DriveItem item = await _graphServiceClient.Me.Drive.Special.AppRoot.ItemWithPath(remotePath).Request().GetAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(item?.Id))
                {
                    return false;
                }
            }
            catch (ServiceException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // File not exist as expected. Keep running next by uploading.
            }
            using (Stream localFileContentStream = System.IO.File.OpenRead(localPath))
            {
                DriveItem result = await _graphServiceClient.Me.Drive.Special.AppRoot.ItemWithPath(remotePath).Content.Request().PutAsync<DriveItem>(localFileContentStream, cancellationToken);
                return !string.IsNullOrEmpty(result.Id);
            }
        }

        private async Task<bool> DownSyncAsync(string remotePath, string localPath, CancellationToken cancellationToken)
        {
            // File already exist.
            if (System.IO.File.Exists(localPath))
            {
                return false;
            }

            // Local file doesn't exist:
            string tempFile = System.IO.Path.GetTempFileName();
            using (Stream tempStream = System.IO.File.OpenWrite(tempFile))
            using (Stream downloadStream = await _graphServiceClient.Me.Drive.Special.AppRoot.ItemWithPath(remotePath).Content.Request().GetAsync())
            {
                await downloadStream.CopyToAsync(tempStream);
            }

            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            System.IO.File.Copy(tempFile, localPath, overwrite: true);
            // Best effort
            _ = Task.Run(async () =>
            {
                await Task.Yield();
                System.IO.File.Delete(tempFile);
            });
            return true;
        }

        public async IAsyncEnumerable<DataPointPathInfo> ListAllDataPointsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Queue<DriveItem> works = new Queue<DriveItem>();
            DriveItem appRootItem = await _graphServiceClient.Me.Drive.Special.AppRoot.Request().GetAsync(cancellationToken).ConfigureAwait(false);
            works.Enqueue(appRootItem);
            string appRootPath = Path.Combine(appRootItem.ParentReference.Path, appRootItem.Name);
            while (works.TryDequeue(out DriveItem work))
            {
                if (work.Folder.ChildCount == 0)
                {
                    continue;
                }

                IDriveItemChildrenCollectionPage items = await _graphServiceClient.Me.Drive.Items[work.Id].Children.Request().GetAsync(cancellationToken).ConfigureAwait(false);
                while (items is not null && items.Any())
                {
                    foreach (DriveItem item in items)
                    {
                        // Folder
                        if (item.Folder is not null)
                        {
                            works.Enqueue(item);
                            continue;
                        }

                        if (item.File is null)
                        {
                            continue;
                        }

                        // File
                        int appRootPathLength = appRootPath.Length + 1;
                        if (item.ParentReference.Path.Length < appRootPathLength)
                        {
                            continue;
                        }

                        string remotePath = item.ParentReference.Path.Substring(appRootPathLength) + '/' + item.Name;
                        if (_remotePathProvider.TryGetDataPointInfo(remotePath, _remoteBasePath, out DataPointPathInfo? pathInfo))
                        {
                            yield return pathInfo!;
                        }
                    }

                    if (items.NextPageRequest is null)
                    {
                        break;
                    }
                    items = await items.NextPageRequest.GetAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}