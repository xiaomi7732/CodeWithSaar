using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.Core.Utilities;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        private readonly LocalStoreOptions _localStoreOptions;

        public OneDriveSync(
            GraphServiceClient graphServiceClient,
            IRemotePathProvider remotePathProvider,
            ILocalPathProvider localPathProvider,
            IOptions<LocalStoreOptions> localStoreOptions,
            ILogger<OneDriveSync> logger
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _graphServiceClient = graphServiceClient ?? throw new ArgumentNullException(nameof(graphServiceClient));
            _remotePathProvider = remotePathProvider ?? throw new ArgumentNullException(nameof(remotePathProvider));
            _localPathProvider = localPathProvider ?? throw new ArgumentNullException(nameof(localPathProvider));
            _localStoreOptions = localStoreOptions?.Value ?? throw new ArgumentNullException(nameof(localStoreOptions));
        }

        public async IAsyncEnumerable<DataPointPathInfo> DownSyncAsync(
            IEnumerable<DataPointPathInfo> data,
            IProgress<double>? progress = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            List<DataPointPathInfo> dataList = data.ToList();
            int total = dataList.Count;
            int processed = 0;
            List<DataPointPathInfo> results = new List<DataPointPathInfo>();
            await dataList.ForEachAsync(10, async item =>
            {
                if (await DownSyncAsync(
                    remotePath: _remotePathProvider.GetRemotePath(item, _remoteBasePath),
                    localPath: _localPathProvider.GetLocalPath(item, _localStoreOptions.DataStorePath),
                    cancellationToken).ConfigureAwait(false))
                {
                    results.Add(item);
                }
                Interlocked.Increment(ref processed);
                TryUpdateProgress(progress, processed, total);
            });

            foreach (DataPointPathInfo item in results)
            {
                yield return item;
            }
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

        public async IAsyncEnumerable<DataPointPathInfo> UpSyncAsync(IEnumerable<DataPointPathInfo> source, IProgress<double>? progress = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            List<DataPointPathInfo> pathInfoList = source.ToList();
            int processed = 0;

            if (pathInfoList.Count == 0)
            {
                TryUpdateProgress(progress, 1, 1); // So that it is 100%.
                yield break;
            }

            foreach (DataPointPathInfo pathInfo in pathInfoList)
            {
                string localPath = _localPathProvider.GetLocalPath(pathInfo, _localStoreOptions.DataStorePath);
                string remotePath = _remotePathProvider.GetRemotePath(pathInfo, _remoteBasePath);
                if (await UpSyncAsync(localPath, remotePath, cancellationToken))
                {
                    yield return pathInfo;
                }

                TryUpdateProgress(progress, ++processed, pathInfoList.Count);
            }
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
    }
}