using System.Net;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    public class GetCustomerRequest : WorldpayRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets a customer identifier.
        /// </summary>
        [JsonIgnore]
        public string CustomerId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get a request endpoint URL
        /// </summary>
        /// <returns>URL</returns>
        public override string GetRequestUrl() => $"api/Customers/{CustomerId}";

        /// <summary>
        /// Get a request method
        /// </summary>
        /// <returns>Request method</returns>
        public override string GetRequestMethod() => WebRequestMethods.Http.Get;

        #endregion
    }
}