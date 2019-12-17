using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.OrderItems
{
    public class OrderItemsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}