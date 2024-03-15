using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class TrackingItem
{
    [JsonProperty("code")] public string Code { get; set; }
    [JsonProperty("courierTitle")] public string CourierTitle { get; set; }
    [JsonProperty("courierURL")] public string CourierURL { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
}