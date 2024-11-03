using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the instruction to vault a payment source
/// </summary>
public class VaultInstruction : Vault
{
    #region Properties

    /// <summary>
    /// Gets or sets the value how and when the payment source gets vaulted.
    /// </summary>
    [JsonProperty(PropertyName = "store_in_vault")]
    public string StoreInVault { get; set; }

    /// <summary>
    /// Gets or sets the value how and when the payment source gets vaulted.
    /// </summary>
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the description displayed to PayPal consumer on the approval flow for PayPal, as well as on the PayPal payment token management experience on PayPal.com.
    /// </summary>
    [JsonProperty(PropertyName = "usage_pattern")]
    public string UsagePattern { get; set; }

    /// <summary>
    /// Gets or sets the usage type associated with the PayPal payment token.
    /// </summary>
    [JsonProperty(PropertyName = "usage_type")]
    public string UsageType { get; set; }

    /// <summary>
    /// Gets or sets the customer type associated with the PayPal payment token. This is to indicate whether the customer acting on the merchant / platform is either a business or a consumer.
    /// </summary>
    [JsonProperty(PropertyName = "customer_type")]
    public string CustomerType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the multiple payment tokens for the same payer are enabled. Use this when the customer has not logged in at merchant/platform. The payment token thus generated, can then also be used to create the customer account at merchant/platform. Use this also when multiple payment tokens are required for the same payer, different customer at merchant/platform. This helps to identify customers distinctly even though they may share the same PayPal account. This only applies to PayPal payment source.
    /// </summary>
    [JsonProperty(PropertyName = "permit_multiple_payment_tokens")]
    public bool? PermitMultiplePaymentTokens { get; set; }

    #endregion
}