using System;
using System.Collections.Concurrent;
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
        private readonly GraphServiceClient _graphServiceClient;

        private readonly IRemotePathProvider _remotePathProvider;
        private readonly ILocalPathProvider _localPathProvider;
        private readonly OneDriveTokenCredential _tokenCredential;
        private readonly MSALAppOptions<OneDriveSync> _graphAPIOptions;

        private readonly ILogger _logger;

        public OneDriveSync(
            IRemotePathProvider remotePathProvider,
            ILocalPathProvider localPathProvider,
            IOptions<MSALAppOptions<OneDriveSync>> graphAPIOptions,
            OneDriveTokenCredential tokenCredential,
            ILogger<OneDriveSync> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _remotePathProvider = remotePathProvider ?? throw new ArgumentNullException(nameof(remotePathProvider));
            _localPathProvider = localPathProvider ?? throw new ArgumentNullException(nameof(localPathProvider));
            _tokenCredential = tokenCredential ?? throw new ArgumentNullException(nameof(tokenCredential));
            _graphAPIOptions = graphAPIOptions?.Value ?? throw new ArgumentNullException(nameof(graphAPIOptions));

            _graphServiceClient = CreateGraphServiceClient(_graphAPIOptions);
        }

        public IAsyncEnumerable<DataPointPathInfo> ListAllDataPointsAsync(CancellationToken cancellationToken)
            => ListAllDataPointsAsync(relativeRemotePath: _remotePathProvider.BasePath, cancellationToken);

        public async IAsyncEnumerable<Category> ListCategoriesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            string basePath = _remotePathProvider.BasePath;
            DriveItem baseDriveItem = await _graphServiceClient.Me.Drive.Special.AppRoot.ItemWithPath(basePath).Request().GetAsync(cancellationToken);

            if (baseDriveItem.Folder is null || baseDriveItem.Folder.ChildCount == 0)
            {
                // No children.
                yield break;
            }

            IDriveItemChildrenCollectionPage items = await _graphServiceClient.Me.Drive.Items[baseDriveItem.Id].Children.Request().GetAsync(cancellationToken).ConfigureAwait(false);
            while (items != null && items.Any())
            {
                foreach (DriveItem item in items)
                {
                    if (item.Folder is null)
                    {
                        continue;
                    }

                    if (_remotePathProvider.TryGetCategory(item.Name, out Category? category))
                    {
                        if (category is null)
                        {
                            continue;
                        }
                        yield return category;
                    }

                }

                if (items.NextPageRequest is null)
                {
                    break;
                }
                items = await items.NextPageRequest.GetAsync(cancellationToken).ConfigureAwait(false);
            }

        }

        public IAsyncEnumerable<DataPointPathInfo> ListAllDataPointsAsync(Category category, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(category.Id))
            {
                throw new ArgumentNullException(nameof(category));
            }
            return ListAllDataPointsAsync(relativeRemotePath: _remotePathProvider.GetRemotePath(category), cancellationToken);
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
            await dataList.ForEachAsync(50, async item =>
            {
                if (await DownSyncAsync(item, cancellationToken).ConfigureAwait(false))
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

        public Task<bool> DownSyncAsync(DataPointPathInfo data, CancellationToken cancellationToken = default)
        {
            string localPath = _localPathProvider.GetLocalPath(data);
            string remotePath = _remotePathProvider.GetRemotePath(data);
            return DownSyncAsync(remotePath, localPath, cancellationToken);
        }

        public async IAsyncEnumerable<DataPointPathInfo> UpSyncAsync(IEnumerable<DataPointPathInfo> source, IProgress<double>? progress = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            List<DataPointPathInfo> allPoints = source.ToList();
            int total = allPoints.Count;
            int processed = 0;
            ConcurrentBag<DataPointPathInfo> results = new ConcurrentBag<DataPointPathInfo>();

            await source.ForEachAsync(50, async item =>
            {
                if (await UpSyncAsync(item, cancellationToken).ConfigureAwait(false))
                {
                    results.Add(item);
                }
                Interlocked.Increment(ref processed);
                TryUpdateProgress(progress, processed, total);
            }).ConfigureAwait(false);

            foreach (DataPointPathInfo item in results)
            {
                yield return item;
            }
        }

        public Task<bool> UpSyncAsync(DataPointPathInfo pathInfo, CancellationToken cancellationToken = default)
        {
            string localPath = _localPathProvider.GetLocalPath(pathInfo);
            string remotePath = _remotePathProvider.GetRemotePath(pathInfo);
            return UpSyncAsync(localPath, remotePath, cancellationToken);
        }

        private GraphServiceClient CreateGraphServiceClient(MSALAppOptions<OneDriveSync> graphAPIOptions)
        {
            GraphServiceClient graphClient = new GraphServiceClient(_tokenCredential, graphAPIOptions.Scopes);
            return graphClient;
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

        private async IAsyncEnumerable<DataPointPathInfo> ListAllDataPointsAsync(string relativeRemotePath, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Queue<DriveItem> works = new Queue<DriveItem>();
            DriveItem appRootItem = await _graphServiceClient.Me.Drive.Special.AppRoot.Request().GetAsync(cancellationToken);

            DriveItem? workRoot = null;
            if (string.IsNullOrEmpty(relativeRemotePath))
            {
                workRoot = appRootItem;
            }
            else
            {
                try
                {
                    workRoot = await _graphServiceClient.Me.Drive.Special.AppRoot.ItemWithPath(relativeRemotePath).Request().GetAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (ServiceException ex) when (string.Equals(ex.Error?.Code, "itemNotFound"))
                {
                    _logger.LogWarning("Remote item doesn't exist: {path}", relativeRemotePath);
                }
            };

            if (workRoot is null)
            {
                // Nothing.
                yield break;
            }

            works.Enqueue(workRoot);
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
                        if (_remotePathProvider.TryGetDataPointInfo(remotePath, out DataPointPathInfo? pathInfo))
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