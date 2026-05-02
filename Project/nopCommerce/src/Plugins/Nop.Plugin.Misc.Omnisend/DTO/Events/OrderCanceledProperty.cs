using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class OrderCanceledProperty : OrderEventBaseProperty, IEventProperty
{
    [JsonProperty("cancelReason")] public string CancelReason { get; set; }
    [JsonIgnore] public string EventName => CustomerEventType.OrderCanceled;
    [JsonIgnore] public string EventVersion => "v2";
}