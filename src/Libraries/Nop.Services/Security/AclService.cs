using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual async Task DeleteAclRecord(AclRecord aclRecord)
        {
            await _aclRecordRepository.Delete(aclRecord);
        }

        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        public virtual async Task<AclRecord> GetAclRecordById(int aclRecordId)
        {
            return await _aclRecordRepository.GetById(aclRecordId, cache => default);
        }

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        public virtual async Task<IList<AclRecord>> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported
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
        public virtual async Task InsertAclRecord(AclRecord aclRecord)
        {
            await _aclRecordRepository.Insert(aclRecord);
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="customerRoleId">Customer role id</param>
        /// <param name="entity">Entity</param>
        public virtual async Task InsertAclRecord<T>(T entity, int customerRoleId) where T : BaseEntity, IAclSupported
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

            await InsertAclRecord(aclRecord);
        }

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual async Task UpdateAclRecord(AclRecord aclRecord)
        {
            await _aclRecordRepository.Update(aclRecord);
        }

        /// <summary>
        /// Find customer role identifiers with granted access
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Customer role identifiers</returns>
        public virtual async Task<int[]> GetCustomerRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported
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

            return await _staticCacheManager.Get(key, async () => await query.ToAsyncEnumerable().ToArrayAsync());
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> Authorize<T>(T entity) where T : BaseEntity, IAclSupported
        {
            return await Authorize(entity, await _workContext.GetCurrentCustomer());
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> Authorize<T>(T entity, Customer customer) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (customer == null)
                return false;

            if (_catalogSettings.IgnoreAcl)
                return true;

            if (!entity.SubjectToAcl)
                return true;

            foreach (var role1 in await _customerService.GetCustomerRoles(customer))
                foreach (var role2Id in await GetCustomerRoleIdsWithAccess(entity))
                    if (role1.Id == role2Id)
                        //yes, we have such permission
                        return true;

            //no permission found
            return false;
        }

        #endregion
    }
}