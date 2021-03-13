using System;
using DeepCloneWithSerializer;
using Xunit;

namespace DeepCloneWithSerializerTests
{
    public class UnitTests
    {
        [Fact]
        public void Test1()
        {
            Product product = new Product()
            {
                Id = new IdInfo() { IdNumber = Guid.NewGuid() },
            };

            // Same reference by shallow clone
            Product shallowCloned = product.ShallowClone();
            Assert.Same(product.Id, shallowCloned.Id);

            // Not the same object anymore by deep clone
            Product deepCloned = product.DeepClone();
            Assert.NotSame(product.Id, deepCloned.Id);

            // Update the one on Product, the other changes alone with it.
            product.Id.IdNumber = Guid.NewGuid();
            Assert.Equal(product.Id.IdNumber, shallowCloned.Id.IdNumber);
            // The one on the deep cloned won't change:
            Assert.NotEqual(product.Id.IdNumber, deepCloned.Id.IdNumber);

        }
    }
}
