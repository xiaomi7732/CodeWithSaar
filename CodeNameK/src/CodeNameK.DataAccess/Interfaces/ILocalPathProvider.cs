using CodeNameK.DataContracts;

namespace CodeNameK.DAL.Interfaces;
public interface ILocalPathProvider
{
    string GetDirectoryName(Category category);
    bool TryGetDataPointInfo(string localPath, string localStoreBasePath, out DataPointPathInfo? pathInfo);

    /// <summary>
    /// Gets a relative path for a data point
    /// </summary>
    string GetLocalPath(DataPointPathInfo dataPointInfo, string? localStoreBasePath = null);

    /// <summary>
    /// Gets a relative path to the file marking a deleted data point.
    /// </summary>
    string GetDeletedMarkerFilePath(DataPointPathInfo dataPoint, string? baseDirectory = null);
}