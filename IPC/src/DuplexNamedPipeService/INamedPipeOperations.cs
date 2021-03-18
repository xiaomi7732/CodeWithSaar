using System.Threading.Tasks;

namespace CodeWithSaar.IPC
{
    public interface INamedPipeOperations
    {
        string PipeName { get; }
        Task SendMessageAsync(string message);
        Task<string> ReadMessageAsync();
    }
}
