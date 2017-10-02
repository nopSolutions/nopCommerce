using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents an installment payment plan.
    /// </summary>
    public class InstallmentPaymentPlan : RecurringPaymentPlan
    {
        /// <summary>
        /// Gets or sets a total amount to be billed under the plan.
        /// </summary>
        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets a total number of installments in the plan.
        /// </summary>
        [JsonProperty("numberOfPayments")]
        public int NumberOfPayments { get; set; }

        /// <summary>
        /// Gets or sets an individual installment amount.
        /// </summary>
        [JsonProperty("installmentAmount")]
        public decimal InstallmentAmount { get; set; }

        /// <summary>
        /// Gets or sets a balloon amount.
        /// </summary>
        [JsonProperty("balloonAmount")]
        public decimal BalloonAmount { get; set; }

        /// <summary>
        /// Gets or sets an amount of the final payment.
        /// </summary>
        [JsonProperty("remainderAmount")]
        public decimal RemainderAmount { get; set; }
    }
}