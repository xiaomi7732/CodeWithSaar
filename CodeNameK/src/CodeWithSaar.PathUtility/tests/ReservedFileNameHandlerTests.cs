using Xunit;

namespace CodeWithSaar.FileUtilityTests
{
    public class ReservedFileNameHandlerTests
    {
        [Theory]
        [InlineData("CON", "%CON")] // Simple case
        [InlineData("cOn", "%cOn")] // Case insensitive
        [InlineData("%LPT2", "%%LPT2")] // Do NOT over encode
        [InlineData("CONContainsCON", "CONContainsCON")] // Do NOT partial match
        [InlineData("CON.con", "%CON.con")] // File with extension
        public void ShouldEncode(string input, string expected)
        {
            ReservedFileNameProcessor target = new ReservedFileNameProcessor();
            string actual = target.Encode(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("%CON", "CON")] // Simple case
        [InlineData("%cON", "cON")] // case insensitive
        [InlineData("%CON.txt", "CON.txt")] // File with extension
        public void ShouldDecodeFullMatch(string input, string expected)
        {
            ReservedFileNameProcessor target = new ReservedFileNameProcessor();
            string actual = target.Decode(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldAllowUseEscapeCharByUser()
        {
            string content="%CON";
            ReservedFileNameProcessor target = new ReservedFileNameProcessor();
            string actual = target.Decode(target.Encode(content));
            // If the user put in a file name that is same as encoded result, after encode and decode, the user input should not be changed.
            Assert.Equal(content, actual);
        }
    }
}