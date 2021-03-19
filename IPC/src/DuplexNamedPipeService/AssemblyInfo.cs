using System.Runtime.CompilerServices;
using CodeWithSaar.IPC;

[assembly: InternalsVisibleToAttribute(Constants.UnitTestsAssemblyName)]

namespace CodeWithSaar.IPC
{
    struct Constants
    {
        public const string UnitTestsAssemblyName = "UnitTests";

    }
}