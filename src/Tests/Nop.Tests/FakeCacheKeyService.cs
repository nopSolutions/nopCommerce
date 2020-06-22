using Nop.Core.Caching;
using Nop.Services.Caching;

namespace Nop.Tests
{
    public class FakeCacheKeyService : CacheKeyService
    {
        public FakeCacheKeyService() : base(new CachingSettings())
        {
        }
    }
}
