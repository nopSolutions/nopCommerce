using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Nop.Core.Caching
{
    public partial class CacheKey
    {
        #region Ctor

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

            if (cacheTime.HasValue)
                CacheTime = cacheTime.Value;

            Prefixes.AddRange(prefixes);
        }

        /// <summary>
        /// Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <returns>Data hash</returns>
        private static string CreateHash(byte[] data)
        {
            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName("SHA512");
            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name");

            var hashByteArray = algorithm.ComputeHash(data);
            return BitConverter.ToString(hashByteArray).Replace("-", string.Empty);
        }

        /// <summary>
        /// Creates the hash sum of identifiers list
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private static string CreateIdsHash(IEnumerable<int> ids)
        {
            var identifiers = ids.ToList();

            if (!identifiers.Any())
                return string.Empty;

            return CreateHash(Encoding.UTF8.GetBytes(string.Join(", ", identifiers.OrderBy(id => id))));
        }

        /// <summary>
        /// Converts an object to cache parameter
        /// </summary>
        /// <param name="parameter">Object to convert</param>
        /// <returns>Cache parameter</returns>
        private static object CreateCacheKeyParameters(object parameter)
        {
            return parameter switch
            {
                null => "null",
                IEnumerable<int> ids => CreateIdsHash(ids),
                IEnumerable<BaseEntity> entities => CreateIdsHash(entities.Select(e => e.Id)),
                BaseEntity entity => entity.Id,
                decimal param => param.ToString(CultureInfo.InvariantCulture),
                _ => parameter,
            };
        }

        #endregion

        /// <summary>
        /// Fills the cache key
        /// </summary>
        /// <param name="keyObjects">Parameters to create cache key</param>
        /// <returns>Cache key</returns>
        public CacheKey FillCacheKey(params object[] keyObjects)
        {
            Key = keyObjects?.Any() ?? false
                ? string.Format(Key, keyObjects.Select(CreateCacheKeyParameters).ToArray())
                : Key;

            for (var i = 0; i < Prefixes.Count; i++)
            {
                Prefixes[i] = keyObjects?.Any() ?? false
                    ? string.Format(Prefixes[i], keyObjects.Select(CreateCacheKeyParameters).ToArray())
                    : Prefixes[i];
            }

            return this;
        }

        /// <summary>
        /// Cache key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Prefixes to remove by prefix functionality
        /// </summary>
        public List<string> Prefixes { get; set; } = new List<string>();

        /// <summary>
        /// Cache time in minutes
        /// </summary>
        public int CacheTime { get; set; } = NopCachingDefaults.CacheTime;
    }
}
