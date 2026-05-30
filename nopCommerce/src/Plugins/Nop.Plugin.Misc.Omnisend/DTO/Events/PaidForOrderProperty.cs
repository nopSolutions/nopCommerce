using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class PaidForOrderProperty : OrderEventBaseProperty, IEventProperty
{
    [JsonIgnore] public string EventName => CustomerEventType.OrderPaid;
    [JsonIgnore] public string EventVersion => "v2";
}