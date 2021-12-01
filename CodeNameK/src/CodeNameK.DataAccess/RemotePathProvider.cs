using System;
using System.Net;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using CodeWithSaar;

namespace CodeNameK.DAL;
internal class RemotePathProvider : PathProviderBase, IRemotePathProvider
{
    public override string GetDeletedMarkerFilePath(DataPointPathInfo dataPoint, string? baseDirectory = null)
        => throw new InvalidOperationException("Unsupported operation on remote store.");

    public override bool PhysicalFileExists(DataPointPathInfo dataPointPathInfo, string? localStoreBasePath = null)
        => throw new NotImplementedException();

    protected override string DecodeCategory(string categoryName) => OneDriveFileUtility.Decode(categoryName);
    protected override string EncodeCategory(string categoryName) => OneDriveFileUtility.Encode(categoryName);

    protected override string DecodePath(string path) => WebUtility.UrlDecode(path);

    protected override string EncodePath(string path) => WebUtility.UrlEncode(path);
}
