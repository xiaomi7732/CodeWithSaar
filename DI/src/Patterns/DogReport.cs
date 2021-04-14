using System;

namespace DI.ServiceContainerBasics
{
    public class DogReport
    {
        private readonly ISerializer _serializer;
        private readonly ConsoleOutputterFactory _outputterFactory;

        public DogReport(ISerializer serializer, ConsoleOutputterFactory outputterFactory)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _outputterFactory = outputterFactory ?? throw new ArgumentNullException(nameof(outputterFactory));
        }

        public void Print(Dog dog)
        {
            string dogJson = _serializer.SerializeObject(dog);
            using (IOutputter outputter = _outputterFactory.Create())
            {
                outputter.WriteLine(dogJson);
            }
        }
    }
}