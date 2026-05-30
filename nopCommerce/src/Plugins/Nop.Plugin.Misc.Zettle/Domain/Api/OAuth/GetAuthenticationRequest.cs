using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.OAuth;

/// <summary>
/// Represents request to get access token
/// </summary>
public class GetAuthenticationRequest : OAuthApiRequest
{
    /// <summary>
    /// Gets or sets the JWT assertion to authenticate
    /// </summary>
    [JsonIgnore]
    public string Assertion { get; set; }

    /// <summary>
    /// Gets or sets the partner client ID
    /// </summary>
    [JsonIgnore]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the grant type
    /// </summary>
    [JsonIgnore]
    public string GrantType { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => "token";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Post;
}