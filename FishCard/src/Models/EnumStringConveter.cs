using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeWithSaar.FishCard;

internal class EnumStringConverter<T> : JsonConverter<T>
    where T : notnull
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? stringValue = reader.GetString();
        if (string.IsNullOrEmpty(stringValue))
        {
            throw new InvalidCastException("Can't deserialize enum value of empty.");
        }
        if (Enum.TryParse(typeof(T), stringValue, out object? level) && level is not null)
        {
            Console.WriteLine("Parsed level: {0}", level.ToString());
            return (T)level;
        }
        throw new InvalidCastException($"Can't parse enum value of {stringValue}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}