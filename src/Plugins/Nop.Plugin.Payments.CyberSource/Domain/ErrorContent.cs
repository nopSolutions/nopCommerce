using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.CyberSource.Domain
{
    /// <summary>
    /// Represents error content
    /// </summary>
    public class ErrorContent
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the submit time UTC
        /// </summary>
        [JsonProperty(PropertyName = "submitTimeUtc")]
        public DateTime? SubmitTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the errors details
        /// </summary>
        [JsonProperty(PropertyName = "details")]
        public List<ErrorDetails> Details { get; set; }

        /// <summary>
        /// Represents error details
        /// </summary>
        public class ErrorDetails
        {
            /// <summary>
            /// Gets or sets the cfieldode
            /// </summary>
            [JsonProperty(PropertyName = "field")]
            public string Field { get; set; }

            /// <summary>
            /// Gets or sets the reason
            /// </summary>
            [JsonProperty(PropertyName = "Reason")]
            public string Reason { get; set; }
        }
    }
}