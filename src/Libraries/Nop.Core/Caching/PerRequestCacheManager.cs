using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Nop.Core.ComponentModel;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching during an HTTP request (short term caching)
    /// </summary>
    public partial class PerRequestCacheManager : ICacheManager
    {
        #region Ctor

        public PerRequestCacheManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            _locker = new ReaderWriterLockSlim();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a key/value collection that can be used to share data within the scope of this request
        /// </summary>
        protected virtual IDictionary<object, object> GetItems()
        {
            return _httpContextAccessor.HttpContext?.Items;
        }

        #endregion

        #region Fields
        // Flag: Has Dispose already been called?
        private bool _disposed;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ReaderWriterLockSlim _locker;

        #endregion

        #region Methods

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>The cached value associated with the specified key</returns>
        public virtual T Get<T>(CacheKey key, Func<T> acquire)
        {
            IDictionary<object, object> items;

            if (_disposed)
                return acquire();

            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
            {
                items = GetItems();
                if (items == null)
                    return acquire();

                //item already is in cache, so return it
                if (items[key.Key] != null)
                    return (T)items[key.Key];
            }
            
            //or create it using passed function
            var result = acquire();
            
            if (result == null || key.CacheTime <= 0) 
                return result;

            //and set in cache (if cache time is defined)
            using (new ReaderWriteLockDisposable(_locker))
            {
                items[key.Key] = result;
            }

            return result;
        }
        
        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        public virtual void Set(CacheKey key, object data)
        {
            if (data == null)
                return;

            using (new ReaderWriteLockDisposable(_locker))
            {
                var items = GetItems();
                if (items == null)
                    return;

                items[key.Key] = data;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        public virtual bool IsSet(CacheKey key)
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
            {
                var items = GetItems();
                return items?[key.Key] != null;
            }
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public virtual void Remove(CacheKey key)
        {
            using (new ReaderWriteLockDisposable(_locker))
            {
                var items = GetItems();
                items?.Remove(key.Key);
            }
        }

        /// <summary>
        /// Removes items by key prefix
        /// </summary>
        /// <param name="prefix">String key prefix</param>
        public virtual void RemoveByPrefix(string prefix)
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.UpgradeableRead))
            {
                var items = GetItems();
                if (items == null)
                    return;

                //get cache keys that matches pattern
                var regex = new Regex(prefix,
                    RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matchesKeys = items.Keys.Select(p => p.ToString()).Where(key => regex.IsMatch(key)).ToList();

                if (!matchesKeys.Any())
                    return;

                using (new ReaderWriteLockDisposable(_locker))
                {
                    //remove matching values
                    foreach (var key in matchesKeys)
                    {
                        items.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            using (new ReaderWriteLockDisposable(_locker))
            {
                var items = GetItems();
                items?.Clear();
            }
        }

        /// <summary>
        /// Dispose cache manager
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _locker?.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}