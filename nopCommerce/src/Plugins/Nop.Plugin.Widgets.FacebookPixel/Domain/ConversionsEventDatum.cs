using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.FacebookPixel.Domain;

public class ConversionsEventDatum
{
    /// <summary>
    /// Gets or sets a meta pixel standard event or custom event name
    /// </summary>
    [JsonProperty(PropertyName = "event_name")]
    public string EventName { get; set; }

    /// <summary>
    /// Gets or sets a unix timestamp in seconds indicating when the actual event occurred
    /// </summary>
    [JsonProperty(PropertyName = "event_time")]
    public long EventTime { get; set; }

    /// <summary>
    /// Gets or sets the browser url where the event happened. the url must begin with http:// or https:// and should match the verified domain
    /// </summary>
    [JsonProperty(PropertyName = "event_source_url")]
    public string EventSourceUrl { get; set; }

    /// <summary>
    /// Gets or sets to specify where your conversions occurred
    /// </summary>
    [JsonProperty(PropertyName = "action_source")]
    public string ActionSource { get; set; }

    /// <summary>
    /// Gets or sets a store identifier
    /// </summary>
    [JsonIgnore]
    public int? StoreId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the event is custom
    /// </summary>
    [JsonIgnore]
    public bool IsCustomEvent { get; set; }

    /// <summary>
    /// Gets or sets a map that contains customer information data
    /// </summary>
    [JsonProperty(PropertyName = "user_data")]
    public ConversionsEventUserData UserData { get; set; }

    /// <summary>
    /// Gets or sets a map that includes additional business data about the event
    /// </summary>
    [JsonProperty(PropertyName = "custom_data")]
    public ConversionsEventCustomData CustomData { get; set; }
}