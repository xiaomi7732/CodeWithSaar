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
        Task SendMessageAsync(string message);

        /// <summary>
        /// Retrieves a string message from a named pipe;
        /// </summary>
        /// <returns></returns>
        Task<string> ReadMessageAsync();

        /// <summary>
        /// Sends an object over the named pipe.
        /// </summary>
        Task SendAsync<T>(T payload);

        /// <summary>
        /// Retrieves an object over the named pipe.
        /// </summary>
        Task<T> ReadAsync<T>();
    }
}
