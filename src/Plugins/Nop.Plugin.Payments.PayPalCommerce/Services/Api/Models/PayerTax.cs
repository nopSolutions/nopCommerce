using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the payer tax information
/// </summary>
public class PayerTax
{
    #region Properties

    /// <summary>
    /// Gets or sets the customer's tax ID value.
    /// </summary>
    [JsonProperty(PropertyName = "tax_id")]
    public string TaxId { get; set; }

    /// <summary>
    /// Gets or sets the customer's tax ID type.
    /// </summary>
    [JsonProperty(PropertyName = "tax_id_type")]
    public string TaxIdType { get; set; }

    #endregion
}