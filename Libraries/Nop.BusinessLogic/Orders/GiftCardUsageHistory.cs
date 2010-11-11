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


namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents a gift card usage history entry
    /// </summary>
    public partial class GiftCardUsageHistory : BaseEntity
    {
        #region Fields
        private GiftCard _gc = null;
        private Customer _customer = null;
        private Order _order = null;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the GiftCardUsageHistory class
        /// </summary>
        public GiftCardUsageHistory()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the gift card usage history entry identifier
        /// </summary>
        public int GiftCardUsageHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the gift card identifier
        /// </summary>
        public int GiftCardId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the used value (amount)
        /// </summary>
        public decimal UsedValue { get; set; }

        /// <summary>
        /// Gets or sets the used value (customer currency)
        /// </summary>
        public decimal UsedValueInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Customer Properties

        /// <summary>
        /// Gets the gift card
        /// </summary>
        public GiftCard GiftCard
        {
            get
            {
                if (_gc == null)
                    _gc = IoC.Resolve<IOrderService>().GetGiftCardById(this.GiftCardId);
                return _gc;
            }
        }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public Customer Customer
        {
            get
            {
                if (_customer == null)
                    _customer = IoC.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);
                return _customer;
            }
        }

        /// <summary>
        /// Gets the order
        /// </summary>
        public Order Order
        {
            get
            {
                if (_order == null)
                    _order = IoC.Resolve<IOrderService>().GetOrderById(this.OrderId);
                return _order;
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the gift card
        /// </summary>
        public virtual GiftCard NpGiftCard { get; set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer NpCustomer { get; set; }
        
        #endregion

        
    }
}
