using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the webhook event notification
/// </summary>
public class Event<TResource> where TResource : IWebhookResource
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the webhook event notification.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the webhook event notification was created, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6).
    /// </summary>
    [JsonProperty(PropertyName = "create_time")]
    public string CreateTime { get; set; }

    /// <summary>
    /// Gets or sets the name of the resource related to the webhook notification event.
    /// </summary>
    [JsonProperty(PropertyName = "resource_type")]
    public string ResourceType { get; set; }

    /// <summary>
    /// Gets or sets the event version in the webhook notification.
    /// </summary>
    [JsonProperty(PropertyName = "event_version")]
    public string EventVersion { get; set; }

    /// <summary>
    /// Gets or sets the event that triggered the webhook event notification.
    /// </summary>
    [JsonProperty(PropertyName = "event_type")]
    public string EventType { get; set; }

    /// <summary>
    /// Gets or sets the summary description for the event notification. For example, `A payment authorization was created.`
    /// </summary>
    [JsonProperty(PropertyName = "summary")]
    public string Summary { get; set; }

    /// <summary>
    /// Gets or sets the resource version in the webhook notification.
    /// </summary>
    [JsonProperty(PropertyName = "resource_version")]
    public string ResourceVersion { get; set; }

    /// <summary>
    /// Gets or sets the resource that triggered the webhook event notification.
    /// </summary>
    [JsonProperty(PropertyName = "resource")]
    public TResource Resource { get; set; }

    /// <summary>
    /// Gets or sets the array of request-related [HATEOAS links](/docs/api/hateoas-links).
    /// </summary>
    [JsonProperty(PropertyName = "links")]
    public List<Link> Links { get; set; }

    #endregion
}