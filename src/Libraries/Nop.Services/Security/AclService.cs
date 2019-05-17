using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Security
{
    /// <summary>
    /// ACL service
    /// </summary>
    public partial class AclService : IAclService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<AclRecord> _aclRecordRepository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AclService(CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IRepository<AclRecord> aclRecordRepository,
            IStaticCacheManager cacheManager,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _eventPublisher = eventPublisher;
            _aclRecordRepository = aclRecordRepository;
            _cacheManager = cacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void DeleteAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Delete(aclRecord);

            //cache
            _cacheManager.RemoveByPrefix(NopSecurityDefaults.AclRecordPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(aclRecord);
        }

        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        public virtual AclRecord GetAclRecordById(int aclRecordId)
        {
            if (aclRecordId == 0)
                return null;

            return _aclRecordRepository.GetById(aclRecordId);
        }

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        public virtual IList<AclRecord> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                        ur.EntityName == entityName
                        select ur;
            var aclRecords = query.ToList();
            return aclRecords;
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void InsertAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Insert(aclRecord);

            //cache
            _cacheManager.RemoveByPrefix(NopSecurityDefaults.AclRecordPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(aclRecord);
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="customerRoleId">Customer role id</param>
        /// <param name="entity">Entity</param>
        public virtual void InsertAclRecord<T>(T entity, int customerRoleId) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (customerRoleId == 0)
                throw new ArgumentOutOfRangeException(nameof(customerRoleId));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var aclRecord = new AclRecord
            {
                EntityId = entityId,
                EntityName = entityName,
                CustomerRoleId = customerRoleId
            };

            InsertAclRecord(aclRecord);
        }

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void UpdateAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Update(aclRecord);

            //cache
            _cacheManager.RemoveByPrefix(NopSecurityDefaults.AclRecordPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(aclRecord);
        }

        /// <summary>
        /// Find customer role identifiers with granted access
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Customer role identifiers</returns>
        public virtual int[] GetCustomerRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var key = string.Format(NopSecurityDefaults.AclRecordByEntityIdNameCacheKey, entityId, entityName);
            return _cacheManager.Get(key, () =>
            {
                var query = from ur in _aclRecordRepository.Table
                            where ur.EntityId == entityId &&
                            ur.EntityName == entityName
                            select ur.CustomerRoleId;
                return query.ToArray();
            });
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity) where T : BaseEntity, IAclSupported
        {
            return Authorize(entity, _workContext.CurrentCustomer);
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity, Customer customer) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (customer == null)
                return false;

            if (_catalogSettings.IgnoreAcl)
                return true;

            if (!entity.SubjectToAcl)
                return true;

            foreach (var role1 in customer.CustomerRoles.Where(cr => cr.Active))
                foreach (var role2Id in GetCustomerRoleIdsWithAccess(entity))
                    if (role1.Id == role2Id)
                        //yes, we have such permission
                        return true;

            //no permission found
            return false;
        }

        #endregion
    }
}