using System.Net;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    /// <summary>
    /// Represents request parameters to refund a transaction.
    /// </summary>
    public class RefundRequest : WorldpayPaymentRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets an identifier for the transaction to be refunded.
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets an amount to be refunded; needed only if the refund amount is less than the original authorization amount.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

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
        public override string GetRequestUrl() => "api/Payments/Refund";

        /// <summary>
        /// Get a request method
        /// </summary>
        /// <returns>Request method</returns>
        public override string GetRequestMethod() => WebRequestMethods.Http.Post;

        #endregion
    }
}