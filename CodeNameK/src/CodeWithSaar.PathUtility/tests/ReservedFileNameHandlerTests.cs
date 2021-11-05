using Xunit;

namespace CodeWithSaar.FileUtilityTests
{
    public class ReservedFileNameHandlerTests
    {
        [Theory]
        [InlineData("CON", "%CON")] // Simple case
        [InlineData("cOn", "%cOn")] // Case insensitive
        [InlineData("Test/COM4", "Test/%COM4")] // Filename at the end
        [InlineData("Test/COM4/abc", "Test/%COM4/abc")] // Filename in the middle
        [InlineData("COM4/Test/COM4/abc", "%COM4/Test/%COM4/abc")] // Same match, multiple times
        [InlineData("LPT4/Test/COM4/abc", "%LPT4/Test/%COM4/abc")] // Mixed match, multiple times
        [InlineData("%LPT2", "%%LPT2")] // Do NOT over encode
        [InlineData("CONContainsCON", "CONContainsCON")] // Do NOT partial match
        [InlineData(@"Test\CON", @"Test\%CON")] // Back slash
        [InlineData("CON.txt", "%CON.txt")] // File with extension
        [InlineData("Test/CON.txt/abc", "Test/%CON.txt/abc")] // File with extension in the middle

        public void ShouldEncode(string input, string expected)
        {
            ReservedFileNameProcessor target = new ReservedFileNameProcessor();
            string actual = target.Encode(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("%CON", "CON")]
        [InlineData("%cON", "cON")] // case insensitive
        [InlineData("test/%COM4", "test/COM4")] // case insensitive
        [InlineData("test/PartCOM4", "test/PartCOM4")] // no decode
        [InlineData("test/%%abcd", "test/%abcd")] // Escape character
        [InlineData("test/%%CON", "test/%CON")] // The value was '%CON', not 'CON', so it encodes to '%%CON', verify it code be correctly decoded.
        [InlineData(@"test\%%CON", @"test\%CON")] // Back slash
        [InlineData("%CON.txt", "CON.txt")] // File with extension
        [InlineData("Test/%CON.txt/abc", "Test/CON.txt/abc")] // File with extension in the middle
        public void ShouldDecodeFullMatch(string input, string expected)
        {
            ReservedFileNameProcessor target = new ReservedFileNameProcessor();
            string actual = target.Decode(input);

            Assert.Equal(expected, actual);
        }
    }
}