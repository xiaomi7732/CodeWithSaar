using DeepCloneWithSerializer;
using Xunit;

namespace DeepCloneWithSerializerTests
{
    public class UnitTests
    {
        [Fact]
        public void Test1()
        {
            Person person = new Person()
            {
                Id = new IdInfo() { IdNumber = 6365 },
            };

            // Same reference by shallow clone
            Person shallowCloned = person.ShallowClone();
            Assert.Same(person.Id, shallowCloned.Id);

            // Not the same object anymore by deep clone
            Person deepCloned = person.DeepClone();
            Assert.NotSame(person.Id, deepCloned.Id);

            // Update the one on Product, the other changes alone with it.
            person.Id.IdNumber = 6400;
            Assert.Equal(person.Id.IdNumber, shallowCloned.Id.IdNumber);
            // The one on the deep cloned won't change:
            Assert.NotEqual(person.Id.IdNumber, deepCloned.Id.IdNumber);

        }
    }
}
