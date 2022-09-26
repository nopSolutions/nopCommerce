using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Caching
{
    [TestFixture]
    public class MemoryCacheManagerTests : BaseNopTest
    {
        private MemoryCacheManager _staticCacheManager;

        [OneTimeSetUp]
        public void Setup()
        {
            _staticCacheManager = GetService<IStaticCacheManager>() as MemoryCacheManager;
        }

        [Test]
        public async Task CanSetAndGetObjectFromCache()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);
            var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 0);
            rez.Should().Be(3);
        }

        [Test]
        public async Task CanValidateWhetherObjectIsCached()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);
            await _staticCacheManager.SetAsync(new CacheKey("some_key_2"), 4);

            var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 2);
            rez.Should().Be(3);
            rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), () => 2);
            rez.Should().Be(4);
        }

        [Test]
        public async Task CanClearCache()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);

            await _staticCacheManager.ClearAsync();

            var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => Task.FromResult((object)null));
            rez.Should().BeNull();
        }
    }
}
