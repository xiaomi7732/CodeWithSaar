using System;
using System.Globalization;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Core.Utilities;

namespace CodeNameK.BIZ
{
    public class BizDateRangeService : IDateRangeService
    {
        /// <summary>
        /// Gets a date range for the last N days.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public (DateTime, DateTime) GetLastNDays(uint n, DateTime providedNow = default)
        {
            // Business logic: N can't be less than 1. 
            if(n<1)
            {
                throw new ArgumentOutOfRangeException("N must be greater than 0 for last N day date range.");
            }

            if (providedNow == default)
            {
                providedNow = DateTime.Now;
            }

            DateTime start = providedNow.Date.AddDays(-n + 1);
            DateTime end = providedNow.Date.AddDays(1);

            return (start, end);
        }

        public (DateTime, DateTime) GetThisMonth(DateTime providedNow = default)
        {
            if (providedNow == default)
            {
                providedNow = DateTime.Now;
            }

            DateTime start = providedNow.FirstDayOfMonth();
            DateTime end = providedNow.LastDayOfMonth().AddDays(1);

            return (start, end);
        }

        public (DateTime, DateTime) GetThisWeek(DateTime providedNow = default, CultureInfo? cultureInfo = null)
        {
            if (providedNow == default)
            {
                providedNow = DateTime.Now;
            }
            cultureInfo = cultureInfo ?? CultureInfo.CurrentUICulture;

            DateTime start = providedNow.StartOfWeek(cultureInfo.DateTimeFormat.FirstDayOfWeek);
            DateTime end = start.AddDays(7);

            return (start, end);
        }

        public (DateTime, DateTime) GetToday(DateTime providedNow = default)
        {
            if (providedNow == default)
            {
                providedNow = DateTime.Now;
            }

            return (providedNow.Date, providedNow.Date.AddDays(1));
        }
    }
}