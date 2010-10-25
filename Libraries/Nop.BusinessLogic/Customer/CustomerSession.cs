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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.CustomerManagement
{
    /// <summary>
    /// Represents a customer session
    /// </summary>
    public partial class CustomerSession : BaseEntity
    {
        #region Fields
        private Customer _customer = null;
        #endregion 

        #region Ctor
        /// <summary>
        /// Creates a new instance of the CustomerSession class
        /// </summary>
        public CustomerSession()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the customer session identifier
        /// </summary>
        public Guid CustomerSessionGuid { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the last accessed date and time
        /// </summary>
        public DateTime LastAccessed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer session is expired
        /// </summary>
        public bool IsExpired { get; set; }
        #endregion

        #region Custom Properties
        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public Customer Customer
        {
            get
            {
                if (_customer == null)
                    _customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerById(this.CustomerId);
                return _customer;
            }
        }
        #endregion
    }

}
