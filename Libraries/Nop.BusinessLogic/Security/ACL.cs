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


namespace NopSolutions.NopCommerce.BusinessLogic.Security
{
    /// <summary>
    /// Represents an ACL
    /// </summary>
    public partial class ACL : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ACL class
        /// </summary>
        public ACL()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ACL identifier
        /// </summary>
        public int AclId { get; set; }

        /// <summary>
        /// Gets or sets the customer action identifier
        /// </summary>
        public int CustomerActionId { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        public int CustomerRoleId { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether action is allowed
        /// </summary>
        public bool Allow { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the customer action
        /// </summary>
        public virtual CustomerAction NpCustomerAction { get; set; }

        /// <summary>
        /// Gets the customer role
        /// </summary>
        public virtual CustomerRole NpCustomerRole { get; set; }

        #endregion
    }

}
