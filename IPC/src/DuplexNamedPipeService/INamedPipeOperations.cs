using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeWithSaar.IPC
{
    public interface INamedPipeOperations
    {
        /// <summary>
        /// Gets the name of the pipe stream.
        /// </summary>
        string PipeName { get; }

        /// <summary>
        /// Sends a string message over the named pipe;
        /// </summary>
        /// <param name="message"></param>
        Task SendMessageAsync(string message, TimeSpan timeout = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a string message from a named pipe;
        /// </summary>
        Task<string> ReadMessageAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object over the named pipe.
        /// </summary>
        Task SendAsync<T>(T payload, TimeSpan timeout = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an object over the named pipe.
        /// </summary>
        Task<T> ReadAsync<T>(TimeSpan timeout = default, CancellationToken cancellationToken = default);
    }
}
