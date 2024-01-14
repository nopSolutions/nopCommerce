using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class Product
    {
        [JsonProperty("productNumber")]
        public string ProductNumber { get; set; }
    }
}
