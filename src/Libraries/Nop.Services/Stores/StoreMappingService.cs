using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store mapping service
    /// </summary>
    public partial class StoreMappingService : IStoreMappingService
    {
        #region Fields

        protected readonly CatalogSettings _catalogSettings;
        protected readonly IRepository<StoreMapping> _storeMappingRepository;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public StoreMappingService(CatalogSettings catalogSettings,
            IRepository<StoreMapping> storeMappingRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext)
        {
            _catalogSettings = catalogSettings;
            _storeMappingRepository = storeMappingRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <param name="storeMapping">Store mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertStoreMappingAsync(StoreMapping storeMapping)
        {
            await _storeMappingRepository.InsertAsync(storeMapping);
        }

        /// <summary>
        /// Get a value indicating whether a store mapping exists for an entity type
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if exists; otherwise false
        /// </returns>
        protected virtual async Task<bool> IsEntityMappingExistsAsync<TEntity>() where TEntity : BaseEntity, IStoreMappingSupported
        {
            var entityName = typeof(TEntity).Name;
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopStoreDefaults.StoreMappingExistsCacheKey, entityName);

            var query = from sm in _storeMappingRepository.Table
                        where sm.EntityName == entityName
                        select sm.StoreId;

            return await _staticCacheManager.GetAsync(key, query.Any);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply store mapping to the passed query
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="query">Query to filter</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the filtered query
        /// </returns>
        public virtual async Task<IQueryable<TEntity>> ApplyStoreMapping<TEntity>(IQueryable<TEntity> query, int storeId)
            where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (storeId == 0 || _catalogSettings.IgnoreStoreLimitations || !await IsEntityMappingExistsAsync<TEntity>())
                return query;

            return from entity in query
                   where !entity.LimitedToStores || _storeMappingRepository.Table.Any(sm =>
                         sm.EntityName == typeof(TEntity).Name && sm.EntityId == entity.Id && sm.StoreId == storeId)
                   select entity;
        }

        /// <summary>
        /// Deletes a store mapping record
        /// </summary>
        /// <param name="storeMapping">Store mapping record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteStoreMappingAsync(StoreMapping storeMapping)
        {
            await _storeMappingRepository.DeleteAsync(storeMapping);
        }

        /// <summary>
        /// Gets store mapping records
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store mapping records
        /// </returns>
        public virtual async Task<IList<StoreMapping>> GetStoreMappingsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopStoreDefaults.StoreMappingsCacheKey, entityId, entityName);

            var query = from sm in _storeMappingRepository.Table
                        where sm.EntityId == entityId &&
                        sm.EntityName == entityName
                        select sm;

            var storeMappings = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return storeMappings;
        }

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="storeId">Store id</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertStoreMappingAsync<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (storeId == 0)
                throw new ArgumentOutOfRangeException(nameof(storeId));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var storeMapping = new StoreMapping
            {
                EntityId = entityId,
                EntityName = entityName,
                StoreId = storeId
            };

            await InsertStoreMappingAsync(storeMapping);
        }

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store identifiers
        /// </returns>
        public virtual async Task<int[]> GetStoresIdsWithAccessAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopStoreDefaults.StoreMappingIdsCacheKey, entityId, entityName);

            var query = from sm in _storeMappingRepository.Table
                        where sm.EntityId == entityId &&
                              sm.EntityName == entityName
                        select sm.StoreId;

            return await _staticCacheManager.GetAsync(key, () => query.ToArray());
        }

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// The store identifiers
        /// </returns>
        public virtual int[] GetStoresIdsWithAccess<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopStoreDefaults.StoreMappingIdsCacheKey, entityId, entityName);

            var query = from sm in _storeMappingRepository.Table
                        where sm.EntityId == entityId &&
                              sm.EntityName == entityName
                        select sm.StoreId;

            return _staticCacheManager.Get(key, () => query.ToArray());
        }

        /// <summary>
        /// Authorize whether entity could be accessed in the current store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            return await AuthorizeAsync(entity, store.Id);
        }

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported
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

            foreach (var storeIdWithAccess in await GetStoresIdsWithAccessAsync(entity))
                if (storeId == storeIdWithAccess)
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// The rue - authorized; otherwise, false
        /// </returns>
        public virtual bool Authorize<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported
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