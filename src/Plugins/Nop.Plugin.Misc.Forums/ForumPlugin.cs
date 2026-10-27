using Nop.Core.Domain.Cms;
using Nop.Plugin.Misc.Forums.Public.Components;
using Nop.Plugin.Misc.Forums.Services;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Forums;

/// <summary>
/// Represents the Forums plugin
/// </summary>
public class ForumPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
{
    #region Fields

    private readonly ForumInstallService _forumsInstallService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public ForumPlugin(ForumInstallService forumsInstallService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
        _forumsInstallService = forumsInstallService;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    #region IWidgetPlugin

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (string.Equals(PublicWidgetZones.CustomerInfoBottom, widgetZone, StringComparison.OrdinalIgnoreCase))
            return typeof(ForumAccountInfoViewComponent);

        return typeof(ProfileForumPostLinkViewComponent);
    }

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget zones
    /// </returns>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>([PublicWidgetZones.CustomerInfoBottom, PublicWidgetZones.ProfilePageInfoUserstats]);
    }

    #endregion

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(ForumDefaults.Routes.Admin.CONFIGURATION);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await _forumsInstallService.InstallRequiredDataAsync();

        //widget
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(ForumDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(ForumDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }
    }

    /// <summary>
    /// Install sample data of plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override Task InstallSampleDataAsync()
    {
        return _forumsInstallService.InstallSampleDataAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        await _forumsInstallService.UninstallRequiredDataAsync();

        //widget
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(ForumDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(ForumDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }
    }

    #endregion

    #region Properties

    #region IWidgetPlugin

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => true;

    #endregion

    #endregion
}