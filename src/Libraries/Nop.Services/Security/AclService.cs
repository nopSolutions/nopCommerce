using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Customers;

namespace Nop.Services.Security
{
    /// <summary>
    /// ACL service
    /// </summary>
    public partial class AclService : IAclService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerService _customerService;
        private readonly IRepository<AclRecord> _aclRecordRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AclService(CatalogSettings catalogSettings,
            ICustomerService customerService,
            IRepository<AclRecord> aclRecordRepository,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _customerService = customerService;
            _aclRecordRepository = aclRecordRepository;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get an expression predicate to apply the ACL
        /// </summary>
        /// <param name="customerRoleIds">Identifiers of customer's roles</param>
        /// <typeparam name="TEntity">Type of entity with supported the ACL</typeparam>
        /// <returns>Lambda expression</returns>
        public virtual Expression<Func<TEntity, bool>> ApplyAcl<TEntity>(int[] customerRoleIds) where TEntity : BaseEntity, IAclSupported
        {
            return (subjectEntity) => (from acl in _aclRecordRepository.Table
                                       where !subjectEntity.SubjectToAcl ||
                                           (acl.EntityId == subjectEntity.Id &&
                                               acl.EntityName == typeof(TEntity).Name &&
                                               customerRoleIds.Contains(acl.CustomerRoleId))
                                       select acl.EntityId).Any();
        }

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual async Task DeleteAclRecordAsync(AclRecord aclRecord)
        {
            await _aclRecordRepository.DeleteAsync(aclRecord);
        }

        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        public virtual async Task<AclRecord> GetAclRecordByIdAsync(int aclRecordId)
        {
            return await _aclRecordRepository.GetByIdAsync(aclRecordId, cache => default);
        }

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        public virtual async Task<IList<AclRecord>> GetAclRecordsAsync<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                        ur.EntityName == entityName
                        select ur;
            var aclRecords = await query.ToAsyncEnumerable().ToListAsync();

            return aclRecords;
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual async Task InsertAclRecordAsync(AclRecord aclRecord)
        {
            await _aclRecordRepository.InsertAsync(aclRecord);
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="customerRoleId">Customer role id</param>
        /// <param name="entity">Entity</param>
        public virtual async Task InsertAclRecordAsync<T>(T entity, int customerRoleId) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

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
        /// Get a value indicating whether any ACL records exist for entity type are related to customer roles
        /// </summary>
        /// <param name="customerRoleIds">Customer's role identifiers</param>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>True if exist; otherwise false</returns>
        public virtual bool IsEntityAclMappingExist<T>(int[] customerRoleIds) where T : BaseEntity, IAclSupported
        {
            if (!customerRoleIds.Any())
                return false;

            var entityName = typeof(T).Name;
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.EntityAclRecordExistsCacheKey, entityName, customerRoleIds);

            var query = from acl in _aclRecordRepository.Table
                        where acl.EntityName == entityName &&
                              customerRoleIds.Contains(acl.CustomerRoleId)
                        select acl;

            return _staticCacheManager.Get(key, query.Any);
        }

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual async Task UpdateAclRecordAsync(AclRecord aclRecord)
        {
            await _aclRecordRepository.UpdateAsync(aclRecord);
        }

        /// <summary>
        /// Find customer role identifiers with granted access
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Customer role identifiers</returns>
        public virtual async Task<int[]> GetCustomerRoleIdsWithAccessAsync<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.AclRecordCacheKey, entityId, entityName);

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                              ur.EntityName == entityName
                        select ur.CustomerRoleId;

            return await _staticCacheManager.GetAsync(key, async () => await query.ToAsyncEnumerable().ToArrayAsync());
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> AuthorizeAsync<T>(T entity) where T : BaseEntity, IAclSupported
        {
            return await AuthorizeAsync(entity, await _workContext.GetCurrentCustomerAsync());
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> AuthorizeAsync<T>(T entity, Customer customer) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (customer == null)
                return false;

            if (_catalogSettings.IgnoreAcl)
                return true;

            if (!entity.SubjectToAcl)
                return true;

            foreach (var role1 in await _customerService.GetCustomerRolesAsync(customer))
                foreach (var role2Id in await GetCustomerRoleIdsWithAccessAsync(entity))
                    if (role1.Id == role2Id)
                        //yes, we have such permission
                        return true;

            //no permission found
            return false;
        }

        #endregion
    }
}