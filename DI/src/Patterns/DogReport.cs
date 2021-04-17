using System;

namespace DI.ServiceContainerBasics
{
    public class DogReport
    {
        private readonly ISerializer serializer;
        private readonly ScoppedServiceFactory<IOutputter, ConsoleOutputter> outputterFactory;

        public DogReport(ISerializer serializer, ScoppedServiceFactory<IOutputter, ConsoleOutputter> outputterFactory)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.outputterFactory = outputterFactory ?? throw new ArgumentNullException(nameof(outputterFactory));
        }

        public void Print(Dog dog)
        {
            string dogJson = serializer.SerializeObject(dog);
            using IOutputter outputter = outputterFactory.Create();
            outputter.WriteLine(dogJson);
        }
    }
}