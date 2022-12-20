using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Nop.Core.Caching;
using Nop.Core.Configuration;

namespace Nop.Services.Caching
{
    public class MemoryDistributedCacheManager : DistributedCacheManager
    {
        #region Fields

        private static readonly List<string> _keysList = new();

        #endregion

        #region Ctor

        public MemoryDistributedCacheManager(AppSettings appSettings, IDistributedCache distributedCache) : base(
            appSettings, distributedCache)
        {
            _onKeyAdded += key =>
            {
                using var _ = _locker.Lock();

                if (!_keysList.Contains(key.Key))
                    _keysList.Add(key.Key);
            };

            _onKeyRemoved += key =>
            {
                using var _ = _locker.Lock();

                if (_keysList.Contains(key.Key))
                    _keysList.Remove(key.Key);
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Remove items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            using (var _ = _locker.Lock())
            {
                prefix = PrepareKeyPrefix(prefix, prefixParameters);

                foreach (var key in _keysList
                             .Where(key => key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                             .ToList())
                {
                    await _distributedCache.RemoveAsync(key);
                    _keysList.Remove(key);
                }
            }

            await RemoveByPrefixInstanceDataAsync(prefix);
        }

        /// <summary>
        /// Remove items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        public override void RemoveByPrefix(string prefix, params object[] prefixParameters)
        {
            using (var _ = _locker.Lock())
            {
                prefix = PrepareKeyPrefix(prefix, prefixParameters);

                foreach (var key in _keysList
                             .Where(key => key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                             .ToList())
                {
                    _distributedCache.Remove(key);
                    _keysList.Remove(key);
                }
            }

            RemoveByPrefixInstanceData(prefix);
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task ClearAsync()
        {
            using (var _ = _locker.Lock())
            {
                foreach (var key in _keysList)
                    await _distributedCache.RemoveAsync(key);

                _keysList.Clear();
            }

            ClearInstanceData();
        }

        #endregion
    }
}