using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class KachingDeleteModel
    {
        public static List<string> FromJson(string json) => JsonConvert.DeserializeObject<List<string>>(json, Converter.Settings);

        [JsonProperty("ids")]
        public List<string> Ids { get; set; }
    }
}
