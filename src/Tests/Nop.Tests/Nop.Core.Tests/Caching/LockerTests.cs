using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Caching;
using Nop.Services.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Caching
{
    [TestFixture]
    public class LockerTests : BaseNopTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CanPerformLock(bool isDistributed)
        {
            var staticCacheManager = isDistributed ? GetService<MemoryDistributedCacheManager>() : GetService<IStaticCacheManager>();
            staticCacheManager.Should().NotBeNull();
            var locker = staticCacheManager as ILocker;
            locker.Should().NotBeNull();

            var key = new CacheKey("Nop.Task");
            var expiration = TimeSpan.FromMinutes(2);

            var actionCount = 0;
            var action = new Func<Task>(async () =>
            {
                var isSet = staticCacheManager.GetAsync<object>(key, () => null);
                isSet.Should().NotBeNull();

                var rez = await locker.PerformActionWithLockAsync(key.Key, expiration,
                    () =>
                    {
                        Assert.Fail("Action in progress");
                        return Task.CompletedTask;
                    });
                    
                    rez.Should().BeFalse();

                if (++actionCount % 2 == 0)
                    throw new ApplicationException("Alternating actions fail");
            });

            var rez = await locker.PerformActionWithLockAsync(key.Key, expiration, action);
            rez.Should().BeTrue();

            actionCount.Should().Be(1);

            Assert.Throws<AggregateException>(() =>
                locker.PerformActionWithLockAsync(key.Key, expiration, action).Wait());

            actionCount.Should().Be(2);

            rez = await locker.PerformActionWithLockAsync(key.Key, expiration, action);
            rez.Should().BeTrue();
            actionCount.Should().Be(3);

            var dt = DateTime.Now;

            await locker.PerformActionWithLockAsync("action_with_lock", TimeSpan.FromSeconds(1), async () =>
            {
                while (!await locker.PerformActionWithLockAsync("action_with_lock", TimeSpan.FromSeconds(1), () => Task.CompletedTask))
                {
                }
            });

            var span = DateTime.Now - dt;

            span.Seconds.Should().Be(1);
        }
    }
}
