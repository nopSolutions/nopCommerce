using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Plugin.Payments.AmazonPay.Models;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.AmazonPay.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class AmazonPayController : BasePaymentController
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly AmazonPayOnboardingService _amazonPayOnboardingService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AmazonPayController(AmazonPayApiService amazonPayApiService,
        AmazonPayOnboardingService amazonPayOnboardingService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _amazonPayApiService = amazonPayApiService;
        _amazonPayOnboardingService = amazonPayOnboardingService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure()
    {
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<AmazonPaySettings>(storeId);

        var customer = await _workContext.GetCurrentCustomerAsync();

        var model = new ConfigurationModel
        {
            SetCredentialsManually = settings.SetCredentialsManually,
            Region = (int)settings.PaymentRegion,
            Payload = settings.Payload,
            PaymentRegion = (int)settings.PaymentRegion,
            PrivateKey = settings.PrivateKey,
            PublicKeyId = settings.PublicKeyId,
            MerchantId = settings.MerchantId,
            StoreId = settings.StoreId,
            UseSandbox = settings.UseSandbox,
            PaymentType = (int)settings.PaymentType,
            ButtonPlacement = settings.ButtonPlacement.Select(p => (int)p).ToList(),
            ButtonColor = (int)settings.ButtonColor,
            EnableLogging = settings.EnableLogging,

            IsConnected = !string.IsNullOrEmpty(settings.MerchantId) && !string.IsNullOrEmpty(settings.StoreId) && !string.IsNullOrEmpty(settings.PublicKeyId),
            ActiveStoreScopeConfiguration = storeId,
            HideCredentialsBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, AmazonPayDefaults.HideCredentialsBlock),
            HideConfigurationBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, AmazonPayDefaults.HideConfigurationBlock),
            IpnUrl = _amazonPayApiService.GetUrl(AmazonPayDefaults.IPNHandlerRouteName)
        };

        if (storeId > 0)
        {
            model.SetCredentialsManually_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.SetCredentialsManually, storeId);
            model.Region_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PaymentRegion, storeId);
            model.Payload_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.Payload, storeId);
            model.PaymentRegion_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PaymentRegion, storeId);
            model.PrivateKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PrivateKey, storeId);
            model.PublicKeyId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PublicKeyId, storeId);
            model.MerchantId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.MerchantId, storeId);
            model.StoreId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.StoreId, storeId);

            model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.UseSandbox, storeId);
            model.PaymentType_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PaymentType, storeId);
            model.ButtonPlacement_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.ButtonPlacement, storeId);
            model.ButtonColor_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.ButtonColor, storeId);
            model.EnableLogging_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.EnableLogging, storeId);
        }

        //check currency
        if (model.IsConnected)
            await _amazonPayApiService.EnsureCurrencyIsValidAsync();

        return View("~/Plugins/Payments.AmazonPay/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var amazonPaySettings = await _settingService.LoadSettingAsync<AmazonPaySettings>(storeId);

        //set credentials from payload
        if (!string.IsNullOrEmpty(model.Payload))
        {
            var (credentials, error) = await _amazonPayOnboardingService.PrepareCredentialsAsync(model.Payload, amazonPaySettings.PrivateKey);
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(credentials?.PublicKeyId))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.Onboarding.Error");
                var errorMessage = string.Format(locale, error, Url.Action("List", "Log"));
                _notificationService.ErrorNotification(errorMessage, false);
            }
            else
            {
                model.MerchantId = credentials.MerchantId;
                model.StoreId = credentials.StoreId;
                model.PublicKeyId = credentials.PublicKeyId;
                model.MerchantId_OverrideForStore = model.Payload_OverrideForStore;
                model.StoreId_OverrideForStore = model.Payload_OverrideForStore;
                model.PublicKeyId_OverrideForStore = model.Payload_OverrideForStore;
            }
        }

        amazonPaySettings.SetCredentialsManually = model.SetCredentialsManually;
        amazonPaySettings.PaymentRegion = (PaymentRegion)model.PaymentRegion;
        amazonPaySettings.Payload = model.Payload;
        amazonPaySettings.PrivateKey = model.PrivateKey;
        amazonPaySettings.PublicKeyId = model.PublicKeyId;
        amazonPaySettings.MerchantId = model.MerchantId;
        amazonPaySettings.StoreId = model.StoreId;
        amazonPaySettings.UseSandbox = model.UseSandbox;
        amazonPaySettings.PaymentType = model.PaymentType > 0 ? (PaymentType)model.PaymentType : amazonPaySettings.PaymentType;
        amazonPaySettings.ButtonPlacement = model.ButtonPlacement.Any(p => p > 0) ? model.ButtonPlacement.Select(p => (ButtonPlacement)p).ToList() : amazonPaySettings.ButtonPlacement;
        amazonPaySettings.ButtonColor = model.ButtonColor > 0 ? (ButtonColor)model.ButtonColor : amazonPaySettings.ButtonColor;
        amazonPaySettings.EnableLogging = model.EnableLogging;

        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.SetCredentialsManually, model.SetCredentialsManually_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.PaymentRegion, model.PaymentRegion_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.Payload, model.Payload_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.PrivateKey, model.PrivateKey_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.PublicKeyId, model.PublicKeyId_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.MerchantId, model.MerchantId_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.StoreId, model.StoreId_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.UseSandbox, model.UseSandbox_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.PaymentType, model.PaymentType_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.ButtonPlacement, model.ButtonPlacement_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.ButtonColor, model.ButtonColor_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.EnableLogging, model.EnableLogging_OverrideForStore, storeId, false);

        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return RedirectToAction("Configure");
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("onboarding")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Onboarding(OnboardingModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var amazonPaySettings = await _settingService.LoadSettingAsync<AmazonPaySettings>(storeId);

        amazonPaySettings.PaymentRegion = (PaymentRegion)model.Region;
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.PaymentRegion, model.Region_OverrideForStore, storeId, false);

        if (model.Region == (int)PaymentRegion.Eu || model.Region == (int)PaymentRegion.Uk || model.Region == (int)PaymentRegion.Jp)
        {
            amazonPaySettings.SetCredentialsManually = true;
            await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.SetCredentialsManually, model.Region_OverrideForStore, storeId, false);
            await _settingService.ClearCacheAsync();

            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.Onboarding.Region.Warning"));

            return RedirectToAction("Configure");
        }

        //try to register merchant (US only)
        var (privateKey, publicKey) = await _amazonPayOnboardingService.RegisterAsync((PaymentRegion)model.Region, amazonPaySettings);

        amazonPaySettings.SetCredentialsManually = model.SetCredentialsManually;
        amazonPaySettings.PrivateKey = privateKey;
        amazonPaySettings.PublicKey = publicKey;
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.SetCredentialsManually, model.Region_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.PrivateKey, model.Region_OverrideForStore, storeId, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(amazonPaySettings, settings => settings.PublicKey, model.Region_OverrideForStore, storeId, false);
        await _settingService.ClearCacheAsync();

        return Content("Redirected");
    }

    #endregion
}