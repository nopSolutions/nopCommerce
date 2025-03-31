using Microsoft.AspNetCore.Mvc;
using Nop.Core.Configuration;
using Nop.Plugin.Misc.AzureBlob.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.AzureBlob.Components;

/// <summary>
/// Represents a plugin view component to edit app settings
/// </summary>
public class AzureBlobConfigViewComponent : NopViewComponent
{
    #region Fields

    private readonly AppSettings _appSettings;

    #endregion

    #region Ctor

    public AzureBlobConfigViewComponent(AppSettings appSettings)
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
        var config = _appSettings.Get<AzureBlobConfig>();

        var model = new ConfigurationModel
        {
            AzureBlobConfiguration = new AzureBlobConfigurationModel
            {
                AppendContainerName = config.AppendContainerName,
                ConnectionString = config.ConnectionString,
                ContainerName = config.ContainerName,
                DataProtectionKeysContainerName = config.DataProtectionKeysContainerName,
                DataProtectionKeysVaultId = config.DataProtectionKeysVaultId,
                Enabled = config.Enabled,
                EndPoint = config.EndPoint,
                StoreDataProtectionKeys = config.StoreDataProtectionKeys
            }
        };

        return View("~/Plugins/Misc.AzureBlob/Views/appSettings.cshtml", model);
    }

    #endregion
}