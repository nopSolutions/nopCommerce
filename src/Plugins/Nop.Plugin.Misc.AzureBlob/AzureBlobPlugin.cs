using Nop.Core.Domain.Cms;
using Nop.Plugin.Misc.AzureBlob.Components;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.AzureBlob;
/// <summary>
/// Rename this file and change to the correct type
/// </summary>
public class AzureBlobPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public AzureBlobPlugin(ILocalizationService localizationService,
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
            ["Nop.Plugin.Misc.AzureBlob.Enabled"] = "Enabled",
            ["Nop.Plugin.Misc.AzureBlob.Enabled.Hint"] = "Enable this setting to use Azure Blob storage",
            ["Nop.Plugin.Misc.AzureBlob.AppSettings"] = "Azure Blob storage configuration",
            ["Nop.Plugin.Misc.AzureBlob.ConnectionString"] = "Connection string",
            ["Nop.Plugin.Misc.AzureBlob.ConnectionString.Hint"] = "Specify the connection string for Azure Blob storage.",
            ["Nop.Plugin.Misc.AzureBlob.ContainerName"] = "Container name",
            ["Nop.Plugin.Misc.AzureBlob.ContainerName.Hint"] = "Specify the container name for Azure Blob storage.",
            ["Nop.Plugin.Misc.AzureBlob.EndPoint"] = "Endpoint",
            ["Nop.Plugin.Misc.AzureBlob.EndPoint.Hint"] = "Specify the endpoint for Azure Blob storage.",
            ["Nop.Plugin.Misc.AzureBlob.AppendContainerName"] = "Append container name",
            ["Nop.Plugin.Misc.AzureBlob.AppendContainerName.Hint"] = "Enable this setting to append the endpoint with the container name when constructing the URL.",
            ["Nop.Plugin.Misc.AzureBlob.StoreDataProtectionKeys"] = "Store Data Protection keys",
            ["Nop.Plugin.Misc.AzureBlob.StoreDataProtectionKeys.Hint"] = "Enable this setting to store the Data Protection keys in Azure Blob Storage.",
            ["Nop.Plugin.Misc.AzureBlob.DataProtectionKeysContainerName"] = "Container name for Data Protection keys",
            ["Nop.Plugin.Misc.AzureBlob.DataProtectionKeysContainerName.Hint"] = "Specify the container name for the Data Protection keys. This should be a private container separate from the Blob container used for media storage.",
            ["Nop.Plugin.Misc.AzureBlob.DataProtectionKeysVaultId"] = "Key vault ID",
            ["Nop.Plugin.Misc.AzureBlob.DataProtectionKeysVaultId.Hint"] = "Specify the Azure key vault ID used to encrypt the Data Protection keys."
        });

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(AzureBlobDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(AzureBlobDefaults.SystemName);
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
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(AzureBlobDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(AzureBlobDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Nop.Plugin.Misc.AzureBlob");
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
        return typeof(AzureBlobConfigViewComponent);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    #endregion
}