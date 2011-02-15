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
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security.Permissions;
using Nop.Services.Security.Permissions;

namespace Nop.Services.Catalog
{
    public partial class CatalogPermissionProvider : IPermissionProvider
    {
        private const string PERMISSIONCATEGORY = "Catalog";

        public static readonly PermissionRecord ManageCategories = new PermissionRecord { Name = "Manage categories", SystemName = "ManageCategories", Category = PERMISSIONCATEGORY };
        public static readonly PermissionRecord ManageManufacturers = new PermissionRecord { Name = "Manage manufacturers", SystemName = "ManageManufacturers", Category = PERMISSIONCATEGORY };
        
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                ManageCategories,
                ManageManufacturers
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[] 
            {
                new DefaultPermissionRecord 
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
                    PermissionRecords = new[] {ManageCategories, ManageManufacturers}
                },
                new DefaultPermissionRecord //TODO remove this default permission (added for testing)
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Guests,
                    PermissionRecords = new[] {ManageCategories, ManageManufacturers}
                },
                new DefaultPermissionRecord //TODO remove this default permission (added for testing)
                {
                    CustomerRoleSystemName = "SomeCustomRoleSystemName",
                    PermissionRecords = new[] {ManageCategories}
                },
            };
        }
    }
}