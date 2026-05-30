using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api;

/// <summary>
/// Represents the identity error response
/// </summary>
public class IdentityErrorResponse : IApiResponse
{
    #region Properties

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    [JsonProperty(PropertyName = "error")]
    public string Error { get; set; }

    /// <summary>
    /// Gets or sets the error description
    /// </summary>
    [JsonProperty(PropertyName = "error_description")]
    public string ErrorDescription { get; set; }

    #endregion
}