using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Models;

namespace Nop.Plugin.Payments.Worldpay.Domain.Responses
{
    /// <summary>
    /// Represents return values of get customer requests
    /// </summary>
    public class GetCustomerResponse : WorldpayResponse
    {
        /// <summary>
        /// Gets or sets a customer identifier.
        /// </summary>
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the retrieved customer record.
        /// </summary>
        [JsonProperty("vaultCustomer")]
        public VaultCustomer Customer { get; set; }
    }
}