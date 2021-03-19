using System;
using System.Threading;

namespace CodeWithSaar.IPC
{
    public sealed class NamedPipeServerFactory : NamedPipeServiceFactoryBase<INamedPipeServerService>, INamedPipeServerFactory
    {
        private static Lazy<NamedPipeServerFactory> _lazyInstance = new Lazy<NamedPipeServerFactory>(valueFactory: ()=> new NamedPipeServerFactory(), mode: LazyThreadSafetyMode.ExecutionAndPublication);
        private NamedPipeServerFactory()
        {

        }
        public static NamedPipeServerFactory Instance => _lazyInstance.Value;
    }
}