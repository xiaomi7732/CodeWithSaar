using CodeNameK.DataContracts;

namespace CodeNameK.DAL.Interfaces;
public interface IRemotePathProvider
{
    string BasePath { get; }
    bool TryGetDataPointInfo(string remotePath, out DataPointPathInfo? pathInfo);
    string GetRemotePath(DataPointPathInfo dataPointInfo);

    /// <summary>
    /// Get relative remote path for a specific category. The category name is encoded correctly.
    /// </summary>
    string GetRemotePath(Category category);
}