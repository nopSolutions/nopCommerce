using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the seller protection
/// </summary>
public class SellerProtection
{
    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether the transaction is eligible for seller protection.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the array of conditions that are covered for the transaction.
    /// </summary>
    [JsonProperty(PropertyName = "dispute_categories")]
    public List<string> DisputeCategories { get; set; }

    #endregion
}