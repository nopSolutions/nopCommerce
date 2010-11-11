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
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents a return request
    /// </summary>
    public partial class ReturnRequest : BaseEntity
    {
        #region Fields
        private OrderProductVariant _opv = null;
        private Customer _customer = null;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of ReturnRequest class
        /// </summary>
        public ReturnRequest()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the return request identifier
        /// </summary>
        public int ReturnRequestId { get; set; }

        /// <summary>
        /// Gets or sets the order product variant identifier
        /// </summary>
        public int OrderProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the reason to return
        /// </summary>
        public string ReasonForReturn { get; set; }

        /// <summary>
        /// Gets or sets the requested action
        /// </summary>
        public string RequestedAction { get; set; }

        /// <summary>
        /// Gets or sets the customer comments
        /// </summary>
        public string CustomerComments { get; set; }

        /// <summary>
        /// Gets or sets the staff notes
        /// </summary>
        public string StaffNotes { get; set; }

        /// <summary>
        /// Gets or sets the return status identifier
        /// </summary>
        public int ReturnStatusId { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity update
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets or sets the return status
        /// </summary>
        public ReturnStatusEnum ReturnStatus
        {
            get
            {
                return (ReturnStatusEnum)this.ReturnStatusId;
            }
            set
            {
                this.ReturnStatusId = (int)value;
            }
        }

        /// <summary>
        /// Gets the order product variant
        /// </summary>
        public OrderProductVariant OrderProductVariant
        {
            get
            {
                if (_opv == null)
                    _opv = IoC.Resolve<IOrderService>().GetOrderProductVariantById(this.OrderProductVariantId);
                return _opv;
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

        #endregion
    }
}
