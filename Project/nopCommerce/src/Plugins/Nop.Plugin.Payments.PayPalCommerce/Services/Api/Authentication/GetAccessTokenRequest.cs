using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Authentication;

/// <summary>
/// Represents the request to get an access token
/// </summary>
public class GetAccessTokenRequest : IApiRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the client ID
    /// </summary>
    [JsonIgnore]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret
    /// </summary>
    [JsonIgnore]
    public string Secret { get; set; }

    /// <summary>
    /// Gets or sets the grant type
    /// </summary>
    [JsonProperty(PropertyName = "grant_type")]
    public string GrantType { get; set; }

    /// <summary>
    /// Gets or sets the code
    /// </summary>
    [JsonProperty(PropertyName = "code")]
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the code verifier
    /// </summary>
    [JsonProperty(PropertyName = "code_verifier")]
    public string CodeVerifier { get; set; }

    /// <summary>
    /// Gets or sets the response type
    /// </summary>
    [JsonProperty(PropertyName = "response_type")]
    public string ResponseType { get; set; }

    /// <summary>
    /// Gets or sets the target customer id
    /// </summary>
    [JsonProperty(PropertyName = "target_customer_id")]
    public string TargetCustomerId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => "v1/oauth2/token";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}