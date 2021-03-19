namespace CodeWithSaar.IPC
{
    public interface ISerializationProvider
    {
        bool TrySerialize<T>(T payload, out string serialized);

        bool TryDeserialize<T>(string serialized, out T payload);
    }
}