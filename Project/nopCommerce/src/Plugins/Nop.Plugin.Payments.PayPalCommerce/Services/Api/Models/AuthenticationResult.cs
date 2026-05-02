using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the authentication result
/// </summary>
public class AuthenticationResult
{
    #region Properties

    /// <summary>
    /// Gets or sets the liability shift.
    /// </summary>
    [JsonProperty(PropertyName = "liability_shift")]
    public string LiabilityShift { get; set; }

    /// <summary>
    /// Gets or sets the results of 3D Secure Authentication.
    /// </summary>
    [JsonProperty(PropertyName = "three_d_secure")]
    public AuthenticationResult3DS ThreeDSecure { get; set; }

    #endregion
}