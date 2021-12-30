using CodeNameK.Contracts;
using CodeNameK.Contracts.DataContracts;
using CodeNameK.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.BIZ.Interfaces
{
    public interface ISync
    {
        Task<OperationResult<SyncStatistic>> Sync(IProgress<SyncProgress>? progress, CancellationToken cancellationToken = default);
        int UpSyncQueueLength { get; }
        int DownSyncQueueLength { get; }

        IAsyncEnumerable<Category> PeekRemoteCategoriesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Actively sign in the user to OneDrive.
        /// </summary>
        /// <returns>
        /// Returns true when signed in successfully. False otherwise.
        /// </returns>
        Task<bool> SignInAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Wait until the sign in without acttively trigger it.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task WaitForSignInSuccessAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Initiate a up sync request in the background.
        /// </summary>
        /// <param name="request">Request for uploading target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        ValueTask EnqueueUpSyncAsync(UpSyncRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiate download in background for a specific category in the background.
        /// </summary>
        /// <param name="forCategory">Target category.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        ValueTask EnqueueDownSyncAsync(Category forCategory, CancellationToken cancellationToken = default);

        /// <summary>
        /// Down sync a specific item. Returns true when sync happened successfully.
        /// </summary>
        Task<bool> DownSyncAsync(DataPointPathInfo item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Up sync a specific item. Returns true when sync happened successfully.
        /// </summary>
        Task<bool> UpSyncAsync(DataPointPathInfo item, CancellationToken cancellationToken = default);
    }
}