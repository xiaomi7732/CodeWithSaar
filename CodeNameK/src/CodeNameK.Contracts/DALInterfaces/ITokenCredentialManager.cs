using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.DAL.Interfaces
{
    public interface ITokenCredentialManager<T>
    {
        /// <summary>
        /// Actively sign in.
        /// </summary>
        /// <param name="timeout">Timeout before sign in finished.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The sign in status.</returns>
        Task<T> SignInAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets the current sign in status.
        /// </summary>
        /// <value></value>
        T CurrentStatus { get; }

        /// <summary>
        /// Gets a task that completes when sign in successfully.
        /// Waiting on this task won't trigger login diaglog.
        /// </summary>
        Task SigningWaiter { get; }

        /// <summary>
        /// An event happens when the sign in status changed.
        /// </summary>
        event EventHandler<T>? StatusChanged;
    }
}