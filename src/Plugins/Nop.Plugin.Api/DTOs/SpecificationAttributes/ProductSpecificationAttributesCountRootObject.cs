using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.SpecificationAttributes
{
    public class ProductSpecificationAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}