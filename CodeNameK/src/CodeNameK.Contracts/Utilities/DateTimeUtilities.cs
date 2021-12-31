using System;
using System.Globalization;

namespace CodeNameK.Core.Utilities
{
    public static class DateTimeUtilities
    {
        /// <summary>
        /// Gets the begining of the date of a given date.
        /// </summary>
        public static DateTime StartOfWeek(this DateTime givenDateTime, DayOfWeek? dayOfWeek = null)
        {
            DayOfWeek effectiveStartDayOfWeek = dayOfWeek ?? CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek;

            int diff = (7 + (givenDateTime.DayOfWeek - (effectiveStartDayOfWeek))) % 7;
            return givenDateTime.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Gets the begining of the date of a given date.
        /// </summary>
        public static DateTime StartOfWeek(this DateTime givenDateTime, CultureInfo? cultureInfo)
        {
            cultureInfo ??= CultureInfo.CurrentUICulture;
            return StartOfWeek(givenDateTime, cultureInfo.DateTimeFormat.FirstDayOfWeek);
        }

        /// <summary>
        /// Gets the first day of the month the specific day lies in.
        /// </summary>
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        /// <summary>
        /// Gets the number of days in the month.
        /// </summary>
        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        /// <summary>
        /// Gets the start of the day of this month. For example, 2021-1-31 00:00:00.000 in local time.
        /// </summary>
        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, DaysInMonth(value));
        }
    }
}