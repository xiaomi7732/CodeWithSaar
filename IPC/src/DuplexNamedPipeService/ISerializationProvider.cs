namespace CodeWithSaar.IPC
{
    public interface ISerializationProvider
    {
        bool TrySerialize<T>(T payload, out string serialized);

        bool TryDeserialze<T>(string serialized, out T payload);
    }
}