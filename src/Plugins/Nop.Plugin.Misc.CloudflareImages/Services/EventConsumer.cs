using Nop.Core.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.CloudflareImages.Services;

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
        var config = _appSettings.Get<CloudflareImagesConfig>();

        string getKey(string name) => $"CloudflareImagesConfiguration.{name}";

        var form = eventMessage.FormData;

        if (form.TryGetValue(getKey(nameof(CloudflareImagesConfig.Enabled)), out var val))
            config.Enabled = val.Contains("true", StringComparer.InvariantCultureIgnoreCase);

        if (form.TryGetValue(getKey(nameof(CloudflareImagesConfig.DeliveryUrl)), out val))
            config.DeliveryUrl = val;

        if (form.TryGetValue(getKey(nameof(CloudflareImagesConfig.AccessToken)), out val))
            config.AccessToken = val;

        if (form.TryGetValue(getKey(nameof(CloudflareImagesConfig.AccountId)), out val))
            config.AccountId = val;

        if (form.TryGetValue(getKey(nameof(CloudflareImagesConfig.RequestTimeout)), out val) && int.TryParse(val, out var result))
            config.RequestTimeout = result;

        eventMessage.AddConfig(config);

        return Task.CompletedTask;
    }

    #endregion
}