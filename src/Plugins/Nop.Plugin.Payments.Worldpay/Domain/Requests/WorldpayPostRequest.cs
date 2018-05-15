using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Models;

namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    /// <summary>
    /// Represents base parameters for Worldpay POST, PUT and DELETE requests.
    /// </summary>
    public abstract class WorldpayPostRequest : WorldpayRequest
    {
        /// <summary>
        /// Gets or sets a developer Id and version information related to the integration.
        /// </summary>
        [JsonProperty("developerApplication")]
        public DeveloperApplication DeveloperApplication { get; set; }
    }
}