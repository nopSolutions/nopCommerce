using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class Line
    {
        [JsonProperty("lineNumber")]
        public int LineNumber { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }        

        [JsonProperty("sortKey")]
        public int SortKey { get; set; }

        [JsonProperty("unit")]
        public Unit Unit { get; set; }

        [JsonProperty("product")]
        public Product Product { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        [JsonProperty("unitNetPrice")]
        public decimal UnitNetPrice { get; set; }

        [JsonProperty("discountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [JsonProperty("unitCostPrice")]
        public decimal UnitCostPrice { get; set; }

        [JsonProperty("totalNetAmount")]
        public decimal TotalNetAmount { get; set; }

        [JsonProperty("marginInBaseCurrency")]
        public decimal MarginInBaseCurrency { get; set; }

        [JsonProperty("marginPercentage")]
        public decimal MarginPercentage { get; set; }
    }


}
