using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class OrderCanceledProperty : IEventProperty
{
    [JsonProperty("cancelReason")] public string CancelReason { get; set; }
    [JsonIgnore] public string EventName => CustomerEventType.OrderCanceled;
    [JsonIgnore] public string EventVersion => "v2";
    [JsonProperty("properties")] public OrderEventProperties Properties { get; set; } = new();
}