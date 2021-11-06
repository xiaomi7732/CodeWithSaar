using Xunit;

namespace CodeWithSaar.FileUtilityUnitTests
{
    public class FileUtilityUnitTests
    {
        [Theory]
        [InlineData("con", "%con")]
        [InlineData("special * char", "special %002A char")]
        [InlineData("*CON", "%002ACON")]
        [InlineData("CON.TXT.con", "%CON.TXT.con")]
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