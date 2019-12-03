using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Customers
{
    public class CustomersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; } 
    }
}