using System;
using Xunit;

namespace ComputedProperty.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void GoodExamples()
        {
            Person smith = new Person() { FirstName = "Adam", LastName = "Smith" };
            Assert.Equal("Adam Smith", smith.FullName);

            smith.FirstName = "Josh";
            // Change first name, full name will be updated too.
            Assert.Equal("Josh Smith", smith.FullName);
        }

        [Fact]
        public void Id2ShouldBeAvoid()
        {
            Product p = new Product();
            
            Guid productIdOne = p.Id1;
            Assert.Equal(productIdOne, p.Id1);

            Guid productIdTwo = p.Id2;
            Assert.Equal(productIdTwo, p.Id2); // Normally, we would expect this to pass.
        }
    }
}
