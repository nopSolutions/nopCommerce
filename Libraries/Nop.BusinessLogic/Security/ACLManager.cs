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


namespace NopSolutions.NopCommerce.BusinessLogic.Security
{
    /// <summary>
    /// ACL manager
    /// </summary>
    public partial class ACLManager
    {
        #region Constants
        private const string CUSTOMERACTIONS_ALL_KEY = "Nop.customeraction.all";
        private const string CUSTOMERACTIONS_BY_ID_KEY = "Nop.customeraction.id-{0}";
        private const string CUSTOMERACTIONS_PATTERN_KEY = "Nop.customeraction.";
        #endregion

        #region Methods

        #region ACL

        /// <summary>
        /// Deletes a customer action
        /// </summary>
        /// <param name="customerActionId">Customer action identifier</param>
        public static void DeleteCustomerAction(int customerActionId)
        {
            var customerAction = GetCustomerActionById(customerActionId);
            if (customerAction == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customerAction))
                context.CustomerActions.Attach(customerAction);
            context.DeleteObject(customerAction);
            context.SaveChanges();

            if (ACLManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CUSTOMERACTIONS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a customer action by identifier
        /// </summary>
        /// <param name="customerActionId">Customer action identifier</param>
        /// <returns>Customer action</returns>
        public static CustomerAction GetCustomerActionById(int customerActionId)
        {
            if (customerActionId == 0)
                return null;

            string key = string.Format(CUSTOMERACTIONS_BY_ID_KEY, customerActionId);
            object obj2 = NopRequestCache.Get(key);
            if (ACLManager.CacheEnabled && (obj2 != null))
            {
                return (CustomerAction)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ca in context.CustomerActions
                        where ca.CustomerActionId == customerActionId
                        select ca;
            var customerAction = query.SingleOrDefault();

            if (ACLManager.CacheEnabled)
            {
                NopRequestCache.Add(key, customerAction);
            }
            return customerAction;
        }

        /// <summary>
        /// Gets all customer actions
        /// </summary>
        /// <returns>Customer action collection</returns>
        public static List<CustomerAction> GetAllCustomerActions()
        {
            string key = string.Format(CUSTOMERACTIONS_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (ACLManager.CacheEnabled && (obj2 != null))
            {
                return (List<CustomerAction>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ca in context.CustomerActions
                        orderby ca.DisplayOrder, ca.Name
                        select ca;
            var customerActions = query.ToList();

            if (ACLManager.CacheEnabled)
            {
                NopRequestCache.Add(key, customerActions);
            }
            return customerActions;
        }

        /// <summary>
        /// Deletes an ACL
        /// </summary>
        /// <param name="aclId">ACL identifier</param>
        public static void DeleteAcl(int aclId)
        {
            var acl = GetAclById(aclId);
            if (acl == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(acl))
                context.ACL.Attach(acl);
            context.DeleteObject(acl);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets an ACL by identifier
        /// </summary>
        /// <param name="aclId">ACL identifier</param>
        /// <returns>ACL</returns>
        public static ACL GetAclById(int aclId)
        {
            if (aclId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from a in context.ACL
                        where a.AclId == aclId
                        select a;
            var acl = query.SingleOrDefault();
            return acl;
        }

        /// <summary>
        /// Gets all ACL
        /// </summary>
        /// <param name="customerActionId">Customer action identifier; 0 to load all records</param>
        /// <param name="customerRoleId">Customer role identifier; 0 to load all records</param>
        /// <param name="allow">Value indicating whether action is allowed; null to load all records</param>
        /// <returns>ACL collection</returns>
        public static List<ACL> GetAllAcl(int customerActionId,
            int customerRoleId, bool? allow)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = (IQueryable<ACL>)context.ACL;
            if (customerActionId > 0)
                query = query.Where(a => a.CustomerActionId == customerActionId);
            if (customerRoleId > 0)
                query = query.Where(a => a.CustomerRoleId == customerRoleId);
            if (allow.HasValue)
                query = query.Where(a => a.Allow == allow.Value);
            query = query.OrderByDescending(a => a.AclId);

            var aclCollection = query.ToList();

            return aclCollection;
        }

        /// <summary>
        /// Inserts an ACL
        /// </summary>
        /// <param name="acl">ACL</param>
        public static void InsertAcl(ACL acl)
        {
            if (acl == null)
                throw new ArgumentNullException("acl");

            var context = ObjectContextHelper.CurrentObjectContext;
            
            context.ACL.AddObject(acl);
            context.SaveChanges();
        }

        /// <summary>
        /// Updates the ACL
        /// </summary>
        /// <param name="acl">ACL</param>
        public static void UpdateAcl(ACL acl)
        {
            if (acl == null)
                throw new ArgumentNullException("acl");

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(acl))
                context.ACL.Attach(acl);

            context.SaveChanges();
        }

        /// <summary>
        /// Indicates whether action is allowed
        /// </summary>
        /// <param name="actionSystemKeyword">Action system keyword</param>
        /// <returns>Result</returns>
        public static bool IsActionAllowed(string actionSystemKeyword)
        {
            int userId = 0;
            if (NopContext.Current != null &&
                NopContext.Current.User != null)
                userId = NopContext.Current.User.CustomerId;
            return IsActionAllowed(userId, actionSystemKeyword);
        }

        /// <summary>
        /// Indicates whether action is allowed
        /// </summary>
        /// <param name="customerId">Customer identifer</param>
        /// <param name="actionSystemKeyword">Action system keyword</param>
        /// <returns>Result</returns>
        public static bool IsActionAllowed(int customerId, string actionSystemKeyword)
        {
            if (!ACLManager.Enabled)
                return true;

            var context = ObjectContextHelper.CurrentObjectContext;

            var query = from c in context.Customers
                        from cr in c.NpCustomerRoles
                        join acl in context.ACL on cr.CustomerRoleId equals acl.CustomerRoleId
                        join ca in context.CustomerActions on acl.CustomerActionId equals ca.CustomerActionId
                        where c.CustomerId == customerId &&
                        !cr.Deleted &&
                        cr.Active &&
                        acl.Allow &&
                        ca.SystemKeyword == actionSystemKeyword
                        select acl;

            bool result = query.Count() > 0;
            return result;
        }

        #endregion

        #region ACL per object

        /// <summary>
        /// Deletes an ACL per object entry
        /// </summary>
        /// <param name="aclPerObjectId">ACL per object entry identifier</param>
        public static void DeleteAclPerObject(int aclPerObjectId)
        {
            var aclPerObject = GetAclPerObjectById(aclPerObjectId);
            if (aclPerObject == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(aclPerObject))
                context.ACLPerObject.Attach(aclPerObject);
            context.DeleteObject(aclPerObject);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets an ACL per object entry by identifier
        /// </summary>
        /// <param name="aclId">ACL per object entry identifier</param>
        /// <returns>ACL per object entry</returns>
        public static ACLPerObject GetAclPerObjectById(int aclPerObjectId)
        {
            if (aclPerObjectId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from a in context.ACLPerObject
                        where a.ACLPerObjectId == aclPerObjectId
                        select a;
            var aclPerObject = query.SingleOrDefault();
            return aclPerObject;
        }

        /// <summary>
        /// Gets all ACL per object entries
        /// </summary>
        /// <param name="objectId">Object identifier; 0 to load all records</param>
        /// <param name="objectTypeId">Object type identifier; 0 to load all records</param>
        /// <param name="customerRoleId">Customer role identifier; 0 to load all records</param>
        /// <param name="deny">Value indicating whether action is denied; null to load all records</param>
        /// <returns>ACL per object entries</returns>
        public static List<ACLPerObject> GetAllAclPerObject(int objectId, int objectTypeId,
            int customerRoleId, bool? deny)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = (IQueryable<ACLPerObject>)context.ACLPerObject;
            if (objectId > 0)
                query = query.Where(a => a.ObjectId == objectId);
            if (objectTypeId > 0)
                query = query.Where(a => a.ObjectTypeId == objectTypeId);
            if (customerRoleId > 0)
                query = query.Where(a => a.CustomerRoleId == customerRoleId);
            if (deny.HasValue)
                query = query.Where(a => a.Deny == deny.Value);
            query = query.OrderByDescending(a => a.ACLPerObjectId);

            var aclCollection = query.ToList();

            return aclCollection;
        }

        /// <summary>
        /// Inserts an ACL per object entry
        /// </summary>
        /// <param name="aclPerObject">ACL per object entry</param>
        public static void InsertAclPerObject(ACLPerObject aclPerObject)
        {
            if (aclPerObject == null)
                throw new ArgumentNullException("aclPerObject");

            var context = ObjectContextHelper.CurrentObjectContext;
            
            context.ACLPerObject.AddObject(aclPerObject);
            context.SaveChanges();
        }

        /// <summary>
        /// Updates the ACL per object entry
        /// </summary>
        /// <param name="aclPerObject">ACL per object entry</param>
        public static void UpdateAclPerObject(ACLPerObject aclPerObject)
        {
            if (aclPerObject == null)
                throw new ArgumentNullException("aclPerObject");

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(aclPerObject))
                context.ACLPerObject.Attach(aclPerObject);

            context.SaveChanges();
        }

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating ACL feature is enabled
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("ACL.Enabled");
            }
        }

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.ACLManager.CacheEnabled");
            }
        }

        #endregion
    }
}