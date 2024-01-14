using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class KachingProductModel
    {
        [JsonProperty("product")]
        public Product Product { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        public static KachingProductModel FromJson(string json) => JsonConvert.DeserializeObject<KachingProductModel>(json, Converter.Settings);

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters = {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                }
            };
        }
    }
}