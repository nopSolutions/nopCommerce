using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class AddedProductToCartProperty : IEventProperty
{
    [JsonProperty("addedItem")] public ProductItem AddedItem { get; set; }
    [JsonIgnore] public string EventName => CustomerEventType.AddedProduct;
    [JsonIgnore] public string EventVersion => string.Empty;

    [JsonProperty("properties")]
    public CartEventProperties Properties { get; set; }
}