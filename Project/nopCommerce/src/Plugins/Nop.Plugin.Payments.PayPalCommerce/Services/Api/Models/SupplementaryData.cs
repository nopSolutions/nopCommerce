using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the supplementary data
/// </summary>
public class SupplementaryData
{
    #region Properties

    /// <summary>
    /// Gets or sets the Level 2 and 3 data added to payments to reduce risk and payment processing costs.
    /// </summary>
    [JsonProperty(PropertyName = "card")]
    public CardData Card { get; set; }

    #endregion
}