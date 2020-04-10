using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Services.Defaults;

namespace Nop.Services.Caching.CachingDefaults
{
    public static partial class CacheKeyHelper
    {
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
            
            return HashHelper.CreateHash(Encoding.UTF8.GetBytes(string.Join(", ", identifiers.OrderBy(id => id))), NopCustomerServiceDefaults.DefaultHashedPasswordFormat);
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

        /// <summary>
        /// Creates the cache key
        /// </summary>
        /// <param name="keyFormater">Key formater string</param>
        /// <param name="keyObjects">Parameters to create cache key</param>
        /// <returns>Cache key</returns>
        public static string ToCacheKey(this string keyFormater, params object[] keyObjects)
        {
            return keyObjects?.Any() ?? false
                ? string.Format(keyFormater, keyObjects.Select(CreateCacheKeyParameters).ToArray())
                : keyFormater;
        }
    }
}
