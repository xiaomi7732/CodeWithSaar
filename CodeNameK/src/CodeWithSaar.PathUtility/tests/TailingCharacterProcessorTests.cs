using Xunit;

namespace CodeWithSaar.FileUtilityUnitTests
{
    public class TailingCharacterProcessorTests
    {
        [Theory]
        [InlineData(".", "%002E")] // Can't end with period
        [InlineData("..", ".%002E")] // Only last period will be encoded
        [InlineData(" ", "%0020")] // Can't end with space
        [InlineData("  ", " %0020")] // Only can't tail space
        [InlineData("002E", "002E")] // Expect to do nothing
        [InlineData("SpaceInTheMiddleIs .", "SpaceInTheMiddleIs %002E")] // Space in the middle, period on tail.
        [InlineData("CanNotEndWithSpace ", "CanNotEndWithSpace%0020")] // Even it is invalid, shall not encode
        [InlineData("CanNotEndWithDot.", "CanNotEndWithDot%002E")] // Even it is invalid, shall not encode
        public void ShouldEncodeDecode(string input, string expected)
        {
            TailingCharacterProcessor target = new TailingCharacterProcessor();
            string actual = target.Encode(input);
            Assert.Equal(expected, actual);

            string decoded = target.Decode(actual);
            Assert.Equal(input, decoded);
        }
    }
}