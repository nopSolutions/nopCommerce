using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class BatchRequest
{
    [JsonProperty("method")] public string Method { get; set; } = "POST";
    [JsonProperty("endpoint")] public string Endpoint { get; set; }
    [JsonProperty("items")] public List<IBatchSupport> Items { get; set; } = new();
}