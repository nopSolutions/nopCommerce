using System;
using Microsoft.Extensions.Caching.Memory;
using Nop.Core.Caching;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Caching
{
    [TestFixture]
    public class MemoryCacheManagerTests
    {
        [Test]
        public void Can_set_and_get_object_from_cache()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
            cacheManager.Set("some_key_1", 3, int.MaxValue);

            cacheManager.Get("some_key_1", () => 0).ShouldEqual(3);
        }

        [Test]
        public void Can_validate_whetherobject_is_cached()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
            cacheManager.Set("some_key_1", 3, int.MaxValue);
            cacheManager.Set("some_key_2", 4, int.MaxValue);

            cacheManager.IsSet("some_key_1").ShouldEqual(true);
            cacheManager.IsSet("some_key_3").ShouldEqual(false);
        }

        [Test]
        public void Can_clear_cache()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
            cacheManager.Set("some_key_1", 3, int.MaxValue);

            cacheManager.Clear();

            cacheManager.IsSet("some_key_1").ShouldEqual(false);
        }

        [Test]
        public void Can_perform_lock()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));

            var key = "Nop.Task";
            var expiration = TimeSpan.FromMinutes(2);

            var actionCount = 0;
            var action = new Action(() =>
            {
                cacheManager.IsSet(key).ShouldBeTrue();

                cacheManager.PerformActionWithLock(key, expiration,
                    () => Assert.Fail("Action in progress"))
                    .ShouldBeFalse();

                if (++actionCount % 2 == 0)
                    throw new ApplicationException("Alternating actions fail");
            });

            cacheManager.PerformActionWithLock(key, expiration, action)
                .ShouldBeTrue();
            actionCount.ShouldEqual(1);

            Assert.Throws<ApplicationException>(() =>
                cacheManager.PerformActionWithLock(key, expiration, action));
            actionCount.ShouldEqual(2);

            cacheManager.PerformActionWithLock(key, expiration, action)
                .ShouldBeTrue();
            actionCount.ShouldEqual(3);
        }
    }
}
