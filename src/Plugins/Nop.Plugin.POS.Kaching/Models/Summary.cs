using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Summary
    {
        [JsonProperty("base_price")]
        public long BasePrice { get; set; }

        [JsonProperty("sales_tax_amount")]
        public long SalesTaxAmount { get; set; }

        [JsonProperty("sub_total")]
        public long SubTotal { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("total_discounts")]
        public long TotalDiscounts { get; set; }

        [JsonProperty("total_tax_amount")]
        public long TotalTaxAmount { get; set; }

        [JsonProperty("vat_amount")]
        public long VatAmount { get; set; }

        [JsonProperty("line_items")]
        public LineItem[] LineItems { get; set; }

        [JsonProperty("taxes")]
        public Tax[] Taxes { get; set; }

        [JsonProperty("is_return")]
        public bool IsReturn { get; set; }

        [JsonProperty("margin")]
        public long Margin { get; set; }

        [JsonProperty("margin_total")]
        public long MarginTotal { get; set; }
    }
}