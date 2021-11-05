using Xunit;

namespace CodeWithSaar.FileUtilityTests
{
    public class ReservedCharacterProcessorTests
    {
        [Theory]
        [InlineData("%", "%0025")] // Escaper
        [InlineData("*", "%002A")] // Special character
        [InlineData("**", "%002A%002A")] // Special character x 2
        [InlineData("New Category?", "New Category%003F")] // Mixed special character
        [InlineData("A new file Name/New Category?/New whatever", "A new file Name%002FNew Category%003F%002FNew whatever")] // Multiple match
        public void ShouldEncode(string input, string expected)
        {
            ReservedCharacterProcessor target = new ReservedCharacterProcessor();
            string actual = target.Encode(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("%0025", "%")] // Escaper
        [InlineData("%002A", "*")] // Special character
        [InlineData("%002A%002A", "**")] // Special character x2
        [InlineData("New Category%003F", "New Category?")] // Mixed special character
        [InlineData("A new file Name%002FNew Category%003F%002FNew whatever", "A new file Name/New Category?/New whatever")] // Multiple match
        public void ShouldDecode(string input, string expected)
        {
            ReservedCharacterProcessor target = new ReservedCharacterProcessor();
            string actual = target.Decode(input);
            Assert.Equal(expected, actual);
        }
    }
}