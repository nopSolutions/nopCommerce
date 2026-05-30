using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class BatchItem
{
    [JsonProperty("itemID")] public string ItemId { get; set; }
    [JsonProperty("request")] public object Request { get; set; }
    [JsonProperty("responseCode")] public int ResponseCode { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
}