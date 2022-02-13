using System.Threading;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.BIZ.Interfaces
{
    /// <summary>
    /// Services to allow access to enqueue data for uploading or downloading.
    /// </summary>
    public interface ISyncQueueRequestService<T>
    {
        /// <summary>
        /// Enqueue a payload for uploading or downloading.
        /// </summary>
        ValueTask WriteRequestAsync(DataPointPathInfo payload, CancellationToken cancellationToken);
    }
}