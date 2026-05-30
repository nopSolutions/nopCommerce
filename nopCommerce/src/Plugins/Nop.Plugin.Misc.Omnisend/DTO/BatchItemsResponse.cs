using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class BatchItemsResponse : BatchResponse
{
    [JsonProperty("errors")] public IList<BatchItem> Errors { get; set; }
    [JsonProperty("responses")] public IList<BatchItem> Responses { get; set; }
}