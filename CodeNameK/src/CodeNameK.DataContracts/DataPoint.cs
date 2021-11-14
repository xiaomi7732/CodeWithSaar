using System;
using System.Text.Json.Serialization;

namespace CodeNameK.DataContracts
{
    public record DataPoint
    {
        public Guid Id { get; init; }
        public DateTime WhenUTC { get; init; }
        public double Value { get; init; }

        [JsonIgnore]
        public Category? Category { get; init; }
    }
}