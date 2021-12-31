using System;
using System.Globalization;
using Xunit;

namespace CodeNameK.BIZ.UnitTests
{
    public class BizDateRangeServiceTests
    {
        [Fact]
        public void TestGetTodayRange()
        {
            // 2021-12-10 09:12:11 am , local time.
            DateTime assumedNow = new DateTime(2021, 12, 10, 9, 12, 11, DateTimeKind.Local);
            DateTime expectedBegin = new DateTime(2021, 12, 10, 0, 0, 0, DateTimeKind.Local);
            DateTime expectedEnd = new DateTime(2021, 12, 11, 0, 0, 0, DateTimeKind.Local);

            BizDateRangeService target = new BizDateRangeService();
            (DateTime actualBegin, DateTime actualEnd) = target.GetToday(assumedNow);

            Assert.Equal(actualBegin, expectedBegin);
            Assert.Equal(expectedEnd, actualEnd);
        }

        [Fact]
        public void TestGetLast2DaysRange()
        {
            // 2021-12-10 09:12:11 am , local time.
            DateTime assumedNow = new DateTime(2021, 12, 10, 9, 12, 11, DateTimeKind.Local);
            DateTime expectedBegin = new DateTime(2021, 12, 9, 0, 0, 0, DateTimeKind.Local); // Beginning of yesterday
            DateTime expectedEnd = new DateTime(2021, 12, 11, 0, 0, 0, DateTimeKind.Local); // End of day today

            BizDateRangeService target = new BizDateRangeService();
            (DateTime actualBegin, DateTime actualEnd) = target.GetLastNDays(2, assumedNow);

            Assert.Equal(actualBegin, expectedBegin);
            Assert.Equal(expectedEnd, actualEnd);
        }

        [Fact]
        public void TestGetThisWeekRangeStartOnSunday()
        {
            // 2021-12-10 09:12:11 am , local time.
            DateTime assumedNow = new DateTime(2021, 12, 10, 9, 12, 11, DateTimeKind.Local);
            DateTime expectedBegin = new DateTime(2021, 12, 5, 0, 0, 0, DateTimeKind.Local); // Beginning of the week
            DateTime expectedEnd = new DateTime(2021, 12, 12, 0, 0, 0, DateTimeKind.Local); // End of the week
            CultureInfo enUs = new CultureInfo("en-us");

            BizDateRangeService target = new BizDateRangeService();
            (DateTime actualBegin, DateTime actualEnd) = target.GetThisWeek(providedNow: assumedNow, enUs);

            Assert.Equal(actualBegin, expectedBegin);
            Assert.Equal(expectedEnd, actualEnd);

        }

        [Fact]
        public void TestGetThisWeekRangeStartOnMonday()
        {
            // 2021-12-10 09:12:11 am , local time.
            DateTime assumedNow = new DateTime(2021, 12, 10, 9, 12, 11, DateTimeKind.Local);
            DateTime expectedBegin = new DateTime(2021, 12, 6, 0, 0, 0, DateTimeKind.Local); // Beginning of the week
            DateTime expectedEnd = new DateTime(2021, 12, 13, 0, 0, 0, DateTimeKind.Local); // End of the week
            CultureInfo deDE = new CultureInfo("de-DE");

            BizDateRangeService target = new BizDateRangeService();
            (DateTime actualBegin, DateTime actualEnd) = target.GetThisWeek(providedNow: assumedNow, deDE);

            Assert.Equal(actualBegin, expectedBegin);
            Assert.Equal(expectedEnd, actualEnd);
        }

        [Fact]
        public void TestGetMonthRange()
        {
            // 2021-12-10 09:12:11 am , local time.
            DateTime assumedNow = new DateTime(2021, 12, 10, 9, 12, 11, DateTimeKind.Local);
            DateTime expectedBegin = new DateTime(2021, 12, 1, 0, 0, 0, DateTimeKind.Local); // Beginning of the week
            DateTime expectedEnd = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Local); // End of the week

            BizDateRangeService target = new BizDateRangeService();
            (DateTime actualBegin, DateTime actualEnd) = target.GetThisMonth(providedNow: assumedNow);

            Assert.Equal(actualBegin, expectedBegin);
            Assert.Equal(expectedEnd, actualEnd);
        }
    }
}
