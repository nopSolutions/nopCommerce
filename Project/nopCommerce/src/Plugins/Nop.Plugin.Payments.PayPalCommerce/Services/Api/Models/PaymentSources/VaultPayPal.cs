using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.PaymentSources;

/// <summary>
/// Represents the vaulted PayPal Wallet to use to fund a payment
/// </summary>
public class VaultPayPal : VaultInstruction
{
    #region Properties

    /// <summary>
    /// Gets or sets the shipping address for the Payer.
    /// </summary>
    [JsonProperty(PropertyName = "shipping")]
    public Shipping Shipping { get; set; }

    /// <summary>
    /// Gets or sets the merchant level Recurring Billing plan metadata for the Billing Agreement.
    /// </summary>
    [JsonProperty(PropertyName = "billing_plan")]
    public BillingPlan BillingPlan { get; set; }

    /// <summary>
    /// Gets or sets the Vault creation flow experience for the customers.
    /// </summary>
    [JsonProperty(PropertyName = "experience_context")]
    public ExperienceContext ExperienceContext { get; set; }

    #endregion
}