using Nop.Core.Caching;
using Nop.Services.Caching;

namespace Nop.Services.Tests.FakeServices
{
    public class FakeCacheKeyService : ICacheKeyService
    {
        public CacheKey PrepareKey(CacheKey cacheKey, params object[] keyObjects)
        {
            return cacheKey;
        }

        public CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] keyObjects)
        {
            return cacheKey;
        }

        public CacheKey PrepareKeyForShortTermCache(CacheKey cacheKey, params object[] keyObjects)
        {
            return cacheKey;
        }

        public string PrepareKeyPrefix(string keyFormatter, params object[] keyObjects)
        {
            return keyFormatter;
        }
    }
}
