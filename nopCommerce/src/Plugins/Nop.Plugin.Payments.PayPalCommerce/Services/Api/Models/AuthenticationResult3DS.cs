using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the results of 3D Secure Authentication
/// </summary>
public class AuthenticationResult3DS
{
    #region Properties

    /// <summary>
    /// Gets or sets the outcome of the issuer's authentication.
    /// </summary>
    [JsonProperty(PropertyName = "authentication_status")]
    public string AuthenticationStatus { get; set; }

    /// <summary>
    /// Gets or sets the status of authentication eligibility.
    /// </summary>
    [JsonProperty(PropertyName = "enrollment_status")]
    public string EnrollmentStatus { get; set; }

    #endregion
}