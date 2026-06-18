using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Jotform.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Jotform.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class JotformController : BasePluginController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public JotformController(ILocalizationService localizationService,
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
        //load settings for a chosen store scope
        var store = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var jotformSettings = await _settingService.LoadSettingAsync<JotformSettings>(store);

        var model = new ConfigurationModel
        {
            Enabled = jotformSettings.Enabled,
            EmbedCode = jotformSettings.EmbedCode
        };

        if (store > 0)
        {
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(jotformSettings, x => x.Enabled, store);
            model.EmbedCode_OverrideForStore = await _settingService.SettingExistsAsync(jotformSettings, x => x.EmbedCode, store);
        }

        return View("~/Plugins/Widgets.Jotform/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var store = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var jotformSettings = await _settingService.LoadSettingAsync<JotformSettings>(store);
        jotformSettings.EmbedCode = model.EmbedCode;
        jotformSettings.Enabled = model.Enabled;

        await _settingService.SaveSettingOverridablePerStoreAsync(jotformSettings, x => x.Enabled, model.Enabled_OverrideForStore, store, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(jotformSettings, x => x.EmbedCode, model.EmbedCode_OverrideForStore, store, false);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion
}