using Nop.Core;

namespace Nop.Plugin.Payments.PayPalCommerce
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class PayPalCommerceDefaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Payments.PayPalCommerce";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets the nopCommerce partner code
        /// </summary>
        public static string PartnerCode => "NopCommerce_PPCP";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Payments.PayPalCommerce.Configure";

        /// <summary>
        /// Gets the webhook route name
        /// </summary>
        public static string WebhookRouteName => "Plugin.Payments.PayPalCommerce.Webhook";

        /// <summary>
        /// Gets the one page checkout route name
        /// </summary>
        public static string OnePageCheckoutRouteName => "CheckoutOnePage";

        /// <summary>
        /// Gets the shopping cart route name
        /// </summary>
        public static string ShoppingCartRouteName => "ShoppingCart";

        /// <summary>
        /// Gets the session key to get process payment request
        /// </summary>
        public static string PaymentRequestSessionKey => "OrderPaymentInfo";

        /// <summary>
        /// Gets the name of a generic attribute to store the refund identifier
        /// </summary>
        public static string RefundIdAttributeName => "PayPalCommerceRefundId";

        /// <summary>
        /// Gets the service js script URL
        /// </summary>
        public static string ServiceScriptUrl => "https://www.paypal.com/sdk/js";

        /// <summary>
        /// Gets a default period (in seconds) before the request times out
        /// </summary>
        public static int RequestTimeout => 10;

        /// <summary>
        /// Gets webhook event names to subscribe
        /// </summary>
        public static List<string> WebhookEventNames => new()
        {
            "CHECKOUT.ORDER.APPROVED",
            "CHECKOUT.ORDER.COMPLETED",
            "PAYMENT.AUTHORIZATION.CREATED",
            "PAYMENT.AUTHORIZATION.VOIDED",
            "PAYMENT.CAPTURE.COMPLETED",
            "PAYMENT.CAPTURE.DENIED",
            "PAYMENT.CAPTURE.PENDING",
            "PAYMENT.CAPTURE.REFUNDED"
        };

        /// <summary>
        /// Gets a list of currencies that do not support decimals. 
        /// Refer to https://developer.paypal.com/docs/integration/direct/rest/currency-codes/ for more information 
        /// </summary>
        public static List<string> CurrenciesWithoutDecimals => new() { "HUF", "JPY", "TWD" };

        #region Onboarding

        /// <summary>
        /// Represents onboarding constants
        /// </summary>
        public class Onboarding
        {
            /// <summary>
            /// Gets the base URL of onboarding services
            /// </summary>
            public static string ServiceUrl => "https://www.nopcommerce.com/";

            /// <summary>
            /// Gets the onboarding js script URL
            /// </summary>
            public static string ScriptUrl => "https://www.sandbox.paypal.com/webapps/merchantboarding/js/lib/lightbox/partner.js";

            /// <summary>
            /// Gets a period (in seconds) before the onboarding request times out
            /// </summary>
            public static int RequestTimeout => 20;
        }

        #endregion
    }
}