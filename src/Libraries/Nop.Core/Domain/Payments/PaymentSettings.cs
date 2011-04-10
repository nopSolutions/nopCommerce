
using Nop.Core.Configuration;
using System.Collections.Generic;

namespace Nop.Core.Domain.Payments
{
    public class PaymentSettings : ISettings
    {
        public PaymentSettings()
        {
            ActivePaymentMethodSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets an system names of active payment methods
        /// </summary>
        public List<string> ActivePaymentMethodSystemNames { get; set; }
    }
}