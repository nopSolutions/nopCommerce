using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Onboarding;

/// <summary>
/// Represents the request to get the REST API application credentials
/// </summary>
public class GetCredentialsRequest : IApiRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID
    /// </summary>
    [JsonIgnore]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the access token
    /// </summary>
    [JsonIgnore]
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v1/customer/partners/{Uri.EscapeDataString(Id)}/merchant-integrations/credentials?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Get;

    #endregion
}