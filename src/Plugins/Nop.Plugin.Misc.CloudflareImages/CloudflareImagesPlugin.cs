using Nop.Core.Domain.Cms;
using Nop.Plugin.Misc.CloudflareImages.Components;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.CloudflareImages;
/// <summary>
/// Rename this file and change to the correct type
/// </summary>
public class CloudflareImagesPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public CloudflareImagesPlugin(ILocalizationService localizationService,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Nop.Plugin.Misc.CloudflareImages.Enabled"] = "Enabled",
            ["Nop.Plugin.Misc.CloudflareImages.Enabled.Hint"] = "Enable this setting to use Cloudflare images",
            ["Nop.Plugin.Misc.CloudflareImages.AppSettings"] = "Cloudflare images storage configuration",
            ["Nop.Plugin.Misc.CloudflareImages.DeliveryUrl"] = "Delivery URL",
            ["Nop.Plugin.Misc.CloudflareImages.DeliveryUrl.Hint"] = "Specify the Image Delivery URL for Cloudflare images. Normally it look like https://imagedelivery.net/8gpWmT5e4kJqST1MOqpoVg/<image_id>/<variant_name>. Please just copy it from Cloudflare and live as is.",
            ["Nop.Plugin.Misc.CloudflareImages.RequestTimeout"] = "Request timeout",
            ["Nop.Plugin.Misc.CloudflareImages.RequestTimeout.Hint"] = "Period (in seconds) before the request times out",
            ["Nop.Plugin.Misc.CloudflareImages.AccountId"] = "Account ID",
            ["Nop.Plugin.Misc.CloudflareImages.AccountId.Hint"] = "Cloudflare images Account ID.",
            ["Nop.Plugin.Misc.CloudflareImages.AccessToken"] = "Access token",
            ["Nop.Plugin.Misc.CloudflareImages.AccessToken.Hint"] = "Cloudflare images Access token."
        });

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(CloudflareImagesDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(CloudflareImagesDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(CloudflareImagesDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(CloudflareImagesDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Nop.Plugin.Misc.CloudflareImages");
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
        return Task.FromResult<IList<string>>(new List<string> { AdminWidgetZones.AppSettingsBlock });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(CloudflareImagesConfigViewComponent);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    #endregion
}