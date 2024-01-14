using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Nop.Plugin.POS.Kaching.Models
{
    public partial class Sale
    {
        public static Dictionary<string, Sale> FromJson(string json) => JsonConvert.DeserializeObject<Dictionary<string, Sale>>(json, Converter.Settings);

        [JsonProperty("base_currency")]
        public string BaseCurrency { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("sequence_number")]
        public long SequenceNumber { get; set; }

        [JsonProperty("source")]
        public Source Source { get; set; }

        [JsonProperty("payments")]
        public Payment[] Payments { get; set; }

        [JsonProperty("summary")]
        public Summary Summary { get; set; }

        [JsonProperty("timing")]
        public Timing Timing { get; set; }

        [JsonProperty("base_currency_code")]
        public string BaseCurrencyCode { get; set; }

        [JsonProperty("receipt_metadata")]
        public ReceiptMetadata ReceiptMetadata { get; set; }

        [JsonProperty("response_code")]
        public ReceiptMetadata ResponseCode { get; set; }        
    }

    public static class Serialize
    {
        public static string ToJson(this Sale self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new NameUnionConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class NameUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(NameUnion) || t == typeof(NameUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new NameUnion { String = stringValue };
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<Description>(reader);
                    return new NameUnion { NameClass = objectValue };
            }
            throw new Exception("Cannot unmarshal type NameUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (NameUnion)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String); return;
            }
            if (value.NameClass != null)
            {
                serializer.Serialize(writer, value.NameClass); return;
            }
            throw new Exception("Cannot marshal type NameUnion");
        }
    }
}