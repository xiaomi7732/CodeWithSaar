// Usually: One class one file.
// Refer to MS Doc: https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone?view=net-5.0
using System.Text.Json;

namespace DeepCloneWithSerializer
{
    public class IdInfo
    {
        public int IdNumber { get; set; }
    }

    public class Person
    {
        public IdInfo Id { get; set; }

        public Person ShallowClone()
            => (Person)this.MemberwiseClone();

        public Person DeepClone()
            => JsonSerializer.Deserialize<Person>(JsonSerializer.Serialize(this, this.GetType()));
    }
}
