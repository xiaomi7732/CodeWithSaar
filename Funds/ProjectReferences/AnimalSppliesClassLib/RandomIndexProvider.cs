using System;
using System.Collections.Generic;

namespace AnimalSuppliesClassLib
{
    class RandomIndexProvider
    {
        public int GetRandomIndex<T>(ICollection<T> collection)
        {
            return (new Random()).Next(collection.Count);
        }
    }
}