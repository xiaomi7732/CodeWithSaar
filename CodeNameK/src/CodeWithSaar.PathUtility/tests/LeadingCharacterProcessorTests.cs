using Xunit;

namespace CodeWithSaar.FileUtilityUnitTests
{
    public class LeadingCharacterProcessorTests
    {
        [Theory]
        [InlineData("~", "%007E")] // Can't start with ~
        [InlineData("~~", "%007E~")] // Only first ~ will be encoded
        [InlineData("$", "%0024")] // Can't start with $
        [InlineData("002E", "002E")] // Expect to do nothing
        [InlineData("This~is~$InTheMiddleOrTail$", "This~is~$InTheMiddleOrTail$")] // Space in the middle, period on tail.
        [InlineData("~WhatIfItIsLeading?", "%007EWhatIfItIsLeading?")] // Even it is invalid, shall not encode
        [InlineData("EscapeTheEscaper%", "EscapeTheEscaper%%")]
        public void ShouldEncodeDecode(string input, string expected)
        {
            RunTest(input, expected);
        }

        [Fact]
        public void DoNotEscapeEscaperWhenSpecified()
        {
            // Do not escape the escaper
            RunTest("EscapeTheEscaper%", "EscapeTheEscaper%", escapeTheEscaper: false);
        }

        private void RunTest(string input, string expected, bool escapeTheEscaper = true)
        {
            LeadingCharacterProcessor target = new LeadingCharacterProcessor(
                new LeadingCharacterProcessorOptions()
                {
                    EscapeEscaper = escapeTheEscaper,
                });
            string actual = target.Encode(input);
            Assert.Equal(expected, actual);

            string decoded = target.Decode(actual);
            Assert.Equal(input, decoded);
        }

    }
}