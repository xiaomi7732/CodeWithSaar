using Xunit;

namespace CodeWithSaar.FileUtilityUnitTests
{
    public class OneDriveFileUtilityUnitTests
    {
        [Theory]
        [InlineData("con", "%con")]
        [InlineData("nul", "%nul")]
        [InlineData("special * char", "special %002A char")]
        [InlineData("*CON", "%002ACON")]
        [InlineData("CON.TXT.con", "%CON.TXT.con")]
        [InlineData(".", "%002E")]
        [InlineData("No end with space ", "No end with space%0020")]
        [InlineData(@"""", @"%0022")]
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