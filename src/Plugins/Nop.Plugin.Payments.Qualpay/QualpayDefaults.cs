namespace Nop.Plugin.Payments.Qualpay
{
    /// <summary>
    /// Represents Qualpay payment gateway constants
    /// </summary>
    public class QualpayDefaults
    {
        /// <summary>
        /// Name of the view component to display payment info in public store
        /// </summary>
        public const string PAYMENT_INFO_VIEW_COMPONENT_NAME = "QualpayPaymentInfo";

        /// <summary>
        /// Name of the view component to disaply Qualpay Customer Vault block on the customer details page
        /// </summary>
        public const string CUSTOMER_VIEW_COMPONENT_NAME = "QualpayCustomer";

        /// <summary>
        /// Qualpay payment method system name
        /// </summary>
        public static string SystemName => "Payments.Qualpay";

        /// <summary>
        /// User agent using for requesting Qualpay services
        /// </summary>
        public static string UserAgent => "nopCommerce-plugin";

        /// <summary>
        /// nopCommerce developer application ID
        /// </summary>
        public static string DeveloperId => "nopCommerce";

        /// <summary>
        /// Numeric ISO code of the USD currency
        /// </summary>
        public static int UsdNumericIsoCode => 840;

        /// <summary>
        /// Generic attribute name to hide Qualpay Customer Vault block on the customer details page
        /// </summary>
        public static string HideBlockAttribute = "CustomerPage.HideQualpayBlock";

        /// <summary>
        /// One page checkout route name
        /// </summary>
        public static string OnePageCheckoutRouteName => "CheckoutOnePage";

        /// <summary>
        /// Path to Qualpay Embedded Fields js script
        /// </summary>
        public static string EmbeddedFieldsScriptPath => "https://app.qualpay.com/hosted/embedded/js/qp-embedded-sdk.min.js";

        /// <summary>
        /// Path to Qualpay Embedded Fields css styles
        /// </summary>
        public static string EmbeddedFieldsStylePath => "https://app.qualpay.com/hosted/embedded/css/qp-embedded.css";

        /// <summary>
        /// Webhook label
        /// </summary>
        public static string WebhookLabel => "nopCommerce-plugin-webhook";

        /// <summary>
        /// Webhook route name
        /// </summary>
        public static string WebhookRouteName => "Plugin.Payments.Qualpay.Webhook";

        /// <summary>
        /// Subscription suspended webhook event
        /// </summary>
        public static string SubscriptionSuspendedWebhookEvent => "subscription_suspended";

        /// <summary>
        /// Subscription complete webhook event
        /// </summary>
        public static string SubscriptionCompleteWebhookEvent => "subscription_complete";

        /// <summary>
        /// Subscription payment success webhook event
        /// </summary>
        public static string SubscriptionPaymentSuccessWebhookEvent => "subscription_payment_success";

        /// <summary>
        /// Subscription payment failure webhook event
        /// </summary>
        public static string SubscriptionPaymentFailureWebhookEvent => "subscription_payment_failure";

        /// <summary>
        /// Validate URL webhook event
        /// </summary>
        public static string ValidateUrlWebhookEvent => "validate_url";

        /// <summary>
        /// Webhook signature header name
        /// </summary>
        public static string WebhookSignatureHeaderName => "x-qualpay-webhook-signature";

        /// <summary>
        /// Subscription email
        /// </summary>
        public static string SubscriptionEmail => "jgilbert@qualpay.com";
    }
}