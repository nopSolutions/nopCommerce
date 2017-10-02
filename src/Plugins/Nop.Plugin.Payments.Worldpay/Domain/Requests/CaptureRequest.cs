using System.Net;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Models;

namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    /// <summary>
    /// Represents request parameters to capture a transaction.
    /// </summary>
    public class CaptureRequest : WorldpayPaymentRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets an identifier of the previously authorized transaction to be captured.
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets a final amount of the transaction.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets an additional data to assist in reporting, ecommerce or moto transactions, and level 2 or level 3 processing. 
        /// Includes user-defined fields and invoice-related information.
        /// If a gratuity is to be added to the previously authorized amount, it can be sent in the serviceData object field.
        /// </summary>
        [JsonProperty("extendedInformation")]
        public ExtendedInformation ExtendedInformation { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get a request endpoint URL
        /// </summary>
        /// <returns>URL</returns>
        public override string GetRequestUrl() => "api/Payments/Capture";

        /// <summary>
        /// Get a request method
        /// </summary>
        /// <returns>Request method</returns>
        public override string GetRequestMethod() => WebRequestMethods.Http.Post;

        #endregion
    }
}
