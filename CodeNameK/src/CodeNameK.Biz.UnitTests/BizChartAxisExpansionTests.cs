using CodeNameK.BIZ;
using Xunit;

namespace CodeNameK.Biz.UnitTests
{
    public class BizChartAxisExpansionTests
    {
        [Theory]
        [InlineData(20, 21)]
        [InlineData(-20, -19)]
        public void TestRoughExpandUp(double input, long expected)
        {
            BizChartAxisExpansion target = new BizChartAxisExpansion();
            double actual = target.ExpandUp(input);
            Assert.Equal(expected, (long)actual);
        }

        [Theory]
        [InlineData(20, 19)]
        [InlineData(-20, -21)]
        public void TestRoughExpandDown(double input, long expected)
        {
            BizChartAxisExpansion target = new BizChartAxisExpansion();
            double actual = target.ExpandDown(input);
            Assert.Equal(expected, (long)actual);
        }
    }
}
