using System;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Nop.Core.Caching;
using NUnit.Framework;

namespace Nop.Core.Tests.Caching
{
    [TestFixture]
    public class MemoryCacheManagerTests
    {
        private MemoryCacheManager _cacheManager;

        [SetUp]
        public void Setup()
        {
            _cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void Can_set_and_get_object_from_cache()
        {
            _cacheManager.Set(new CacheKey("some_key_1"), 3);
            _cacheManager.Get(new CacheKey("some_key_1"), () => 0).Should().Be(3);
        }

        [Test]
        public void Can_validate_whetherobject_is_cached()
        {
            _cacheManager.Set(new CacheKey("some_key_1"), 3);
            _cacheManager.Set(new CacheKey("some_key_2"), 4);

            _cacheManager.IsSet(new CacheKey("some_key_1")).Should().BeTrue();
            _cacheManager.IsSet(new CacheKey("some_key_3")).Should().BeFalse();
        }

        [Test]
        public void Can_clear_cache()
        {
            _cacheManager.Set(new CacheKey("some_key_1"), 3);

            _cacheManager.Clear();

            _cacheManager.IsSet(new CacheKey("some_key_1")).Should().BeFalse();
        }

        [Test]
        public void Can_perform_lock()
        {
            var key = new CacheKey("Nop.Task");
            var expiration = TimeSpan.FromMinutes(2);

            var actionCount = 0;
            var action = new Action(() =>
            {
                _cacheManager.IsSet(key).Should().BeTrue();

                _cacheManager.PerformActionWithLock(key.Key, expiration,
                    () => Assert.Fail("Action in progress"))
                    .Should().BeFalse();

                if (++actionCount % 2 == 0)
                    throw new ApplicationException("Alternating actions fail");
            });

            _cacheManager.PerformActionWithLock(key.Key, expiration, action)
                .Should().BeTrue();
            actionCount.Should().Be(1);

            _cacheManager.Invoking(a => a.PerformActionWithLock(key.Key, expiration, action)).Should().Throw<ApplicationException>();
            actionCount.Should().Be(2);

            _cacheManager.PerformActionWithLock(key.Key, expiration, action)
                .Should().BeTrue();
            actionCount.Should().Be(3);
        }
    }
}
