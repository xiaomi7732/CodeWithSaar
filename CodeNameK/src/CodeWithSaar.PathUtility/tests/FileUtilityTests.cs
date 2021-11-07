using Xunit;

namespace CodeWithSaar.FileUtilityUnitTests
{
    public class FileUtilityUnitTests
    {
        [Theory]
        [InlineData("con", "%con")]
        [InlineData("nul", "%nul")]
        [InlineData("special * char", "special %002A char")]
        [InlineData("*CON", "%002ACON")]
        [InlineData("CON.TXT.con", "%CON.TXT.con")]
        [InlineData(".", "%002E")]
        [InlineData("No end with space ", "No end with space%0020")]
        public void ShouldHandleSimpleCase(string input, string encoded)
        {
            string origin = input;
            string encodedActual = FileUtility.Encode(input);

            Assert.Equal(encoded, encodedActual);

            string decodedActual = FileUtility.Decode(encodedActual);
            Assert.Equal(origin, decodedActual);
        }
    }
}