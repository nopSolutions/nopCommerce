namespace Nop.Plugin.Payments.Paystack;

/// <summary>
/// Paystack API and route defaults
/// </summary>
public static class PaystackDefaults
{
    /// <summary>
    /// Paystack API base address
    /// </summary>
    public const string BaseAddress = "https://api.paystack.co";

    /// <summary>
    /// Initialize transaction endpoint
    /// </summary>
    public const string InitializeTransactionEndpoint = "/transaction/initialize";

    /// <summary>
    /// Verify transaction endpoint (append /{reference})
    /// </summary>
    public const string VerifyTransactionEndpoint = "/transaction/verify";

    /// <summary>
    /// Route name for customer callback (redirect after payment)
    /// </summary>
    public const string CallbackRouteName = "Plugin.Payments.Paystack.Callback";

    /// <summary>
    /// Route name for webhook (server-to-server)
    /// </summary>
    public const string WebhookRouteName = "Plugin.Payments.Paystack.Webhook";

    /// <summary>
    /// Route name for popup complete (redirect opener and close popup)
    /// </summary>
    public const string CompleteRouteName = "Plugin.Payments.Paystack.Complete";
}
