using CodeNameK.DataContracts;

namespace CodeNameK.DAL.Interfaces;
public interface IRemotePathProvider
{
    string GetDirectoryName(Category category);
    bool TryGetDataPointInfo(string remotePath, string remoteStoreBasePath, out DataPointPathInfo? pathInfo);
    string GetRemotePath(DataPointPathInfo dataPointInfo, string remoteStoreBasePath);
}