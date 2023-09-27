using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Pusher
{
    /// <summary>
    /// Represents the subscription details
    /// </summary>
    public class Subscription : ApiResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the message option (currently only WEBHOOK is supported)
        /// </summary>
        [JsonProperty(PropertyName = "transportName")]
        public string TransportName { get; set; }

        /// <summary>
        /// Gets or sets the event names to subscribe
        /// </summary>
        [JsonProperty(PropertyName = "eventNames")]
        public List<string> EventNames { get; set; }

        /// <summary>
        /// Gets or sets the publicly exposed service URL
        /// </summary>
        [JsonProperty(PropertyName = "destination")]
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the email address used to notify in case of any errors in subscription or the destination
        /// </summary>
        [JsonProperty(PropertyName = "contactEmail")]
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the status of the subscription
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the key used to verify that all incoming webhook messages originate from the service
        /// </summary>
        [JsonProperty(PropertyName = "signingKey")]
        public string SigningKey { get; set; }

        /// <summary>
        /// Gets or sets the date when the subscription was last updated
        /// </summary>
        [JsonProperty(PropertyName = "updated")]
        public DateTime? Updated { get; set; }
    }
}