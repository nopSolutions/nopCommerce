using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class CartItemDto : ProductItemDto
{
    [JsonProperty("cartProductID")] public string CartProductId { get; set; }

    [JsonProperty("currency")] public string Currency { get; set; }
}