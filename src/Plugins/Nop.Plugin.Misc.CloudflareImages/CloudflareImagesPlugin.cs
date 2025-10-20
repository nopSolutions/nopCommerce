using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.CloudflareImages;

/// <summary>
/// Represents Cloudflare Images plugin
/// </summary>
public class CloudflareImagesPlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public CloudflareImagesPlugin(ILocalizationService localizationService,
        ISettingService settingService,
        IWebHelper webHelper)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/CloudflareImages/Configure";
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //settings
        await _settingService.SaveSettingAsync(new CloudflareImagesSettings
        {
            Enabled = false,
            AccessToken = string.Empty,
            AccountId = string.Empty,
            DeliveryUrl = string.Empty,
            RequestTimeout = CloudflareImagesDefaults.RequestTimeout
        });

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Nop.Plugin.Misc.CloudflareImages.Enabled"] = "Enabled",
            ["Nop.Plugin.Misc.CloudflareImages.Enabled.Hint"] = "Enable this setting to use Cloudflare Images",
            ["Nop.Plugin.Misc.CloudflareImages.Title"] = "Cloudflare Images configuration",
            ["Nop.Plugin.Misc.CloudflareImages.DeliveryUrl"] = "Delivery URL",
            ["Nop.Plugin.Misc.CloudflareImages.DeliveryUrl.Hint"] = "Specify the Image Delivery URL for Cloudflare Images. Normally it looks like https://imagedelivery.net/[account_hash]/<image_id>/<variant_name>. Please just copy it from Cloudflare and live as is.",
            ["Nop.Plugin.Misc.CloudflareImages.RequestTimeout"] = "Request timeout",
            ["Nop.Plugin.Misc.CloudflareImages.RequestTimeout.Hint"] = "Period (in seconds) before the request times out",
            ["Nop.Plugin.Misc.CloudflareImages.AccountId"] = "Account ID",
            ["Nop.Plugin.Misc.CloudflareImages.AccountId.Hint"] = "Cloudflare Images Account ID.",
            ["Nop.Plugin.Misc.CloudflareImages.AccessToken"] = "Access token",
            ["Nop.Plugin.Misc.CloudflareImages.AccessToken.Hint"] = "Cloudflare Images Access token."
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
        await _settingService.DeleteSettingAsync<CloudflareImagesSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Nop.Plugin.Misc.CloudflareImages");
    }

    #endregion
}