using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store mapping service
    /// </summary>
    public partial class StoreMappingService : IStoreMappingService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public StoreMappingService(CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IRepository<StoreMapping> storeMappingRepository,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext)
        {
            _catalogSettings = catalogSettings;
            _eventPublisher = eventPublisher;
            _storeMappingRepository = storeMappingRepository;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a store mapping record
        /// </summary>
        /// <param name="storeMapping">Store mapping record</param>
        public virtual void DeleteStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentNullException(nameof(storeMapping));

            _storeMappingRepository.Delete(storeMapping);

            //cache
            _cacheManager.RemoveByPrefix(NopStoreDefaults.StoreMappingPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(storeMapping);
        }

        /// <summary>
        /// Gets a store mapping record
        /// </summary>
        /// <param name="storeMappingId">Store mapping record identifier</param>
        /// <returns>Store mapping record</returns>
        public virtual StoreMapping GetStoreMappingById(int storeMappingId)
        {
            if (storeMappingId == 0)
                return null;

            return _storeMappingRepository.GetById(storeMappingId);
        }

        /// <summary>
        /// Gets store mapping records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Store mapping records</returns>
        public virtual IList<StoreMapping> GetStoreMappings<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var query = from sm in _storeMappingRepository.Table
                        where sm.EntityId == entityId &&
                        sm.EntityName == entityName
                        select sm;
            var storeMappings = query.ToList();
            return storeMappings;
        }

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <param name="storeMapping">Store mapping</param>
        protected virtual void InsertStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentNullException(nameof(storeMapping));

            _storeMappingRepository.Insert(storeMapping);

            //cache
            _cacheManager.RemoveByPrefix(NopStoreDefaults.StoreMappingPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(storeMapping);
        }

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store id</param>
        /// <param name="entity">Entity</param>
        public virtual void InsertStoreMapping<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (storeId == 0)
                throw new ArgumentOutOfRangeException(nameof(storeId));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var storeMapping = new StoreMapping
            {
                EntityId = entityId,
                EntityName = entityName,
                StoreId = storeId
            };

            InsertStoreMapping(storeMapping);
        }

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Store identifiers</returns>
        public virtual int[] GetStoresIdsWithAccess<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var key = string.Format(NopStoreDefaults.StoreMappingByEntityIdNameCacheKey, entityId, entityName);
            return _cacheManager.Get(key, () =>
            {
                var query = from sm in _storeMappingRepository.Table
                            where sm.EntityId == entityId &&
                            sm.EntityName == entityName
                            select sm.StoreId;
                return query.ToArray();
            });
        }

        /// <summary>
        /// Authorize whether entity could be accessed in the current store (mapped to this store)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            return Authorize(entity, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                return false;

            if (storeId == 0)
                //return true if no store specified/found
                return true;

            if (_catalogSettings.IgnoreStoreLimitations)
                return true;

            if (!entity.LimitedToStores)
                return true;

            foreach (var storeIdWithAccess in GetStoresIdsWithAccess(entity))
                if (storeId == storeIdWithAccess)
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        #endregion
    }
}