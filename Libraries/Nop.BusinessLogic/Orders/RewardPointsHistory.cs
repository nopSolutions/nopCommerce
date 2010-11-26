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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents a reward point history entry
    /// </summary>
    public partial class RewardPointsHistory : BaseEntity
    {
        #region Fields
        private Customer _customer;
        private Order _order;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reward point history entry identifier
        /// </summary>
        public int RewardPointsHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier (order for which points were used as a payment)
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the points redeemed/added
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the points balance
        /// </summary>
        public int PointsBalance { get; set; }

        /// <summary>
        /// Gets or sets the used amount
        /// </summary>
        public decimal UsedAmount { get; set; }

        /// <summary>
        /// Gets or sets the used amount (customer currency)
        /// </summary>
        public decimal UsedAmountInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the customer currency code
        /// </summary>
        public string CustomerCurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Customer Properties

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
        /// Gets the customer
        /// </summary>
        public virtual Customer NpCustomer { get; set; }

        #endregion
    }
}
