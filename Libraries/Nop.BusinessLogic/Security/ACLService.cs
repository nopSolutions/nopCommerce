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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Security
{
    /// <summary>
    /// ACL service
    /// </summary>
    public partial class ACLService : IACLService
    {
        #region Constants
        private const string CUSTOMERACTIONS_ALL_KEY = "Nop.customeraction.all";
        private const string CUSTOMERACTIONS_BY_ID_KEY = "Nop.customeraction.id-{0}";
        private const string CUSTOMERACTIONS_PATTERN_KEY = "Nop.customeraction.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache service
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public ACLService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        #region ACL

        /// <summary>
        /// Deletes a customer action
        /// </summary>
        /// <param name="customerActionId">Customer action identifier</param>
        public void DeleteCustomerAction(int customerActionId)
        {
            var customerAction = GetCustomerActionById(customerActionId);
            if (customerAction == null)
                return;

            
            if (!_context.IsAttached(customerAction))
                _context.CustomerActions.Attach(customerAction);
            _context.DeleteObject(customerAction);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CUSTOMERACTIONS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a customer action by identifier
        /// </summary>
        /// <param name="customerActionId">Customer action identifier</param>
        /// <returns>Customer action</returns>
        public CustomerAction GetCustomerActionById(int customerActionId)
        {
            if (customerActionId == 0)
                return null;

            string key = string.Format(CUSTOMERACTIONS_BY_ID_KEY, customerActionId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (CustomerAction)obj2;
            }

            
            var query = from ca in _context.CustomerActions
                        where ca.CustomerActionId == customerActionId
                        select ca;
            var customerAction = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, customerAction);
            }
            return customerAction;
        }

        /// <summary>
        /// Gets all customer actions
        /// </summary>
        /// <returns>Customer action collection</returns>
        public List<CustomerAction> GetAllCustomerActions()
        {
            string key = string.Format(CUSTOMERACTIONS_ALL_KEY);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<CustomerAction>)obj2;
            }

            
            var query = from ca in _context.CustomerActions
                        orderby ca.DisplayOrder, ca.Name
                        select ca;
            var customerActions = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, customerActions);
            }
            return customerActions;
        }

        /// <summary>
        /// Deletes an ACL
        /// </summary>
        /// <param name="aclId">ACL identifier</param>
        public void DeleteAcl(int aclId)
        {
            var acl = GetAclById(aclId);
            if (acl == null)
                return;

            
            if (!_context.IsAttached(acl))
                _context.ACL.Attach(acl);
            _context.DeleteObject(acl);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets an ACL by identifier
        /// </summary>
        /// <param name="aclId">ACL identifier</param>
        /// <returns>ACL</returns>
        public ACL GetAclById(int aclId)
        {
            if (aclId == 0)
                return null;

            
            var query = from a in _context.ACL
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
        public List<ACL> GetAllAcl(int customerActionId,
            int customerRoleId, bool? allow)
        {
            
            var query = (IQueryable<ACL>)_context.ACL;
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
        public void InsertAcl(ACL acl)
        {
            if (acl == null)
                throw new ArgumentNullException("acl");

            
            
            _context.ACL.AddObject(acl);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the ACL
        /// </summary>
        /// <param name="acl">ACL</param>
        public void UpdateAcl(ACL acl)
        {
            if (acl == null)
                throw new ArgumentNullException("acl");

            
            if (!_context.IsAttached(acl))
                _context.ACL.Attach(acl);

            _context.SaveChanges();
        }

        /// <summary>
        /// Indicates whether action is allowed
        /// </summary>
        /// <param name="actionSystemKeyword">Action system keyword</param>
        /// <returns>Result</returns>
        public bool IsActionAllowed(string actionSystemKeyword)
        {
            int userId = 0;
            if (NopContext.Current.User != null)
                userId = NopContext.Current.User.CustomerId;
            return IsActionAllowed(userId, actionSystemKeyword);
        }

        /// <summary>
        /// Indicates whether action is allowed
        /// </summary>
        /// <param name="customerId">Customer identifer</param>
        /// <param name="actionSystemKeyword">Action system keyword</param>
        /// <returns>Result</returns>
        public bool IsActionAllowed(int customerId, string actionSystemKeyword)
        {
            if (!this.Enabled)
                return true;

            

            var query = from c in _context.Customers
                        from cr in c.NpCustomerRoles
                        join acl in _context.ACL on cr.CustomerRoleId equals acl.CustomerRoleId
                        join ca in _context.CustomerActions on acl.CustomerActionId equals ca.CustomerActionId
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
        public void DeleteAclPerObject(int aclPerObjectId)
        {
            var aclPerObject = GetAclPerObjectById(aclPerObjectId);
            if (aclPerObject == null)
                return;

            
            if (!_context.IsAttached(aclPerObject))
                _context.ACLPerObject.Attach(aclPerObject);
            _context.DeleteObject(aclPerObject);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets an ACL per object entry by identifier
        /// </summary>
        /// <param name="aclPerObjectId">ACL per object entry identifier</param>
        /// <returns>ACL per object entry</returns>
        public ACLPerObject GetAclPerObjectById(int aclPerObjectId)
        {
            if (aclPerObjectId == 0)
                return null;

            
            var query = from a in _context.ACLPerObject
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
        public List<ACLPerObject> GetAllAclPerObject(int objectId, int objectTypeId,
            int customerRoleId, bool? deny)
        {
            
            var query = (IQueryable<ACLPerObject>)_context.ACLPerObject;
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
        public void InsertAclPerObject(ACLPerObject aclPerObject)
        {
            if (aclPerObject == null)
                throw new ArgumentNullException("aclPerObject");

            
            
            _context.ACLPerObject.AddObject(aclPerObject);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the ACL per object entry
        /// </summary>
        /// <param name="aclPerObject">ACL per object entry</param>
        public void UpdateAclPerObject(ACLPerObject aclPerObject)
        {
            if (aclPerObject == null)
                throw new ArgumentNullException("aclPerObject");

            
            if (!_context.IsAttached(aclPerObject))
                _context.ACLPerObject.Attach(aclPerObject);

            _context.SaveChanges();
        }

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating ACL feature is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("ACL.Enabled");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("ACL.Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.ACLManager.CacheEnabled");
            }
        }

        #endregion
    }
}