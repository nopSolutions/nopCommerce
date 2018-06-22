using System.Net;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Models;

namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    /// <summary>
    /// Represents request parameters to void a transaction.
    /// </summary>
    public class VoidRequest : WorldpayPaymentRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets an identifier for the transaction to be voided.
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets an amount to be voided. A partial void will be performed if the amount is lower than the authorized amount.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the void is merchant generated or system generated. 
        /// </summary>
        [JsonProperty("VoidType")]
        public VoidType? VoidType { get; set; }

        /// <summary>
        /// Gets or sets an additional data to assist in reporting, ecommerce or moto transactions, and level 2 or level 3 processing. 
        /// Includes user-defined fields and invoice-related information.
        /// </summary>
        [JsonProperty("extendedInformation")]
        public ExtendedInformation ExtendedInformation { get; set; }

        /// <summary>
        /// Gets or sets a client-generated unique ID for each transaction, used as a way to prevent the processing of duplicate transactions. 
        /// The orderId must be unique to the merchant's Worldpay ID; however, uniqueness is only evaluated for APPROVED transactions and only for the last 30 days. 
        /// If a transaction is declined, the corresponding orderId may be used again.
        /// The orderId is limited to 25 characters.
        /// </summary>
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get a request endpoint URL
        /// </summary>
        /// <returns>URL</returns>
        public override string GetRequestUrl() => "api/Payments/Void";

        /// <summary>
        /// Get a request method
        /// </summary>
        /// <returns>Request method</returns>
        public override string GetRequestMethod() => WebRequestMethods.Http.Post;

        #endregion
    }
}