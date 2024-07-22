namespace Nop.Plugin.Payments.AmazonPay;

/// <summary>
/// Represents the plugin defaults
/// </summary>
public static class AmazonPayDefaults
{
    #region Routes names

    /// <summary>
    /// Gets the IPN handler route name
    /// </summary>
    public static string IPNHandlerRouteName => "Plugin.Payments.AmazonPay.IPNHandler";

    /// <summary>
    /// Gets the return handler route name
    /// </summary>
    public static string SignInHandlerRouteName => "Plugin.Payments.AmazonPay.SignInHandler";

    /// <summary>
    /// Gets the confirm route name
    /// </summary>
    public static string ConfirmRouteName => "Plugin.Payments.AmazonPay.Confirm";

    /// <summary>
    /// Gets the checkout result handler route name
    /// </summary>
    public static string CheckoutResultHandlerRouteName => "Plugin.Payments.AmazonPay.CheckoutResult";

    /// <summary>
    /// Gets the one page checkout route name
    /// </summary>
    public static string OnePageCheckoutRouteName => "CheckoutOnePage";

    /// <summary>
    /// Gets the currencies page route name
    /// </summary>
    public static string CurrenciesPageRouteName => "CurrencyList";

    #endregion

    #region Configuration

    /// <summary>
    /// Gets the generic attribute name to hide credentials settings block on the plugin configuration page
    /// </summary>
    public static string HideCredentialsBlock = "AmazonPay.HideGeneralBlock";

    /// <summary>
    /// Gets the generic attribute name to hide configuration settings block on the plugin configuration page
    /// </summary>
    public static string HideConfigurationBlock = "AmazonPay.HideConfigurationBlock";

    #endregion

    #region InProgress attribute names

    /// <summary>
    /// Gets the generic attribute name for refund "in progress"
    /// </summary>
    public static string RefundInProgressAttributeName => "AmazonPay.RefundInProgress";

    /// <summary>
    /// Gets the generic attribute name for void "in progress"
    /// </summary>
    public static string VoidInProgressAttributeName => "AmazonPay.VoidInProgress";

    /// <summary>
    /// Gets the generic attribute name for capture "in progress"
    /// </summary>
    public static string CaptureInProgressAttributeName => "AmazonPay.CaptureInProgress";

    #endregion

    #region Plugin specification

    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public static string PluginSystemName => "Payments.AmazonPay";

    /// <summary>
    /// Gets the identifier of the Solution Provider (SP)
    /// </summary>
    public static string SpId => "A2I7IXCTR3J174";

    /// <summary>
    /// Gets the plugin version
    /// </summary>
    public static string PluginVersion => "4.60.1";

    /// <summary>
    /// Gets the integration name
    /// </summary>
    public static string IntegrationName => $"Created by nopCommerce, nopCommerce, V{PluginVersion}";

    #endregion

    /// <summary>
    /// Gets the query parameter name for buyerToken
    /// </summary>
    public static string BuyerTokenQueryParamName => "buyerToken";

    /// <summary>
    /// Gets the query parameter name for checkoutSessionId
    /// </summary>
    public static string CheckoutSessionQueryParamName => "amazonCheckoutSessionId";

    /// <summary>
    /// Gets the generic attribute name for checkoutSessionId
    /// </summary>
    public static string CheckoutSessionIdAttributeName => "AmazonPay.CheckoutSessionId";

    /// <summary>
    /// Gets the generic attribute name for refundRequest
    /// </summary>
    public static string RefundRequestAttributeName => "AmazonPay.RefundRequest";

    /// <summary>
    /// Gets the generic attribute name for skipFillDataBySession
    /// </summary>
    public static string SkipFillDataBySessionAttributeName => "AmazonPay.SkipFillDataBySession";

    /// <summary>
    /// Gets the generic attribute name for DoNotUseWithAmazonPay
    /// </summary>
    public static string DoNotUseWithAmazonPayAttributeName => "AmazonPay.DoNotUseWithAmazonPay";

    /// <summary>
    /// Gets the generic attribute name for address
    /// </summary>
    public static string AddressAttributeName => "AmazonPay.Address";

    /// <summary>
    /// Gets the product types
    /// </summary>
    public static (string PayOnly, string PayAndShip) ProductType => ("PayOnly", "PayAndShip");

    #region Onboarding

    /// <summary>
    /// Represents onboarding constants
    /// </summary>
    public class Onboarding
    {
        /// <summary>
        /// Gets the form name
        /// </summary>
        public static string FormName => "AmazonPay";

        /// <summary>
        /// Gets the URL to initiate registration with Amazon Pay
        /// </summary>
        public static (string Eu, string Us) RegisterUrl => ("https://payments-eu.amazon.com/register", "https://payments.amazon.com/register");

        /// <summary>
        /// Gets the allowed origin URLs
        /// </summary>
        public static List<string> OriginUrls => new() { "payments.amazon.com", "payments-eu.amazon.com", "sellercentral.amazon.com", "sellercentral-europe.amazon.com" };

        /// <summary>
        /// Gets the key share route name
        /// </summary>
        public static string KeyShareRouteName => "Plugin.Payments.AmazonPay.KeyShare";

        /// <summary>
        /// Gets the URL to sign up
        /// </summary>
        public static (string Us, string Eu, string Uk, string Jp) RegistrationLink =>
            ("https://pay.amazon.com/signup", "https://pay.amazon.eu/signup", "https://pay.amazon.co.uk/signup", "https://pay.amazon.co.jp/signup");
    }

    #endregion
}