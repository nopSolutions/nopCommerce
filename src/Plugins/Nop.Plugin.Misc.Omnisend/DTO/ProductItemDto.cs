using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public abstract class ProductItemDto
{
    [JsonProperty("productID")] public string ProductId { get; set; }
    [JsonProperty("sku")] public string Sku { get; set; }
    [JsonProperty("variantID")] public string VariantId { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("quantity")] public int Quantity { get; set; }
    [JsonProperty("price")] public int Price { get; set; }
}