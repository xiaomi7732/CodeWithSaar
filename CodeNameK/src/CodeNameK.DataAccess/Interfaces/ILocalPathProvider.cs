using CodeNameK.DataContracts;
using System.Collections.Generic;

namespace CodeNameK.DAL.Interfaces;
public interface ILocalPathProvider
{
    string BasePath { get; }

    string GetDirectoryName(Category category);
    bool TryGetDataPointInfo(string localPath, out DataPointPathInfo? pathInfo);

    /// <summary>
    /// Gets a relative path for a data point
    /// </summary>
    string GetLocalPath(DataPointPathInfo dataPointInfo);

    /// <summary>
    /// Gets a relative path to the file marking a deleted data point.
    /// </summary>
    string GetDeletedMarkerFilePath(DataPointPathInfo dataPoint);

    /// <summary>
    /// Check whether the physical file exists
    /// </summary>
    bool PhysicalFileExists(DataPointPathInfo dataPointPathInfo);

    /// <summary>
    /// Gets all local data point path in a list.
    /// </summary>
    IEnumerable<DataPointPathInfo> ListAllDataPointPaths();
}