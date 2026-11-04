using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class CartEventProperties
{
    [JsonProperty("abandonedCheckoutURL")] public string AbandonedCheckoutUrl { get; set; }
    [JsonProperty("cartID")] public string CartId { get; set; }
    [JsonProperty("currency")] public string Currency { get; set; }
    [JsonProperty("lineItems")] public IList<ProductItem> LineItems { get; set; }
    [JsonProperty("value")] public float Value { get; set; }
}