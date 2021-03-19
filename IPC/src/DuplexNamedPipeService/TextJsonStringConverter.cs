using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeWithSaar.IPC
{
    internal class TextJsonStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string result;
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    result = reader.GetInt64().ToString(CultureInfo.InvariantCulture);
                    break;
                case JsonTokenType.String:
                    result = reader.GetString();
                    break;
                default:
                    throw new JsonException();
            }
            return result;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteStringValue(value);
    }
}