using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Pusher
{
    /// <summary>
    /// Represents webhook message details
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the organization unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "organizationUuid")]
        public string OrganizationUuid { get; set; }

        /// <summary>
        /// Gets or sets the message unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "messageUuid")]
        public string MessageUuid { get; set; }

        /// <summary>
        /// Gets or sets the message id
        /// </summary>
        [JsonProperty(PropertyName = "messageId")]
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the event name
        /// </summary>
        [JsonProperty(PropertyName = "eventName")]
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the message payload
        /// </summary>
        [JsonProperty(PropertyName = "payload")]
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets the message timestamp
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }
    }
}