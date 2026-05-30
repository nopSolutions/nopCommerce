using Nop.Core.Configuration;
using Nop.Plugin.Payments.PayPalCommerce.Domain;

namespace Nop.Plugin.Payments.PayPalCommerce;

/// <summary>
/// Represents the plugin settings
/// </summary>
public class PayPalCommerceSettings : ISettings
{
    #region Properties

    #region Onboarding

    /// <summary>
    /// Gets or sets internal merchant id
    /// </summary>
    public string MerchantGuid { get; set; }

    /// <summary>
    /// Gets or sets the merchant id
    /// </summary>
    public string MerchantId { get; set; }

    /// <summary>
    /// Gets or sets a webhook URL
    /// </summary>
    public string WebhookUrl { get; set; }

    #endregion

    #region Credentials

    /// <summary>
    /// Gets or sets a value indicating whether to manually set the credentials (for example, there is already the REST API App created)
    /// </summary>
    public bool SetCredentialsManually { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use sandbox environment
    /// </summary>
    public bool UseSandbox { get; set; }

    /// <summary>
    /// Gets or sets client identifier
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets client secret
    /// </summary>
    public string SecretKey { get; set; }

    #endregion

    #region Configuration

    /// <summary>
    /// Gets or sets the payment type
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use Advanced Credit and Debit Card Payments (Custom Card Fields)
    /// </summary>
    public bool UseCardFields { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the strong customer authentication (SCA/3DS) is required
    /// </summary>
    public bool CustomerAuthenticationRequired { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use Apple Pay
    /// </summary>
    public bool UseApplePay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use Google Pay
    /// </summary>
    public bool UseGooglePay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use Alternative Payment Methods (iDEAL, Bancontact, etc)
    /// </summary>
    public bool UseAlternativePayments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use Vault (store buyers payment information)
    /// </summary>
    public bool UseVault { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to skip order confirmation page during checkout process
    /// </summary>
    public bool SkipOrderConfirmPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use shipment tracking
    /// </summary>
    public bool UseShipmentTracking { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display buttons on the shopping cart page
    /// </summary>
    public bool DisplayButtonsOnShoppingCart { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display buttons on a product details page
    /// </summary>
    public bool DisplayButtonsOnProductDetails { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display buttons on the checkout payment method page
    /// </summary>
    public bool DisplayButtonsOnPaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display logo in header links
    /// </summary>
    public bool DisplayLogoInHeaderLinks { get; set; }

    /// <summary>
    /// Gets or sets the source code of logo in header links
    /// </summary>
    public string LogoInHeaderLinks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display logo in footer
    /// </summary>
    public bool DisplayLogoInFooter { get; set; }

    /// <summary>
    /// Gets or sets the source code of logo in footer
    /// </summary>
    public string LogoInFooter { get; set; }

    #endregion

    #region Advanced settings

    /// <summary>
    /// Gets or sets a period (in seconds) before the request times out
    /// </summary>
    public int? RequestTimeout { get; set; }

    /// <summary>
    /// Gets or sets the disabled funding sources for the transaction (separated by comma)
    /// By default, funding source eligibility is determined based on a variety of factors
    /// </summary>
    public string DisabledFunding { get; set; }

    /// <summary>
    /// Gets or sets the enabled funding sources for the transaction (separated by comma)
    /// Enable funding can be used to ensure a funding source is rendered, if eligible
    /// </summary>
    public string EnabledFunding { get; set; }

    /// <summary>
    /// Gets or sets the layout option to determine the button layout when multiple buttons are available (vertical, horizontal)
    /// </summary>
    public string StyleLayout { get; set; }

    /// <summary>
    /// Gets or sets the color option (gold, blue, silver, white, black)
    /// </summary>
    public string StyleColor { get; set; }

    /// <summary>
    /// Gets or sets the shape option (rect, pill, sharp)
    /// </summary>
    public string StyleShape { get; set; }

    /// <summary>
    /// Gets or sets the label option (paypal, checkout, buynow, pay, installment)
    /// </summary>
    public string StyleLabel { get; set; }

    /// <summary>
    /// Gets or sets the tagline option (true, false)
    /// </summary>
    public string StyleTagline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to hide the checkout button on the shopping cart page
    /// </summary>
    public bool HideCheckoutButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the only immediate payment (credit card, PayPal balance, or instant ACH)
    /// from the customer is accepted
    /// </summary>
    public bool ImmediatePaymentRequired { get; set; }

    /// <summary>
    /// Gets or sets the order validity interval (in seconds) when the existing order will be used. 
    /// Set to 0 to create a new order for each payment attempt
    /// </summary>
    public int OrderValidityInterval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Messaging Configurator is supported for the merchant
    /// </summary>
    public bool ConfiguratorSupported { get; set; }

    /// <summary>
    /// Gets or sets Pay Later messaging configurations
    /// </summary>
    public string PayLaterConfig { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the plugin was updated but no merchant ID was specified. If not, the merchant must specify it
    /// </summary>
    public bool MerchantIdRequired { get; set; }

    #endregion

    #endregion
}