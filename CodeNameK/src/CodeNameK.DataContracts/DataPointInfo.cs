using System;

namespace CodeNameK.DataContracts
{
    public record DataPointInfo
    {
        public string PhysicalLocation { get; init; }
        public Guid DataPointId { get; init; }
    }
}