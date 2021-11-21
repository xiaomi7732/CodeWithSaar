using System;

namespace CodeNameK.DataContracts
{
    public record DataPointInfo
    {
        public string PhysicalLocation { get; init; } = string.Empty;
        public Guid DataPointId { get; init; }
    }
}