using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Services.Defaults;
using Nop.Services.Security;

namespace Nop.Services.Caching.CachingDefaults
{
    public partial class CacheKeyFactory : ICacheKeyFactory
    {
        private readonly IEncryptionService _encryptionService;

        public CacheKeyFactory(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        /// <summary>
        /// Creates the cache key
        /// </summary>
        /// <param name="keyFormater">Key formater string</param>
        /// <param name="keyObjects">Parameters to create cache key</param>
        /// <returns></returns>
        public virtual string CreateCacheKey(string keyFormater, params object[] keyObjects)
        {
            return keyObjects.Any() ? string.Format(keyFormater, keyObjects) : keyFormater;
        }

        /// <summary>
        /// Creates the hash sum of identifiers list
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual string CreateIdsHash(IEnumerable<int> ids)
        {
            return _encryptionService.CreateHash(Encoding.UTF8.GetBytes(string.Join(", ", ids.OrderBy(id => id))),
                NopCustomerServiceDefaults.DefaultHashedPasswordFormat);
        }
    }
}
