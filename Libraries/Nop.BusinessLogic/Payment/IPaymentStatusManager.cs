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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Payment status manager interface
    /// </summary>
    public partial interface IPaymentStatusManager
    {
        /// <summary>
        /// Gets a payment status full name
        /// </summary>
        /// <param name="paymentStatusId">Payment status identifier</param>
        /// <returns>Payment status name</returns>
        string GetPaymentStatusName(int paymentStatusId);

        /// <summary>
        /// Gets a payment status by identifier
        /// </summary>
        /// <param name="paymentStatusId">payment status identifier</param>
        /// <returns>Payment status</returns>
        PaymentStatus GetPaymentStatusById(int paymentStatusId);

        /// <summary>
        /// Gets all payment statuses
        /// </summary>
        /// <returns>Payment status collection</returns>
        List<PaymentStatus> GetAllPaymentStatuses();
    }
}
