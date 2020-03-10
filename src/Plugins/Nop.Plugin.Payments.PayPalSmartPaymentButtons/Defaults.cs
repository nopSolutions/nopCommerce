using System.Collections.Generic;
using Nop.Core;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class Defaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Payments.PayPalSmartPaymentButtons";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CurrentVersion}";

        /// <summary>
        /// Gets the nopCommerce partner code
        /// </summary>
        public static string PartnerCode => "NOP_Cart_SPB";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Payments.PayPalSmartPaymentButtons.Configure";

        /// <summary>
        /// Gets the webhook route name
        /// </summary>
        public static string WebhookRouteName => "Plugin.Payments.PayPalSmartPaymentButtons.Webhook";

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
        /// Gets the service js script URL
        /// </summary>
        public static string ServiceScriptUrl => "https://www.paypal.com/sdk/js";

        /// <summary>
        /// Gets webhook event names to subscribe
        /// </summary>
        public static List<string> WebhookEventNames => new List<string>
        {
            "CHECKOUT.ORDER.APPROVED",
            "PAYMENT.AUTHORIZATION.CREATED",
            "PAYMENT.AUTHORIZATION.VOIDED",
            "PAYMENT.CAPTURE.COMPLETED",
            "PAYMENT.CAPTURE.DENIED",
            "PAYMENT.CAPTURE.PENDING",
            "PAYMENT.CAPTURE.REFUNDED"
        };

        /// <summary>
        /// Gets a list of available widget zones to display logo and buttons (Prominently feature)
        /// </summary>
        public static Dictionary<int, string> AvailableButtonsWidgetZones => new Dictionary<int, string>
        {
            //[1] = PublicWidgetZones.ProductDetailsAddInfo, //PayPal asked for now to remove the buttons from the product details page, well ok.
            [2] = PublicWidgetZones.OrderSummaryContentAfter,
            [3] = PublicWidgetZones.HeaderLinksBefore,
            [4] = PublicWidgetZones.Footer,
        };

        /// <summary>
        /// Gets a name of the view component to display payment info in public store
        /// </summary>
        public const string PAYMENT_INFO_VIEW_COMPONENT_NAME = "PayPalSmartPaymentButtonsPaymentInfo";

        /// <summary>
        /// Gets a name of the view component to add script to pages
        /// </summary>
        public const string SCRIPT_VIEW_COMPONENT_NAME = "PayPalSmartPaymentButtonsScript";

        /// <summary>
        /// Gets a name of the view component to display logo and buttons
        /// </summary>
        public const string BUTTONS_VIEW_COMPONENT_NAME = "PayPalSmartPaymentButtonsButtons";
    }
}