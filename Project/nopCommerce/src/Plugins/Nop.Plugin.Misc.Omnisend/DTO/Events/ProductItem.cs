using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class ProductItem
{
    [JsonProperty("productCategories")] public IList<ProductItemCategories> ProductCategories { get; set; }
    [JsonProperty("productDescription")] public string ProductDescription { get; set; }
    [JsonProperty("productDiscount")] public float ProductDiscount { get; set; }
    [JsonProperty("productID")] public int ProductId { get; set; }
    [JsonProperty("productImageURL")] public string ProductImageURL { get; set; }
    [JsonProperty("productPrice")] public float ProductPrice { get; set; }
    [JsonProperty("productQuantity")] public float ProductQuantity { get; set; }
    [JsonProperty("productSKU")] public string ProductSku { get; set; }
    [JsonProperty("productStrikeThroughPrice")] public float ProductStrikeThroughPrice { get; set; }
    [JsonProperty("productTitle")] public string ProductTitle { get; set; }
    [JsonProperty("productURL")] public string ProductURL { get; set; }
    [JsonProperty("productVariantID")] public int ProductVariantId { get; set; }
    [JsonProperty("productVariantImageURL")] public string ProductVariantImageURL { get; set; }

    public class ProductItemCategories
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
    }
}