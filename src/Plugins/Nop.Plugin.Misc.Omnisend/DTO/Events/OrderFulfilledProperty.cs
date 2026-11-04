using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class OrderFulfilledProperty : IEventProperty
{
    [JsonIgnore] public string EventName => CustomerEventType.OrderFulfilled;
    [JsonIgnore] public string EventVersion => "v2";
    [JsonProperty("properties")] public OrderEventProperties Properties { get; set; } = new();
}