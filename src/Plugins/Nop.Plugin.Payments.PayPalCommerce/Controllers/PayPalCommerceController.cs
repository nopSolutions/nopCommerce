using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Factories;
using Nop.Plugin.Payments.PayPalCommerce.Models.Admin;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.PayPalCommerce.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
public class PayPalCommerceController(AppSettings appSettings,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWorkContext workContext,
        PayPalCommerceModelFactory modelFactory,
        PayPalCommerceServiceManager serviceManager,
        ShoppingCartSettings shoppingCartSettings) : BasePluginController
{
    #region Utilities

    /// <summary>
    /// Load plugin settings
    /// </summary>
    /// <param name="storeId">Store id; pass null to use active store scope configuration</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugin settings; store id
    /// </returns>
    private async Task<(PayPalCommerceSettings Settings, int StoreId)> LoadSettingsAsync(int? storeId = null)
    {
        storeId ??= await storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await settingService.LoadSettingAsync<PayPalCommerceSettings>(storeId ?? 0);

        //we don't need some of the shared settings that loaded above, so load them separately for the chosen store
        if (storeId > 0)
        {
            async Task<TPropType> getSettingAsync<TPropType>(Expression<Func<PayPalCommerceSettings, TPropType>> keySelector) =>
                await settingService.GetSettingByKeyAsync<TPropType>(settingService.GetSettingKey(settings, keySelector), storeId: storeId ?? 0);

            settings.MerchantGuid = await getSettingAsync(setting => setting.MerchantGuid);
            settings.MerchantId = await getSettingAsync(setting => setting.MerchantId);
            settings.WebhookUrl = await getSettingAsync(setting => setting.WebhookUrl);
            settings.SetCredentialsManually = await getSettingAsync(setting => setting.SetCredentialsManually);
            settings.UseSandbox = await getSettingAsync(setting => setting.UseSandbox);
            settings.ClientId = await getSettingAsync(setting => setting.ClientId);
            settings.SecretKey = await getSettingAsync(setting => setting.SecretKey);
            settings.ConfiguratorSupported = await getSettingAsync(setting => setting.ConfiguratorSupported);
        }

        return (settings, storeId ?? 0);
    }

    /// <summary>
    /// Save settings for the passed store
    /// </summary>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="settings">Plugin settings</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="storeId">Store id</param>
    /// <param name="overrideForStore">Whether to overridde this setting for the passed store</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task SaveSettingAsync<TPropType>(PayPalCommerceSettings settings,
        Expression<Func<PayPalCommerceSettings, TPropType>> keySelector, int storeId, bool? overrideForStore = null)
    {
        //save overridden settings
        await settingService.SaveSettingOverridablePerStoreAsync(settings, keySelector, overrideForStore ?? true, storeId, false);

        //save shared settings
        if (storeId > 0 && overrideForStore is null)
            await settingService.SaveSettingOverridablePerStoreAsync(settings, keySelector, true, 0, false);
    }

    /// <summary>
    /// Get details to sign up a merchant
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the URL to sign up; merchant internal id
    /// </returns>
    private async Task<(string SandboxUrl, string LiveUrl, string MerchantGuid)> GetSignUpDetailsAsync(PayPalCommerceSettings settings)
    {
        if (!string.IsNullOrEmpty(settings.MerchantGuid))
            return (null, null, settings.MerchantGuid);

        //set new GUID
        var merchantGuid = Guid.NewGuid().ToString();

        //prepare URL to sign up
        var ((sandboxUrl, liveUrl), error) = await serviceManager.PrepareSignUpUrlAsync(merchantGuid);
        if (!string.IsNullOrEmpty(error))
        {
            var locale = await localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Configuration.Error");
            var errorMessage = string.Format(locale, error, Url.Action("List", "Log"));
            notificationService.ErrorNotification(errorMessage, false);
        }

        return (sandboxUrl, liveUrl, merchantGuid);
    }

    /// <summary>
    /// Check the merchant status
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="storeId">Store id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the merchant model
    /// </returns>
    private async Task<MerchantModel> CheckMerchantStatusAsync(PayPalCommerceSettings settings, int storeId)
    {
        //no need to check the status when credentials were manually set
        if (string.IsNullOrEmpty(settings.MerchantGuid) || settings.SetCredentialsManually)
            return new();

        var model = await modelFactory.PrepareMerchantModelAsync(settings);

        //disable appropriate settings when unavailable
        if (settings.ConfiguratorSupported != model.ConfiguratorSupported)
        {
            settings.ConfiguratorSupported = model.ConfiguratorSupported;
            await SaveSettingAsync(settings, setting => setting.ConfiguratorSupported, storeId);
        }
        if (!model.AdvancedCardsEnabled && settings.UseCardFields)
        {
            settings.UseCardFields = false;
            await SaveSettingAsync(settings, setting => setting.UseCardFields, storeId);
        }
        if (!model.ApplePayEnabled && settings.UseApplePay)
        {
            settings.UseApplePay = false;
            await SaveSettingAsync(settings, setting => setting.UseApplePay, storeId);
        }
        if (!model.GooglePayEnabled && settings.UseGooglePay)
        {
            settings.UseGooglePay = false;
            await SaveSettingAsync(settings, setting => setting.UseGooglePay, storeId);
        }
        if (!model.VaultingEnabled && settings.UseVault)
        {
            settings.UseVault = false;
            await SaveSettingAsync(settings, setting => setting.UseVault, storeId);
        }
        await settingService.ClearCacheAsync();

        //display notifications
        foreach (var warning in model.Messages.Warning)
        {
            notificationService.WarningNotification(warning, false);
        }

        foreach (var error in model.Messages.Error)
        {
            notificationService.ErrorNotification(error, false);
        }

        foreach (var message in model.Messages.Success)
        {
            notificationService.SuccessNotification(message);
        }

        return model;
    }

    /// <summary>
    /// Set credentials manually
    /// </summary>
    /// <param name="model">Configuration model</param>
    /// <param name="settings">Plugin settings</param>
    /// <param name="storeId">Store id</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task SetCredentialsManuallyAsync(ConfigurationModel model, PayPalCommerceSettings settings, int storeId)
    {
        if (!model.SetCredentialsManually)
            return;

        //first delete the unused webhook on a previous client, if changed
        if (PayPalCommerceServiceManager.IsConnected(settings) && !string.Equals(model.ClientId, settings.ClientId))
        {
            await serviceManager.DeleteWebhookAsync(settings);
            settings.WebhookUrl = string.Empty;
        }

        settings.ClientId = model.ClientId;
        settings.SecretKey = model.SecretKey;
        settings.MerchantId = model.MerchantId;
        await SaveSettingAsync(settings, setting => setting.ClientId, storeId);
        await SaveSettingAsync(settings, setting => setting.SecretKey, storeId);
        await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
    }

    /// <summary>
    /// Ensure that webhook created, display warning in case of fail
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="storeId">Store id</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task EnsureWebhookCreatedAsync(PayPalCommerceSettings settings, int storeId)
    {
        if (!PayPalCommerceServiceManager.IsConfigured(settings))
            return;

        var (webhook, _) = await serviceManager.CreateWebhookAsync(settings, storeId);
        if (string.IsNullOrEmpty(webhook?.Url))
        {
            var url = Url.Action("List", "Log");
            var warningMessage = string.Format(await localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.WebhookWarning"), url);
            notificationService.WarningNotification(warningMessage, false);

            return;
        }

        if (string.Equals(settings.WebhookUrl, webhook.Url, StringComparison.InvariantCultureIgnoreCase))
            return;

        settings.WebhookUrl = webhook.Url;
        await SaveSettingAsync(settings, setting => setting.WebhookUrl, storeId);
        await settingService.ClearCacheAsync();
    }

    #endregion

    #region Methods

    #region Configuration

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure()
    {
        var (settings, storeId) = await LoadSettingsAsync();

        var model = new ConfigurationModel
        {
            SetCredentialsManually = settings.SetCredentialsManually,
            UseSandbox = settings.UseSandbox,
            ClientId = settings.SetCredentialsManually ? settings.ClientId : string.Empty,
            SecretKey = settings.SetCredentialsManually ? settings.SecretKey : string.Empty,
            MerchantId = settings.SetCredentialsManually ? settings.MerchantId : string.Empty,
            PaymentTypeId = (int)settings.PaymentType,
            UseCardFields = settings.UseCardFields,
            CustomerAuthenticationRequired = settings.CustomerAuthenticationRequired,
            UseApplePay = settings.UseApplePay,
            UseGooglePay = settings.UseGooglePay,
            UseAlternativePayments = settings.UseAlternativePayments,
            UseVault = settings.UseVault,
            SkipOrderConfirmPage = settings.SkipOrderConfirmPage,
            UseShipmentTracking = settings.UseShipmentTracking,
            DisplayButtonsOnShoppingCart = settings.DisplayButtonsOnShoppingCart,
            DisplayButtonsOnProductDetails = settings.DisplayButtonsOnProductDetails,
            DisplayLogoInHeaderLinks = settings.DisplayLogoInHeaderLinks,
            LogoInHeaderLinks = settings.LogoInHeaderLinks,
            DisplayLogoInFooter = settings.DisplayLogoInFooter,
            LogoInFooter = settings.LogoInFooter,
            ActiveStoreScopeConfiguration = storeId,
            IsConfigured = PayPalCommerceServiceManager.IsConfigured(settings)
        };

        if (storeId > 0)
        {
            model.SetCredentialsManually_OverrideForStore = true;
            model.UseSandbox_OverrideForStore = true;
            model.ClientId_OverrideForStore = true;
            model.SecretKey_OverrideForStore = true;
            model.MerchantId_OverrideForStore = true;

            model.PaymentTypeId_OverrideForStore = await settingService.SettingExistsAsync(settings, setting => setting.PaymentType, storeId);
            model.UseCardFields_OverrideForStore = await settingService.SettingExistsAsync(settings, setting => setting.UseCardFields, storeId);
            model.CustomerAuthenticationRequired_OverrideForStore = await settingService
                .SettingExistsAsync(settings, setting => setting.CustomerAuthenticationRequired, storeId);
            model.UseApplePay_OverrideForStore = await settingService.SettingExistsAsync(settings, setting => setting.UseApplePay, storeId);
            model.UseGooglePay_OverrideForStore = await settingService.SettingExistsAsync(settings, setting => setting.UseGooglePay, storeId);
            model.UseAlternativePayments_OverrideForStore = await settingService
                .SettingExistsAsync(settings, setting => setting.UseAlternativePayments, storeId);
            model.UseVault_OverrideForStore = await settingService.SettingExistsAsync(settings, setting => setting.UseVault, storeId);
            model.SkipOrderConfirmPage_OverrideForStore = await settingService
                .SettingExistsAsync(settings, setting => setting.SkipOrderConfirmPage, storeId);
            model.DisplayButtonsOnShoppingCart_OverrideForStore = await settingService
                .SettingExistsAsync(settings, setting => setting.DisplayButtonsOnShoppingCart, storeId);
            model.DisplayButtonsOnProductDetails_OverrideForStore = await settingService
                .SettingExistsAsync(settings, setting => setting.DisplayButtonsOnProductDetails, storeId);
            model.DisplayLogoInHeaderLinks_OverrideForStore = await settingService
                .SettingExistsAsync(settings, setting => setting.DisplayLogoInHeaderLinks, storeId);
            model.LogoInHeaderLinks_OverrideForStore = await settingService
                .SettingExistsAsync(settings, setting => setting.LogoInHeaderLinks, storeId);
            model.DisplayLogoInFooter_OverrideForStore = await settingService
                .SettingExistsAsync(settings, setting => setting.DisplayLogoInFooter, storeId);
            model.LogoInFooter_OverrideForStore = await settingService.SettingExistsAsync(settings, setting => setting.LogoInFooter, storeId);
        }

        model.PaymentTypes = (await PaymentType.Capture.ToSelectListAsync(false, [(int)PaymentType.Subscription, (int)PaymentType.Tokenize]))
            .Select(item => new SelectListItem(item.Text, item.Value))
            .ToList();

        //whether the plugin was updated, but no merchant ID was specified
        if (settings.MerchantIdRequired)
        {
            if (settings.SetCredentialsManually)
                notificationService.WarningNotification("Merchant ID is required for payments, please specify it below");
            else
            {
                var url = Url.Action("AllSettings", "Setting", new { settingName = nameof(PayPalCommerceSettings.MerchantId) });
                notificationService.WarningNotification($"PayPal account ID of the merchant was not set correctly when updating the plugin. " +
                    $"You should either complete onboarding process again on this page or set the ID yourself on the " +
                    $"<a href=\"{url}\" target=\"_blank\">All Settings page</a> (you can find this ID in your PayPal account)", false);
            }
        }

        //merchant and onboarding details
        (model.SandboxSignUpUrl, model.LiveSignUpUrl, model.MerchantGuid) = await GetSignUpDetailsAsync(settings);
        model.MerchantModel = await CheckMerchantStatusAsync(settings, storeId);
        if (!settings.SetCredentialsManually)
            model.MerchantId = model.MerchantModel.MerchantId;

        await EnsureWebhookCreatedAsync(settings, storeId);

        if (PayPalCommerceServiceManager.IsConnected(settings) && !shoppingCartSettings.RoundPricesDuringCalculation)
        {
            //prices and total aren't rounded, so display warning
            var url = Url.Action("AllSettings", "Setting", new { settingName = nameof(ShoppingCartSettings.RoundPricesDuringCalculation) });
            var warningMessage = string.Format(await localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.RoundingWarning"), url);
            notificationService.WarningNotification(warningMessage, false);
        }

        //ensure credentials are valid
        if (PayPalCommerceServiceManager.IsConnected(settings))
        {
            var (_, credentialsError) = await serviceManager.GetAccessTokenAsync(settings);
            if (!string.IsNullOrEmpty(credentialsError))
            {
                notificationService.ErrorNotification(await localizationService
                    .GetResourceAsync("Plugins.Payments.PayPalCommerce.Credentials.Invalid"));
            }
            else
            {
                notificationService.SuccessNotification(await localizationService
                    .GetResourceAsync("Plugins.Payments.PayPalCommerce.Credentials.Valid"));
            }
        }

        return View("~/Plugins/Payments.PayPalCommerce/Views/Admin/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var (settings, storeId) = await LoadSettingsAsync();

        //set new settings values
        var (configureCredentials, _) = await Request.TryGetFormValueAsync(nameof(model.SetCredentialsManually));
        if (configureCredentials)
        {
            settings.SetCredentialsManually = model.SetCredentialsManually;
            settings.UseSandbox = model.UseSandbox;
        }
        settings.PaymentType = (PaymentType)model.PaymentTypeId;
        settings.UseCardFields = model.UseCardFields;
        settings.CustomerAuthenticationRequired = model.CustomerAuthenticationRequired;
        settings.UseApplePay = model.UseApplePay;
        settings.UseGooglePay = model.UseGooglePay;
        settings.UseAlternativePayments = model.UseAlternativePayments;
        settings.UseVault = model.UseVault;
        settings.SkipOrderConfirmPage = model.SkipOrderConfirmPage;
        settings.UseShipmentTracking = model.UseShipmentTracking;
        settings.DisplayButtonsOnShoppingCart = model.DisplayButtonsOnShoppingCart;
        settings.DisplayButtonsOnProductDetails = model.DisplayButtonsOnProductDetails;
        settings.DisplayLogoInHeaderLinks = model.DisplayLogoInHeaderLinks;
        settings.LogoInHeaderLinks = model.LogoInHeaderLinks;
        settings.DisplayLogoInFooter = model.DisplayLogoInFooter;
        settings.LogoInFooter = model.LogoInFooter;

        await SetCredentialsManuallyAsync(model, settings, storeId);

        settings.MerchantIdRequired = settings.MerchantIdRequired && string.IsNullOrEmpty(settings.MerchantId);

        await SaveSettingAsync(settings, setting => setting.SetCredentialsManually, storeId);
        await SaveSettingAsync(settings, setting => setting.UseSandbox, storeId);
        await SaveSettingAsync(settings, setting => setting.PaymentType, storeId, model.PaymentTypeId_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseCardFields, storeId, model.UseCardFields_OverrideForStore);
        await SaveSettingAsync(settings, setting
            => setting.CustomerAuthenticationRequired, storeId, model.CustomerAuthenticationRequired_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseApplePay, storeId, model.UseApplePay_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseGooglePay, storeId, model.UseGooglePay_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseAlternativePayments, storeId, model.UseAlternativePayments_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseVault, storeId, model.UseVault_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.SkipOrderConfirmPage, storeId, model.SkipOrderConfirmPage_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseShipmentTracking, storeId);
        await SaveSettingAsync(settings, setting
            => setting.DisplayButtonsOnShoppingCart, storeId, model.DisplayButtonsOnShoppingCart_OverrideForStore);
        await SaveSettingAsync(settings, setting
            => setting.DisplayButtonsOnProductDetails, storeId, model.DisplayButtonsOnProductDetails_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.DisplayLogoInHeaderLinks, storeId, model.DisplayLogoInHeaderLinks_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.LogoInHeaderLinks, storeId, model.LogoInHeaderLinks_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.DisplayLogoInFooter, storeId, model.DisplayLogoInFooter_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.LogoInFooter, storeId, model.LogoInFooter_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.MerchantIdRequired, storeId);
        await settingService.ClearCacheAsync();

        notificationService.SuccessNotification(await localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return RedirectToAction("Configure");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Onboarding(ConfigurationModel model)
    {
        var (settings, storeId) = await LoadSettingsAsync();

        //set onboarding values
        settings.MerchantGuid = model.MerchantGuid;
        settings.MerchantId = string.Empty;
        settings.WebhookUrl = string.Empty;

        //set credentials values
        settings.SetCredentialsManually = false;
        settings.UseSandbox = model.UseSandbox;
        settings.ClientId = string.Empty;
        settings.SecretKey = string.Empty;

        settings.ConfiguratorSupported = false;

        await SaveSettingAsync(settings, setting => setting.MerchantGuid, storeId);
        await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
        await SaveSettingAsync(settings, setting => setting.WebhookUrl, storeId);
        await SaveSettingAsync(settings, setting => setting.SetCredentialsManually, storeId);
        await SaveSettingAsync(settings, setting => setting.UseSandbox, storeId);
        await SaveSettingAsync(settings, setting => setting.ClientId, storeId);
        await SaveSettingAsync(settings, setting => setting.SecretKey, storeId);
        await SaveSettingAsync(settings, setting => setting.ConfiguratorSupported, storeId);
        await settingService.ClearCacheAsync();

        return Json(new { success = true });
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Onboarding(OnboardingCallbackModel model)
    {
        var storeId = model.StoreId;
        (var settings, storeId) = await LoadSettingsAsync(storeId);

        if (!string.IsNullOrEmpty(settings.MerchantGuid))
        {
            //we need some time to complete the create credentials request before redirecting the merchant
            await Task.Delay(TimeSpan.FromSeconds(5));

            if (string.IsNullOrEmpty(settings.MerchantId))
            {
                settings.MerchantId = model.MerchantIdInPayPal;
                await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
                await settingService.ClearCacheAsync();
            }
        }
        else
            notificationService.ErrorNotification(await localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.Error"));

        return RedirectToAction("Configure");
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(AuthenticationModel model)
    {
        var storeId = model.StoreId;
        (var settings, storeId) = await LoadSettingsAsync(storeId);

        //try to get credentials by authentication parameters
        var (credentials, _) = await serviceManager.SignUpAsync(settings, model.AuthCode, model.SharedId);
        if (credentials is null)
            return ErrorJson(await localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.Error"));

        //first delete the unused webhook on a previous client, if changed
        if (PayPalCommerceServiceManager.IsConnected(settings) && !string.Equals(credentials.ClientId, settings.ClientId))
        {
            await serviceManager.DeleteWebhookAsync(settings);
            settings.WebhookUrl = string.Empty;
        }

        //set onboarding values
        settings.MerchantId = credentials.PayerId;

        //set credentials values
        settings.ClientId = credentials.ClientId;
        settings.SecretKey = credentials.ClientSecret;

        await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
        await SaveSettingAsync(settings, setting => setting.WebhookUrl, storeId);
        await SaveSettingAsync(settings, setting => setting.ClientId, storeId);
        await SaveSettingAsync(settings, setting => setting.SecretKey, storeId);
        await settingService.ClearCacheAsync();

        return Json(new { success = true });
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("revoke")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> RevokeAccess()
    {
        var (settings, storeId) = await LoadSettingsAsync();

        //delete webhook
        if (PayPalCommerceServiceManager.IsConnected(settings))
        {
            await serviceManager.DeleteWebhookAsync(settings);
            settings.WebhookUrl = string.Empty;
        }

        //clear onboarding values
        settings.MerchantGuid = string.Empty;
        settings.MerchantId = string.Empty;

        //clear credentials values
        settings.ClientId = string.Empty;
        settings.SecretKey = string.Empty;

        settings.ConfiguratorSupported = false;

        await SaveSettingAsync(settings, setting => setting.MerchantGuid, storeId);
        await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
        await SaveSettingAsync(settings, setting => setting.WebhookUrl, storeId);
        await SaveSettingAsync(settings, setting => setting.ClientId, storeId);
        await SaveSettingAsync(settings, setting => setting.SecretKey, storeId);
        await SaveSettingAsync(settings, setting => setting.ConfiguratorSupported, storeId);
        await settingService.ClearCacheAsync();

        var accessRevokedMessage = await localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.AccessRevoked");
        notificationService.SuccessNotification(accessRevokedMessage);

        return RedirectToAction("Configure");
    }

    public async Task<IActionResult> ChangeUseApplePay(bool enabled)
    {
        var message = string.Empty;
        if (enabled)
        {
            if (!appSettings.Get<CommonConfig>().ServeUnknownFileTypes)
            {
                //this setting should be enabled for domain verification
                var locale = await localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Fields.UseApplePay.Warning");
                message = string.Format(locale, Url.Action("AppSettings", "Setting"));
            }
        }

        return Json(new { Result = message });
    }

    #endregion

    #region Pay Later

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> PayLater()
    {
        var (settings, _) = await LoadSettingsAsync();
        if (!settings.UseSandbox && !settings.ConfiguratorSupported)
            return RedirectToAction("Configure");

        var language = await workContext.GetWorkingLanguageAsync();
        var model = new PayLaterConfigurationModel
        {
            ClientId = settings.ClientId,
            UseSandbox = settings.UseSandbox,
            Config = !string.IsNullOrEmpty(settings.PayLaterConfig) ? settings.PayLaterConfig : "{}",
            Locale = language.LanguageCulture?.Replace('-', '_') ?? "en_US"
        };

        return View("~/Plugins/Payments.PayPalCommerce/Views/Admin/PayLater.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> PayLaterConfig(string config)
    {
        var (settings, storeId) = await LoadSettingsAsync();
        if (!settings.UseSandbox && !settings.ConfiguratorSupported)
            return ErrorJson("Merchant messaging configurator is not available");

        settings.PayLaterConfig = config;
        await SaveSettingAsync(settings, setting => setting.PayLaterConfig, storeId);
        await settingService.ClearCacheAsync();

        return Json(new { message = await localizationService.GetResourceAsync("Admin.Plugins.Saved") });

    }

    #endregion

    #endregion
}