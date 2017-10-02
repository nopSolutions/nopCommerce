using System.Net;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Models;

namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    /// <summary>
    /// Represents request parameters to charge.
    /// </summary>
    public class ChargeRequest : WorldpayPaymentRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets an amount of the charge to be authorized.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a credit-card-specific data. 
        /// Required for credit card charges.
        /// </summary>
        [JsonProperty("card")]
        public Card Card { get; set; }

        /// <summary>
        /// Gets or sets a data from a Vault payment account. 
        /// Required for transactions where a Vault token is sent in the place of card or check data.
        /// </summary>
        [JsonProperty("paymentVaultToken")]
        public VaultToken PaymentVaultToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the card object is to be added to the vault to be stored as a token.
        /// </summary>
        [JsonProperty("addToVault")]
        public bool AddToVault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the card object is to be added to the vault to be stored as a token even if the attempted authorization is declined.
        /// </summary>
        [JsonProperty("addToVaultOnFailure")]
        public bool AddToVaultOnFailure { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is permissible to authorize less than the total balance available on a prepaid card.
        /// </summary>
        [JsonProperty("allowPartialCharges")]
        public bool AllowPartialChanges { get; set; }

        /// <summary>
        /// Gets or sets an additional data to assist in reporting, ecommerce or moto transactions, and level 2 or level 3 processing. 
        /// Includes user-defined fields and invoice-related information.
        /// </summary>
        [JsonProperty("extendedInformation")]
        public ExtendedInformation ExtendedInformation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how checks for duplicate transactions should behave. 
        /// Duplicates are evaluated on the basis of amount, card number, and order ID; these evaluation criteria can be extended to also include customer ID, invoice number, or a user-defined field. 
        /// The parameter must be enabled in the Virtual Terminal under Tools -> Duplicate Transactions. 
        /// Duplicates are checked only for approved transactions.
        /// </summary>
        [JsonProperty("transactionDuplicateCheckIndicator")]
        public TransactionDuplicateCheckType? TransactionDuplicateCheckType { get; set; }

        /// <summary>
        /// Gets or sets a client-generated unique ID for each transaction, used as a way to prevent the processing of duplicate transactions. 
        /// The orderId must be unique to the merchant's Worldpay ID; however, uniqueness is only evaluated for approved transactions and only for the last 30 days. 
        /// If a transaction is declined, the corresponding orderId may be used again.
        /// The orderId is limited to 25 characters.
        /// </summary>
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// Gets or sets a Visa Checkout details.
        /// </summary>
        [JsonProperty("wallet")]
        public VisaCheckoutData VisaCheckoutData { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get a request endpoint URL
        /// </summary>
        /// <returns>URL</returns>
        public override string GetRequestUrl() => "api/Payments/Charge";

        /// <summary>
        /// Get a request method
        /// </summary>
        /// <returns>Request method</returns>
        public override string GetRequestMethod() => WebRequestMethods.Http.Post;

        #endregion
    }
}