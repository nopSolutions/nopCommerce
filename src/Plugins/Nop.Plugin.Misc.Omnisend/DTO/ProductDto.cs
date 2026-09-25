using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class ProductDto : IBatchSupport
{
    [JsonProperty("id")] public string ProductId { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("currency")] public string Currency { get; set; }
    [JsonProperty("url")] public string ProductUrl { get; set; }
    [JsonProperty("createdAt")] public string CreatedAt { get; set; }
    [JsonProperty("updatedAt")] public string UpdatedAt { get; set; }
    [JsonProperty("categoryIDs")] public List<string> CategoryIDs { get; set; }
    [JsonProperty("variants")] public List<Variant> Variants { get; set; }
    [JsonProperty("images")] public IList<string> Images { get; set; }

    public class Variant
    {
        [JsonProperty("id")] public string VariantId { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("sku")] public string Sku { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("price")] public float Price { get; set; }
        [JsonProperty("url")] public string ProductUrl { get; set; }
    }
}