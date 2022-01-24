using System.IO;
using System.Reflection;

namespace CodeNameK.Core.Utilities
{
    public static class DirectoryUtilities
    {
        public static string GetExecutingAssemblyDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        }
    }
}