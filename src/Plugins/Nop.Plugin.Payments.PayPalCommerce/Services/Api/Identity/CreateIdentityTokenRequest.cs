using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Identity;

/// <summary>
/// Represents the request to create identity token
/// </summary>
public class CreateIdentityTokenRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the unique ID for a customer in merchant's system of records
    /// </summary>
    [JsonProperty(PropertyName = "customer_id")]
    public string CustomerId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => "v1/identity/generate-token?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}