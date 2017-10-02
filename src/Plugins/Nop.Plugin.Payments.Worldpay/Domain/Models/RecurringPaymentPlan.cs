using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Enums.Converters;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a recurring payment plan.
    /// </summary>
    public class RecurringPaymentPlan : PaymentPlan
    {
        /// <summary>
        /// Gets or sets an amount to be billed under the plan.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a time unit of billing cycle.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("cycleType")]
        public RecurringCycleType? CycleType { get; set; }

        /// <summary>
        /// Gets or sets a day of the month on which the plan is set to bill.
        /// </summary>
        [JsonProperty("dayOfTheMonth")]
        public int DayOfTheMonth { get; set; }

        /// <summary>
        /// Gets or sets a day of the week on which the plan is set to bill.
        /// </summary>
        [JsonProperty("dayOfTheWeek")]
        public int DayOfTheWeek { get; set; }

        /// <summary>
        /// Gets or sets a month the plan is set to bill.
        /// </summary>
        [JsonProperty("month")]
        public string Month { get; set; }

        /// <summary>
        /// Gets or sets a frequency (in terms of cycles) with which to bill the account. 
        /// If cycleType = MONTHLY: 1 - 11 (e.g., 2 = every 2 months) 
        /// if cycleType = WEEKLY: 1 - 51 (e.g., 6 = every 6 weeks)
        /// For cycleType = QUARTERLY, SEMI_ANNUALLY and ANNUALLY, frequency is set to 1 automatically.
        /// </summary>
        [JsonProperty("frequency")]
        public int Frequency { get; set; }
    }
}