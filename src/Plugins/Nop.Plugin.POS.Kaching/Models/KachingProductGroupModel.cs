using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class KachingProductGroupModel
    {
        public static KachingProductModel FromJson(string json) => JsonConvert.DeserializeObject<KachingProductModel>(json, Converter.Settings);

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("name")]
        public Description Name { get; set; }
    }
}