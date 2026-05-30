using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class CartDto : IBatchSupport
{
    [JsonProperty("cartID")] public string CartId { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
    [JsonProperty("currency")] public string Currency { get; set; }
    [JsonProperty("cartSum")] public int CartSum { get; set; }
    [JsonProperty("products")] public IList<CartItemDto> Products { get; set; }
    [JsonProperty("cartRecoveryUrl")] public string CartRecoveryUrl { get; set; }
}