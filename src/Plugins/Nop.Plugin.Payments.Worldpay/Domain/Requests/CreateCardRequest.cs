using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Models;

namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    /// <summary>
    /// Represents request parameters to create a credit card in Vault.
    /// </summary>
    public class CreateCardRequest : WorldpayPostRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets a identifier for the customer whose payment method is being added.
        /// </summary>
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier (per customer), which can be used with customerId to reference the payment method.
        /// </summary>
        [JsonProperty("paymentMethodId")]
        public string PaymentId { get; set; }

        /// <summary>
        /// Gets or sets a credit-card-specific data for a card-based payment method. 
        /// Track data and security code information will not be stored in the Vault due to PCI regulations.
        /// </summary>
        [JsonProperty("card")]
        public Card Card { get; set; }

        /// <summary>
        /// Gets or sets a notes associated with the payment account.
        /// </summary>
        [JsonProperty("notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets a phone number of the customer.
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this payment method is to be the primary account for the customer. 
        /// If there is no primary payment method currently associated with the customer, this will default to true; otherwise it will default to false.
        /// </summary>
        [JsonProperty("primary")]
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets custom user-defined fields for reporting purposes.
        /// </summary>
        [JsonProperty("userDefinedFields")]
        public IList<KeyValuePair<string, string>> UserDefinedFields { get; set; }

        /// <summary>
        /// Gets o sets a value indicating how (and whether) the method should check for duplicate cards. 
        /// </summary>
        [JsonProperty("accountDuplicateCheckIndicator")]
        public CardDuplicateCheckType? CardDuplicateCheckType { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get a request endpoint URL
        /// </summary>
        /// <returns>URL</returns>
        public override string GetRequestUrl() => $"api/Customers/{CustomerId}/PaymentMethod";

        /// <summary>
        /// Get a request method
        /// </summary>
        /// <returns>Request method</returns>
        public override string GetRequestMethod() => WebRequestMethods.Http.Post;

        #endregion
    }
}