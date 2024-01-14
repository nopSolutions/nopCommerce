using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models.ReconciliationModels
{
    public class All
    {
        [JsonProperty("base_price")]
        public double BasePrice { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("item_count")]
        public long ItemCount { get; set; }

        [JsonProperty("payments")]
        public Payment[] Payments { get; set; }

        [JsonProperty("sub_total")]
        public double SubTotal { get; set; }

        [JsonProperty("tax_summaries")]
        public TaxSummary[] TaxSummaries { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("total_discounts")]
        public long TotalDiscounts { get; set; }

        [JsonProperty("total_tax_amount")]
        public double TotalTaxAmount { get; set; }

        [JsonProperty("vat_amount")]
        public double VatAmount { get; set; }
    }
}