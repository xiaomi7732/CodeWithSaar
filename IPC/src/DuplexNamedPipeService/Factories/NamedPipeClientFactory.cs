using System;
using System.Threading;

namespace CodeWithSaar.IPC
{
    public sealed class NamedPipeClientFactory : NamedPipeServiceFactoryBase<INamedPipeClientService>, INamedPipeClientFactory
    {
        private static Lazy<NamedPipeClientFactory> _lazyFactory = new Lazy<NamedPipeClientFactory>(valueFactory: () => new NamedPipeClientFactory(), mode: LazyThreadSafetyMode.ExecutionAndPublication);
        private NamedPipeClientFactory()
        {
        }
        public static NamedPipeClientFactory Instance => _lazyFactory.Value;
    }
}