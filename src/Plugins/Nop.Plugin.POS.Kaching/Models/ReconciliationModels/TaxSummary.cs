using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models.ReconciliationModels
{
    public class TaxSummary
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rate")]
        public double Rate { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}