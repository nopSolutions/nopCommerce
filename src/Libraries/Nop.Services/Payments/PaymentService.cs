//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

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
