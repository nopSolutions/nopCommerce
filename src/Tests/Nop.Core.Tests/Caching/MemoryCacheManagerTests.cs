using System;
using System.Collections.Generic;
using EasyCaching.InMemory;
using FluentAssertions;
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
            _cacheManager = new MemoryCacheManager(new DefaultInMemoryCachingProvider("nopCommerce.tests",
                new List<IInMemoryCaching> {new InMemoryCaching("nopCommerce.tests", new InMemoryCachingOptions())},
                new InMemoryOptions()));
        }

        [Test]
        public void Can_set_and_get_object_from_cache()
        {
            _cacheManager.Set("some_key_1", 3, int.MaxValue);
            _cacheManager.Get("some_key_1", () => 0).Should().Be(3);
        }

        [Test]
        public void Can_validate_whetherobject_is_cached()
        {
            _cacheManager.Set("some_key_1", 3, int.MaxValue);
            _cacheManager.Set("some_key_2", 4, int.MaxValue);

            _cacheManager.IsSet("some_key_1").Should().BeTrue();
            _cacheManager.IsSet("some_key_3").Should().BeFalse();
        }

        [Test]
        public void Can_clear_cache()
        {
            _cacheManager.Set("some_key_1", 3, int.MaxValue);

            _cacheManager.Clear();

            _cacheManager.IsSet("some_key_1").Should().BeFalse();
        }

        [Test]
        public void Can_perform_lock()
        {
            const string key = "Nop.Task";
            var expiration = TimeSpan.FromMinutes(2);

            var actionCount = 0;
            var action = new Action(() =>
            {
                _cacheManager.IsSet(key).Should().BeTrue();

                _cacheManager.PerformActionWithLock(key, expiration,
                    () => Assert.Fail("Action in progress"))
                    .Should().BeFalse();

                if (++actionCount % 2 == 0)
                    throw new ApplicationException("Alternating actions fail");
            });

            _cacheManager.PerformActionWithLock(key, expiration, action)
                .Should().BeTrue();
            actionCount.Should().Be(1);

            _cacheManager.Invoking(a => a.PerformActionWithLock(key, expiration, action)).Should().Throw<ApplicationException>();
            actionCount.Should().Be(2);

            _cacheManager.PerformActionWithLock(key, expiration, action)
                .Should().BeTrue();
            actionCount.Should().Be(3);
        }
    }
}
