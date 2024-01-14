using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models.ReconciliationModels
{
    public class Totals
    {
        [JsonProperty("base_currency_total")]
        public double BaseCurrencyTotal { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }
    }
}