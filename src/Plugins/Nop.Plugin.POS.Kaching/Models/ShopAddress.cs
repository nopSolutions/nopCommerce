using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class ShopAddress
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}