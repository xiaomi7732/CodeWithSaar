using System;

namespace ComputedProperty
{
    public class Product
    {
        public Guid Id1 { get; } = Guid.NewGuid();
        public Guid Id2 => Guid.NewGuid();
    }
}
