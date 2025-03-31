using Microsoft.AspNetCore.Mvc;
using Nop.Core.Configuration;
using Nop.Plugin.Misc.CloudflareImages.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.CloudflareImages.Components;

/// <summary>
/// Represents a plugin view component to edit app settings
/// </summary>
public class CloudflareImagesConfigViewComponent : NopViewComponent
{
    #region Fields

    private readonly AppSettings _appSettings;

    #endregion

    #region Ctor

    public CloudflareImagesConfigViewComponent(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        var config = _appSettings.Get<CloudflareImagesConfig>();

        var model = new ConfigurationModel
        {
            CloudflareImagesConfiguration = new CloudflareImagesConfigurationModel
            {
                Enabled = config.Enabled,
                DeliveryUrl = config.DeliveryUrl,
                AccessToken = config.AccessToken,
                AccountId = config.AccountId,
                RequestTimeout = config.RequestTimeout
            }
        };

        return View("~/Plugins/Misc.CloudflareImages/Views/appSettings.cshtml", model);
    }

    #endregion
}