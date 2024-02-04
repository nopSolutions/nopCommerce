using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.Manufacturers
{
    public class ManufacturersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
