using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a scheduled payment.
    /// </summary>
    public class ScheduledPayment
    {
        /// <summary>
        /// Gets or sets an amount of the scheduled payment.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a scheduled date of the payment.
        /// </summary>
        [JsonProperty("scheduledDate")]
        public DateTime? ScheduledDate { get; set; }

        /// <summary>
        /// Gets or sets a number of times to retry if the payment is declined.
        /// </summary>
        [JsonProperty("numberOfRetries")]
        public int NumberOfRetries { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payment has been made.
        /// </summary>
        [JsonProperty("paid")]
        public bool IsPaid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payment has been processed.
        /// </summary>
        [JsonProperty("processed")]
        public bool IsProcessed { get; set; }

        /// <summary>
        /// Gets or sets a date of the completed payment.
        /// </summary>
        [JsonProperty("paymentDate")]
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// Gets or sets a payment method to be charged.
        /// </summary>
        [JsonProperty("paymentMethodId")]
        public string PaymentMethodId { get; set; }

        /// <summary>
        /// Gets or sets a plan identifier
        /// </summary>
        [JsonProperty("planId")]
        public int PlanId { get; set; }

        /// <summary>
        /// Gets or sets a scheduleId
        /// </summary>
        [JsonProperty("scheduleId")]
        public int ScheduleId { get; set; }

        /// <summary>
        /// Gets or sets a transaction ID of the completed payment.
        /// </summary>
        [JsonProperty("transactionId")]
        public int TransactionId { get; set; }
    }
}