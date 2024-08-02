using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Customers;

namespace Nop.Services.Security;

/// <summary>
/// ACL service
/// </summary>
public partial class AclService : IAclService
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICustomerService _customerService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IRepository<AclRecord> _aclRecordRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly Lazy<IWorkContext> _workContext;

    #endregion

    #region Ctor

    public AclService(CatalogSettings catalogSettings,
        ICustomerService customerService,
        INopDataProvider dataProvider,
        IRepository<AclRecord> aclRecordRepository,
        IStaticCacheManager staticCacheManager,
        Lazy<IWorkContext> workContext)
    {
        _catalogSettings = catalogSettings;
        _customerService = customerService;
        _dataProvider = dataProvider;
        _aclRecordRepository = aclRecordRepository;
        _staticCacheManager = staticCacheManager;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Inserts an ACL record
    /// </summary>
    /// <param name="aclRecord">ACL record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertAclRecordAsync(AclRecord aclRecord)
    {
        await _aclRecordRepository.InsertAsync(aclRecord);
    }

    /// <summary>
    /// Get a value indicating whether any ACL records exist for entity type are related to customer roles
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if exist; otherwise false
    /// </returns>
    protected virtual async Task<bool> IsEntityAclMappingExistAsync<TEntity>() where TEntity : BaseEntity, IAclSupported
    {
        var entityName = typeof(TEntity).Name;
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.EntityAclRecordExistsCacheKey, entityName);

        var query = from acl in _aclRecordRepository.Table
            where acl.EntityName == entityName
            select acl;

        return await _staticCacheManager.GetAsync(key, query.Any);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Apply ACL to the passed query
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="query">Query to filter</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filtered query
    /// </returns>
    public virtual async Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, Customer customer)
        where TEntity : BaseEntity, IAclSupported
    {
        ArgumentNullException.ThrowIfNull(query);

        ArgumentNullException.ThrowIfNull(customer);

        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
        return await ApplyAcl(query, customerRoleIds);
    }

    /// <summary>
    /// Apply ACL to the passed query
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="query">Query to filter</param>
    /// <param name="customerRoleIds">Identifiers of customer's roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filtered query
    /// </returns>
    public virtual async Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, int[] customerRoleIds)
        where TEntity : BaseEntity, IAclSupported
    {
        ArgumentNullException.ThrowIfNull(query);

        ArgumentNullException.ThrowIfNull(customerRoleIds);

        if (!customerRoleIds.Any() || _catalogSettings.IgnoreAcl || !await IsEntityAclMappingExistAsync<TEntity>())
            return query;

        return from entity in query
            where !entity.SubjectToAcl || _aclRecordRepository.Table.Any(acl =>
                acl.EntityName == typeof(TEntity).Name && acl.EntityId == entity.Id && customerRoleIds.Contains(acl.CustomerRoleId))
            select entity;
    }

    /// <summary>
    /// Deletes an ACL record
    /// </summary>
    /// <param name="aclRecord">ACL record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAclRecordAsync(AclRecord aclRecord)
    {
        await _aclRecordRepository.DeleteAsync(aclRecord);
    }

    /// <summary>
    /// Gets ACL records
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ACL records
    /// </returns>
    public virtual async Task<IList<AclRecord>> GetAclRecordsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;

        var query = from ur in _aclRecordRepository.Table
            where ur.EntityId == entityId &&
                  ur.EntityName == entityName
            select ur;
        var aclRecords = await query.ToListAsync();

        return aclRecords;
    }

    /// <summary>
    /// Inserts an ACL record
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="customerRoleId">Customer role id</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAclRecordAsync<TEntity>(TEntity entity, int customerRoleId) where TEntity : BaseEntity, IAclSupported
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (customerRoleId == 0)
            throw new ArgumentOutOfRangeException(nameof(customerRoleId));

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;

        var aclRecord = new AclRecord
        {
            EntityId = entityId,
            EntityName = entityName,
            CustomerRoleId = customerRoleId
        };

        await InsertAclRecordAsync(aclRecord);
    }

    /// <summary>
    /// Find customer role identifiers with granted access
    /// </summary>
    /// <param name="entityId">Entity ID</param>
    /// <param name="entityName">Entity name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role identifiers
    /// </returns>
    public virtual async Task<int[]> GetCustomerRoleIdsWithAccessAsync(int entityId, string entityName)
    {
        if (entityId == 0)
            return [];

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.AclRecordCacheKey, entityId, entityName);

        var query = from ur in _aclRecordRepository.Table
            where ur.EntityId == entityId &&
                  ur.EntityName == entityName
            select ur.CustomerRoleId;

        return await _staticCacheManager.GetAsync(key, () => query.ToArray());
    }

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
    {
        return await AuthorizeAsync(entity, await _workContext.Value.GetCurrentCustomerAsync());
    }

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity, Customer customer) where TEntity : BaseEntity, IAclSupported
    {
        if (entity == null)
            return false;

        if (!entity.SubjectToAcl)
            return true;

        return await AuthorizeAsync(entity.GetType().Name, entity.Id, customer);
    }

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <param name="entityTypeName">Type name of entity that supports the ACL</param>
    /// <param name="entityId">Entity ID</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(string entityTypeName, int entityId, Customer customer)
    {
        if (string.IsNullOrEmpty(entityTypeName))
            return false;

        if (entityId <= 0)
            return false;

        if (customer == null)
            return false;

        if (_catalogSettings.IgnoreAcl)
            return true;

        foreach (var role1 in await _customerService.GetCustomerRolesAsync(customer))
            foreach (var role2Id in await GetCustomerRoleIdsWithAccessAsync(entityId, entityTypeName))
                if (role1.Id == role2Id)
                    //yes, we have such permission
                    return true;

        //no permission found
        return false;
    }

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="allowedCustomerRoleIds">List of allowed customer role IDs</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(Customer customer, IList<int> allowedCustomerRoleIds)
    {
        return _catalogSettings.IgnoreAcl || allowedCustomerRoleIds.Intersect(await _customerService.GetCustomerRoleIdsAsync(customer)).Any();
    }

    /// <summary>
    /// Save ACL mapping
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="selectedCustomerRoleIds">Customer roles for mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveAclAsync<TEntity>(TEntity entity, IList<int> selectedCustomerRoleIds) where TEntity : BaseEntity, IAclSupported
    {
        if (entity == null)
            return;

        if (entity.SubjectToAcl != selectedCustomerRoleIds.Any())
        {
            entity.SubjectToAcl = selectedCustomerRoleIds.Any();
            await _dataProvider.UpdateEntityAsync(entity);
        }

        var existingAclRecords = await GetAclRecordsAsync(entity);
        var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
        foreach (var customerRole in allCustomerRoles)
            if (selectedCustomerRoleIds.Contains(customerRole.Id))
            {
                //new role
                if (existingAclRecords.All(acl => acl.CustomerRoleId != customerRole.Id))
                    await InsertAclRecordAsync(entity, customerRole.Id);
            }
            else
            {
                //remove role
                var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                if (aclRecordToDelete != null)
                    await DeleteAclRecordAsync(aclRecordToDelete);
            }
    }

    #endregion
}