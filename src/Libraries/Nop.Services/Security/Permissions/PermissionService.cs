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
using System.ComponentModel;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Core.Domain.Security.Permissions;
using Nop.Services.Customers;

namespace Nop.Services.Security.Permissions
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Fields

        private readonly IRepository<PermissionRecord> _permissionPecordRepository;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="permissionPecordRepository">Permission repository</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="workContext">Work context</param>
        public PermissionService(IRepository<PermissionRecord> permissionPecordRepository,
            ICustomerService customerService,
            IWorkContext workContext)
        {
            this._permissionPecordRepository = permissionPecordRepository;
            this._customerService = customerService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public void DeletePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                return;

            //clear many-to-many navigation property because EF doesn't allow to configure cascade delete for this type of associations
            permission.CustomerRoles.Clear();

            _permissionPecordRepository.Delete(permission);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        public PermissionRecord GetPermissionRecordById(int permissionId)
        {
            if (permissionId == 0)
                return null;

            return _permissionPecordRepository.GetById(permissionId);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        public PermissionRecord GetPermissionRecordBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in _permissionPecordRepository.Table
                        orderby pr.Id
                        where pr.SystemName == systemName
                        select pr;
            var permission = query.FirstOrDefault();
            return permission;
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public IList<PermissionRecord> GetAllPermissionRecords()
        {
            var query = from cr in _permissionPecordRepository.Table
                        orderby cr.Category, cr.Name
                        select cr;
            var permissions = query.ToList();
            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public void InsertPermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionPecordRepository.Insert(permission);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public void UpdatePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionPecordRepository.Update(permission);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public void InstallPermissions(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 == null)
                {
                    //new permission (install it)
                    permission1 = new PermissionRecord()
                    {
                        Name = permission.Name,
                        SystemName = permission.SystemName,
                        Category = permission.Category,
                    };


                    //default customer role mappings
                    var defaultPermissions = permissionProvider.GetDefaultPermissions();
                    foreach (var defaultPermission in defaultPermissions)
                    {
                        var customerRole = _customerService.GetCustomerRoleBySystemName(defaultPermission.CustomerRoleSystemName);
                        if (customerRole == null)
                        {
                            //new role (save it)
                            customerRole = new CustomerRole()
                            {
                                Name = defaultPermission.CustomerRoleSystemName,
                                Active = true,
                                SystemName = defaultPermission.CustomerRoleSystemName
                            };
                            _customerService.InsertCustomerRole(customerRole);
                        }


                        var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                             where p.SystemName == permission1.SystemName
                                                    select p).Any();
                        var mappingExists = (from p in customerRole.PermissionRecords
                                                    where p.SystemName == permission1.SystemName
                                                    select p).Any();
                        if (defaultMappingProvided && !mappingExists)
                        {
                            permission1.CustomerRoles.Add(customerRole);
                        }
                    }

                    //save new permission
                    InsertPermissionRecord(permission1);
                }
            }
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public bool Authorize(string permissionRecordSystemName)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var permission = GetPermissionRecordBySystemName(permissionRecordSystemName);
            return Authorize(permission);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public bool Authorize(PermissionRecord permission)
        {
            if (permission == null)
                return false;

            if (_workContext.CurrentCustomer == null)
                return false;

            var customerRoles = _workContext.CurrentCustomer.CustomerRoles;
            foreach (var role in customerRoles)
                foreach (var permission1 in role.PermissionRecords)
                    if (permission1.SystemName.Equals(permission.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

            return false;
        }
        #endregion
    }
}