using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the integration details object
/// </summary>
public class Integration
{
    #region Properties

    /// <summary>
    /// Gets or sets the type of integration between the partner and the seller.
    /// </summary>
    [JsonProperty(PropertyName = "integration_type")]
    public string IntegrationType { get; set; }

    /// <summary>
    /// Gets or sets the integration method that the partner uses to integrate with PayPal APIs.
    /// </summary>
    [JsonProperty(PropertyName = "integration_method")]
    public string IntegrationMethod { get; set; }

    /// <summary>
    /// Gets or sets the integration status.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    #endregion
}