

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Payment service interface
    /// </summary>
    public partial interface IPaymentService
    {
        /// <summary>
        /// Gets an additional handling fee of a payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Additional handling fee</returns>
        decimal GetAdditionalHandlingFee(string paymentMethodSystemName);
    }
}
