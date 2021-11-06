using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{
    public interface IDataPointPathService
    {
        /// <summary>
        /// Get Directory Name based on category id.
        /// </summary>
        string GetDirectoryName(Category category);

        /// <summary>
        /// Get a relative path to the file for a data point
        /// </summary>
        string GetRelativePath(DataPoint dataPoint, string baseDirectory = null);
    }
}