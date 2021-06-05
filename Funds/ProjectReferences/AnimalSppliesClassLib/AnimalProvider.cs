using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalSuppliesClassLib
{
    public class AnimalProvider
    {
        private static readonly HashSet<string> _animals = new HashSet<string>(){
            "A cat: meow",
            "Dog dog: woof",
            "A cattle: moo",
            "A lion: roar",
        };

        public string GetAnAnimal()
        {
            int randomIndex = GetRandomAnimalIndex();
            return _animals.Skip(randomIndex).First();
        }

        private int GetRandomAnimalIndex() => (new Random()).Next(_animals.Count - 1);
    }
}