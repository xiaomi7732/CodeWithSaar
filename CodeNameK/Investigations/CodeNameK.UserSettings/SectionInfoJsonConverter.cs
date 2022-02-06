using System.Text.Json;
using System.Text.Json.Serialization;

namespace UserSettingsDemo;

public sealed class SectionInfoJsonConverter<T> : JsonConverter<SectionInfo<T>>
{
    public override SectionInfo<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Use IOptions<T> for reading...
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, SectionInfo<T> section, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        try
        {
            writer.WritePropertyName(section.SectionName);
            writer.WriteRawValue(JsonSerializer.Serialize(section.Value, options));
        }
        finally
        {
            writer.WriteEndObject();
        }
    }
}