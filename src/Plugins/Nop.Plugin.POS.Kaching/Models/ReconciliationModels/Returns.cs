using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models.ReconciliationModels
{
    public class Returns
    {
        [JsonProperty("base_price")]
        public long BasePrice { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("item_count")]
        public long ItemCount { get; set; }

        [JsonProperty("sub_total")]
        public long SubTotal { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("total_discounts")]
        public long TotalDiscounts { get; set; }

        [JsonProperty("total_tax_amount")]
        public long TotalTaxAmount { get; set; }
    }
}