using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.ProductManufacturerMappings
{
    public class ProductManufacturerMappingsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
