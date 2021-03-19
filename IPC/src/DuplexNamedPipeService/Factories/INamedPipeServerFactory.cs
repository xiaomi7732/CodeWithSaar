using Microsoft.Extensions.Logging;

namespace CodeWithSaar.IPC
{
    public interface INamedPipeServerFactory
    {
        INamedPipeServerService CreateNamedPipeService(NamedPipeOptions options = null, ISerializationProvider serializer = null, ILoggerFactory loggerFactory = null);
    }
}