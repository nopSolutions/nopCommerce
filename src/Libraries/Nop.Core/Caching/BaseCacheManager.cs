using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Base class for cache manager
    /// </summary>
    /// <remarks>This class provides methods for cache keys managed</remarks>
    public abstract class BaseCacheManager
    {
        #region Fields

        /// <summary>
        /// Cancellation token for clear cache
        /// </summary>
        protected CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// All keys of cache
        /// </summary>
        /// <remarks>Dictionary value indicating whether a key still exists in cache</remarks> 
        protected static readonly ConcurrentDictionary<string, bool> _allKeys;

        #endregion

        #region Ctor

        static BaseCacheManager()
        {
            _allKeys = new ConcurrentDictionary<string, bool>();
        }

        protected BaseCacheManager()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Create entry options to item of memory cache 
        /// </summary>
        /// <param name="cacheTime">Cache time in minutes</param>
        /// <returns></returns>
        protected MemoryCacheEntryOptions GetMemoryCacheEntryOptions(int cacheTime)
        {
            var options = new MemoryCacheEntryOptions()
                // add cancellation token for clear cache
                .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
                //add post eviction callback
                .RegisterPostEvictionCallback(PostEviction);

            //set cache time
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTime);

            return options;
        }

        /// <summary>
        /// Add key to dictionary
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>Itself key</returns>
        protected string AddKey(string key)
        {
            _allKeys.TryAdd(key, true);
            return key;
        }

        /// <summary>
        /// Remove key from dictionary
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>Itself key</returns>
        protected string RemoveKey(string key)
        {
            TryRemoveKey(key);
            return key;
        }

        /// <summary>
        /// Try to remove a key from dictionary, or mark a key as not existing in cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        protected void TryRemoveKey(string key)
        {
            //try to remove key from dictionary
            if (_allKeys.TryRemove(key, out bool _))
                //if not possible to remove key from dictionary, then try to mark key as not existing in cache
                _allKeys.TryUpdate(key, false, false);
        }

        /// <summary>
        /// Remove all keys marked as not existing
        /// </summary>
        private void ClearKeys()
        {
            foreach (var key in _allKeys.Where(p => !p.Value).Select(p => p.Key).ToList())
            {
                RemoveKey(key);
            }
        }

        /// <summary>
        /// Post eviction
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="value">Value of cached item</param>
        /// <param name="reason">Eviction reason</param>
        /// <param name="state">State</param>
        private void PostEviction(object key, object value, EvictionReason reason, object state)
        {
            //if cached item just change, then nothing doing
            if (reason == EvictionReason.Replaced)
                return;

            //try to remove all keys marked as not existing
            ClearKeys();

            //try to remove this key from dictionary
            TryRemoveKey(key.ToString());
        }

        #endregion
    }
}
