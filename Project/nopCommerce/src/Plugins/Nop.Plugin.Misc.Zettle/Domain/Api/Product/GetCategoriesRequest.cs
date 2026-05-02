using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product;

/// <summary>
/// Represents request to get all categories
/// </summary>
public class GetCategoriesRequest : ProductApiRequest
{
    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => "organizations/self/categories/v2";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Get;
}