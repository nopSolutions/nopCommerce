using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class PlacedOrderProperty : OrderEventBaseProperty, IEventProperty
{
    [JsonIgnore] public string EventName => CustomerEventType.PlacedOrder;
    [JsonIgnore] public string EventVersion => "v2";
}