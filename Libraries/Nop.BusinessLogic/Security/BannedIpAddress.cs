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

namespace NopSolutions.NopCommerce.BusinessLogic.Security
{
    /// <summary>
    /// Network IP address implementation
    /// </summary>
    public partial class BannedIpAddress : BaseEntity
    {
        #region Constructor
        /// <summary>
        /// Default constructor. Creates a new instance of IpAddress
        /// </summary>
        public BannedIpAddress() { }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the IP address unique identifier
        /// </summary>
        public int BannedIpAddressId { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a reason why the IP was banned
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets when the IP address record was banned
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets when the banned IP address record was last updated
        /// </summary>
        public DateTime UpdatedOn { get; set; }
        #endregion
    }
}
