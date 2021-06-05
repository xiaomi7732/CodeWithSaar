using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalSuppliesClassLib
{
    public class AnimalProvider
    {
        HashSet<string> animals = new HashSet<string>(){
            "A cat: meow",
            "Dog dog: woof",
            "A cattle: moo"
        };

        public string GetAnAnimal()
        {
            int randomIndex = (new Random()).Next(animals.Count - 1);
            return animals.Skip(randomIndex).First();
        }
    }
}