using System.IO;
using System.Reflection;

namespace CodeNameK.Core.Utilities
{
    public static class DirectoryUtilities
    {
        private static string? _assemblyExecutionPath;

        /// <summary>
        /// Gets exe directory. This is a simple lazy load version so that reflection won't be invoked every time.
        /// </summary>
        public static string GetExecutingAssemblyDirectory()
        {
            if (string.IsNullOrEmpty(_assemblyExecutionPath))
            {
                _assemblyExecutionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (string.IsNullOrEmpty(_assemblyExecutionPath))
                {
                    throw new InvalidDataException("Can't locate assembly folder.");
                }
            }
            return _assemblyExecutionPath;
        }
    }
}