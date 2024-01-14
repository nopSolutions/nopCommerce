using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models.ReconciliationModels
{
    public class Payment
    {
        [JsonProperty("totals")]
        public Totals Totals { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}