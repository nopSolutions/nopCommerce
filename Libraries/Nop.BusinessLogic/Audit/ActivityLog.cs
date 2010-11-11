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
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Audit
{
    /// <summary>
    /// Represents an activity log record
    /// </summary>
    public partial class ActivityLog : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ActivityLog class
        /// </summary>
        public ActivityLog()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the activity log identifier
        /// </summary>
        public int ActivityLogId { get; set; }

        /// <summary>
        /// Gets or sets the activity log type identifier
        /// </summary>
        public int ActivityLogTypeId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the activity comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }
        #endregion

        #region Custom properties

        /// <summary>
        /// Gets the activity log type
        /// </summary>
        public ActivityLogType ActivityLogType
        {
            get
            {
                return IoCFactory.Resolve<ICustomerActivityService>().GetActivityTypeById(this.ActivityLogTypeId);
            }
        }

        /// <summary>
        /// Gers the customer
        /// </summary>
        public Customer Customer
        {
            get
            {
                return IoCFactory.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the activity log type
        /// </summary>
        public virtual ActivityLogType NpActivityLogType { get; set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer NpCustomer { get; set; }
        
        #endregion
    }
}
