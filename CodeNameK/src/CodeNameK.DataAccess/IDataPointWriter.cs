using System.Threading.Tasks;

namespace CodeNameK.DataAccess
{
    /// <summary>
    /// Writes object to a target path.
    /// </summary>
    public interface IDataWriter<T>
    {
        Task WriteAsync(T data, string filePath);
    }
}