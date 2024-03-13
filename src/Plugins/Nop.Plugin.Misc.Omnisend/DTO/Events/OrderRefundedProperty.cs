using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class OrderRefundedProperty : OrderEventBaseProperty, IEventProperty
{
    [JsonProperty("refundedLineItems")] public IList<ProductItem> RefundedLineItems { get; set; }
    [JsonProperty("totalRefundedAmount")] public float TotalRefundedAmount { get; set; }
    [JsonIgnore] public string EventName => CustomerEventType.OrderRefunded;
    [JsonIgnore] public string EventVersion => "v2";
}