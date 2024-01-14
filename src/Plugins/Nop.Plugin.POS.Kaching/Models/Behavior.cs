using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Behavior
    {
        [JsonProperty("shipping")]
        public Shipping Shipping { get; set; }
    }
}