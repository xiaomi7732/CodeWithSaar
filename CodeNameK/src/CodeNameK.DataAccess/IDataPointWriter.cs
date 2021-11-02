using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{
    public interface IDataPointWriter
    {
        Task WriteDataPointAsync(DataPoint newPoint, string baseDirectory);
    }
}