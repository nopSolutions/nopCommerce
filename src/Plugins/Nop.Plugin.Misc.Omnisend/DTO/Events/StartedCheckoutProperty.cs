using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class StartedCheckoutProperty : CartEventBaseProperty, IEventProperty
{
    [JsonIgnore] public string EventName => CustomerEventType.StartedCheckout;
    [JsonIgnore] public string EventVersion => string.Empty;
}