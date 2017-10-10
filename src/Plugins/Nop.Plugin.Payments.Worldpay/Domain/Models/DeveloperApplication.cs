using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents information related to the integration.
    /// </summary>
    public class DeveloperApplication
    {
        /// <summary>
        /// Gets or sets the developer ID of integrator as assigned by Worldpay.
        /// </summary>
        [JsonProperty("developerId")]
        public int DeveloperId { get; set; }

        /// <summary>
        /// Gets or sets the version number of the integrator's application.
        /// </summary>
        [JsonProperty("version")]
        public string DeveloperVersion { get; set; }
    }
}