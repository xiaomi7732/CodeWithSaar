using Xunit;

namespace CodeWithSaar.FileUtilityUnitTests
{
    public class PeriodsProcessorTests
    {
        [Theory]
        [InlineData(".", "%002E")]
        [InlineData("..", "%002E%002E")]
        [InlineData("002E", "002E")] // Expect to do nothing
        [InlineData("%002E", "%%002E")] // %002E should have been encoded away and restored, avoid conflict to the encoded result of the single period: '.'
        [InlineData("NoTouch.", "NoTouch.")] // Even it is invalid, shall not encode
        public void ShouldEncodeDecode(string input, string expected)
        {
            PeriodsProcessor target = new PeriodsProcessor();
            string actual = target.Encode(input);
            Assert.Equal(expected, actual);

            string decoded = target.Decode(actual);
            Assert.Equal(input, decoded);
        }
    }
}