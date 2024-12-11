using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.PaymentSources;

/// <summary>
/// Represents the PayPal Wallet to use to fund a payment
/// </summary>
public class PayPal : Payer
{
    #region Properties

    /// <summary>
    /// Gets or sets the account identifier for a PayPal account.
    /// </summary>
    [JsonProperty(PropertyName = "account_id")]
    public string AccountId { get; set; }

    /// <summary>
    /// Gets or sets the payer experience during the approval process for payment with PayPal.
    /// </summary>
    [JsonProperty(PropertyName = "experience_context")]
    public ExperienceContext ExperienceContext { get; set; }

    /// <summary>
    /// Gets or sets the PayPal billing agreement ID. References an approved recurring payment for goods or services.
    /// </summary>
    [JsonProperty(PropertyName = "billing_agreement_id")]
    public string BillingAgreementId { get; set; }

    /// <summary>
    /// Gets or sets the PayPal-generated ID for the saved card payment source. Typically stored on the merchant's server.
    /// </summary>
    [JsonProperty(PropertyName = "vault_id")]
    public string VaultId { get; set; }

    /// <summary>
    /// Gets or sets the additional attributes associated with the use of this wallet.
    /// </summary>
    [JsonProperty(PropertyName = "attributes")]
    public Attributes Attributes { get; set; }

    #endregion
}