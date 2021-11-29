using System;

namespace CodeNameK.DataContracts
{
    public record DataPoint : DataPointInfo
    {
        public DateTime WhenUTC { get; init; }
        public double Value { get; init; }

        public static implicit operator DataPointPathInfo(DataPoint input)
        {
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