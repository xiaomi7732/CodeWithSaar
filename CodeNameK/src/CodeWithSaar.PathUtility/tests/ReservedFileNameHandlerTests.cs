using Xunit;

namespace CodeWithSaar.FileUtilityUnitTests
{
    public class ReservedFileNameProcessorTests
    {
        [Theory]
        [InlineData("CON", "%CON")] // Simple case
        [InlineData("cOn", "%cOn")] // Case insensitive
        [InlineData("%LPT2", "%%LPT2")] // Encode the escaper, but do NOT over encode
        [InlineData("CONContainsCON", "CONContainsCON")] // Do NOT encode partial match
        [InlineData("CON.con", "%CON.con")] // File with extension
        [InlineData("COM3.com.txt", "%COM3.com.txt")] // With multiple extensions
        public void ShouldEncode(string input, string expected)
        {
            ReservedFileNameProcessor target = new ReservedFileNameProcessor(new ReservedFilenameProcessorOptions() { EscapeEscaper = true });
            string actual = target.Encode(input);
            Assert.Equal(expected, actual);

            string decoded = target.Decode(actual);
            Assert.Equal(input, decoded);
        }
    }
}