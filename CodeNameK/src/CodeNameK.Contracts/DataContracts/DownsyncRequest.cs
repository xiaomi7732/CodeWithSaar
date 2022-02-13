using CodeNameK.DataContracts;

namespace CodeNameK.Contracts;
public record DownSyncRequest : SyncRequestBase<DataPointPathInfo>
{
    public override DataPointPathInfo Payload { get; }

    public DownSyncRequest(DataPointPathInfo payload)
    {
        Payload = payload;
    }
}