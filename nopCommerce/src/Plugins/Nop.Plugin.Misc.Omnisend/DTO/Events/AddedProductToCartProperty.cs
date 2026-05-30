using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class AddedProductToCartProperty : CartEventBaseProperty, IEventProperty
{
    [JsonProperty("addedItem")] public ProductItem AddedItem { get; set; }
    [JsonIgnore] public string EventName => CustomerEventType.AddedProduct;
    [JsonIgnore] public string EventVersion => string.Empty;
}