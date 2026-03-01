using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Paystack;

/// <summary>
/// Paystack payment settings
/// </summary>
public class PaystackPaymentSettings : ISettings
{
    /// <summary>
    /// Gets or sets the secret key
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the public key (used on frontend)
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the callback URL (redirect after payment)
    /// </summary>
    public string CallbackUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the webhook secret for verifying webhook signatures
    /// </summary>
    public string WebhookSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the additional fee
    /// </summary>
    public decimal AdditionalFee { get; set; }

    /// <summary>
    /// Gets or sets whether additional fee is a percentage
    /// </summary>
    public bool AdditionalFeePercentage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to validate webhook signature
    /// </summary>
    public bool EnableWebhookValidation { get; set; } = true;
}
