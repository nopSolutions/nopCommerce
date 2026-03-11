namespace Nop.Plugin.Payments.Paystack;

/// <summary>
/// Paystack API and route defaults
/// </summary>
public static class PaystackDefaults
{
    /// <summary>
    /// Paystack API base address
    /// </summary>
    public const string BASE_ADDRESS = "https://api.paystack.co";

    /// <summary>
    /// Initialize transaction endpoint
    /// </summary>
    public const string INITIALIZE_TRANSACTION_ENDPOINT = "/transaction/initialize";

    /// <summary>
    /// Verify transaction endpoint (append /{reference})
    /// </summary>
    public const string VERIFY_TRANSACTION_ENDPOINT = "/transaction/verify";

    /// <summary>
    /// Route name for customer callback (redirect after payment)
    /// </summary>
    public const string CALLBACK_ROUTE_NAME = "Plugin.Payments.Paystack.Callback";

    /// <summary>
    /// Route name for webhook (server-to-server)
    /// </summary>
    public const string WEBHOOK_ROUTE_NAME = "Plugin.Payments.Paystack.Webhook";

    /// <summary>
    /// Route name for popup complete (redirect opener and close popup)
    /// </summary>
    public const string COMPLETE_ROUTE_NAME = "Plugin.Payments.Paystack.Complete";

    /// <summary>
    /// Call back url for payment processing
    /// </summary>
    public const string PAYSTACK_CALLBACK_ENDPOINT = "/paystack/callback";

    public const string DEFAULT_CURRENCY_VALUE = "NGN";
    
    public const string CANCEL_PAYMENT = "Plugin.Payments.Paystack.CancelPayment";
}
