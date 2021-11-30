using System;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using CodeWithSaar;

namespace CodeNameK.DAL;
internal class RemotePathProvider : PathProviderBase, IRemotePathProvider
{
    public override string GetDeletedMarkerFilePath(DataPointPathInfo dataPoint, string? baseDirectory = null)
        => throw new InvalidOperationException("Unsupported operation on remote store.");

    protected override string DecodeCategory(string categoryName) => OneDriveFileUtility.Decode(categoryName);

    protected override string EncodeCategory(string categoryName) => OneDriveFileUtility.Encode(categoryName);
}
