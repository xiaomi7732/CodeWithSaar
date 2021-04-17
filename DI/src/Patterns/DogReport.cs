using System;

namespace DI.ServiceContainerBasics
{
    public class DogReport
    {
        private readonly ISerializer serializer;
        private readonly ConsoleOutputter outputter;

        public DogReport(ISerializer serializer, ConsoleOutputter outputter)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.outputter = outputter ?? throw new ArgumentNullException(nameof(outputter));
        }

        public void Print(Dog dog)
        {
            string dogJson = serializer.SerializeObject(dog);
            outputter.WriteLine(dogJson);
        }
    }
}