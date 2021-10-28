using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;

namespace Nop.Services.Security
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Fields

        protected ICustomerService CustomerService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IRepository<PermissionRecord> PermissionRecordRepository { get; }
        protected IRepository<PermissionRecordCustomerRoleMapping> PermissionRecordCustomerRoleMappingRepository { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public PermissionService(ICustomerService customerService,
            ILocalizationService localizationService,
            IRepository<PermissionRecord> permissionRecordRepository,
            IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            CustomerService = customerService;
            LocalizationService = localizationService;
            PermissionRecordRepository = permissionRecordRepository;
            PermissionRecordCustomerRoleMappingRepository = permissionRecordCustomerRoleMappingRepository;
            StaticCacheManager = staticCacheManager;
            WorkContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get permission records by customer role identifier
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permissions
        /// </returns>
        protected virtual async Task<IList<PermissionRecord>> GetPermissionRecordsByCustomerRoleIdAsync(int customerRoleId)
        {
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.PermissionRecordsAllCacheKey, customerRoleId);

            var query = from pr in PermissionRecordRepository.Table
                join prcrm in PermissionRecordCustomerRoleMappingRepository.Table on pr.Id equals prcrm
                    .PermissionRecordId
                where prcrm.CustomerRoleId == customerRoleId
                orderby pr.Id
                select pr;

            return await StaticCacheManager.GetAsync(key, async ()=> await query.ToListAsync());
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permission
        /// </returns>
        protected virtual async Task<PermissionRecord> GetPermissionRecordBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in PermissionRecordRepository.Table
                where pr.SystemName == systemName
                orderby pr.Id
                select pr;

            var permissionRecord = await query.FirstOrDefaultAsync();
            return permissionRecord;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permissions
        /// </returns>
        public virtual async Task<IList<PermissionRecord>> GetAllPermissionRecordsAsync()
        {
            var permissions = await PermissionRecordRepository.GetAllAsync(query =>
            {
                return from pr in query
                    orderby pr.Name
                    select pr;
            });

            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPermissionRecordAsync(PermissionRecord permission)
        {
            await PermissionRecordRepository.InsertAsync(permission);
        }

        /// <summary>
        /// Gets a permission record by identifier
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a permission record
        /// </returns>
        public virtual async Task<PermissionRecord> GetPermissionRecordByIdAsync(int permissionId)
        {
            return await PermissionRecordRepository.GetByIdAsync(permissionId);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdatePermissionRecordAsync(PermissionRecord permission)
        {
            await PermissionRecordRepository.UpdateAsync(permission);
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePermissionRecordAsync(PermissionRecord permission)
        {
            await PermissionRecordRepository.DeleteAsync(permission);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InstallPermissionsAsync(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            //default customer role mappings
            var defaultPermissions = permissionProvider.GetDefaultPermissions().ToList();

            foreach (var permission in permissions)
            {
                var permission1 = await GetPermissionRecordBySystemNameAsync(permission.SystemName);
                if (permission1 != null)
                    continue;

                //new permission (install it)
                permission1 = new PermissionRecord
                {
                    Name = permission.Name,
                    SystemName = permission.SystemName,
                    Category = permission.Category
                };

                //save new permission
                await InsertPermissionRecordAsync(permission1);

                foreach (var defaultPermission in defaultPermissions)
                {
                    var customerRole = await CustomerService.GetCustomerRoleBySystemNameAsync(defaultPermission.systemRoleName);
                    if (customerRole == null)
                    {
                        //new role (save it)
                        customerRole = new CustomerRole
                        {
                            Name = defaultPermission.systemRoleName,
                            Active = true,
                            SystemName = defaultPermission.systemRoleName
                        };
                        await CustomerService.InsertCustomerRoleAsync(customerRole);
                    }

                    var defaultMappingProvided = defaultPermission.permissions.Any(p => p.SystemName == permission1.SystemName);

                    if (!defaultMappingProvided)
                        continue;

                    await InsertPermissionRecordCustomerRoleMappingAsync(new PermissionRecordCustomerRoleMapping { CustomerRoleId = customerRole.Id, PermissionRecordId = permission1.Id });
                }

                //save localization
                await LocalizationService.SaveLocalizedPermissionNameAsync(permission1);
            }
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UninstallPermissionsAsync(IPermissionProvider permissionProvider)
        {
            //default customer role mappings
            var defaultPermissions = permissionProvider.GetDefaultPermissions().ToList();

            //uninstall permissions
            foreach (var permission in permissionProvider.GetPermissions())
            {
                var permission1 = await GetPermissionRecordBySystemNameAsync(permission.SystemName);
                if (permission1 == null)
                    continue;

                //clear permission record customer role mapping
                foreach (var defaultPermission in defaultPermissions)
                {
                    var customerRole = await CustomerService.GetCustomerRoleBySystemNameAsync(defaultPermission.systemRoleName);
                    
                    await DeletePermissionRecordCustomerRoleMappingAsync(permission1.Id, customerRole.Id);
                }

                //delete permission
                await DeletePermissionRecordAsync(permission1);

                //save localization
                await LocalizationService.DeleteLocalizedPermissionNameAsync(permission1);
            }
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission)
        {
            return await AuthorizeAsync(permission, await WorkContext.GetCurrentCustomerAsync());
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission, Customer customer)
        {
            if (permission == null)
                return false;

            if (customer == null)
                return false;

            return await AuthorizeAsync(permission.SystemName, customer);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName)
        {
            return await AuthorizeAsync(permissionRecordSystemName, await WorkContext.GetCurrentCustomerAsync());
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, Customer customer)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var customerRoles = await CustomerService.GetCustomerRolesAsync(customer);
            foreach (var role in customerRoles)
                if (await AuthorizeAsync(permissionRecordSystemName, role.Id))
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, int customerRoleId)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.PermissionAllowedCacheKey, permissionRecordSystemName, customerRoleId);

            return await StaticCacheManager.GetAsync(key, async () =>
            {
                var permissions = await GetPermissionRecordsByCustomerRoleIdAsync(customerRoleId);
                foreach (var permission in permissions)
                    if (permission.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        /// <summary>
        /// Gets a permission record-customer role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IList<PermissionRecordCustomerRoleMapping>> GetMappingByPermissionRecordIdAsync(int permissionId)
        {
            var query = PermissionRecordCustomerRoleMappingRepository.Table;

            query = query.Where(x => x.PermissionRecordId == permissionId);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Delete a permission record-customer role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePermissionRecordCustomerRoleMappingAsync(int permissionId, int customerRoleId)
        {
            var mapping = PermissionRecordCustomerRoleMappingRepository.Table
                .FirstOrDefault(prcm => prcm.CustomerRoleId == customerRoleId && prcm.PermissionRecordId == permissionId);
            if (mapping is null)
                return;

            await PermissionRecordCustomerRoleMappingRepository.DeleteAsync(mapping);
        }

        /// <summary>
        /// Inserts a permission record-customer role mapping
        /// </summary>
        /// <param name="permissionRecordCustomerRoleMapping">Permission record-customer role mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPermissionRecordCustomerRoleMappingAsync(PermissionRecordCustomerRoleMapping permissionRecordCustomerRoleMapping)
        {
            await PermissionRecordCustomerRoleMappingRepository.InsertAsync(permissionRecordCustomerRoleMapping);
        }

        #endregion
    }
}