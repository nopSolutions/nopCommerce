using Nop.Core.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.AzureBlob.Services;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer : IConsumer<AppSettingsSavingEvent>
{
    #region Fields

    private readonly AppSettings _appSettings;

    #endregion

    #region Ctor

    public EventConsumer(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(AppSettingsSavingEvent eventMessage)
    {
        var config = _appSettings.Get<AzureBlobConfig>();

        string getKey(string name) => $"AzureBlobConfiguration.{name}";

        var form = eventMessage.FormData;

        if (form.TryGetValue(getKey(nameof(AzureBlobConfig.Enabled)), out var val))
            config.Enabled = val.Contains("true", StringComparer.InvariantCultureIgnoreCase);

        if (form.TryGetValue(getKey(nameof(AzureBlobConfig.StoreDataProtectionKeys)), out val))
            config.StoreDataProtectionKeys = val.Contains("true", StringComparer.InvariantCultureIgnoreCase);

        if (form.TryGetValue(getKey(nameof(AzureBlobConfig.AppendContainerName)), out val))
            config.AppendContainerName = val.Contains("true", StringComparer.InvariantCultureIgnoreCase);

        if (form.TryGetValue(getKey(nameof(AzureBlobConfig.AzureCacheControlHeader)), out val))
            config.AzureCacheControlHeader = val;

        if (form.TryGetValue(getKey(nameof(AzureBlobConfig.ConnectionString)), out val))
            config.ConnectionString = val;

        if (form.TryGetValue(getKey(nameof(AzureBlobConfig.ContainerName)), out val))
            config.ContainerName = val;

        if (form.TryGetValue(getKey(nameof(AzureBlobConfig.EndPoint)), out val))
            config.EndPoint = val;

        if (form.TryGetValue(getKey(nameof(AzureBlobConfig.DataProtectionKeysContainerName)), out val))
            config.DataProtectionKeysContainerName = val;

        if (form.TryGetValue(getKey(nameof(AzureBlobConfig.DataProtectionKeysVaultId)), out val))
            config.DataProtectionKeysVaultId = val;

        eventMessage.AddConfig(config);

        return Task.CompletedTask;
    }

    #endregion
}