using System;
using Newtonsoft.Json;

namespace DI.ConsoleApp
{
    class DogReport
    {
        public void Print(Dog dog)
        {
            string dogJson = JsonConvert.SerializeObject(dog);
            Console.WriteLine(dogJson);
        }
    }
}