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
        /// Initiate a up sync request in the background.
        /// </summary>
        /// <param name="request">Request for uploading target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        ValueTask EnqueueSyncRequestAsync(UpSyncRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiate download in background for a specific category in the background.
        /// </summary>
        /// <param name="forCategory">Target category.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        ValueTask EnqueueDownSyncRequestAsync(Category forCategory, CancellationToken cancellationToken = default);
    }
}