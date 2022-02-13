using CodeNameK.DataContracts;

namespace CodeNameK.Contracts
{
    public record UpSyncRequest : SyncRequestBase<DataPointPathInfo>
    {
        public UpSyncRequest(DataPointPathInfo payload)
        {
            Payload = payload;
        }
        public override DataPointPathInfo Payload { get; }
    }
}