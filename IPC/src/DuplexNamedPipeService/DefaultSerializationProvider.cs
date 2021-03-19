using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeWithSaar.IPC
{
    internal class DefaultSerializationProvider : ISerializationProvider
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {
                new JsonStringEnumConverter(),
                new TextJsonStringConverter(),
            },
            
        };

        public bool TryDeserialize<T>(string serialized, out T payload)
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
                serialized = JsonSerializer.Serialize<T>(payload, _options);
                return true;
            }
            catch (Exception ex) when (ex is JsonException || ex is ArgumentException || ex is NotSupportedException)
            {
                return false;
            }
        }
    }
}