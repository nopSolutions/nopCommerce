using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Theme.KungFu;
using Nop.Plugin.Theme.KungFu.Models;
using Nop.Plugin.Theme.KungFu.Services;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Theme.KungFu.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class ThemeKungFuController : BasePluginController
{
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly IThemeKungFuService _themeKungFuService;

    public ThemeKungFuController(
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IThemeKungFuService themeKungFuService)
    {
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _themeKungFuService = themeKungFuService;
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure()
    {
        await _themeKungFuService.EnsureSyncedAsync();
        var settings = await _settingService.LoadSettingAsync<ThemeKungFuSettings>();
        var status = await _themeKungFuService.GetStatusAsync();

        var model = new ConfigurationModel
        {
            LastSyncedOn = status.SyncedOnUtc.HasValue
                ? (await _dateTimeHelper.ConvertToUserTimeAsync(status.SyncedOnUtc.Value, DateTimeKind.Utc)).Humanize()
                : await _localizationService.GetResourceAsync("Plugins.Theme.KungFu.Status.Never"),
            IsOutdated = status.WasOutdated,
            SyncAutomatically = settings.SyncAutomatically,
            PluginVersion = status.PluginVersion,
            AccentSummary = status.WasOutdated
                ? await _localizationService.GetResourceAsync("Plugins.Theme.KungFu.Status.Outdated")
                : await _localizationService.GetResourceAsync("Plugins.Theme.KungFu.Status.Fresh"),
            AzureOpenAIEndpoint = settings.AzureOpenAIEndpoint,
            AzureOpenAIKey = settings.AzureOpenAIKey,
            AzureOpenAIDeploymentName = settings.AzureOpenAIDeploymentName,
            EnableAISageMessages = settings.EnableAISageMessages
        };

        return View("~/Plugins/Theme.KungFu/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var settings = await _settingService.LoadSettingAsync<ThemeKungFuSettings>();
        settings.SyncAutomatically = model.SyncAutomatically;
        settings.AzureOpenAIEndpoint = model.AzureOpenAIEndpoint;
        settings.AzureOpenAIKey = model.AzureOpenAIKey;
        settings.AzureOpenAIDeploymentName = model.AzureOpenAIDeploymentName;
        settings.EnableAISageMessages = model.EnableAISageMessages;
        await _settingService.SaveSettingAsync(settings);

        var result = await _themeKungFuService.EnsureSyncedAsync(model.TriggerResync || model.IsOutdated);
        await _settingService.ClearCacheAsync();

        if (result.Synced)
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Theme.KungFu.Notifications.Synced"));
        else
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }
}
