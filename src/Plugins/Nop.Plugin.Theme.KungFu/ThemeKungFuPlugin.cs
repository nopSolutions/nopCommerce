using System.Collections.Generic;
using Nop.Plugin.Theme.KungFu.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Theme.KungFu;

public class ThemeKungFuPlugin : BasePlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISettingService _settingService;
    private readonly IThemeKungFuService _themeKungFuService;

    public ThemeKungFuPlugin(ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        IThemeKungFuService themeKungFuService)
    {
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
        _themeKungFuService = themeKungFuService;
    }

    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(ThemeKungFuDefaults.ConfigurationRouteName);
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new ThemeKungFuSettings
        {
            SyncAutomatically = true,
            LastSyncedVersion = null,
            LastSyncedOnUtc = null
        });

        await _themeKungFuService.EnsureSyncedAsync(force: true);

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Theme.KungFu.Fields.LastSyncedOn"] = "Last sync",
            ["Plugins.Theme.KungFu.Fields.SyncAutomatically"] = "Sync changes automatically",
            ["Plugins.Theme.KungFu.Fields.SyncAutomatically.Hint"] = "If enabled, the plugin will keep the theme files and topics aligned whenever the version changes.",
            ["Plugins.Theme.KungFu.Notifications.Synced"] = "Kung Fu theme assets and topics are synced",
            ["Plugins.Theme.KungFu.Status.Fresh"] = "Everything is live and looking sharp.",
            ["Plugins.Theme.KungFu.Status.Never"] = "No sync has been run yet",
            ["Plugins.Theme.KungFu.Status.Outdated"] = "Updates are waiting to be promoted",
            ["Plugins.Theme.KungFu.Configure.Heading"] = "Kung Fu control center",
            ["Plugins.Theme.KungFu.Configure.Subheading"] = "Ship new theme content with confidence",
            ["Plugins.Theme.KungFu.Configure.Outdated"] = "Outdated",
            ["Plugins.Theme.KungFu.Configure.UpToDate"] = "Up to date",
            ["Plugins.Theme.KungFu.Configure.SyncNow"] = "Resync now",
            ["Plugins.Theme.KungFu.Configure.Version"] = "Plugin version"
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<ThemeKungFuSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Theme.KungFu.");

        await base.UninstallAsync();
    }

    public override Task UpdateAsync(string currentVersion, string targetVersion)
    {
        return _themeKungFuService.EnsureSyncedAsync(force: true);
    }
}
