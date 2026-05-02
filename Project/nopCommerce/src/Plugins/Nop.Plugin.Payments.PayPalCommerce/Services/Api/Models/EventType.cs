using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the event type
/// </summary>
public class EventType
{
    #region Properties

    /// <summary>
    /// Gets or sets the unique event name.
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the human-readable description of the event.
    /// </summary>
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the status of a webhook event.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the event type example: 1.0/2.0 etc.
    /// </summary>
    [JsonProperty(PropertyName = "resource_versions")]
    public List<string> ResourceVersions { get; set; }

    #endregion
}