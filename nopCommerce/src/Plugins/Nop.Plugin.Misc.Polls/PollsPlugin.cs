using Nop.Core.Domain.Cms;
using Nop.Plugin.Misc.Polls.Public.Components;
using Nop.Plugin.Misc.Polls.Services;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Polls;

/// <summary>
/// Represents the Polls plugin
/// </summary>
public class PollsPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
{
    #region Fields

    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISettingService _settingService;
    private readonly PollInstallationService _pollInstallationService;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public PollsPlugin(INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        PollInstallationService pollInstallationService,
        WidgetSettings widgetSettings)
    {
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
        _pollInstallationService = pollInstallationService;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(PollsDefaults.Routes.ConfigurationRouteName);
    }

    /// <summary>
    /// Install sample data of plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallSampleDataAsync()
    {
        await _pollInstallationService.InstallSampleDataAsync();
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PollsDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PollsDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _pollInstallationService.InstallRequiredDataAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(PollsDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(PollsDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _pollInstallationService.UnInstallRequiredDataAsync();
    }

    #region IWidgetPlugin

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        ArgumentNullException.ThrowIfNull(widgetZone);

        if (widgetZone == PublicWidgetZones.HomepageBottom)
            return typeof(HomepagePollsViewComponent);

        return typeof(LeftSidePollsViewComponent);
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
        return Task.FromResult<IList<string>>([PublicWidgetZones.LeftSideColumnAfter, PublicWidgetZones.HomepageBottom]);
    }

    #endregion

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
