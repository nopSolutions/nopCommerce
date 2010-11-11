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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts
{
    /// <summary>
    /// Represents a discount usage history entry
    /// </summary>
    public partial class DiscountUsageHistory : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the DiscountUsageHistory class
        /// </summary>
        public DiscountUsageHistory()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the discount usage history entry identifier
        /// </summary>
        public int DiscountUsageHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the discount identifier
        /// </summary>
        public int DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }


        #endregion

        #region Customer Properties

        /// <summary>
        /// Gets the discount
        /// </summary>
        public Discount Discount
        {
            get
            {
                return IoC.Resolve<IDiscountService>().GetDiscountById(this.DiscountId);
            }
        }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public Customer Customer
        {
            get
            {
                return IoC.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);
            }
        }

        /// <summary>
        /// Gets the order
        /// </summary>
        public Order Order
        {
            get
            {
                return IoC.Resolve<IOrderService>().GetOrderById(this.OrderId);
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer NpCustomer { get; set; }

        /// <summary>
        /// Gets the discount
        /// </summary>
        public virtual Discount NpDiscount { get; set; }

        /// <summary>
        /// Gets the order
        /// </summary>
        public virtual Order NpOrder { get; set; }
        
        #endregion
    }
}
