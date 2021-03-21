using System;
using Newtonsoft.Json;

namespace DI.ConsoleApp
{
    public class DogReport
    {
        public void Print(Dog dog, ISerializer serializer, IOutputer outputer)
        {
            string dogJson = serializer.SerializeObject(dog);
            outputer.WriteLine(dogJson);
        }
    }
}