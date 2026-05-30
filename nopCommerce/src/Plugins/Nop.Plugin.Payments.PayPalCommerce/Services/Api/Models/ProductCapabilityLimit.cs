using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the product capability limit details object
/// </summary>
public class ProductCapabilityLimit
{
    #region Properties

    /// <summary>
    /// Gets or sets the type of the limit.
    /// </summary>
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    #endregion
}