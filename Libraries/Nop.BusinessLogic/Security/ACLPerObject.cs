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
    /// Represents an ACL per object entry
    /// </summary>
    public partial class ACLPerObject : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ACLPerObject class
        /// </summary>
        public ACLPerObject()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ACLPerObject identifier
        /// </summary>
        public int ACLPerObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object identifier (e.g. CategoryId or ManufacturerId)
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object type identifier (look at ObjectTypeEnum enum)
        /// </summary>
        public int ObjectTypeId { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        public int CustomerRoleId { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether action is not allowed
        /// </summary>
        public bool Deny { get; set; }

        #endregion

        #region Navigation Properties
        
        /// <summary>
        /// Gets the customer role
        /// </summary>
        public virtual CustomerRole NpCustomerRole { get; set; }

        #endregion

        #region Custom properties
        
        /// <summary>
        /// Gets the object type (e.g. Category or Manufacturer)
        /// </summary>
        public ObjectTypeEnum ObjectType
        {
            get
            {
                return (ObjectTypeEnum)this.ObjectTypeId;
            }
            set
            {
                this.ObjectTypeId = (int)value;
            }
        }

        #endregion
    }

}
