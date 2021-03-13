using DeepCloneWithSerializer;
using Xunit;

namespace DeepCloneWithSerializerTests
{
    public class Example2IssueTests
    {
        [Fact]
        public void RevealIssue()
        {
            Assert.Equal(0.8, Configuration.Default.CPUTrigger.Rate);
            
            // Start with a default configuration
            Configuration config = Configuration.Default;
            // Needs to update the rate under some condition;
            config.CPUTrigger.Rate = 0.9;

            Assert.Equal(0.9, config.CPUTrigger.Rate);
            // Why default.CPUTrigger.Rate changed?
            Assert.False(Configuration.Default.CPUTrigger.Rate == 0.8);
        }

                [Fact]
        public void ResolveIssue()
        {
            Assert.Equal(0.8, Configuration.GoodDefault.CPUTrigger.Rate);
            
            // Start with a default configuration
            Configuration config = Configuration.GoodDefault;
            // Needs to update the rate under some condition;
            config.CPUTrigger.Rate = 0.9;

            Assert.Equal(0.9, config.CPUTrigger.Rate);
            // Rate won't change alone with cloned item.
            Assert.Equal(0.8, Configuration.GoodDefault.CPUTrigger.Rate);
        }
    }
}