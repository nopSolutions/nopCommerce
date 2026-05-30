using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product;

/// <summary>
/// Represents request to get all products
/// </summary>
public class GetProductsRequest : ProductApiRequest
{
    /// <summary>
    /// Gets or sets a value indicating whether to sorts products by created date
    /// </summary>
    [JsonIgnore]
    public bool SortByDate { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => $"organizations/self/products/v2?sort={SortByDate.ToString().ToLower()}";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Get;
}