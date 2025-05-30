using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the webhook
/// </summary>
public class Webhook
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the webhook.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the URL that is configured to listen on the server for incoming POST notification messages that contain event information.
    /// </summary>
    [JsonProperty(PropertyName = "url")]
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the array of events to which to subscribe your webhook. To subscribe to all events including events as they are added, specify the asterisk (`*`) wild card. To replace the `event_types` array, specify the `*` wild card. To list all supported events, [list available events](#available-event-type.list).
    /// </summary>
    [JsonProperty(PropertyName = "event_types")]
    public List<EventType> EventTypes { get; set; }

    /// <summary>
    /// Gets or sets the array of request-related [HATEOAS links](/docs/api/hateoas-links/).
    /// </summary>
    [JsonProperty(PropertyName = "links")]
    public List<Link> Links { get; set; }

    #endregion
}