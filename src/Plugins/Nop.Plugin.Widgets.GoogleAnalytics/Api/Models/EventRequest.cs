using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api.Models
{
    [JsonObject]
    [Serializable]
    public class EventRequest
    {
        /// <summary>
        /// Required. Uniquely identifies a user instance of a web client
        /// </summary>
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Optional. A unique identifier for a user. See User-ID for cross-platform analysis for more information on this identifier
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Optional. A Unix timestamp (in microseconds) for the time to associate with the event
        /// </summary>
        [JsonProperty("timestamp_micros")]
        public long TimestampMicros { get; set; }

        /// <summary>
        /// Required. An array of event items. Up to 25 events can be sent per request. See the events reference for all valid events
        /// </summary>
        [JsonProperty("events")]
        public List<Event> Events { get; set; }

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public string Method => HttpMethods.Post;
    }
}
