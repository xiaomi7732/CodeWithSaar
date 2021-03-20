using System;

namespace DI.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Dog dogBella = new Dog(){
                Name = "Bella",
                Breed = "Poodle",
                Weight = 20.5
            };

            DogReport report = new DogReport();
            report.Print(dogBella);
        }
    }
}
