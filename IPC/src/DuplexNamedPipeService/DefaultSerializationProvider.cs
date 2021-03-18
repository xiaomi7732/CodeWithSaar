using System;
using System.Text.Json;

namespace CodeWithSaar.IPC
{
    internal class DefaultSerializationProvider : ISerializationProvider
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
        };

        public bool TryDeserialze<T>(string serialized, out T payload)
        {
            payload = default;
            if (string.IsNullOrEmpty(serialized))
            {
                return false;
            }

            try
            {
                payload = JsonSerializer.Deserialize<T>(serialized, _options);
                return true;
            }
            catch (Exception ex) when (ex is JsonException || ex is ArgumentException || ex is NotSupportedException)
            {
                return false;
            }
        }

        public bool TrySerialize<T>(T payload, out string serialized)
        {
            serialized = null;
            if (payload is null)
            {
                return false;
            }

            try
            {
                serialized = JsonSerializer.Serialize<T>(payload);
                return true;
            }
            catch (Exception ex) when (ex is JsonException || ex is ArgumentException || ex is NotSupportedException)
            {
                return false;
            }
        }
    }
}