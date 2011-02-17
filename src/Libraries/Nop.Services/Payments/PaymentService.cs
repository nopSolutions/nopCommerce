

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Payment service
    /// </summary>
    public partial class PaymentService : IPaymentService
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public PaymentService()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an additional handling fee of a payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(string paymentMethodSystemName)
        {
            //TODO implement
            //don't throw exception if not found
            decimal result = decimal.Zero;
            if (result < decimal.Zero)
                result = decimal.Zero;
            result = Math.Round(result, 2);
            return result;
        }

        #endregion
    }
}
