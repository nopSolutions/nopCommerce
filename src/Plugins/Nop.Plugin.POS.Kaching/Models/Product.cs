using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Product
    {
        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("short_description")]
        public Description ShortDescription { get; set; }

        [JsonProperty("dimensions")]
        public Dimension[] Dimensions { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("name")]
        public Description Name { get; set; }

        [JsonProperty("retail_price")]
        public long RetailPrice { get; set; }

        [JsonProperty("sale_price")]
        public long SalePrice { get; set; }

        [JsonProperty("cost_price")]
        public long CostPrice { get; set; }

        [JsonProperty("tags")]
        public Tags Tags { get; set; }

        [JsonProperty("product_group")]
        public string ProductGroup { get; set; }

        [JsonProperty("variants")]
        public Variant[] Variants { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }
    }
}