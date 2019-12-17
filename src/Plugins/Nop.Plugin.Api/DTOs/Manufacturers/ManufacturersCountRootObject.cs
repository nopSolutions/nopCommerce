using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Manufacturers
{
    public class ManufacturersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}