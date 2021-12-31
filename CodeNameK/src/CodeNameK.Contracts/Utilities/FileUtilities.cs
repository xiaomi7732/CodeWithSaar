using System.IO;
using System.Threading.Tasks;

namespace CodeNameK.Core.Utilities
{
    public static class FileUtilities
    {
        /// <summary>
        /// This is only useful when it is in .NET Standard. The same signature exists in System.File for .NET Core 3.1 or above.
        /// </summary>
        public static void Move(string from, string to, bool overwrite)
        {
            File.Copy(from, to, overwrite);
            // Best effort
            _ = Task.Run(() =>
            {
                File.Delete(from);
            });
        }
    }
}