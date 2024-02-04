using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.ProductCategoryMappings
{
    public class ProductCategoryMappingsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
