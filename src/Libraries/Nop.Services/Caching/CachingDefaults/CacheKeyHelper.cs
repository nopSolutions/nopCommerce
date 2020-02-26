using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Defaults;
using Nop.Services.Security;

namespace Nop.Services.Caching.CachingDefaults
{
    public static partial class CacheKeyHelper
    {
        /// <summary>
        /// Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <returns>Data hash</returns>
        private static string CreateHash(byte[] data)
        {
            var hashAlgorithm = NopCustomerServiceDefaults.DefaultHashedPasswordFormat;

            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm);
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
            switch (parameter)
            {
                case null:
                    return "null";
                case IEnumerable<int> ids:
                    return CreateIdsHash(ids);
                case IEnumerable<BaseEntity> entities:
                    return CreateIdsHash(entities.Select(e => e.Id));
                case BaseEntity entity:
                    return entity.Id;
                case decimal param:
                    return param.ToString(CultureInfo.InvariantCulture);
            }

            return parameter;
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
