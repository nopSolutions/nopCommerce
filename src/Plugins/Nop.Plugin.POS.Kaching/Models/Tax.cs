using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Tax
    {
        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("rate")]
        public double Rate { get; set; }

        [JsonProperty("tax_name")]
        public string TaxName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}