using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;

[JsonObject]
[Serializable]
public class Parameters
{
    /// <summary>
    /// Currency of the items associated with the event, in 3-letter ISO 4217 format
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; }

    /// <summary>
    /// The unique identifier of a transaction
    /// </summary>
    [JsonProperty("transaction_id")]
    public string TransactionId { get; set; }

    /// <summary>
    /// The unique identifier of a session
    /// </summary>
    [JsonProperty("session_id")]
    public string SessionId { get; set; }

    /// <summary>
    /// Engagement time
    /// </summary>
    [JsonProperty("engagement_time_msec")]
    public int EngagementTime { get; set; }

    /// <summary>
    /// The monetary value of the event
    /// </summary>
    [JsonProperty("value")]
    public decimal Value { get; set; }

    /// <summary>
    /// Shipping cost associated with a transaction
    /// </summary>
    [JsonProperty("shipping")]
    public decimal Shipping { get; set; }

    /// <summary>
    /// Tax cost associated with a transaction
    /// </summary>
    [JsonProperty("tax")]
    public decimal Tax { get; set; }

    /// <summary>
    /// The items for the event
    /// </summary>
    [JsonProperty("items")]
    public List<Item> Items { get; set; }
}