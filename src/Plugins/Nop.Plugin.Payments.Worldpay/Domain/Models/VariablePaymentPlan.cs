using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a variable payment plan.
    /// </summary>
    public class VariablePaymentPlan : PaymentPlan
    {
        /// <summary>
        /// Gets or sets scheduled payments.
        /// </summary>
        [JsonProperty("scheduledPayments")]
        public IList<ScheduledPayment> ScheduledPayments { get; set; }
    }
}