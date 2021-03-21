using System;
using DI.ConsoleApp;
using Xunit;

namespace DI.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void ShouldOutputCorrect()
        {
            Dog dog = new Dog
            {
                Name = "Dana",
                Breed = "Chihuahua",
                Weight = 10,
            };

            DogReport report = new DogReport();
            MockOutputer o1 = new MockOutputer();
            report.Print(dog, new Serializer1(), o1);
            MockOutputer o2 = new MockOutputer();
            report.Print(dog, new Serializer2(), o2);

            Assert.Equal(o1.OutputValue, o2.OutputValue);
        }

        class MockOutputer : IOutputer
        {
            public string OutputValue { get; private set; }
            public void WriteLine(string value) => OutputValue = value;
        }
    }
}
