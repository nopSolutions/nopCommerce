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
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents a recurring payment history
    /// </summary>
    public partial class RecurringPaymentHistory : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the RecurringPaymentHistory class
        /// </summary>
        public RecurringPaymentHistory()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the recurring payment history identifier
        /// </summary>
        public int RecurringPaymentHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the recurring payment identifier
        /// </summary>
        public int RecurringPaymentId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Custom Properties
        /// <summary>
        /// Gets the initial order
        /// </summary>
        public Order Order
        {
            get
            {
                return IoCFactory.Resolve<IOrderManager>().GetOrderById(this.OrderId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the recurring payment
        /// </summary>
        public virtual RecurringPayment NpRecurringPayment { get; set; }

        #endregion
    }
}
