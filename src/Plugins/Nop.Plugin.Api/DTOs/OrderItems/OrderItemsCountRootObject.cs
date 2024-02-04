using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.OrderItems
{
    public class OrderItemsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
