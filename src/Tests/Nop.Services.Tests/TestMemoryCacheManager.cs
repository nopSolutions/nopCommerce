using Microsoft.Extensions.Caching.Memory;
using Nop.Core.Caching;

namespace Nop.Services.Tests
{
    public class TestMemoryCacheManager : MemoryCacheManager
    {
        public override void Set(string key, object data, int cacheTime)
        {

        }

        public TestMemoryCacheManager(IMemoryCache cache) : base(cache)
        {
        }
    }
}
