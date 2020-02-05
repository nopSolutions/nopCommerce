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
                        orderby sm.EntityName
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
                            orderby sm.EntityName
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

        #region Extensions by QuanNH

        public virtual IList<StoreMapping> GetAllStoreMapping(string entityName)
        {
            var query = from s in _storeMappingRepository.Table
                        where s.EntityName == entityName || s.EntityName == "Admin"
                        //orderby s.StoreId
                        orderby s.EntityName
                        select s;
            var stores = query.Distinct().ToList();
            return stores;
        }

        public virtual List<int> GetStoreIdByEntityId(int entityId, string entityName)
        {
            var query = from sm in _storeMappingRepository.Table
                        where sm.EntityId == entityId &&
                        sm.EntityName == entityName
                        //orderby sm.EntityName
                        select sm.StoreId;
            var result = query.Distinct().ToList();
            return result;
        }

        public virtual List<int> GetEntityIdByListStoreId(int[] storeIds, string entityName)
        {
            var query = from sm in _storeMappingRepository.Table
                        where storeIds.Contains(sm.StoreId) &&
                        sm.EntityName == entityName
                        orderby sm.EntityName
                        select sm.EntityId;
            var result = query.Distinct().ToList();
            return result;
        }

        public virtual void InsertStoreMappingByEntity(int entityId, string entityName, int storeId)
        {
            if (storeId == 0)
                throw new ArgumentOutOfRangeException("storeId");

            var storeMapping = new StoreMapping()
            {
                EntityId = entityId,
                EntityName = entityName,
                StoreId = storeId
            };

            InsertStoreMapping(storeMapping);
        }

        public virtual bool TableEdit(int storeId = 0)
        {
            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IWorkContext>();
            List<int> customerIds = this.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Stores");

            if (storeId == customerIds.FirstOrDefault())
            {
                return true;
            }

            if (customerIds.Count <= 0)
                //return true if no store specified/found
                return true;

            return false;
        }

        public int CurrentStore()
        {
            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IWorkContext>();
            List<int> storeIds = this.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Stores");
            return storeIds.Count > 0 ? storeIds.FirstOrDefault() : 0;
        }

        public bool IsAdminStore()
        {
            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IWorkContext>();
            List<int> storeIds = this.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Admin");
            if (storeIds.Count > 0)
            {
                return true;
            }
            return false;
        }

        public virtual void UpdateStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentNullException("storeMapping");

            _storeMappingRepository.Update(storeMapping);
            //cache
            _cacheManager.RemoveByPrefix(NopStoreDefaults.StoresPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(storeMapping);
        }

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <param name="storeMapping">Store mapping</param>
        public virtual void Insert_Store_Mapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentNullException("storeMapping");

            _storeMappingRepository.Insert(storeMapping);

            //cache
            _cacheManager.RemoveByPrefix(NopStoreDefaults.StoresPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(storeMapping);
        }

        public virtual bool AuthorizeCustomer(int customerId)
        {
            int storeId = this.GetStoreIdByEntityId(customerId, "Stores").FirstOrDefault();
            if (storeId == _storeContext.CurrentStore.Id)
            {
                return true;
            }

            return false;
        }

        public virtual IPagedList<StoreMapping> GetAllStoreMappings(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _storeMappingRepository.Table;
            query = query.Where(sm => sm.EntityName == "Stores");
            
            //filter by storeMapping
            if (storeId > 0)
            {
                query = query.Where(sm => sm.StoreId == storeId);
            }

            //order records by display order
            //query = query.OrderBy(_sm => _sm.StoreId).ThenBy(_sm => _sm.Id);
            query = query.OrderBy(_sm => _sm.EntityName).ThenBy(_sm => _sm.Id);

            return new PagedList<StoreMapping>(query, pageIndex, pageSize);
        }
        #endregion
    }
}