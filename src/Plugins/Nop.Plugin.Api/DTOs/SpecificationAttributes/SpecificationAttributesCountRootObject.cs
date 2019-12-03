using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.SpecificationAttributes
{
    public class SpecificationAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}