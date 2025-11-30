using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.ProductAttributes
{
    public class ProductAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
