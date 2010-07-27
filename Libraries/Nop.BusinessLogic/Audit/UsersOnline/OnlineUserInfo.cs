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

namespace NopSolutions.NopCommerce.BusinessLogic.Audit.UsersOnline
{
    /// <summary>
    /// Represents an online user info
    /// </summary>
    public partial class OnlineUserInfo
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the OnlineUserInfo class
        /// </summary>
        public OnlineUserInfo()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        public Guid OnlineUserGuid { get; set; }

        /// <summary>
        /// Gets or sets the associated customer identifier (if he exists)
        /// </summary>
        public int? AssociatedCustomerId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last visit date and time
        /// </summary>
        public DateTime LastVisit { get; set; }

        #endregion
    }
}
