using System.Collections.Generic;

namespace HelloOptions
{
    class AnimalInfoOptions
    {
        public IEnumerable<string> KnownAnimals { get; set; }

        public IDictionary<string, string> AnimalSpeech { get; set; }
    }
}