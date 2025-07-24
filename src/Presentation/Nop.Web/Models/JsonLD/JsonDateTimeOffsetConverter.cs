using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nop.Web.Models.JsonLD;

public class JsonDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTimeOffset.Parse(reader.GetString());

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString("o"));
}
