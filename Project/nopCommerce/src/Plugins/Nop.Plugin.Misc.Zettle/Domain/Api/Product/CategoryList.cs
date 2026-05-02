using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product;

/// <summary>
/// Represents categories details
/// </summary>
public class CategoryList : ApiResponse
{
    /// <summary>
    /// Gets or sets a list of all categories
    /// </summary>
    [JsonProperty(PropertyName = "categories")]
    public List<Product.ProductCategory> Categories { get; set; }
}