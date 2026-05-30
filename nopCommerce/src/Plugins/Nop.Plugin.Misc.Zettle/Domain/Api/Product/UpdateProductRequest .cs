using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product;

/// <summary>
/// Represents request to update the product
/// </summary>
public class UpdateProductRequest : Product, IApiRequest, IAuthorizedRequest, IConditionalRequest
{
    /// <summary>
    /// Gets the request base URL
    /// </summary>
    [JsonIgnore]
    public string BaseUrl => "https://products.izettle.com/";

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"organizations/self/products/v2/{Uuid}";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Put;
}