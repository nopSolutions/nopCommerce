using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Products
{
    public class ProductsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}