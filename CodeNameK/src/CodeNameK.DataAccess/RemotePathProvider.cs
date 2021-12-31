using System;
using System.Collections.Generic;
using System.Net;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using CodeWithSaar;

namespace CodeNameK.DAL;
internal class RemotePathProvider : PathProviderBase, IRemotePathProvider
{
    // TODO: Make this into an option. Hardcode for now.
    public override string BasePath => "Data";

    public override bool PhysicalFileExists(DataPointPathInfo dataPointPathInfo)
        => throw new NotImplementedException();

    protected override string DecodeCategory(string categoryName) => OneDriveFileUtility.Decode(categoryName);
    protected override string EncodeCategory(string categoryName) => OneDriveFileUtility.Encode(categoryName);

    protected override string DecodePath(string path) => WebUtility.UrlDecode(path);

    protected override string EncodePath(string path) => path;

    public override IEnumerable<DataPointPathInfo> ListAllDataPointPaths()
    {
        throw new NotImplementedException();
    }
}
