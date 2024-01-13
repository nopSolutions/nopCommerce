using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.What3words.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.What3words;

/// <summary>
/// Represents what3words plugin
/// </summary>
public class What3wordsPlugin : BasePlugin, IWidgetPlugin
{
    #region Fields

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly ILocalizationService _localizationService;
    protected readonly ISettingService _settingService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public What3wordsPlugin(IActionContextAccessor actionContextAccessor,
        ILocalizationService localizationService,
        ISettingService settingService,
        IUrlHelperFactory urlHelperFactory,
        WidgetSettings widgetSettings)
    {
        _actionContextAccessor = actionContextAccessor;
        _localizationService = localizationService;
        _settingService = settingService;
        _urlHelperFactory = urlHelperFactory;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget zones
    /// </returns>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            PublicWidgetZones.AddressBottom,
            PublicWidgetZones.OrderSummaryBillingAddress,
            PublicWidgetZones.OrderSummaryShippingAddress,
            PublicWidgetZones.OrderDetailsBillingAddress,
            PublicWidgetZones.OrderDetailsShippingAddress,

            AdminWidgetZones.OrderBillingAddressDetailsBottom,
            AdminWidgetZones.OrderShippingAddressDetailsBottom
        });
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(What3wordsDefaults.ConfigurationRouteName);
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        ArgumentNullException.ThrowIfNull(widgetZone);

        if (widgetZone.Equals(PublicWidgetZones.OrderSummaryBillingAddress) ||
            widgetZone.Equals(PublicWidgetZones.OrderSummaryShippingAddress) ||
            widgetZone.Equals(PublicWidgetZones.OrderDetailsBillingAddress) ||
            widgetZone.Equals(PublicWidgetZones.OrderDetailsShippingAddress))
        {
            return typeof(What3wordsOrderPublicViewComponent);
        }

        if (widgetZone.Equals(AdminWidgetZones.OrderBillingAddressDetailsBottom) ||
            widgetZone.Equals(AdminWidgetZones.OrderShippingAddressDetailsBottom))
        {
            return typeof(What3wordsOrderAdminViewComponent);
        }

        return typeof(What3wordsViewComponent);
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new What3wordsSettings());

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(What3wordsDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(What3wordsDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Widgets.What3words.Configuration"] = "Configuration",
            ["Plugins.Widgets.What3words.Configuration.Fields.Enabled"] = "Enabled",
            ["Plugins.Widgets.What3words.Configuration.Fields.Enabled.Hint"] = "Toggle to enable/disable what3words service.",
            ["Plugins.Widgets.What3words.Configuration.Failed"] = "Failed to get the generated API key",
            ["Plugins.Widgets.What3words.Address.Field.Label"] = "what3words address",
            ["Plugins.Widgets.What3words.Address.Field.Tooltip"] = "Is your property hard to find? To help your delivery driver find your exact location, please enter your what3words delivery address.",
            ["Plugins.Widgets.What3words.Address.Field.Tooltip.Link"] = "Find yours here"
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<What3wordsSettings>();
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(What3wordsDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(What3wordsDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.What3words");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    #endregion
}