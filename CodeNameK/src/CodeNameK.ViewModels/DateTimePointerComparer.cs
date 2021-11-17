using System;
using System.Collections.Generic;
using LiveChartsCore.Defaults;

namespace CodeNameK.ViewModels
{
    internal class DateTimePointComparers : IComparer<DateTimePoint>
    {
        public static DateTimePointComparers DateTimeComparer { get; } = new DateTimePointComparers();

        public int Compare(DateTimePoint? x, DateTimePoint? y)
        {
            if (x is null)
            {
                throw new InvalidOperationException("DateTimePoint can't be null for comparison.");
            }
            if (y is null)
            {
                throw new InvalidOperationException("DateTimePoint can't be null for comparison.");
            }
            return x.DateTime.CompareTo(y.DateTime);
        }
    }
}