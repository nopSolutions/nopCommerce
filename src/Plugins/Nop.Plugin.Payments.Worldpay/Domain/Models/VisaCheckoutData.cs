using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a Visa Checkout information.
    /// </summary>
    public class VisaCheckoutData
    {
        /// <summary>
        /// Gets or sets a Visa Checkout transaction ID associated with a payment request.
        /// </summary>
        [JsonProperty("callId")]
        public string CallId { get; set; }
    }
}