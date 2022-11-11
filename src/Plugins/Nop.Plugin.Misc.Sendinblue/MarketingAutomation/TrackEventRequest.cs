using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Sendinblue.MarketingAutomation
{
    /// <summary>
    /// Represents request to track an event
    /// </summary>
    public class TrackEventRequest : Request
    {
        /// <summary>
        /// Gets or sets the email address of the user
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the event name
        /// </summary>
        [JsonProperty(PropertyName = "event")]
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the object that contents all custom fields. Keep in mind that those user properties will populate your database on the Marketing Automation platform to create rich scenarios
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public object Properties { get; set; }

        /// <summary>
        /// Gets or sets the object that contents all additional data, it has three filed id of type string contain a unique number and data which contain information which which one can be send in smtp template
        /// </summary>
        [JsonProperty(PropertyName = "eventdata")]
        public object EventData { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "trackEvent";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;
    }
}