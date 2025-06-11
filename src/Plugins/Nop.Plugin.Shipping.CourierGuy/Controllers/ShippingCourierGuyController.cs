using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Shipping.CourierGuy.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Org.BouncyCastle.Crypto.Engines;

namespace Nop.Plugin.Shipping.CourierGuy.Controllers;

public class ShippingCourierGuyController(
    IWorkContext workContext,
    IStoreService storeService,
    ISettingService settingService,
    IOrderService orderService,
    IOrderProcessingService orderProcessingService,
    ILocalizationService localizationService,
    ILogger logger,
    INotificationService notificationService,
    IPermissionService permissionService,
    IStoreContext storeContext)
    : BasePluginController
{
    private readonly CourierGuySettings _courierGuyShippingSettings;

    // GET
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin] //confirms access to the admin panel
    [Area(AreaNames.ADMIN)] //specifies the area containing a controller or action
    [ActionName("Configure")]
    [HttpGet]
    public async Task<IActionResult> ConfigureAsync()
    {
        if (!await permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS))
            return AccessDeniedView();

        //load settings for a chosen store scope
        var storeScope = await storeContext.GetActiveStoreScopeConfigurationAsync();
        var courierGuySettings = await settingService.LoadSettingAsync<CourierGuySettings>(storeScope);

        var model = new ConfigurationModel
        {
            UseSandbox = true,
            BaseUrl = courierGuySettings.BaseUrl,
            ApiKey = courierGuySettings.ApiKey,
            SandBoxApiKey = courierGuySettings.SandBoxApiKey,
            TrackingUri = courierGuySettings.TrackingUri,
            RateRequestUri = courierGuySettings.RateRequestUri,
            ShipmentRequestUri = courierGuySettings.ShipmentRequestUri
        };

        if (storeScope > 0)
        {
            // model.TestMode_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.TestMode, storeScope);
            model.UseSandbox_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.UseSandbox, storeScope);
            model.BaseUrl_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.BaseUrl, storeScope);
            model.ApiKey_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.ApiKey, storeScope);
            model.SandBoxApiKey_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.SandBoxApiKey, storeScope);
            model.TrackingUri_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.TrackingUri, storeScope);
            model.RateRequestUri_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.RateRequestUri, storeScope);
            model.ShipmentRequestUri_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.ShipmentRequestUri, storeScope);
            model.PushoverApiKey_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.PushoverApiKey, storeScope);
            model.PushoverUserKey_OverrideForStore = await settingService.SettingExistsAsync(courierGuySettings, x => x.PushoverUserKey, storeScope);
        }

        return View("~/Plugins/Shipping.CourierGuy/Views/Configure.cshtml", model);
    }
    [HttpPost]
    [FormValueRequired("save")]
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin]
    [ActionName("Configure")]
    [Area(AreaNames.ADMIN)]
    public async Task<IActionResult> ConfigureAsync(ConfigurationModel model)
    {
        if (!await permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS))
            return AccessDeniedView();

        var storeId = await storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await settingService.LoadSettingAsync<CourierGuySettings>(storeId);

        //set new settings values
        settings.UseSandbox = model.UseSandbox;
        settings.BaseUrl = model.BaseUrl;
        settings.ApiKey = model.ApiKey;
        settings.SandBoxApiKey = model.SandBoxApiKey;
        settings.TrackingUri = model.TrackingUri;
        settings.RateRequestUri = model.RateRequestUri;
        settings.ShipmentRequestUri = model.ShipmentRequestUri;
        settings.PushoverApiKey = model.PushoverApiKey;
        settings.PushoverUserKey = model.PushoverUserKey;

        await settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.UseSandbox, model.UseSandbox_OverrideForStore, storeId, false);
        await settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.BaseUrl, model.BaseUrl_OverrideForStore, storeId, false);
        await settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.ApiKey, model.ApiKey_OverrideForStore, storeId, false);
        await settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SandBoxApiKey, model.SandBoxApiKey_OverrideForStore, storeId, false);
        await settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.TrackingUri, model.TrackingUri_OverrideForStore, storeId, false);
        await settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.RateRequestUri, model.RateRequestUri_OverrideForStore, storeId, false);
        await settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.ShipmentRequestUri, model.ShipmentRequestUri_OverrideForStore, storeId, false);
        await settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.PushoverApiKey, model.PushoverApiKey_OverrideForStore, storeId, false);
        await settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.PushoverUserKey, model.PushoverUserKey_OverrideForStore, storeId, false);

        await settingService.ClearCacheAsync();
        notificationService.SuccessNotification(await localizationService.GetResourceAsync("Admin.Plugins.Saved"));
        return await ConfigureAsync();
    }

}