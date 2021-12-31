using CodeNameK.DataContracts;

namespace CodeNameK.Contracts
{
    public record UpSyncRequest
    {
        public UpSyncRequest(DataPointPathInfo payload)
        {
            Payload = payload;
        }
        public DataPointPathInfo Payload { get; }
    }
}