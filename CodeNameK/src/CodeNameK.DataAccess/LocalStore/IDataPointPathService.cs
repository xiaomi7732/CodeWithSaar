using CodeNameK.DataContracts;

namespace CodeNameK.DAL
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
        string GetRelativePath(DataPoint dataPoint, string? baseDirectory = null);

        /// <summary>
        /// Gets a relative path to the file marking a deleted data point.
        /// </summary>
        string GetDeletedMarkerFilePath(DataPoint dataPoint, string? baseDirectory = null);
    }
}