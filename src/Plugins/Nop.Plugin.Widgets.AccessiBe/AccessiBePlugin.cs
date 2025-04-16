using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.AccessiBe.Components;
using Nop.Plugin.Widgets.AccessiBe.Domain;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.AccessiBe;

/// <summary>
/// Represents the accessiBe plugin
/// </summary>
public class AccessiBePlugin : BasePlugin, IWidgetPlugin
{
    #region Fields

    private readonly AccessiBeSettings _accessiBeSettings;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;
    private readonly IUrlHelperFactory _urlHelperFactory;


    #endregion

    #region Ctor

    public AccessiBePlugin(AccessiBeSettings accessiBeSettings,
        IActionContextAccessor actionContextAccessor,
        ILocalizationService localizationService,
        ISettingService settingService,
        IStoreService storeService,
        IUrlHelperFactory urlHelperFactory)
    {
        _accessiBeSettings = accessiBeSettings;
        _actionContextAccessor = actionContextAccessor;
        _localizationService = localizationService;
        _settingService = settingService;
        _storeService = storeService;
        _urlHelperFactory = urlHelperFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(AccessiBeDefaults.ConfigurationRouteName);
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
        return Task.FromResult<IList<string>>(new List<string> { _accessiBeSettings.WidgetZone });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        ArgumentNullException.ThrowIfNull(widgetZone);

        return typeof(AccessiBeViewComponent);
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new AccessiBeSettings
        {
            WidgetZone = PublicWidgetZones.BodyStartHtmlTagAfter,
            Script = @"<script>
                        (function(){
                            var s    = document.createElement('script');
                            var h    = document.querySelector('head') || document.body;
                            s.src    = 'https://acsbapp.com/apps/app/dist/js/app.js';
                            s.async  = true;
                            s.onload = function(){
                                acsbJS.init({WIDGET_CONFIG});
                            };
                            h.appendChild(s);
                        })();
                    </script>",

            LeadColor = "#146FF8",
            StatementLink = "",
            FooterHtml = "",
            HideMobile = true,
            HideTrigger = false,
            Language = "en",
            Position = TriggerHorizontalPosition.Right,
            TriggerColor = "#146FF8",
            TriggerPositionX = TriggerHorizontalPosition.Right,
            TriggerPositionY = TriggerVerticalPosition.Bottom,
            TriggerRadius = TriggerButtonShape.Round,
            TriggerIcon = TriggerIcon.People,
            TriggerSize = TriggerButtonSize.Medium,
            TriggerOffsetX = 20,
            TriggerOffsetY = 20
        });

        await _settingService.SaveSettingAsync(new AccessiBeMobileSettings
        {
            TriggerSize = TriggerButtonSize.Small,
            TriggerPositionX = TriggerHorizontalPosition.Right,
            TriggerPositionY = TriggerVerticalPosition.Bottom,
            TriggerOffsetX = 10,
            TriggerOffsetY = 10,
            TriggerRadius = TriggerButtonShape.Round
        });

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Widgets.AccessiBe.Fields.Enabled"] = "Enable",
            ["Plugins.Widgets.AccessiBe.Fields.Enabled.Hint"] = "Check to activate this widget.",

            ["Plugins.Widgets.AccessiBe.Fields.LeadColor"] = "Interface Lead Color",
            ["Plugins.Widgets.AccessiBe.Fields.LeadColor.Hint"] = "Set the main color using a color grid selector.",
            ["Plugins.Widgets.AccessiBe.Fields.StatementLink"] = "Accessibility Statement Link",
            ["Plugins.Widgets.AccessiBe.Fields.StatementLink.Hint"] = "Insert a valid URL linking to the site's accessibility statement.",
            ["Plugins.Widgets.AccessiBe.Fields.FooterHtml"] = "Interface Footer Content",
            ["Plugins.Widgets.AccessiBe.Fields.FooterHtml.Hint"] = "Add custom text for the interface footer (string).",
            ["Plugins.Widgets.AccessiBe.Fields.Language"] = "Interface Language",
            ["Plugins.Widgets.AccessiBe.Fields.Language.Hint"] = "Select from supported languages provided by accessWidget.",
            ["Plugins.Widgets.AccessiBe.Fields.Position"] = "Interface Position",
            ["Plugins.Widgets.AccessiBe.Fields.Position.Hint"] = "Choose interface alignment (Left/Right).",

            ["Plugins.Widgets.AccessiBe.Fields.TriggerColor"] = "Trigger Button Color",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerColor.Hint"] = "Select the color from a color grid.",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerPositionX"] = "Trigger Horizontal Position",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerPositionX.Hint"] = "Place the trigger on the Left or Right.",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerPositionY"] = "Trigger Vertical Position",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerPositionY.Hint"] = "Align the trigger to the Top, Center, or Bottom.",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerSize"] = "Trigger Button Size",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerSize.Hint"] = "Choose from Small, Medium, or Big.",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerShape"] = "Trigger Button Shape",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerShape.Hint"] = "Select from Round, Square, Squircle Big, or Squircle Small.",
            ["Plugins.Widgets.AccessiBe.Fields.HideTrigger"] = "Hide Trigger Button",
            ["Plugins.Widgets.AccessiBe.Fields.HideTrigger.Hint"] = "Option to Hide or Show the trigger button.",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerOffsetX"] = "Trigger Horizontal Offset",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerOffsetX.Hint"] = "Set custom horizontal offset (numeric value).",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerOffsetY"] = "Trigger Vertical Offset",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerOffsetY.Hint"] = "Set custom vertical offset (numeric value).",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerIcon"] = "Trigger Button Icon",
            ["Plugins.Widgets.AccessiBe.Fields.TriggerIcon.Hint"] = "Choose from 10 available icon options.",

            ["Plugins.Widgets.AccessiBe.Fields.ShowMobile"] = "Show on Mobile",
            ["Plugins.Widgets.AccessiBe.Fields.ShowMobile.Hint"] = "Toggle widget visibility on mobile.",

            ["Plugins.Widgets.AccessiBe.ScriptIsCustomized.Warning"] = "The widget settings contain a custom script, you cannot customize its appearance on this page, go to your accessiBe account.",

            ["Plugins.Widgets.AccessiBe.Notification.CloseLabel"] = "Close announcement",
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<AccessiBeSettings>();

        var stores = await _storeService.GetAllStoresAsync();
        var storeIds = new List<int> { 0 }.Union(stores.Select(store => store.Id));
        foreach (var storeId in storeIds)
        {
            var widgetSettings = await _settingService.LoadSettingAsync<WidgetSettings>(storeId);
            widgetSettings.ActiveWidgetSystemNames.Remove(AccessiBeDefaults.SystemName);
            await _settingService.SaveSettingAsync(widgetSettings);
        }

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.AccessiBe");

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