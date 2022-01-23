using System.Threading;
using System.Threading.Tasks;
using CodeNameK.Contracts;

namespace CodeNameK.BIZ.Interfaces
{
    public interface IBizUserPreferenceService
    {
        /// <summary>
        /// Gets the current preference
        /// </summary>
        UserPreference UserPreference { get; }

        /// <summary>
        /// Enables the sync
        /// </summary>
        Task EnableSyncAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables the sync
        /// </summary>
        Task DisableSyncAsync(CancellationToken cancellationToken = default);
    }
}