using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents key for caching objects
    /// </summary>
    public partial class CacheKey
    {
        #region Fields

        protected string _keyFormat = string.Empty;

        #endregion

        #region Ctor

        /// <summary>
        /// Initialize a new instance of CacheKey with additional parameters
        /// </summary>
        /// <param name="cacheKey">Cache key object</param>
        /// <param name="createCacheKeyParameters">Function to create parameters</param>
        /// <param name="keyObjects">Objects to create parameters</param>
        public CacheKey(CacheKey cacheKey, Func<object, object> createCacheKeyParameters, params object[] keyObjects)
        {
            Init(cacheKey.Key, cacheKey.CacheTime, cacheKey.Prefixes.ToArray());

            if (!keyObjects.Any())
                return;

            Key = string.Format(_keyFormat, keyObjects.Select(createCacheKeyParameters).ToArray());

            for (var i = 0; i < Prefixes.Count; i++)
                Prefixes[i] = string.Format(Prefixes[i], keyObjects.Select(createCacheKeyParameters).ToArray());
        }

        /// <summary>
        /// Initialize a new instance of CacheKey with key, cache time and prefixes
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="cacheTime">Cache time; pass null to use the default value</param>
        /// <param name="prefixes">Prefixes for remove by prefix functionality</param>
        public CacheKey(string key, int? cacheTime = null, params string[] prefixes)
        {
            Init(key, cacheTime, prefixes);
        }

        /// <summary>
        /// Initialize a new instance of CacheKey with key and prefixes
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="prefixes">Prefixes for remove by prefix functionality</param>
        public CacheKey(string key, params string[] prefixes)
        {
            Init(key, null, prefixes);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Init instance of CacheKey
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cacheTime">Cache time; set to null to use the default value</param>
        /// <param name="prefixes">Prefixes for remove by prefix functionality</param>
        protected void Init(string cacheKey, int? cacheTime = null, params string[] prefixes)
        {
            Key = cacheKey;

            _keyFormat = cacheKey;

            if (cacheTime.HasValue)
                CacheTime = cacheTime.Value;

            Prefixes.AddRange(prefixes.Where(prefix => !string.IsNullOrEmpty(prefix)));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a cache key
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Gets or sets prefixes for remove by prefix functionality
        /// </summary>
        public List<string> Prefixes { get; protected set; } = new List<string>();

        /// <summary>
        /// Gets or sets a cache time in minutes
        /// </summary>
        public int CacheTime { get; set; } = Singleton<NopConfig>.Instance.DefaultCacheTime;

        #endregion
    }
}