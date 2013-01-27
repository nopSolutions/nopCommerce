using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Events;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Store service
    /// </summary>
    public partial class StoreService : IStoreService
    {
        #region Constants
        private const string STORES_ALL_KEY = "Nop.stores.all";
        private const string STORES_BY_ID_KEY = "Nop.stores.id-{0}";
        private const string STORES_PATTERN_KEY = "Nop.stores.";
        #endregion
        
        #region Fields
        
        private readonly IRepository<Store> _storeRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="storeRepository">Store repository</param>
        /// <param name="eventPublisher">Event published</param>
        public StoreService(ICacheManager cacheManager,
            IRepository<Store> storeRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._storeRepository = storeRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void DeleteStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            _storeRepository.Delete(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(store);
        }

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <returns>Store collection</returns>
        public virtual IList<Store> GetAllStores()
        {
            string key = STORES_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from s in _storeRepository.Table
                            orderby s.DisplayOrder, s.Name
                            select s;
                var stores = query.ToList();
                return stores;
            });
        }

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Store</returns>
        public virtual Store GetStoreById(int storeId)
        {
            if (storeId == 0)
                return null;

            string key = string.Format(STORES_BY_ID_KEY, storeId);
            return _cacheManager.Get(key, () =>
            {
                var store = _storeRepository.GetById(storeId);
                return store;
            });
        }

        /// <summary>
        /// Inserts a store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void InsertStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            _storeRepository.Insert(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(store);
        }

        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void UpdateStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            _storeRepository.Update(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(store);
        }

        #endregion
    }
}