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

        [Test]
        public void CanPerformLock()
        {
            var key = new CacheKey("Nop.Task");
            var expiration = TimeSpan.FromMinutes(2);

            var actionCount = 0;
            var action = new Action(() =>
            {
                var isSet = _staticCacheManager.GetAsync<object>(key, () => null);
                isSet.Should().NotBeNull();

                _staticCacheManager.PerformActionWithLock(key.Key, expiration,
                    () => Assert.Fail("Action in progress"))
                    .Should().BeFalse();

                if (++actionCount % 2 == 0)
                    throw new ApplicationException("Alternating actions fail");
            });

            _staticCacheManager.PerformActionWithLock(key.Key, expiration, action)
                .Should().BeTrue();
            actionCount.Should().Be(1);

            Assert.Throws<ApplicationException>(() =>
                _staticCacheManager.PerformActionWithLock(key.Key, expiration, action));

            actionCount.Should().Be(2);

            _staticCacheManager.PerformActionWithLock(key.Key, expiration, action)
                .Should().BeTrue();
            actionCount.Should().Be(3);
        }
    }
}
