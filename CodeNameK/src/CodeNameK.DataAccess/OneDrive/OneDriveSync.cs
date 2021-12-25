using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.Core.CustomExceptions;
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
        private readonly MSALAppOptions<OneDriveSync> _graphAPIOptions;
        private readonly ILogger _logger;

        public OneDriveSync(
            IRemotePathProvider remotePathProvider,
            ILocalPathProvider localPathProvider,
            IOptions<MSALAppOptions<OneDriveSync>> graphAPIOptions,
            ILogger<OneDriveSync> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _remotePathProvider = remotePathProvider ?? throw new ArgumentNullException(nameof(remotePathProvider));
            _localPathProvider = localPathProvider ?? throw new ArgumentNullException(nameof(localPathProvider));
            _graphAPIOptions = graphAPIOptions?.Value ?? throw new ArgumentNullException(nameof(graphAPIOptions));

            _graphServiceClient = CreateGraphServiceClient(_graphAPIOptions);
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
                    remotePath: _remotePathProvider.GetRemotePath(item),
                    localPath: _localPathProvider.GetLocalPath(item),
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

            DriveItem? appRootItem = null;
            Task timeoutTask = Task.Delay(_graphAPIOptions.SignInTimeout);
            Task<DriveItem> appRootTask = _graphServiceClient.Me.Drive.Special.AppRoot.Request().GetAsync(cancellationToken);

            await Task.WhenAny(timeoutTask, appRootTask).ConfigureAwait(false);
            if (!appRootTask.IsCompleted)
            {
                throw new SigninTimeoutException(_graphAPIOptions.SignInTimeout);
            }

            appRootItem = appRootTask.Result;
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
                DataPointPathInfo? result = await UpSyncAsync(pathInfo, cancellationToken).ConfigureAwait(false);
                TryUpdateProgress(progress, ++processed, pathInfoList.Count);
                if (result != null)
                {
                    yield return result;
                }
            }
        }

        public async Task<DataPointPathInfo?> UpSyncAsync(DataPointPathInfo pathInfo, CancellationToken cancellationToken = default)
        {
            string localPath = _localPathProvider.GetLocalPath(pathInfo);
            string remotePath = _remotePathProvider.GetRemotePath(pathInfo);
            if (await UpSyncAsync(localPath, remotePath, cancellationToken))
            {
                return pathInfo;
            }
            return null;
        }

        private GraphServiceClient CreateGraphServiceClient(MSALAppOptions<OneDriveSync> graphAPIOptions)
        {
            InteractiveBrowserCredentialOptions options = new InteractiveBrowserCredentialOptions
            {
                TenantId = graphAPIOptions.TenantId,
                ClientId = graphAPIOptions.ClientId,
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                RedirectUri = new Uri(graphAPIOptions.RedirectUri),
            };
            InteractiveBrowserCredential credential = new InteractiveBrowserCredential(options);
            GraphServiceClient graphClient = new GraphServiceClient(credential, graphAPIOptions.Scopes);

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
    }
}