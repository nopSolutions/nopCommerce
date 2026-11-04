using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class OrderRefundedProperty : IEventProperty
{
    [JsonProperty("refundedLineItems")] public IList<ProductItem> RefundedLineItems { get; set; }
    [JsonProperty("totalRefundedAmount")] public float TotalRefundedAmount { get; set; }
    [JsonIgnore] public string EventName => CustomerEventType.OrderRefunded;
    [JsonIgnore] public string EventVersion => "v2";
    [JsonProperty("properties")] public OrderEventProperties Properties { get; set; } = new();
}