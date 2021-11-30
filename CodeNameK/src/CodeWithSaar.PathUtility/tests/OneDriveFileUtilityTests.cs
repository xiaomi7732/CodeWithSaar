using Xunit;

namespace CodeWithSaar.FileUtilityUnitTests
{
    public class OneDriveFileUtilityUnitTests
    {
        [Theory]
        [InlineData("con", "_con")]
        [InlineData("nul", "_nul")]
        [InlineData("special * char", "special _002A char")]
        [InlineData("*CON", "_002ACON")]
        [InlineData("CON.TXT.con", "_CON.TXT.con")]
        [InlineData(".", "_002E")]
        [InlineData("No end with space ", "No end with space_0020")]
        [InlineData(@"""", @"_0022")]
        public void ShouldHandleSimpleCase(string input, string encoded)
        {
            string origin = input;
            string encodedActual = OneDriveFileUtility.Encode(input);

            Assert.Equal(encoded, encodedActual);

            string decodedActual = OneDriveFileUtility.Decode(encodedActual);
            Assert.Equal(origin, decodedActual);
        }
    }
}