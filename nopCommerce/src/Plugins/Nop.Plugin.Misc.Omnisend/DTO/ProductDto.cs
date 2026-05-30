using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class ProductDto : IBatchSupport
{
    [JsonProperty("productID")] public string ProductId { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("currency")] public string Currency { get; set; }
    [JsonProperty("productUrl")] public string ProductUrl { get; set; }
    [JsonProperty("createdAt")] public string CreatedAt { get; set; }
    [JsonProperty("updatedAt")] public string UpdatedAt { get; set; }
    [JsonProperty("categoryIDs")] public List<string> CategoryIDs { get; set; }
    [JsonProperty("variants")] public List<Variant> Variants { get; set; }
    [JsonProperty("imageUrl")] public string ImageUrl => Images.FirstOrDefault()?.Url ?? string.Empty;
    [JsonProperty("images")] public IList<Image> Images { get; set; }

    public class Variant
    {
        [JsonProperty("variantID")] public string VariantId { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("sku")] public string Sku { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("price")] public int Price { get; set; }
    }

    public class Image
    {
        [JsonProperty("imageID")] public string ImageId { get; set; }

        [JsonProperty("url")] public string Url { get; set; }
    }
}