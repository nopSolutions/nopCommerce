using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the additional payment instructions
/// </summary>
public class PaymentInstruction
{
    #region Properties

    /// <summary>
    /// Gets or sets the array of various fees, commissions, tips, or donations. This field is only applicable to merchants that been enabled for PayPal Commerce Platform for Marketplaces and Platforms capability.
    /// </summary>
    [JsonProperty(PropertyName = "platform_fees")]
    public List<PlatformFee> PlatformFees { get; set; }

    /// <summary>
    /// Gets or sets the specific pricing rate/plan for a payment transaction. This field is only enabled for selected merchants/partners to use. The list of eligible 'payee_pricing_tier_id' would be provided to you by your Account Manager. Specifying values other than the one provided to you by your account manager would result in an error.
    /// </summary>
    [JsonProperty(PropertyName = "payee_pricing_tier_id")]
    public string PayeePricingTierId { get; set; }

    /// <summary>
    /// Gets or sets the identifier generated returned by PayPal to be used for payment processing in order to honor FX rate (for eligible integrations) to be used when amount is settled/received into the payee account.
    /// </summary>
    [JsonProperty(PropertyName = "payee_receivable_fx_rate_id")]
    public string PayeeReceivableFxRateId { get; set; }

    /// <summary>
    /// Gets or sets the funds that are held payee by the marketplace/platform. This field is only applicable to merchants that been enabled for PayPal Commerce Platform for Marketplaces and Platforms capability.
    /// </summary>
    [JsonProperty(PropertyName = "disbursement_mode")]
    public string DisbursementMode { get; set; }

    #endregion
}