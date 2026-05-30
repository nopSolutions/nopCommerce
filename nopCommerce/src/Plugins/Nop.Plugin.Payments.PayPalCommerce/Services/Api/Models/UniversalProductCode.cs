using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the Universal Product Code (UPC)
/// </summary>
public class UniversalProductCode
{
    #region Properties

    /// <summary>
    /// Gets or sets the Universal Product Code type.
    /// </summary>
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the UPC product code of the item.
    /// </summary>
    [JsonProperty(PropertyName = "code")]
    public string Code { get; set; }

    #endregion
}