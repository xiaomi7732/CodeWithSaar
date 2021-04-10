using Newtonsoft.Json;

namespace DI.ServiceContainerBasics
{
    public interface ISerializer
    {
        string SerializeObject<T>(T obj);
    }

    public class Serializer1 : ISerializer
    {
        public string SerializeObject<T>(T obj) => JsonConvert.SerializeObject(obj);
    }

    public class Serializer2 : ISerializer
    {
        public string SerializeObject<T>(T obj) => System.Text.Json.JsonSerializer.Serialize(obj);
    }
}