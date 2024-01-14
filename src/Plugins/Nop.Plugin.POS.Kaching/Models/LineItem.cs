using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class LineItem
    {
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("name")]
        public NameUnion Name { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("retail_price")]
        public long RetailPrice { get; set; }

        [JsonProperty("sales_tax_amount")]
        public long SalesTaxAmount { get; set; }

        [JsonProperty("sub_total")]
        public long SubTotal { get; set; }

        [JsonProperty("base_price")]
        public long BasePrice { get; set; }

        [JsonProperty("taxes")]
        public Tax[] Taxes { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("total_tax_amount")]
        public long TotalTaxAmount { get; set; }

        [JsonProperty("vat_amount")]
        public long VatAmount { get; set; }
        
        [JsonProperty("ecom_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EcomId { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("line_item_id")]
        public string LineItemId { get; set; }

        [JsonProperty("variant_id", NullValueHandling = NullValueHandling.Ignore)]
        public string VariantId { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Tags { get; set; }

        [JsonProperty("behavior", NullValueHandling = NullValueHandling.Ignore)]
        public Behavior Behavior { get; set; }

        [JsonProperty("cost_price", NullValueHandling = NullValueHandling.Ignore)]
        public long? CostPrice { get; set; }

        [JsonProperty("margin", NullValueHandling = NullValueHandling.Ignore)]
        public long? Margin { get; set; }

        [JsonProperty("margin_total", NullValueHandling = NullValueHandling.Ignore)]
        public long? MarginTotal { get; set; }
    }
}