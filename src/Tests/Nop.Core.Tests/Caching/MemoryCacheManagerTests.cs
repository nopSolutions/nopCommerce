using System;
using FluentAssertions;
using Nop.Core.Caching;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Caching
{
    [TestFixture]
    public class MemoryCacheManagerTests : BaseNopTest
    {
        private MemoryCacheManager _staticCacheManager;

        [SetUp]
        public void Setup()
        {
            _staticCacheManager = GetService<IStaticCacheManager>() as MemoryCacheManager;
        }

        [Test]
        public void CanSetAndGetObjectFromCache()
        {
            _staticCacheManager.Set(new CacheKey("some_key_1"), 3);
            _staticCacheManager.Get(new CacheKey("some_key_1"), () => 0).Should().Be(3);
        }

        [Test]
        public void CanValidateWhetherObjectIsCached()
        {
            _staticCacheManager.Set(new CacheKey("some_key_1"), 3);
            _staticCacheManager.Set(new CacheKey("some_key_2"), 4);

            _staticCacheManager.IsSet(new CacheKey("some_key_1")).Should().BeTrue();
            _staticCacheManager.IsSet(new CacheKey("some_key_3")).Should().BeFalse();
        }

        [Test]
        public void CanClearCache()
        {
            _staticCacheManager.Set(new CacheKey("some_key_1"), 3);

            _staticCacheManager.Clear();

            _staticCacheManager.IsSet(new CacheKey("some_key_1")).Should().BeFalse();
        }

        [Test]
        public void CanPerformLock()
        {
            var key = new CacheKey("Nop.Task");
            var expiration = TimeSpan.FromMinutes(2);

            var actionCount = 0;
            var action = new Action(() =>
            {
                _staticCacheManager.IsSet(key).Should().BeTrue();

                _staticCacheManager.PerformActionWithLock(key.Key, expiration,
                    () => Assert.Fail("Action in progress"))
                    .Should().BeFalse();

                if (++actionCount % 2 == 0)
                    throw new ApplicationException("Alternating actions fail");
            });

            _staticCacheManager.PerformActionWithLock(key.Key, expiration, action)
                .Should().BeTrue();
            actionCount.Should().Be(1);

            _staticCacheManager.Invoking(a => a.PerformActionWithLock(key.Key, expiration, action)).Should().Throw<ApplicationException>();
            actionCount.Should().Be(2);

            _staticCacheManager.PerformActionWithLock(key.Key, expiration, action)
                .Should().BeTrue();
            actionCount.Should().Be(3);
        }
    }
}
