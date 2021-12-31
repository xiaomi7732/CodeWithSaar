using System;
using System.Globalization;

namespace CodeNameK.BIZ.Interfaces
{
    public interface IDateRangeService
    {
        /// <summary>
        /// Gets a date range from this morning until the end of the day, in local time.
        /// </summary>
        (DateTime, DateTime) GetToday(DateTime providedNow = default);

        /// <summary>
        /// Gets a date range for last N days until the end of the day today, in local time.
        /// </summary>
        /// <remarks>
        /// When N = 1, it is effectively the range of today from 0:00am to 24:00.
        /// </remarks>
        (DateTime, DateTime) GetLastNDays(uint n, DateTime providedNow  = default);

        /// <summary>
        /// Gets a date range from the beginning of this week until the end of this week, in local time.
        /// </summary>
        (DateTime, DateTime) GetThisWeek(DateTime providedNow = default, CultureInfo? cultureInfo = null);

        /// <summary>
        /// Gets a date range from the beginning of the month until the end of it, in local time.
        /// </summary>
        (DateTime, DateTime) GetThisMonth(DateTime providedNow = default);
    }
}