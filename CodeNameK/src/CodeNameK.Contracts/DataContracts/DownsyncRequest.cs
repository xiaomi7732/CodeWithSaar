using CodeNameK.DataContracts;

namespace CodeNameK.Contracts
{
    public class DownSyncRequest
    {
        public DataPointPathInfo Payload { get; }

        public DownSyncRequest(DataPointPathInfo payload)
        {
            Payload = payload;
        }
    }
}