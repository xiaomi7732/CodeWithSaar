using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.DAL
{
    public interface IDataReader<T>
    {
        /// <summary>
        /// Reads an object from a target path.
        /// </summary>
        Task<T?> ReadAsync(string filePath, CancellationToken cancellationToken);
    }
}