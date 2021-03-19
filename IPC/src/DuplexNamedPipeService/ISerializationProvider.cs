namespace CodeWithSaar.IPC
{
    /// <summary>
    /// An extention point to allow different serializers.
    /// Check the unit test of 'DefaultSerializationProviderTests' to see the requirement for implementations.
    /// </summary>
    public interface ISerializationProvider
    {
        bool TrySerialize<T>(T payload, out string serialized);

        bool TryDeserialize<T>(string serialized, out T payload);
    }
}