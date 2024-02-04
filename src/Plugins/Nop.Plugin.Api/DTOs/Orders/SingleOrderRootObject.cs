using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.Orders
{
    public class SingleOrderRootObject
    {
        [JsonProperty("order")]
        public OrderDto Order { get; set; }
    }
}
