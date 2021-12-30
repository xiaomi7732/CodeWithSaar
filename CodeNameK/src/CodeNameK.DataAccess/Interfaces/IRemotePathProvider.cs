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

    /// <summary>
    /// Tries to get a category by the folder name on remote.
    /// </summary>
    /// <param name="name">The encoded folder name.</param>
    /// <param name="category">The category to return.</param>
    /// <returns>Returns true when succeeded. False otherwise.</returns>
    bool TryGetCategory(string name, out Category? category);
}