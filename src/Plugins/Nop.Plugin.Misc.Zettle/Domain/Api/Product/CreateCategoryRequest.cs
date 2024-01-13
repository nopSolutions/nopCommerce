using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product;

/// <summary>
/// Represents request to create category
/// </summary>
public class CreateCategoryRequest : ProductApiRequest
{
    /// <summary>
    /// Gets or sets the categories
    /// </summary>
    [JsonProperty(PropertyName = "categories")]
    public List<Product.ProductCategory> Categories { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public override string Path => "organizations/self/categories/v2";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public override string Method => HttpMethods.Post;
}