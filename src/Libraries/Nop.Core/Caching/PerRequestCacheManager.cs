using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching during an HTTP request (short term caching)
    /// </summary>
    public partial class PerRequestCacheManager : ICacheManager
    {
<<<<<<< HEAD
=======
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
        #region Ctor

        public PerRequestCacheManager(IHttpContextAccessor httpContextAccessor)
        {
<<<<<<< HEAD
            _httpContextAccessor = httpContextAccessor;

            _lock = new ReaderWriterLockSlim();
=======
            this._httpContextAccessor = httpContextAccessor;
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
        }

        #endregion

        #region Utilities

        /// <summary>
<<<<<<< HEAD
        /// Gets a key/value collection that can be used to share data within the scope of this request
=======
        /// Gets a key/value collection that can be used to share data within the scope of this request 
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
        /// </summary>
        protected virtual IDictionary<object, object> GetItems()
        {
            return _httpContextAccessor.HttpContext?.Items;
        }

        #endregion

<<<<<<< HEAD
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ReaderWriterLockSlim _lock;

        #endregion

=======
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
        #region Methods

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <param name="cacheTime">Cache time in minutes; pass 0 to do not cache; pass null to use the default time</param>
        /// <returns>The cached value associated with the specified key</returns>
        public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
<<<<<<< HEAD
            _lock.EnterReadLock();

            try
            {
                var items = GetItems();
                if (items == null)
                {
                    return acquire();
                }

                //item already is in cache, so return it
                if (items[key] != null)
                {
                    return (T) items[key];
                }

                //or create it using passed function
                var result = acquire();

                //and set in cache (if cache time is defined)
                if (result != null && (cacheTime ?? NopCachingDefaults.CacheTime) > 0)
                {
                    items[key] = result;
                }

                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
=======
            var items = GetItems();
            if (items == null)
                return acquire();

            //item already is in cache, so return it
            if (items[key] != null)
                return (T)items[key];

            //or create it using passed function
            var result = acquire();

            //and set in cache (if cache time is defined)
            if (result != null && (cacheTime ?? NopCachingDefaults.CacheTime) > 0)
                items[key] = result;

            return result;
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
<<<<<<< HEAD
            _lock.EnterWriteLock();

            try
            {
                var items = GetItems();
                if (items == null)
                {
                    return;
                }

                if (data != null)
                {
                    items[key] = data;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
=======
            var items = GetItems();
            if (items == null)
                return;

            if (data != null)
                items[key] = data;
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        public virtual bool IsSet(string key)
        {
<<<<<<< HEAD
            _lock.EnterReadLock();

            try
            {
                var items = GetItems();

                return items?[key] != null;
            }
            finally
            {
                _lock.ExitReadLock();
            }
=======
            var items = GetItems();

            return items?[key] != null;
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public virtual void Remove(string key)
        {
<<<<<<< HEAD
            _lock.EnterWriteLock();

            try
            {
                var items = GetItems();

                items?.Remove(key);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
=======
            var items = GetItems();

            items?.Remove(key);
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
        }

        /// <summary>
        /// Removes items by key pattern
        /// </summary>
        /// <param name="pattern">String key pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
<<<<<<< HEAD
            _lock.EnterWriteLock();
=======
            var items = GetItems();
            if (items == null)
                return;
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs

            //get cache keys that matches pattern
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = items.Keys.Select(p => p.ToString()).Where(key => regex.IsMatch(key)).ToList();

            //remove matching values
            foreach (var key in matchesKeys)
            {
<<<<<<< HEAD
                _lock.ExitWriteLock();
=======
                items.Remove(key);
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
<<<<<<< HEAD
            _lock.EnterWriteLock();

            try
            {
                var items = GetItems();

                items?.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
=======
            var items = GetItems();

            items?.Clear();
>>>>>>> parent of 6c7715ddd... Update PerRequestCacheManager.cs
        }

        /// <summary>
        /// Dispose cache manager
        /// </summary>
        public virtual void Dispose()
        {
            //nothing special
        }

        #endregion
    }
}