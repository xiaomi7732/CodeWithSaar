using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.DataAccess
{
    public interface IDataReader<T>
    {
        /// <summary>
        /// Reads an object from a target path.
        /// </summary>
        Task<T> ReadAsync(string filePath, CancellationToken cancellationToken);
    }
}