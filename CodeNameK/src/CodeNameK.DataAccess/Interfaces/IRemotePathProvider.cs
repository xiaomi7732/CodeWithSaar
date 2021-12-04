using CodeNameK.DataContracts;

namespace CodeNameK.DAL.Interfaces;
public interface IRemotePathProvider
{
    string BasePath { get; }
    bool TryGetDataPointInfo(string remotePath, out DataPointPathInfo? pathInfo);
    string GetRemotePath(DataPointPathInfo dataPointInfo);
}