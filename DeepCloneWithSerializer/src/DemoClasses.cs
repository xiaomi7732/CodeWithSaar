// Usually: One class one file.
using System;
using System.Text.Json;

namespace DeepCloneWithSerializer
{
    public class IdInfo
    {
        public Guid IdNumber { get; set; }
    }

    public class Product
    {
        public IdInfo Id { get; set; }

        public Product ShallowClone()
            => (Product)this.MemberwiseClone();

        public Product DeepClone()
            => JsonSerializer.Deserialize<Product>(JsonSerializer.Serialize(this, this.GetType()));
    }
}
