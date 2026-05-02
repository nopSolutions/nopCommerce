using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class OrderFulfilledProperty : OrderEventBaseProperty, IEventProperty
{
    [JsonIgnore] public string EventName => CustomerEventType.OrderFulfilled;
    [JsonIgnore] public string EventVersion => "v2";
}