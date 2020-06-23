using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Caching
{
    public partial class CacheKey
    {
        #region Fields

        protected string _keyFormat = "";

        #endregion

        #region Ctor

        public CacheKey(CacheKey cacheKey, Func<object, object> createCacheKeyParameters, params object[] keyObjects)
        {
            Init(cacheKey.Key, cacheKey.CacheTime, cacheKey.Prefixes.ToArray());

            if(!keyObjects.Any())
                return;

            Key = string.Format(_keyFormat, keyObjects.Select(createCacheKeyParameters).ToArray());

            for (var i = 0; i < Prefixes.Count; i++)
                Prefixes[i] = string.Format(Prefixes[i], keyObjects.Select(createCacheKeyParameters).ToArray());
        }

        public CacheKey(string cacheKey, int? cacheTime = null, params string[] prefixes)
        {
            Init(cacheKey, cacheTime, prefixes);
        }

        public CacheKey(string cacheKey, params string[] prefixes)
        {
            Init(cacheKey, null, prefixes);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Init instance of CacheKey
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cacheTime">Cache time; set to null to use the default value</param>
        /// <param name="prefixes">Prefixes to remove by prefix functionality</param>
        protected void Init(string cacheKey, int? cacheTime = null, params string[] prefixes)
        {
            Key = cacheKey;

            _keyFormat = cacheKey;

            if (cacheTime.HasValue)
                CacheTime = cacheTime.Value;

            Prefixes.AddRange(prefixes.Where(prefix=> !string.IsNullOrEmpty(prefix)));
        }

        #endregion

        /// <summary>
        /// Cache key
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Prefixes to remove by prefix functionality
        /// </summary>
        public List<string> Prefixes { get; protected set; } = new List<string>();

        /// <summary>
        /// Cache time in minutes
        /// </summary>
        public int CacheTime { get; set; } = NopCachingDefaults.CacheTime;
    }
}
