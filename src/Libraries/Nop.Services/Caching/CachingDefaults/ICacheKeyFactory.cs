using System.Collections.Generic;

namespace Nop.Services.Caching.CachingDefaults
{
    public interface ICacheKeyFactory
    {
        /// <summary>
        /// Creates the cache key
        /// </summary>
        /// <param name="keyFormater">Key formater string</param>
        /// <param name="keyObjects">Parameters to create cache key</param>
        /// <returns></returns>
        string CreateCacheKey(string keyFormater, params object[] keyObjects);

        /// <summary>
        /// Creates the hash sum of identifiers list
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        string CreateIdsHash(IEnumerable<int> ids);
    }
}