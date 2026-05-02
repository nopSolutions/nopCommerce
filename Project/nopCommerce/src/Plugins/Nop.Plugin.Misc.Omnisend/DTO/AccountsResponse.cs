using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class AccountsResponse
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("brandID")] public string BrandId { get; set; }
}