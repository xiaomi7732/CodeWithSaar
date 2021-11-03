using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{
    public interface IDataPointPathService
    {
        string GetRelativePath(DataPoint dataPoint, string baseDirectory = null);
    }
}