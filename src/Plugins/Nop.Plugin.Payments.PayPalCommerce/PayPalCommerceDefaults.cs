using Nop.Core;

namespace Nop.Plugin.Payments.PayPalCommerce;

/// <summary>
/// Represents the plugin constants
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
    public static string UserAgent => $"nopCommerce-{NopVersion.FULL_VERSION}";

    /// <summary>
    /// Gets the session key to get process payment request
    /// </summary>
    public static string PaymentRequestSessionKey => "OrderPaymentInfo";

    /// <summary>
    /// Gets the name of a generic attribute to store the refund identifier
    /// </summary>
    public static string RefundIdAttributeName => "PayPalCommerceRefundId";

    /// <summary>
    /// Gets the name of the generic attribute that is used to store shipment carrier
    /// </summary>
    public static string ShipmentCarrierAttribute => "PayPalCommerceShipmentCarrier";

    /// <summary>
    /// Gets the service URL
    /// </summary>
    public static (string Sandbox, string Live) ServiceUrl => ("https://api-m.sandbox.paypal.com/", "https://api-m.paypal.com/");

    /// <summary>
    /// Gets the service JS script URL
    /// </summary>
    public static string ServiceScriptUrl => "https://www.paypal.com/sdk/js";

    /// <summary>
    /// Gets the Apple Pay JS script URL
    /// </summary>
    public static string ApplePayScriptUrl => "https://applepay.cdn-apple.com/jsapi/v1/apple-pay-sdk.js";

    /// <summary>
    /// Gets the Google Pay JS script URL
    /// </summary>
    public static string GooglePayScriptUrl => "https://pay.google.com/gp/p/js/pay.js";

    /// <summary>
    /// Gets the merchant configurator JS script URL
    /// </summary>
    public static string MerchantConfiguratorScriptUrl => "https://www.paypalobjects.com/merchant-library/merchant-configurator.js";

    /// <summary>
    /// Gets the partner attribution header used for each request to APIs
    /// </summary>
    public static (string Name, string Value) PartnerHeader => ("PayPal-Partner-Attribution-Id", "MazSoft_Cart_PPCP");

    /// <summary>
    /// Gets a default period (in seconds) before the request times out
    /// </summary>
    public static int RequestTimeout => 10;

    /// <summary>
    /// Gets the tab id of the payment tokens menu item
    /// </summary>
    public static int PaymentTokensMenuTab => 485;

    /// <summary>
    /// Gets webhook event names to subscribe
    /// </summary>
    public static List<string> WebhookEventNames =>
    [
        "PAYMENT.CAPTURE.COMPLETED",                //a capture has been successfully completed
        "PAYMENT.CAPTURE.DENIED",                   //a capture has been denied
        "PAYMENT.CAPTURE.DECLINED",                 //a capture has been declined
        "PAYMENT.CAPTURE.REFUNDED",                 //the seller has voluntarily refunded a payment back to the buyer
        "PAYMENT.CAPTURE.REVERSED",                 //a payment has been involuntarily refunded back to the buyer
        "PAYMENT.CAPTURE.PENDING",                  //the state of a payment capture changes to pending
        "CHECKOUT.ORDER.COMPLETED",                 //a buyer's order has been completed
        "CHECKOUT.ORDER.APPROVED",                  //a buyer's order has been approved by the seller
        "CHECKOUT.ORDER.PROCESSED",                 //a buyer's order is being processed
        "CHECKOUT.PAYMENT-APPROVAL.REVERSED",       //a problem occurred after the buyer approved the order but before you captured the payment
        "PAYMENT.AUTHORIZATION.CREATED",            //a payment authorization is created, approved or executed
        "PAYMENT.AUTHORIZATION.VOIDED",             //a payment authorization is voided
        "VAULT.PAYMENT-TOKEN.CREATED",              //the payment source is saved (for cards and PayPal vaulting)
        "VAULT.PAYMENT-TOKEN.DELETION-INITIATED",   //the payment source is deleted (for PayPal vaulting only)
        "VAULT.PAYMENT-TOKEN.DELETED",              //the payment source is deleted (for cards and PayPal vaulting)
    ];

    /// <summary>
    /// Gets a list of currencies that do not support decimals. 
    /// Refer to https://developer.paypal.com/docs/integration/direct/rest/currency-codes/ for more information 
    /// </summary>
    public static List<string> CurrenciesWithoutDecimals => ["HUF", "JPY", "TWD"];

    /// <summary>
    /// Gets a list of countries that supported Pay Later feature
    /// Refer to https://developer.paypal.com/docs/checkout/pay-later/us/#eligibility for more information 
    /// </summary>
    public static List<string> PayLaterSupportedCountries => ["US", "AU", "DE", "ES", "FR", "GB", "IT"];

    #region Route names

    /// <summary>
    /// Represents the route names
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string Configuration => "Plugin.Payments.PayPalCommerce.Configure";

        /// <summary>
        /// Gets the onboarding callback route name
        /// </summary>
        public static string OnboardingCallback => "Plugin.Payments.PayPalCommerce.OnboardingCallback";

        /// <summary>
        /// Gets the webhook route name
        /// </summary>
        public static string Webhook => "Plugin.Payments.PayPalCommerce.Webhook";

        /// <summary>
        /// Gets the payment info route name
        /// </summary>
        public static string PaymentInfo => "Plugin.Payments.PayPalCommerce.PaymentInfo";

        /// <summary>
        /// Gets the confirm order route name
        /// </summary>
        public static string ConfirmOrder => "Plugin.Payments.PayPalCommerce.ConfirmOrder";

        /// <summary>
        /// Gets the one page checkout route name
        /// </summary>
        public static string OnePageCheckout => "CheckoutOnePage";

        /// <summary>
        /// Gets the shopping cart route name
        /// </summary>
        public static string ShoppingCart => "ShoppingCart";

        /// <summary>
        /// Gets the checkout completed route name
        /// </summary>
        public static string CheckoutCompleted => "CheckoutCompleted";

        /// <summary>
        /// Gets the customer info route name
        /// </summary>
        public static string CustomerInfo => "CustomerInfo";

        /// <summary>
        /// Gets the payment tokens route name
        /// </summary>
        public static string PaymentTokens => "Plugin.Payments.PayPalCommerce.PaymentTokens";
    }

    #endregion

    #region Onboarding

    /// <summary>
    /// Represents the onboarding constants
    /// </summary>
    public class Onboarding
    {
        /// <summary>
        /// Gets the onboarding JS script URL
        /// </summary>
        public static string ScriptUrl => "https://www.paypal.com/webapps/merchantboarding/js/lib/lightbox/partner.js";

        /// <summary>
        /// Gets the onboarding URL
        /// </summary>
        public static (string Sandbox, string Live) Url =>
            ("https://www.sandbox.paypal.com/bizsignup/partner/entry", "https://www.paypal.com/bizsignup/partner/entry");

        /// <summary>
        /// Gets the id
        /// </summary>
        public static (string Sandbox, string Live) Id => ("4UK3WDQYUYNR6", "8A4FPTZ95K6MN");

        /// <summary>
        /// Gets the client id
        /// </summary>
        public static (string Sandbox, string Live) ClientId =>
            ("AUh4KQIcvc4uOWNlDxTHz0vXbCHASdCYyIenUXDDaiSFlHdD351YPrC0Dwv_rHxrPHFANmCwCoL-2w8j",
            "ATiO0tnu-7qEuOqUSp-WcA5YfzBmXq0kxdfasuKabSULa19wuQf2660qGVXySGyZPZnvtFUuukZ2Os-M");

        /// <summary>
        /// Gets the base URL of onboarding services
        /// </summary>
        public static string ServiceUrl => "https://www.nopcommerce.com/";

        /// <summary>
        /// Gets the logo URL to display in the merchant's onboarding flow
        /// </summary>
        public static string LogoUrl => "https://www.nopcommerce.com/themes/officialsite/content/images/logo.png";
    }

    #endregion
}