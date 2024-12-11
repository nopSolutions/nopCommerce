using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;

namespace Nop.Services.Security;

/// <summary>
/// Permission service
/// </summary>
public partial class PermissionService : IPermissionService
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IRepository<CustomerRole> _customerRoleRepository;
    protected readonly IRepository<PermissionRecord> _permissionRecordRepository;
    protected readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionRecordCustomerRoleMappingRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ITypeFinder _typeFinder;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public PermissionService(ICustomerService customerService,
        ILocalizationService localizationService,
        IRepository<CustomerRole> customerRoleRepository,
        IRepository<PermissionRecord> permissionRecordRepository,
        IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
        IStaticCacheManager staticCacheManager,
        ITypeFinder typeFinder,
        IWorkContext workContext)
    {
        _customerService = customerService;
        _localizationService = localizationService;
        _customerRoleRepository = customerRoleRepository;
        _permissionRecordRepository = permissionRecordRepository;
        _permissionRecordCustomerRoleMappingRepository = permissionRecordCustomerRoleMappingRepository;
        _staticCacheManager = staticCacheManager;
        _typeFinder = typeFinder;
        _workContext = workContext;
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
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.PermissionRecordsAllCacheKey, customerRoleId);

        var query = from pr in _permissionRecordRepository.Table
            join prcrm in _permissionRecordCustomerRoleMappingRepository.Table on pr.Id equals prcrm
                .PermissionRecordId
            where prcrm.CustomerRoleId == customerRoleId
            orderby pr.Id
            select pr;

        return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
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

        var query = from pr in _permissionRecordRepository.Table
            where pr.SystemName == systemName
            orderby pr.Id
            select pr;

        var permissionRecord = await query.FirstOrDefaultAsync();
        return permissionRecord;
    }

    /// <summary>
    /// Insert permissions by list of permission configs
    /// </summary>
    /// <param name="configs">Permission configs</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallPermissionsAsync(IList<PermissionConfig> configs)
    {
        if (!configs?.Any() ?? true)
            return;

        var exists =
            await _permissionRecordCustomerRoleMappingRepository.GetAllAsync(query => query, getCacheKey: _ => default);

        async Task addPermissionRecordCustomerRoleMappingIfNotExists(
            PermissionRecordCustomerRoleMapping permissionRecordCustomerRoleMapping)
        {
            var mapping = exists.FirstOrDefault(m =>
                m.CustomerRoleId == permissionRecordCustomerRoleMapping.CustomerRoleId &&
                m.PermissionRecordId == permissionRecordCustomerRoleMapping.PermissionRecordId);

            if (mapping != null)
            {
                permissionRecordCustomerRoleMapping.Id = mapping.Id;

                return;
            }

            await _permissionRecordCustomerRoleMappingRepository.InsertAsync(permissionRecordCustomerRoleMapping, false);
            exists.Add(permissionRecordCustomerRoleMapping);
        }

        foreach (var config in configs)
        {
            //new permission (install it)
            var permission = new PermissionRecord
            {
                Name = config.Name,
                SystemName = config.SystemName,
                Category = config.Category
            };

            //save new permission
            await _permissionRecordRepository.InsertAsync(permission);

            foreach (var systemRoleName in config.DefaultCustomerRoles)
            {
                var customerRole = await GetCustomerRoleBySystemNameAsync(systemRoleName);

                if (customerRole == null)
                {
                    //new role (save it)
                    customerRole = new CustomerRole
                    {
                        Name = systemRoleName,
                        Active = true,
                        SystemName = systemRoleName
                    };

                    await _customerRoleRepository.InsertAsync(customerRole);
                }

                await addPermissionRecordCustomerRoleMappingIfNotExists(new PermissionRecordCustomerRoleMapping { CustomerRoleId = customerRole.Id, PermissionRecordId = permission.Id });
            }

            //save localization
            await _localizationService.SaveLocalizedPermissionNameAsync(permission);
        }
    }

    /// <summary>
    /// Gets a customer role
    /// </summary>
    /// <param name="systemName">Customer role system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role
    /// </returns>
    protected virtual async Task<CustomerRole> GetCustomerRoleBySystemNameAsync(string systemName)
    {
        if (string.IsNullOrWhiteSpace(systemName))
            return null;

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCustomerServicesDefaults.CustomerRolesBySystemNameCacheKey, systemName);

        var query = from cr in _customerRoleRepository.Table
                    orderby cr.Id
                    where cr.SystemName == systemName
                    select cr;

        var customerRole = await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());

        return customerRole;
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
        var permissions = await _permissionRecordRepository.GetAllAsync(query => from pr in query
            orderby pr.Name
            select pr);

        return permissions;
    }

    /// <summary>
    /// Inserts a permission
    /// </summary>
    /// <param name="permission">Permission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPermissionRecordAsync(PermissionRecord permission)
    {
        await _permissionRecordRepository.InsertAsync(permission);
    }

    /// <summary>
    /// Gets a permission record by identifier
    /// </summary>
    /// <param name="permissionId">Permission identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a permission record
    /// </returns>
    public virtual async Task<PermissionRecord> GetPermissionRecordByIdAsync(int permissionId)
    {
        return await _permissionRecordRepository.GetByIdAsync(permissionId);
    }

    /// <summary>
    /// Updates the permission
    /// </summary>
    /// <param name="permission">Permission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePermissionRecordAsync(PermissionRecord permission)
    {
        await _permissionRecordRepository.UpdateAsync(permission);
    }

    /// <summary>
    /// Delete a permission
    /// </summary>
    /// <param name="permission">Permission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePermissionRecordAsync(PermissionRecord permission)
    {
        await _permissionRecordRepository.DeleteAsync(permission);
    }

    /// <summary>
    /// Delete a permission
    /// </summary>
    /// <param name="permissionSystemName">Permission system name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePermissionAsync(string permissionSystemName)
    {
        var permission = await GetPermissionRecordBySystemNameAsync(permissionSystemName);

        if (permission == null)
            return;

        var mapping = await GetMappingByPermissionRecordIdAsync(permission.Id);

        await _permissionRecordCustomerRoleMappingRepository.DeleteAsync(mapping);
        await _localizationService.DeleteLocalizedPermissionNameAsync(permission);
        await _permissionRecordRepository.DeleteAsync(permission);
    }

    /// <summary>
    /// Authorize permission
    /// </summary>
    /// <param name="permission">Permission record</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission)
    {
        return await AuthorizeAsync(permission, await _workContext.GetCurrentCustomerAsync());
    }

    /// <summary>
    /// Authorize permission
    /// </summary>
    /// <param name="permission">Permission record</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true - authorized; otherwise, false
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
    /// The task result contains the true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName)
    {
        return await AuthorizeAsync(permissionRecordSystemName, await _workContext.GetCurrentCustomerAsync());
    }

    /// <summary>
    /// Authorize permission
    /// </summary>
    /// <param name="permissionRecordSystemName">Permission record system name</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, Customer customer)
    {
        if (string.IsNullOrEmpty(permissionRecordSystemName))
            return false;

        var customerRoles = await _customerService.GetCustomerRolesAsync(customer);
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
    /// The task result contains the true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, int customerRoleId)
    {
        if (string.IsNullOrEmpty(permissionRecordSystemName))
            return false;

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.PermissionAllowedCacheKey, permissionRecordSystemName, customerRoleId);

        return await _staticCacheManager.GetAsync(key, async () =>
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a list of mappings
    /// </returns>
    public virtual async Task<IList<PermissionRecordCustomerRoleMapping>> GetMappingByPermissionRecordIdAsync(int permissionId)
    {
        var query = _permissionRecordCustomerRoleMappingRepository.Table;

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
        var mapping = await _permissionRecordCustomerRoleMappingRepository.Table
            .FirstOrDefaultAsync(prcm => prcm.CustomerRoleId == customerRoleId && prcm.PermissionRecordId == permissionId);
        if (mapping is null)
            return;

        await _permissionRecordCustomerRoleMappingRepository.DeleteAsync(mapping);
    }

    /// <summary>
    /// Inserts a permission record-customer role mapping
    /// </summary>
    /// <param name="permissionRecordCustomerRoleMapping">Permission record-customer role mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPermissionRecordCustomerRoleMappingAsync(PermissionRecordCustomerRoleMapping permissionRecordCustomerRoleMapping)
    {
        await _permissionRecordCustomerRoleMappingRepository.InsertAsync(permissionRecordCustomerRoleMapping);
    }

    /// <summary>
    /// Configure permission manager
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPermissionsAsync()
    {
        var permissionRecords = (await _permissionRecordRepository.GetAllAsync(query => query, getCacheKey: _ => null)).Distinct().ToHashSet();
        var exists = permissionRecords.Select(p => p.SystemName).ToHashSet();

        var configs = _typeFinder.FindClassesOfType<IPermissionConfigManager>()
            .Select(configType => (IPermissionConfigManager)Activator.CreateInstance(configType))
            .SelectMany(config => config?.AllConfigs ?? new List<PermissionConfig>())
            .Where(c => !exists.Contains(c.SystemName))
            .ToList();

        await InstallPermissionsAsync(configs);
    }

    /// <summary>
    /// Inserts a permission record-customer role mappings
    /// </summary>
    /// <param name="customerRoleId">Customer role ID</param>
    /// <param name="permissions">Permissions</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPermissionMappingAsync(int customerRoleId, params string[] permissions)
    {
        var permissionRecords = await GetAllPermissionRecordsAsync();

        foreach (var permissionSystemName in permissions)
        {
            var permission = permissionRecords.FirstOrDefault(p =>
                p.SystemName.Equals(permissionSystemName, StringComparison.CurrentCultureIgnoreCase));

            if (permission == null)
                continue;

            await InsertPermissionRecordCustomerRoleMappingAsync(
                new PermissionRecordCustomerRoleMapping
                {
                    CustomerRoleId = customerRoleId,
                    PermissionRecordId = permission.Id
                });
        }
    }

    #endregion
}
