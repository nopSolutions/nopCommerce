using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Nop.Plugin.Payments.WorldpayHpp.Enums;

namespace Nop.Plugin.Payments.WorldpayHpp.Converters;
public sealed class GooglePayButtonLabelJsonConverter : JsonConverter<GooglePayButtonLabel>
{
    public override GooglePayButtonLabel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing GooglePayButtonLabel.");

        var s = reader.GetString()?.Trim();
        if (string.IsNullOrEmpty(s))
            throw new JsonException("Empty value for GooglePayButtonLabel.");

        if (string.Equals(s, "long", StringComparison.OrdinalIgnoreCase))
            return GooglePayButtonLabel.Long;

        if (string.Equals(s, "short", StringComparison.OrdinalIgnoreCase))
            return GooglePayButtonLabel.Short;

        // Fallback: try to parse enum names (case-insensitive)
        if (Enum.TryParse<GooglePayButtonLabel>(s, ignoreCase: true, out var parsed))
            return parsed;

        throw new JsonException($"Unknown GooglePayButtonLabel value '{s}'.");
    }

    public override void Write(Utf8JsonWriter writer, GooglePayButtonLabel value, JsonSerializerOptions options)
    {
        // Explicit mapping to guarantee lowercase output
        var result = value switch
        {
            GooglePayButtonLabel.Long => "long",
            GooglePayButtonLabel.Short => "short",
            _ => value.ToString().ToLowerInvariant()
        };

        writer.WriteStringValue(result);
    }
}