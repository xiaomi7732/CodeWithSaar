using Xunit;

namespace CodeNameK.BIZ.UnitTests
{
    public class BizChartAxisExpansionTests
    {
        [Theory]
        [InlineData(20, 22)]
        [InlineData(-20, -18)]
        public void TestRoughExpandUp(double input, long expected)
        {
            BizChartAxisExpansion target = new BizChartAxisExpansion();
            double actual = target.ExpandUp(input);
            Assert.Equal(expected, (long)actual);
        }

        [Theory]
        [InlineData(20, 18)]
        [InlineData(-20, -22)]
        public void TestRoughExpandDown(double input, long expected)
        {
            BizChartAxisExpansion target = new BizChartAxisExpansion();
            double actual = target.ExpandDown(input);
            Assert.Equal(expected, (long)actual);
        }
    }
}
