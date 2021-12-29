using System;
using System.Text.Json.Serialization;

namespace CodeNameK.DataContracts
{
    public record DataPoint
    {
        public Guid Id { get; init; }

        [JsonIgnore]
        public Category? Category { get; init; }


        public DateTime WhenUTC { get; init; }
        public double Value { get; init; }

        public static implicit operator DataPointPathInfo(DataPoint input)
        {
            if(input.Category is null)
            {
                throw new InvalidCastException("Can't convert a datapoint to DataPointPathInfo without Category.");
            }
            return new DataPointPathInfo()
            {
                Id = input.Id,
                Category = input.Category,
                YearFolder = (ushort)input.WhenUTC.Year,
                MonthFolder = (ushort)input.WhenUTC.Month,
            };
        }
    }
}