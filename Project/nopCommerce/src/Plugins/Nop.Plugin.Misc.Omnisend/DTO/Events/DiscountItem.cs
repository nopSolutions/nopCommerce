using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class DiscountItem
{
    [JsonProperty("amount")] public float Amount { get; set; }
    [JsonProperty("code")] public string Code { get; set; }
    [JsonProperty("type")] public string Type { get; set; }
}