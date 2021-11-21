using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.DAL
{
    /// <summary>
    /// Writes object to a target path.
    /// </summary>
    public interface IDataWriter<T>
    {
        Task WriteAsync(T data, string filePath, CancellationToken cancellationToken);
        void WriteEmpty(string filePath);
        void Delete(string filePath);
    }
}