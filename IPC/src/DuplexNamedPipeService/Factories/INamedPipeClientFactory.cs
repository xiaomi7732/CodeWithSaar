using Microsoft.Extensions.Logging;

namespace CodeWithSaar.IPC
{
    public interface INamedPipeClientFactory
    {
        INamedPipeClientService CreateNamedPipeService(NamedPipeOptions options = null, ISerializationProvider serializer = null, ILoggerFactory loggerFactory = null);
    }
}