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

namespace NopSolutions.NopCommerce.BusinessLogic.Audit
{
    /// <summary>
    /// Represents a log record
    /// </summary>
    public partial class Log : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the Log class
        /// </summary>
        public Log()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the log identifier
        /// </summary>
        public int LogId { get; set; }

        /// <summary>
        /// Gets or sets the log type identifier
        /// </summary>
        public int LogTypeId { get; set; }

        /// <summary>
        /// Gets or sets the severity
        /// </summary>
        public int Severity { get; set; }

        /// <summary>
        /// Gets or sets the short message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the full exception
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the page URL
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// Gets or sets the referrer URL
        /// </summary>
        public string ReferrerUrl { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Custom Properties

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
        /// Gets the log type
        /// </summary>
        public LogTypeEnum LogType
        {
            get
            {
                return (LogTypeEnum)this.LogTypeId;
            }
        }
        #endregion
    }
}
