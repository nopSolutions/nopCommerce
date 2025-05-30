using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.AzureBlob;

/// <summary>
/// Represents Azure Blob Storage plugin
/// </summary>
public class AzureBlobPlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INopFileProvider _fileProvider;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public AzureBlobPlugin(ILocalizationService localizationService,
        INopFileProvider fileProvider,
        ISettingService settingService,
        IWebHelper webHelper)
    {
        _localizationService = localizationService;
        _fileProvider = fileProvider;
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
        return $"{_webHelper.GetStoreLocation()}Admin/AzureBlob/Configure";
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //try to migrate settings from appsettings.json file
        var filePath = _fileProvider.MapPath(NopConfigurationDefaults.AppSettingsFilePath);
        var data = JsonConvert.DeserializeAnonymousType(await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8),
            new { AzureBlobConfig = new AzureBlobSettings() });

        AzureBlobSettings settings;

        if (data?.AzureBlobConfig != null)
            settings = data.AzureBlobConfig;
        else
            settings = new AzureBlobSettings
            {
                Enabled = false,
                AppendContainerName = true,
                EndPoint = string.Empty,
                ConnectionString = string.Empty,
                ContainerName = string.Empty
            };

        settings.AzureCacheControlHeader = await _settingService.GetSettingByKeyAsync<string>($"{nameof(MediaSettings)}.{nameof(AzureBlobSettings.AzureCacheControlHeader)}") ?? string.Empty;

        await _settingService.SaveSettingAsync(settings);

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Nop.Plugin.Misc.AzureBlob.Enabled"] = "Enabled",
            ["Nop.Plugin.Misc.AzureBlob.Enabled.Hint"] = "Enable this setting to use Azure Blob Storage",
            ["Nop.Plugin.Misc.AzureBlob.Title"] = "Azure Blob Storage configuration",
            ["Nop.Plugin.Misc.AzureBlob.ConnectionString"] = "Connection string",
            ["Nop.Plugin.Misc.AzureBlob.ConnectionString.Hint"] = "Specify the connection string for Azure Blob Storage.",
            ["Nop.Plugin.Misc.AzureBlob.ContainerName"] = "Container name",
            ["Nop.Plugin.Misc.AzureBlob.ContainerName.Hint"] = "Specify the container name for Azure Blob Storage.",
            ["Nop.Plugin.Misc.AzureBlob.EndPoint"] = "Endpoint",
            ["Nop.Plugin.Misc.AzureBlob.EndPoint.Hint"] = "Specify the endpoint for Azure Blob Storage.",
            ["Nop.Plugin.Misc.AzureBlob.AppendContainerName"] = "Append container name",
            ["Nop.Plugin.Misc.AzureBlob.AppendContainerName.Hint"] = "Enable this setting to append the endpoint with the container name when constructing the URL.",
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
        await _settingService.DeleteSettingAsync<AzureBlobSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Nop.Plugin.Misc.AzureBlob");
    }

    #endregion
}