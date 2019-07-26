using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store service
    /// </summary>
    public partial class StoreService : IStoreService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Store> _storeRepository;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public StoreService(IEventPublisher eventPublisher,
            IRepository<Store> storeRepository,
            IStaticCacheManager cacheManager)
        {
            _eventPublisher = eventPublisher;
            _storeRepository = storeRepository;
            _cacheManager = cacheManager;
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
                throw new ArgumentNullException(nameof(store));

            if (store is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            var allStores = GetAllStores();
            if (allStores.Count == 1)
                throw new Exception("You cannot delete the only configured store");

            _storeRepository.Delete(store);

            _cacheManager.RemoveByPrefix(NopStoreDefaults.StoresPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(store);
        }

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached (workaround until Entity Framework supports 2-level caching)</param>
        /// <returns>Stores</returns>
        public virtual IList<Store> GetAllStores(bool loadCacheableCopy = true)
        {
            IList<Store> LoadStoresFunc()
            {
                var query = from s in _storeRepository.Table orderby s.DisplayOrder, s.Id select s;
                return query.ToList();
            }

            if (loadCacheableCopy)
            {
                //cacheable copy
                return _cacheManager.Get(NopStoreDefaults.StoresAllCacheKey, () =>
                {
                    var result = new List<Store>();
                    foreach (var store in LoadStoresFunc())
                        result.Add(new StoreForCaching(store));
                    return result;
                });
            }

            return LoadStoresFunc();
        }

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached (workaround until Entity Framework supports 2-level caching)</param>
        /// <returns>Store</returns>
        public virtual Store GetStoreById(int storeId, bool loadCacheableCopy = true)
        {
            if (storeId == 0)
                return null;

            Store LoadStoreFunc()
            {
                return _storeRepository.GetById(storeId);
            }

            if (!loadCacheableCopy) 
                return LoadStoreFunc();

            //cacheable copy
            var key = string.Format(NopStoreDefaults.StoresByIdCacheKey, storeId);
            return _cacheManager.Get(key, () =>
            {
                var store = LoadStoreFunc();
                if (store == null)
                    return null;
                return new StoreForCaching(store);
            });
        }

        /// <summary>
        /// Inserts a store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void InsertStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (store is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _storeRepository.Insert(store);

            _cacheManager.RemoveByPrefix(NopStoreDefaults.StoresPrefixCacheKey);

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
                throw new ArgumentNullException(nameof(store));

            if (store is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _storeRepository.Update(store);

            _cacheManager.RemoveByPrefix(NopStoreDefaults.StoresPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(store);
        }

        /// <summary>
        /// Parse comma-separated Hosts
        /// </summary>
        /// <param name="store">Store</param>
        /// <returns>Comma-separated hosts</returns>
        public virtual string[] ParseHostValues(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var parsedValues = new List<string>();
            if (string.IsNullOrEmpty(store.Hosts))
                return parsedValues.ToArray();

            var hosts = store.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var host in hosts)
            {
                var tmp = host.Trim();
                if (!string.IsNullOrEmpty(tmp))
                    parsedValues.Add(tmp);
            }

            return parsedValues.ToArray();
        }

        /// <summary>
        /// Indicates whether a store contains a specified host
        /// </summary>
        /// <param name="store">Store</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public virtual bool ContainsHostValue(Store store, string host)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (string.IsNullOrEmpty(host))
                return false;

            var contains = ParseHostValues(store).Any(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase));

            return contains;
        }

        /// <summary>
        /// Returns a list of names of not existing stores
        /// </summary>
        /// <param name="storeIdsNames">The names and/or IDs of the store to check</param>
        /// <returns>List of names and/or IDs not existing stores</returns>
        public string[] GetNotExistingStores(string[] storeIdsNames)
        {
            if (storeIdsNames == null)
                throw new ArgumentNullException(nameof(storeIdsNames));

            var query = _storeRepository.Table;
            var queryFilter = storeIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = query.Select(store => store.Name).Where(store => queryFilter.Contains(store)).ToList();
            queryFilter = queryFilter.Except(filter).ToArray();

            //if some names not found
            if (!queryFilter.Any())
                return queryFilter.ToArray();

            //filtering by IDs
            filter = query.Select(store => store.Id.ToString()).Where(store => queryFilter.Contains(store)).ToList();
            queryFilter = queryFilter.Except(filter).ToArray();

            return queryFilter.ToArray();
        }

        #endregion
    }
}