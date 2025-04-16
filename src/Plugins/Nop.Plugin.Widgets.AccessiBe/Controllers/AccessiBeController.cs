using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.AccessiBe.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.AccessiBe.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class AccessiBeController : BasePluginController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public AccessiBeController(ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure()
    {
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();

        var settings = await _settingService.LoadSettingAsync<AccessiBeSettings>(storeId);
        var settingsMobile = await _settingService.LoadSettingAsync<AccessiBeMobileSettings>(storeId);
        var widgetSettings = await _settingService.LoadSettingAsync<WidgetSettings>(storeId);

        var model = new ConfigurationModel
        {
            Enabled = widgetSettings.ActiveWidgetSystemNames.Contains(AccessiBeDefaults.SystemName),
            ActiveStoreScopeConfiguration = storeId,
            ScriptIsCustomized = !settings.Script?.Contains(AccessiBeDefaults.ConfigToken) ?? true,
            TriggerModel = settings.ToSettingsModel<AccessiBeTriggerModel>(),
            TriggerMobileModel = settingsMobile.ToSettingsModel<AccessiBeTriggerMobileModel>()
        };

        if (storeId > 0)
        {
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(widgetSettings, setting => setting.ActiveWidgetSystemNames, storeId);

            #region Trigger settings

            model.TriggerModel.LeadColor_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.LeadColor, storeId);
            model.TriggerModel.StatementLink_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.StatementLink, storeId);
            model.TriggerModel.FooterHtml_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.FooterHtml, storeId);
            model.TriggerModel.ShowMobile_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.HideMobile, storeId);
            model.TriggerModel.HideTrigger_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.HideTrigger, storeId);
            model.TriggerModel.Language_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.Language, storeId);
            model.TriggerModel.Position_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.Position, storeId);
            model.TriggerModel.TriggerColor_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TriggerColor, storeId);
            model.TriggerModel.TriggerPositionX_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TriggerPositionX, storeId);
            model.TriggerModel.TriggerPositionY_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TriggerPositionY, storeId);
            model.TriggerModel.TriggerRadius_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TriggerRadius, storeId);
            model.TriggerModel.TriggerIcon_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TriggerIcon, storeId);
            model.TriggerModel.TriggerSize_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TriggerSize, storeId);
            model.TriggerModel.TriggerOffsetX_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TriggerOffsetX, storeId);
            model.TriggerModel.TriggerOffsetY_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TriggerOffsetY, storeId);

            #endregion

            #region Mobile trigger settings

            model.TriggerModel.TriggerSize_OverrideForStore = await _settingService.SettingExistsAsync(settingsMobile, setting => setting.TriggerSize, storeId);
            model.TriggerModel.TriggerPositionX_OverrideForStore = await _settingService.SettingExistsAsync(settingsMobile, setting => setting.TriggerPositionX, storeId);
            model.TriggerModel.TriggerPositionY_OverrideForStore = await _settingService.SettingExistsAsync(settingsMobile, setting => setting.TriggerPositionY, storeId);
            model.TriggerModel.TriggerOffsetX_OverrideForStore = await _settingService.SettingExistsAsync(settingsMobile, setting => setting.TriggerOffsetX, storeId);
            model.TriggerModel.TriggerOffsetY_OverrideForStore = await _settingService.SettingExistsAsync(settingsMobile, setting => setting.TriggerOffsetY, storeId);
            model.TriggerModel.TriggerRadius_OverrideForStore = await _settingService.SettingExistsAsync(settingsMobile, setting => setting.TriggerRadius, storeId);

            #endregion
        }

        if (model.ScriptIsCustomized)
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.Widgets.AccessiBe.ScriptIsCustomized.Warning"));

        return View("~/Plugins/Widgets.AccessiBe/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var widgetSettings = await _settingService.LoadSettingAsync<WidgetSettings>(storeScope);
        var triggerSettings = await _settingService.LoadSettingAsync<AccessiBeSettings>(storeScope);
        var triggerMobileSettings = await _settingService.LoadSettingAsync<AccessiBeMobileSettings>(storeScope);


        #region Trigger settings

        triggerSettings = model.TriggerModel.ToSettings(triggerSettings);

        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.LeadColor, model.TriggerModel.LeadColor_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.StatementLink, model.TriggerModel.StatementLink_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.FooterHtml, model.TriggerModel.FooterHtml_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.Language, model.TriggerModel.Language_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.Position, model.TriggerModel.Position_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.TriggerColor, model.TriggerModel.TriggerColor_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.TriggerPositionX, model.TriggerModel.TriggerPositionX_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.TriggerPositionY, model.TriggerModel.TriggerPositionY_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.TriggerSize, model.TriggerModel.TriggerSize_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.TriggerRadius, model.TriggerModel.TriggerRadius_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.HideTrigger, model.TriggerModel.HideTrigger_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.TriggerOffsetX, model.TriggerModel.TriggerOffsetX_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.TriggerOffsetY, model.TriggerModel.TriggerOffsetY_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.TriggerIcon, model.TriggerModel.TriggerIcon_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerSettings, x => x.HideMobile, model.TriggerModel.ShowMobile_OverrideForStore, storeScope, false);

        #endregion

        #region Mobile trigger settings

        triggerMobileSettings = model.TriggerMobileModel.ToSettings(triggerMobileSettings);

        await _settingService.SaveSettingOverridablePerStoreAsync(triggerMobileSettings, x => x.TriggerPositionX, model.TriggerMobileModel.TriggerPositionX_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerMobileSettings, x => x.TriggerPositionY, model.TriggerMobileModel.TriggerPositionY_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerMobileSettings, x => x.TriggerSize, model.TriggerMobileModel.TriggerSize_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerMobileSettings, x => x.TriggerRadius, model.TriggerMobileModel.TriggerRadius_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerMobileSettings, x => x.TriggerOffsetX, model.TriggerMobileModel.TriggerOffsetX_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(triggerMobileSettings, x => x.TriggerOffsetY, model.TriggerMobileModel.TriggerOffsetY_OverrideForStore, storeScope, false);

        #endregion

        if (model.Enabled && !widgetSettings.ActiveWidgetSystemNames.Contains(AccessiBeDefaults.SystemName))
            widgetSettings.ActiveWidgetSystemNames.Add(AccessiBeDefaults.SystemName);
        if (!model.Enabled && widgetSettings.ActiveWidgetSystemNames.Contains(AccessiBeDefaults.SystemName))
            widgetSettings.ActiveWidgetSystemNames.Remove(AccessiBeDefaults.SystemName);
        await _settingService.SaveSettingOverridablePerStoreAsync(widgetSettings, setting => setting.ActiveWidgetSystemNames, model.Enabled_OverrideForStore, storeScope, false);

        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion
}