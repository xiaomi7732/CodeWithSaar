using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.DataAccess
{
    /// <summary>
    /// Writes object to a target path.
    /// </summary>
    public interface IDataWriter<T>
    {
        Task WriteAsync(T data, string filePath, CancellationToken cancellationToken);
        void Delete(string filePath);
    }
}