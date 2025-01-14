using FluentMigrator;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Payments.PayPalCommerce.Data;

[NopMigration("2024-06-06 00:00:01", "Payments.PayPalCommerce 4.70.10. Advanced cards", MigrationProcessType.Update)]
public class AdvancedCardsMigration : MigrationBase
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly OnboardingHttpClient _httpClient;
    private readonly PayPalCommerceServiceManager _serviceManager;
    private readonly PayPalCommerceSettings _settings;

    #endregion

    #region Ctor

    public AdvancedCardsMigration(ILocalizationService localizationService,
        ISettingService settingService,
        OnboardingHttpClient httpClient,
        PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _httpClient = httpClient;
        _serviceManager = serviceManager;
        _settings = settings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        if (!Schema.Table(nameof(PayPalToken)).Exists())
            Create.TableFor<PayPalToken>();

        var (languageId, languages) = this.GetLanguageData();

        _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Enums.Nop.Plugin.Payments.PayPalCommerce.Domain.ButtonPlacement.Cart"] = "Shopping cart",
            ["Enums.Nop.Plugin.Payments.PayPalCommerce.Domain.ButtonPlacement.Product"] = "Product",
            ["Enums.Nop.Plugin.Payments.PayPalCommerce.Domain.ButtonPlacement.PaymentMethod"] = "Checkout",
            ["Plugins.Payments.PayPalCommerce.ApplePay.Discount"] = "Discount",
            ["Plugins.Payments.PayPalCommerce.ApplePay.Shipping"] = "Shipping",
            ["Plugins.Payments.PayPalCommerce.ApplePay.Subtotal"] = "Subtotal",
            ["Plugins.Payments.PayPalCommerce.ApplePay.Tax"] = "Tax",
            ["Plugins.Payments.PayPalCommerce.Card.Button"] = "Pay now with Card",
            ["Plugins.Payments.PayPalCommerce.Card.New"] = "Pay by new card",
            ["Plugins.Payments.PayPalCommerce.Card.Prefix"] = "Pay by",
            ["Plugins.Payments.PayPalCommerce.Card.Save"] = "Save your card",
            ["Plugins.Payments.PayPalCommerce.Configuration"] = "Configuration",
            ["Plugins.Payments.PayPalCommerce.Fields.CustomerAuthenticationRequired"] = "Use 3D Secure",
            ["Plugins.Payments.PayPalCommerce.Fields.CustomerAuthenticationRequired.Hint"] = "3D Secure enables you to authenticate card holders through card issuers. It reduces the likelihood of fraud when you use supported cards and improves transaction performance. A successful 3D Secure authentication can shift liability for chargebacks due to fraud from you to the card issuer.",
            ["Plugins.Payments.PayPalCommerce.Fields.DisplayButtonsOnProductDetails.Hint"] = "Determine whether to display PayPal buttons on product details pages (simple products only) allowing buyers to complete a purchase without going through the full checkout process.",
            ["Plugins.Payments.PayPalCommerce.Fields.DisplayButtonsOnShoppingCart.Hint"] = "Determine whether to display PayPal buttons on the shopping cart page in addition to the default checkout button.",
            ["Plugins.Payments.PayPalCommerce.Fields.MerchantId"] = "Merchant ID",
            ["Plugins.Payments.PayPalCommerce.Fields.MerchantId.Hint"] = "PayPal account ID of the merchant.",
            ["Plugins.Payments.PayPalCommerce.Fields.MerchantId.Required"] = "Merchant ID is required",
            ["Plugins.Payments.PayPalCommerce.Fields.SetCredentialsManually.Hint"] = "Determine whether to manually set the credentials (for example, there is already the REST API application created, or if you want to use the sandbox mode).",
            ["Plugins.Payments.PayPalCommerce.Fields.SkipOrderConfirmPage"] = "Skip 'Confirm Order' page",
            ["Plugins.Payments.PayPalCommerce.Fields.SkipOrderConfirmPage.Hint"] = "Determine whether to skip the 'Confirm Order' step during checkout so that after approving the payment on PayPal site, customers will redirected directly to the 'Order Completed' page.",
            ["Plugins.Payments.PayPalCommerce.Fields.UseAlternativePayments"] = "Use Alternative Payments Methods",
            ["Plugins.Payments.PayPalCommerce.Fields.UseAlternativePayments.Hint"] = "With alternative payment methods, customers across the globe can pay with their bank accounts, wallets, and other local payment methods.",
            ["Plugins.Payments.PayPalCommerce.Fields.UseApplePay"] = "Use Apple Pay",
            ["Plugins.Payments.PayPalCommerce.Fields.UseApplePay.Hint"] = "Apple Pay is a mobile payment and digital wallet service provided by Apple Inc.",
            ["Plugins.Payments.PayPalCommerce.Fields.UseApplePay.Warning"] = "Don't forget to enable 'Serve unknown types of static files' on the <a href=\"{0}\" target=\"_blank\">App settings page</a>, so that the domain association file is processed correctly.",
            ["Plugins.Payments.PayPalCommerce.Fields.UseCardFields"] = "Use Custom Card Fields",
            ["Plugins.Payments.PayPalCommerce.Fields.UseCardFields.Hint"] = "Advanced Credit and Debit Card Payments (Custom Card Fields) are a PCI compliant solution to accept debit and credit card payments.",
            ["Plugins.Payments.PayPalCommerce.Fields.UseGooglePay"] = "Use Google Pay",
            ["Plugins.Payments.PayPalCommerce.Fields.UseGooglePay.Hint"] = "Google Pay is a mobile payment and digital wallet service provided by Alphabet Inc.",
            ["Plugins.Payments.PayPalCommerce.Fields.UseShipmentTracking"] = "Use shipment tracking",
            ["Plugins.Payments.PayPalCommerce.Fields.UseShipmentTracking.Hint"] = "Determine whether to use the package tracking. It allows to automatically sync orders and shipment status with PayPal.",
            ["Plugins.Payments.PayPalCommerce.Fields.UseVault"] = "Use Vault",
            ["Plugins.Payments.PayPalCommerce.Fields.UseVault.Hint"] = "Determine whether to use PayPal Vault. It allows to store buyers payment information and use it in subsequent transactions.",
            ["Plugins.Payments.PayPalCommerce.GooglePay.Discount"] = "Discount",
            ["Plugins.Payments.PayPalCommerce.GooglePay.Shipping"] = "Shipping",
            ["Plugins.Payments.PayPalCommerce.GooglePay.Subtotal"] = "Subtotal",
            ["Plugins.Payments.PayPalCommerce.GooglePay.Tax"] = "Tax",
            ["Plugins.Payments.PayPalCommerce.GooglePay.Total"] = "Total",
            ["Plugins.Payments.PayPalCommerce.Onboarding.Button.Sandbox"] = "Sign up for PayPal (sandbox)",
            ["Plugins.Payments.PayPalCommerce.Onboarding.Process.Account.Success"] = "PayPal account is created",
            ["Plugins.Payments.PayPalCommerce.Onboarding.Process.Email.Success"] = "Email address is confirmed",
            ["Plugins.Payments.PayPalCommerce.Onboarding.Process.Payments.Success"] = "Billing information is set",
            ["Plugins.Payments.PayPalCommerce.Onboarding.Sandbox"] = "After you finish testing the plugin in the PayPal sandbox, move it into the production environment so you can process live transactions. To take the plugin live: 1. Revoke access to the sandbox account, 2. Disable 'Use sandbox' setting, 3. Sign up for the live PayPal account.",
            ["Plugins.Payments.PayPalCommerce.Onboarding.Title"] = "Connect PayPal account",
            ["Plugins.Payments.PayPalCommerce.Order.Adjustment.Name"] = "Adjustment item",
            ["Plugins.Payments.PayPalCommerce.Order.Adjustment.Description"] = "Used to adjust the order total amount when applying complex discounts or/and calculations",
            ["Plugins.Payments.PayPalCommerce.Order.Error"] = "Failed to get order details",
            ["Plugins.Payments.PayPalCommerce.Order.Id"] = "PayPal order ID",
            ["Plugins.Payments.PayPalCommerce.Order.Placement"] = "PayPal component placement",
            ["Plugins.Payments.PayPalCommerce.PaymentTokens"] = "Payment methods",
            ["Plugins.Payments.PayPalCommerce.PaymentTokens.Default"] = "Default",
            ["Plugins.Payments.PayPalCommerce.PaymentTokens.Expiration"] = "Expires",
            ["Plugins.Payments.PayPalCommerce.PaymentTokens.None"] = "No payment methods saved yet",
            ["Plugins.Payments.PayPalCommerce.PaymentTokens.MarkDefault"] = "Make default",
            ["Plugins.Payments.PayPalCommerce.PaymentTokens.Title"] = "Method",
            ["Plugins.Payments.PayPalCommerce.PayLater"] = "Pay Later",
            ["Plugins.Payments.PayPalCommerce.Shipment.Carrier"] = "Carrier",
            ["Plugins.Payments.PayPalCommerce.Shipment.Carrier.Hint"] = "Specify the carrier for the shipment (e.g. UPS or FEDEX_UK, see allowed values on PayPal site).",
        }, languageId);

        if (!_settingService.SettingExists(_settings, settings => settings.MerchantId))
            _settings.MerchantId = null;

        if (!_settingService.SettingExists(_settings, settings => settings.UseCardFields))
            _settings.UseCardFields = false;

        if (!_settingService.SettingExists(_settings, settings => settings.CustomerAuthenticationRequired))
            _settings.CustomerAuthenticationRequired = true;

        if (!_settingService.SettingExists(_settings, settings => settings.UseApplePay))
            _settings.UseApplePay = false;

        if (!_settingService.SettingExists(_settings, settings => settings.UseGooglePay))
            _settings.UseGooglePay = false;

        if (!_settingService.SettingExists(_settings, settings => settings.UseAlternativePayments))
            _settings.UseAlternativePayments = false;

        if (!_settingService.SettingExists(_settings, settings => settings.UseVault))
            _settings.UseVault = false;

        if (!_settingService.SettingExists(_settings, settings => settings.SkipOrderConfirmPage))
            _settings.SkipOrderConfirmPage = false;

        if (!_settingService.SettingExists(_settings, settings => settings.UseShipmentTracking))
            _settings.UseShipmentTracking = false;

        if (!_settingService.SettingExists(_settings, settings => settings.DisplayButtonsOnPaymentMethod))
            _settings.DisplayButtonsOnPaymentMethod = true;

        if (!_settingService.SettingExists(_settings, settings => settings.HideCheckoutButton))
            _settings.HideCheckoutButton = false;

        if (!_settingService.SettingExists(_settings, settings => settings.ImmediatePaymentRequired))
            _settings.ImmediatePaymentRequired = false;

        if (!_settingService.SettingExists(_settings, settings => settings.OrderValidityInterval))
            _settings.OrderValidityInterval = 300;

        if (!_settingService.SettingExists(_settings, settings => settings.ConfiguratorSupported))
            _settings.ConfiguratorSupported = false;

        if (!_settingService.SettingExists(_settings, settings => settings.PayLaterConfig))
            _settings.PayLaterConfig = null;

        if (!_settingService.SettingExists(_settings, settings => settings.MerchantIdRequired))
            _settings.MerchantIdRequired = false;

        try
        {
            if (_settings.SetCredentialsManually)
                _settings.MerchantIdRequired = true;
            else if (!string.IsNullOrEmpty(_settings.MerchantGuid) && _serviceManager.IsActiveAsync(_settings).Result.Active)
            {
                _settings.MerchantIdRequired = true;
                _settings.MerchantId = _httpClient.GetMerchantAsync(_settings.MerchantGuid).Result?.MerchantId;
                _settings.MerchantIdRequired = string.IsNullOrEmpty(_settings.MerchantId);
            }
        }
        catch { }

        _settingService.SaveSetting(_settings);
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
    }

    #endregion
}