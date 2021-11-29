using System;
using System.Text.Json.Serialization;

namespace CodeNameK.DataContracts
{
    public abstract record DataPointInfo
    {
        public Guid Id { get; init; }

        [JsonIgnore]
        public Category? Category { get; init; }
    }
}