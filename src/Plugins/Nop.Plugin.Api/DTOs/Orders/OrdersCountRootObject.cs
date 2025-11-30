using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.Orders
{
    public class OrdersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
