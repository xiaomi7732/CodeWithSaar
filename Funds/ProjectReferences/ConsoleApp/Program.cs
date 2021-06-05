using System;
using AnimalSuppliesClassLib;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AnimalProvider provider = new AnimalProvider();
            string animal = provider.GetAnAnimal();

            Console.WriteLine(animal);
        }
    }
}
