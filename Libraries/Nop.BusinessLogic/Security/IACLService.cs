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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.BusinessLogic.Security
{
    /// <summary>
    /// ACL service interface
    /// </summary>
    public partial interface IACLService
    {
        #region ACL

        /// <summary>
        /// Deletes a customer action
        /// </summary>
        /// <param name="customerActionId">Customer action identifier</param>
        void DeleteCustomerAction(int customerActionId);

        /// <summary>
        /// Gets a customer action by identifier
        /// </summary>
        /// <param name="customerActionId">Customer action identifier</param>
        /// <returns>Customer action</returns>
        CustomerAction GetCustomerActionById(int customerActionId);

        /// <summary>
        /// Gets all customer actions
        /// </summary>
        /// <returns>Customer action collection</returns>
        List<CustomerAction> GetAllCustomerActions();

        /// <summary>
        /// Deletes an ACL
        /// </summary>
        /// <param name="aclId">ACL identifier</param>
        void DeleteAcl(int aclId);

        /// <summary>
        /// Gets an ACL by identifier
        /// </summary>
        /// <param name="aclId">ACL identifier</param>
        /// <returns>ACL</returns>
        ACL GetAclById(int aclId);

        /// <summary>
        /// Gets all ACL
        /// </summary>
        /// <param name="customerActionId">Customer action identifier; 0 to load all records</param>
        /// <param name="customerRoleId">Customer role identifier; 0 to load all records</param>
        /// <param name="allow">Value indicating whether action is allowed; null to load all records</param>
        /// <returns>ACL collection</returns>
        List<ACL> GetAllAcl(int customerActionId,
            int customerRoleId, bool? allow);

        /// <summary>
        /// Inserts an ACL
        /// </summary>
        /// <param name="acl">ACL</param>
        void InsertAcl(ACL acl);

        /// <summary>
        /// Updates the ACL
        /// </summary>
        /// <param name="acl">ACL</param>
        void UpdateAcl(ACL acl);

        /// <summary>
        /// Indicates whether action is allowed
        /// </summary>
        /// <param name="actionSystemKeyword">Action system keyword</param>
        /// <returns>Result</returns>
        bool IsActionAllowed(string actionSystemKeyword);

        /// <summary>
        /// Indicates whether action is allowed
        /// </summary>
        /// <param name="customerId">Customer identifer</param>
        /// <param name="actionSystemKeyword">Action system keyword</param>
        /// <returns>Result</returns>
        bool IsActionAllowed(int customerId, string actionSystemKeyword);

        #endregion

        #region ACL per object

        /// <summary>
        /// Deletes an ACL per object entry
        /// </summary>
        /// <param name="aclPerObjectId">ACL per object entry identifier</param>
        void DeleteAclPerObject(int aclPerObjectId);

        /// <summary>
        /// Gets an ACL per object entry by identifier
        /// </summary>
        /// <param name="aclId">ACL per object entry identifier</param>
        /// <returns>ACL per object entry</returns>
        ACLPerObject GetAclPerObjectById(int aclPerObjectId);

        /// <summary>
        /// Gets all ACL per object entries
        /// </summary>
        /// <param name="objectId">Object identifier; 0 to load all records</param>
        /// <param name="objectTypeId">Object type identifier; 0 to load all records</param>
        /// <param name="customerRoleId">Customer role identifier; 0 to load all records</param>
        /// <param name="deny">Value indicating whether action is denied; null to load all records</param>
        /// <returns>ACL per object entries</returns>
        List<ACLPerObject> GetAllAclPerObject(int objectId, int objectTypeId,
            int customerRoleId, bool? deny);

        /// <summary>
        /// Inserts an ACL per object entry
        /// </summary>
        /// <param name="aclPerObject">ACL per object entry</param>
        void InsertAclPerObject(ACLPerObject aclPerObject);

        /// <summary>
        /// Updates the ACL per object entry
        /// </summary>
        /// <param name="aclPerObject">ACL per object entry</param>
        void UpdateAclPerObject(ACLPerObject aclPerObject);

        #endregion

        /// <summary>
        /// Gets a value indicating ACL feature is enabled
        /// </summary>
        bool Enabled { get; set; }
    }
}