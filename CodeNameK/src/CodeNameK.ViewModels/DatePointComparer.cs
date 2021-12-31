using System.Collections.Generic;
using CodeNameK.DataContracts;

namespace CodeNameK.ViewModels
{
    internal class DatePointComparer : IComparer<DataPoint>
    {
        public static DatePointComparer DateTimeComparer { get; } = new DatePointComparer();

        public int Compare(DataPoint? x, DataPoint? y)
        {
            if (x is null)
            {
                return int.MinValue;
            }
            if (y is null)
            {
                return int.MaxValue;
            }
            return x.WhenUTC.CompareTo(y.WhenUTC);
        }
    }
}