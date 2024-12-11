using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the additional attributes associated with a payment source
/// </summary>
public class Attributes
{
    #region Properties

    /// <summary>
    /// Gets or sets the details about a customer in PayPal's system of record.
    /// </summary>
    [JsonProperty(PropertyName = "customer")]
    public Payer Customer { get; set; }

    /// <summary>
    /// Gets or sets the instruction to vault the payment source based on the specified strategy.
    /// </summary>
    [JsonProperty(PropertyName = "vault")]
    public Vault Vault { get; set; }

    /// <summary>
    /// Gets or sets the instruction to optionally verify the payment source based on the specified strategy.
    /// </summary>
    [JsonProperty(PropertyName = "verification")]
    public VerificationInstruction Verification { get; set; }

    #endregion
}