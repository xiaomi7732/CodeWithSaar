using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeWithSaar.IPC
{
    public abstract class NamedPipeServiceFactoryBase<T>
    {
        public T CreateNamedPipeService(NamedPipeOptions options = null, ISerializationProvider serializer = null, ILoggerFactory loggerFactory = null)
        {
            options = options ?? new NamedPipeOptions();
            IOptions<NamedPipeOptions> opt = Options.Create<NamedPipeOptions>(options);
            DuplexNamedPipeService service = new DuplexNamedPipeService(opt, serializer, loggerFactory?.CreateLogger<DuplexNamedPipeService>());
            if (service is T tservice)
            {
                return tservice;
            }
            throw new NotSupportedException($"Creating named pipe service of type {typeof(T)} isn't supported.");
        }
    }
}