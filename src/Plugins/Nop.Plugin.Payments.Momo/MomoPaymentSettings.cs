using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Momo;

/// <summary>
/// MoMo payment settings
/// </summary>
public class MomoPaymentSettings : ISettings
{
    /// <summary>
    /// Gets or sets the base address
    /// </summary>
    public string BaseAddress { get; set; }

    /// <summary>
    /// Gets or sets the API user
    /// </summary>
    public string ApiUser { get; set; }

    /// <summary>
    /// Gets or sets the API key
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the subscription key
    /// </summary>
    public string SubscriptionKey { get; set; }

    /// <summary>
    /// Gets or sets the environment (sandbox/production)
    /// </summary>
    public string Environment { get; set; }

    /// <summary>
    /// Gets or sets the currency code
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the callback URL
    /// </summary>
    public string CallbackUrl { get; set; }

    /// <summary>
    /// Gets or sets the transaction timeout in minutes
    /// </summary>
    public int TransactionTimeoutMinutes { get; set; } = 15;

    /// <summary>
    /// Gets or sets the minimum allowed amount
    /// </summary>
    public decimal MinimumAmount { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed amount
    /// </summary>
    public decimal MaximumAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to validate callbacks
    /// </summary>
    public bool EnableCallbackValidation { get; set; }

    /// <summary>
    /// Gets or sets the callback validation key
    /// </summary>
    public string CallbackValidationKey { get; set; }
}