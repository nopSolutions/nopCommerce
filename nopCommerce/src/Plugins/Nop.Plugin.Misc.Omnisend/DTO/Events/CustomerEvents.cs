using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class CustomerEvents
{
    [JsonProperty("eventName")] public string EventName => Properties.EventName;
    [JsonProperty("origin")] public string Origin { get; set; } = OmnisendDefaults.IntegrationOrigin;
    [JsonProperty("eventVersion")] public string EventVersion => Properties.EventVersion;
    [JsonProperty("contact")] public IDictionary<string, string> Contact { get; } = new Dictionary<string, string>();
    [JsonProperty("properties")] public IEventProperty Properties { get; set; }
    [JsonIgnore] public string Email { set => Contact["email"] = value; }
}