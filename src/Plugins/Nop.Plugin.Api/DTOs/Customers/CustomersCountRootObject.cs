using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.Customers
{
    public class CustomersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
