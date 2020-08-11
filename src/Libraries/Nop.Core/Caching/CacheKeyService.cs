using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Core.Configuration;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents the default cache key service implementation
    /// </summary>
    public abstract partial class CacheKeyService
    {
        #region Constants

        /// <summary>
        /// Gets an algorithm used to create the hash value of identifiers need to cache
        /// </summary>
        private string HashAlgorithm => "SHA1";

        #endregion

        #region Fields

        protected readonly NopConfig _nopConfig;

        #endregion

        #region Ctor

        protected CacheKeyService(NopConfig nopConfig)
        {
            _nopConfig = nopConfig;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Creates the hash value of the passed identifiers
        /// </summary>
        /// <param name="ids">Collection of identifiers</param>
        /// <returns>String hash value</returns>
        protected virtual string CreateIdsHash(IEnumerable<int> ids)
        {
            var identifiers = ids.ToList();

            if (!identifiers.Any())
                return string.Empty;

            var identifiersString = string.Join(", ", identifiers.OrderBy(id => id));
            return HashHelper.CreateHash(Encoding.UTF8.GetBytes(identifiersString), HashAlgorithm);
        }

        /// <summary>
        /// Converts an object to cache parameter
        /// </summary>
        /// <param name="parameter">Object to convert</param>
        /// <returns>Cache parameter</returns>
        protected virtual object CreateCacheKeyParameters(object parameter)
        {
            return parameter switch
            {
                null => "null",
                IEnumerable<int> ids => CreateIdsHash(ids),
                IEnumerable<BaseEntity> entities => CreateIdsHash(entities.Select(entity => entity.Id)),
                BaseEntity entity => entity.Id,
                decimal param => param.ToString(CultureInfo.InvariantCulture),
                _ => parameter
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of cache key and fills it by set parameters
        /// </summary>
        /// <param name="cacheKey">Initial cache key</param>
        /// <param name="keyObjects">Parameters to create cache key</param>
        /// <returns>Cache key</returns>
        public virtual CacheKey PrepareKey(CacheKey cacheKey, params object[] keyObjects)
        {
            return cacheKey.Create(CreateCacheKeyParameters, keyObjects);
        }

        /// <summary>
        /// Creates a copy of cache key using the default cache time and fills it by set parameters
        /// </summary>
        /// <param name="cacheKey">Initial cache key</param>
        /// <param name="keyObjects">Parameters to create cache key</param>
        /// <returns>Cache key</returns>
        public virtual CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] keyObjects)
        {
            var key = cacheKey.Create(CreateCacheKeyParameters, keyObjects);

            key.CacheTime = _nopConfig.DefaultCacheTime;

            return key;
        }

        /// <summary>
        /// Creates a copy of cache key using the short cache time and fills it by set parameters
        /// </summary>
        /// <param name="cacheKey">Initial cache key</param>
        /// <param name="keyObjects">Parameters to create cache key</param>
        /// <returns>Cache key</returns>
        public virtual CacheKey PrepareKeyForShortTermCache(CacheKey cacheKey, params object[] keyObjects)
        {
            var key = cacheKey.Create(CreateCacheKeyParameters, keyObjects);

            key.CacheTime = _nopConfig.ShortTermCacheTime;

            return key;
        }

        /// <summary>
        /// Creates the cache key prefix
        /// </summary>
        /// <param name="keyFormatter">Key prefix formatter string</param>
        /// <param name="keyObjects">Parameters to create cache key prefix</param>
        /// <returns>Cache key prefix</returns>
        public virtual string PrepareKeyPrefix(string keyFormatter, params object[] keyObjects)
        {
            return keyObjects?.Any() ?? false
                ? string.Format(keyFormatter, keyObjects.Select(CreateCacheKeyParameters).ToArray())
                : keyFormatter;
        }

        #endregion
    }
}