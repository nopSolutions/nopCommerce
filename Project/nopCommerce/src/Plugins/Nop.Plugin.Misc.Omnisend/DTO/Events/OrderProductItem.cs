using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class OrderProductItem : ProductItem
{
    [JsonProperty("productTags")] public IList<string> ProductTags { get; set; }
    [JsonProperty("productVendor")] public string ProductVendor { get; set; }
    [JsonProperty("productWeight")] public int ProductWeight { get; set; }
}