using System.Threading.Tasks;

namespace CodeWithSaar.IPC
{
    public interface INamedPipeOperations
    {
        Task SendMessageAsync(string message);
        Task<string> ReadMessageAsync();
    }
}
