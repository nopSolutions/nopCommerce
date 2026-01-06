// C#
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nop.Plugin.Theme.KungFu.Seeding.Category;

public sealed class JsonBooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.String => bool.TryParse(reader.GetString(), out var strValue) && strValue,
            JsonTokenType.Number => reader.TryGetInt64(out var numValue) && numValue != 0,
            _ => throw new JsonException($"Unsupported token {reader.TokenType} for boolean conversion.")
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
        writer.WriteBooleanValue(value);
}
