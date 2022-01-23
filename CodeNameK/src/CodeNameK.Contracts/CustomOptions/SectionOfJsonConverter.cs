using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeNameK.Contracts.CustomOptions
{
    public class SectionOfJsonSerializer<T> : JsonConverter<SectionOf<T>>
    {
        public override SectionOf<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, SectionOf<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            try
            {
                writer.WritePropertyName(value.Name);
                writer.WriteRawValue(JsonSerializer.Serialize(value.Value, options));
            }
            finally
            {
                writer.WriteEndObject();
            }
        }
    }
}