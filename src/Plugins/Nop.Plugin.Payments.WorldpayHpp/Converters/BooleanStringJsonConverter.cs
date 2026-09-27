using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nop.Plugin.Payments.WorldpayHpp.Enums;

namespace Nop.Plugin.Payments.WorldpayHpp.Converters;
public sealed class BooleanStringJsonConverter : JsonConverter<BooleanString>
{
    public override BooleanString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase))
                return BooleanString.True;
            if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase))
                return BooleanString.False;
        }

        // Accept boolean tokens too (be forgiving)
        if (reader.TokenType == JsonTokenType.True)
            return BooleanString.True;
        if (reader.TokenType == JsonTokenType.False)
            return BooleanString.False;

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing BooleanString.");
    }

    public override void Write(Utf8JsonWriter writer, BooleanString value, JsonSerializerOptions options)
    {
        // Always write the lowercase string "true" or "false"
        writer.WriteStringValue(value == BooleanString.True ? "true" : "false");
    }
}
