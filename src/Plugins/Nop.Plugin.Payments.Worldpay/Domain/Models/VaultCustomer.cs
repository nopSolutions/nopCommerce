using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a contents of the retrieved customer record.
    /// </summary>
    public class VaultCustomer
    {
        /// <summary>
        /// Gets or sets a customer identifier.
        /// </summary>
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets a billing address of the customer.
        /// </summary>
        [JsonProperty("address")]
        public Address BillingAddress { get; set; }

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
        /// Gets o sets an email address of the customer.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets o sets a value indicating whether an email receipt should be sent to the customer whenever a transaction is completed.
        /// </summary>
        [JsonProperty("emailReceipt")]
        public bool EmailReceiptEnabled { get; set; }

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
        /// Gets o sets all payment methods on file.
        /// </summary>
        [JsonProperty("paymentMethods")]
        public IList<PaymentMethod> PaymentMethods { get; set; }

        /// <summary>
        /// Gets o sets a primary payment method for the customer.
        /// </summary>
        [JsonProperty("primaryPaymentMethodId")]
        public string PrimaryPaymentMethodId { get; set; }

        /// <summary>
        /// Gets o sets all variable payment plans associated with the customer.
        /// </summary>
        [JsonProperty("variablePaymentPlans")]
        public IList<VariablePaymentPlan> VariablePaymentPlans { get; set; }

        /// <summary>
        /// Gets o sets all recurring payment plans associated with the customer.
        /// </summary>
        [JsonProperty("recurringPaymentPlans")]
        public IList<RecurringPaymentPlan> RecurringPaymentPlans { get; set; }

        /// <summary>
        /// Gets o sets all installment payment plans associated with the customer.
        /// </summary>
        [JsonProperty("installmentPaymentPlans")]
        public IList<InstallmentPaymentPlan> InstallmentPaymentPlans { get; set; }
    }
}