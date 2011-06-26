using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

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
                //added for testing
                //new DefaultPermissionRecord
                //{
                //    CustomerRoleSystemName = SystemCustomerRoleNames.Guests,
                //    PermissionRecords = new[] {ManageCategories, ManageManufacturers}
                //},
                //added for testing
                //new DefaultPermissionRecord
                //{
                //    CustomerRoleSystemName = "SomeCustomRoleSystemName",
                //    PermissionRecords = new[] {ManageCategories}
                //},
            };
        }
    }
}