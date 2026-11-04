using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class StartedCheckoutProperty : IEventProperty
{
    [JsonIgnore] public string EventName => CustomerEventType.StartedCheckout;
    [JsonIgnore] public string EventVersion => string.Empty;
    
    [JsonProperty("properties")]
    public CartEventProperties Properties { get; set; }
}