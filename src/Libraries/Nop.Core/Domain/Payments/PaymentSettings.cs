using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Payments
{
    /// <summary>
    /// Payment settings
    /// </summary>
    public class PaymentSettings : ISettings
    {
        public PaymentSettings()
        {
            ActivePaymentMethodSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets a system names of active payment methods
        /// </summary>
        public List<string> ActivePaymentMethodSystemNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to repost (complete) payments for redirection payment methods
        /// </summary>
        public bool AllowRePostingPayments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should bypass 'select payment method' page if we have only one payment method
        /// </summary>
        public bool BypassPaymentMethodSelectionIfOnlyOne { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show payment method descriptions on "choose payment method" checkout page in the public store
        /// </summary>
        public bool ShowPaymentMethodDescriptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should skip 'payment info' page for redirection payment methods
        /// </summary>
        public bool SkipPaymentInfoStepForRedirectionPaymentMethods { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the recurring payment after failed last payment 
        /// </summary>
        public bool CancelRecurringPaymentsAfterFailedPayment { get; set; }
    }
}