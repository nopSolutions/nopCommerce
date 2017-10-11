using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Models;

namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    /// <summary>
    /// Represents request parameters to create a customer in Vault.
    /// </summary>
    public class CreateCustomerRequest : WorldpayPostRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets a customer identifier.
        /// </summary>
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets o sets a first name of the customer.
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets o sets a last name of the customer.
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets a billing address of the customer.
        /// </summary>
        [JsonProperty("address")]
        public Address BillingAddress { get; set; }

        /// <summary>
        /// Gets o sets an email address of the customer.
        /// </summary>
        [JsonProperty("emailAddress")]
        public string Email { get; set; }

        /// <summary>
        /// Gets o sets a value indicating whether an email receipt should be sent to the customer whenever a transaction is completed.
        /// </summary>
        [JsonProperty("sendEmailReceipts")]
        public bool EmailReceiptEnabled { get; set; }

        /// <summary>
        /// Gets o sets a phone number of the customer.
        /// </summary>
        [JsonProperty("phoneNumber")]
        public string Phone { get; set; }

        /// <summary>
        /// Gets o sets a company of the customer.
        /// </summary>
        [JsonProperty("company")]
        public string Company { get; set; }

        /// <summary>
        /// Gets o sets a notes associated with the customer.
        /// </summary>
        [JsonProperty("notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets custom user-defined fields for reporting purposes.
        /// </summary>
        [JsonProperty("userDefinedFields")]
        public IList<KeyValuePair<string, string>> UserDefinedFields { get; set; }

        /// <summary>
        /// Gets o sets a value indicating how the method should behave if the Customer ID already exists.
        /// </summary>
        [JsonProperty("customerDuplicateCheckIndicator")]
        public CustomerDuplicateCheckType? CustomerDuplicateCheckType { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get a request endpoint URL
        /// </summary>
        /// <returns>URL</returns>
        public override string GetRequestUrl() => "api/Customers";

        /// <summary>
        /// Get a request method
        /// </summary>
        /// <returns>Request method</returns>
        public override string GetRequestMethod() => WebRequestMethods.Http.Post;

        #endregion
    }
}