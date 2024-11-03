using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the PayPal product details object
/// </summary>
public class PayPalProduct
{
    #region Properties

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the vetting status of the product, if applicable.
    /// </summary>
    [JsonProperty(PropertyName = "vetting_status")]
    public string VettingStatus { get; set; }

    /// <summary>
    /// Gets or sets the status of the product.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the array of the capability names bundled in the parent product.
    /// </summary>
    [JsonProperty(PropertyName = "capabilities")]
    public List<string> Capabilities { get; set; }

    #endregion
}