using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

internal class PagingResponse
{
    [JsonProperty("previous")] public string Previous { get; set; }
    [JsonProperty("next")] public string Next { get; set; }
    [JsonProperty("limit")] public int Limit { get; set; }
}