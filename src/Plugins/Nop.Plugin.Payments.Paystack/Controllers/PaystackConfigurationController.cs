using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Paystack.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Paystack.Controllers;

/// <summary>
/// Admin configuration controller for Paystack payment plugin
/// </summary>
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
public class PaystackConfigurationController : BasePaymentController
{
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;

    public PaystackConfigurationController(
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> ConfigureAsync()
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<PaystackPaymentSettings>(storeScope);

        var model = new ConfigurationModel
        {
            ActiveStoreScopeConfiguration = storeScope,
            SecretKey = settings.SecretKey,
            PublicKey = settings.PublicKey,
            WebhookSecret = settings.WebhookSecret,
            AdditionalFee = settings.AdditionalFee,
            AdditionalFeePercentage = settings.AdditionalFeePercentage
        };

        if (storeScope > 0)
        {
            model.SecretKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.SecretKey, storeScope);
            model.PublicKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.PublicKey, storeScope);
            model.WebhookSecret_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.WebhookSecret, storeScope);
            model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.AdditionalFee, storeScope);
            model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.AdditionalFeePercentage, storeScope);
        }

        return View("~/Plugins/Payments.Paystack/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> ConfigureAsync(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await ConfigureAsync();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<PaystackPaymentSettings>(storeScope);

        if (!string.IsNullOrWhiteSpace(model.SecretKey))
            settings.SecretKey = model.SecretKey;
        settings.PublicKey = model.PublicKey;
        settings.WebhookSecret = model.WebhookSecret;
        settings.AdditionalFee = model.AdditionalFee;
        settings.AdditionalFeePercentage = model.AdditionalFeePercentage;

        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.PublicKey, model.PublicKey_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.WebhookSecret, model.WebhookSecret_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await ConfigureAsync();
    }
}
