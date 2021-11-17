using CodeNameK.DataContracts;
using LiveChartsCore.Defaults;

namespace CodeNameK.ViewModels
{
    internal static class DateTimePointExtensions
    {
        public static DateTimePoint ToDateTimePoint(this DataPoint dataPoint)
        {
            return new DateTimePoint()
            {
                DateTime = dataPoint.WhenUTC.ToLocalTime(),
                Value = dataPoint.Value,
            };
        }
    }
}