using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the product capability details object
/// </summary>
public class ProductCapability
{
    #region Properties

    /// <summary>
    /// Gets or sets the name of the capability.
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the status of the capability.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the array of limitations on the capability.
    /// </summary>
    [JsonProperty(PropertyName = "limits")]
    public List<ProductCapabilityLimit> Limits { get; set; }

    #endregion
}