using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Misc.News.Public.Components;
using Nop.Plugin.Misc.News.Services;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.News;

/// <summary>
/// Represents the News plugin
/// </summary>
public class NewsPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
{
    #region Fields

    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly ISettingService _settingService;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly NewsInstallService _newsInstallService;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public NewsPlugin(IActionContextAccessor actionContextAccessor,
        ISettingService settingService,
        IUrlHelperFactory urlHelperFactory,
        NewsInstallService newsInstallService,
        WidgetSettings widgetSettings)
    {
        _actionContextAccessor = actionContextAccessor;
        _settingService = settingService;
        _urlHelperFactory = urlHelperFactory;
        _newsInstallService = newsInstallService;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        ArgumentNullException.ThrowIfNull(_actionContextAccessor.ActionContext);

        return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(NewsDefaults.Routes.Admin.ConfigurationRouteName);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await _newsInstallService.InstallRequiredDataAsync();

        //widget
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(NewsDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(NewsDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }
    }

    /// <summary>
    /// Install sample data of plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override Task InstallSampleDataAsync()
    {
        return _newsInstallService.InstallSampleDataAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        await _newsInstallService.UnInstallRequiredDataAsync();

        //widget
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(NewsDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(NewsDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }
    }

    #region IWidgetPlugin

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (widgetZone == PublicWidgetZones.HeadHtmlTag)
            return typeof(NewsRssHeaderLinkViewComponent);

        return typeof(HomepageNewsViewComponent);
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
        return Task.FromResult<IList<string>>([PublicWidgetZones.HeadHtmlTag, PublicWidgetZones.HomepageBottom]);
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