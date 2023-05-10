using FluentAssertions;
using Nop.Core.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Caching
{
    [TestFixture]
    public class LockerTests : BaseNopTest
    {
        private ILocker _memCacheLocker;
        private ILocker _distCacheLocker;

        [OneTimeSetUp]
        public void Setup()
        {
            _memCacheLocker = GetService<MemoryCacheLocker>();
            _distCacheLocker = GetService<DistributedCacheLocker>();
        }

        [Test]
        public async Task Distributed_CanPerformLockAsync()
        {
            await TestLockerAsync(_distCacheLocker);
        }

        [Test]
        public async Task Memory_CanPerformLockAsync()
        {
            await TestLockerAsync(_memCacheLocker);
        }

        [Test]
        public async Task Distributed_CanRunWithHeartbeatAsync()
        {
            await TestHeartbeatAsync(_distCacheLocker);
        }

        [Test]
        public async Task Memory_CanRunWithHeartbeatAsync()
        {
            await TestHeartbeatAsync(_memCacheLocker);
        }

        [Test]
        public async Task Distributed_CanCancelAsync()
        {
            await TestCancellationAsync(_distCacheLocker);
        }

        [Test]
        public async Task Memory_CanCancelAsync()
        {
            await TestCancellationAsync(_memCacheLocker);
        }

        private static async Task TestLockerAsync(ILocker locker)
        {
            var key = Guid.NewGuid().ToString();
            var expiration = TimeSpan.FromMinutes(2);

            var actionCount = 0;
            async Task action()
            {
                var res = await locker.PerformActionWithLockAsync(key, expiration,
                    () =>
                    {
                        Assert.Fail("Action in progress");
                        return Task.CompletedTask;
                    });

                res.Should().BeFalse();
                actionCount++;
            }

            var result = await locker.PerformActionWithLockAsync(key, expiration, () => action());
            result.Should().BeTrue();
            actionCount.Should().Be(1);

            Assert.ThrowsAsync<ApplicationException>(() => locker.PerformActionWithLockAsync(key, expiration, FailingAction));

            result = await locker.PerformActionWithLockAsync(key, expiration, action);
            result.Should().BeTrue();
            actionCount.Should().Be(2);
        }

        private static async Task TestHeartbeatAsync(ILocker locker)
        {
            var key = Guid.NewGuid().ToString();
            var expiration = TimeSpan.FromMinutes(2);
            var heartbeat = TimeSpan.FromSeconds(10);

            var actionCount = 0;
            async Task action(CancellationToken _)
            {
                await locker.RunWithHeartbeatAsync(
                    key,
                    expiration,
                    heartbeat,
                    _ =>
                    {
                        Assert.Fail("Action in progress");
                        return Task.CompletedTask;
                    });

                actionCount++;
            }

            await locker.RunWithHeartbeatAsync(key, expiration, heartbeat, action);
            actionCount.Should().Be(1);

            Assert.ThrowsAsync<ApplicationException>(() => locker.RunWithHeartbeatAsync(key, expiration, heartbeat, _ => FailingAction()));

            await locker.RunWithHeartbeatAsync(key, expiration, heartbeat, action);
            actionCount.Should().Be(2);
        }

        private static async Task TestCancellationAsync(ILocker locker)
        {
            var key = Guid.NewGuid().ToString();
            var expiration = TimeSpan.FromSeconds(1);
            var heartbeat = TimeSpan.FromMilliseconds(100);
            var delay = TimeSpan.FromSeconds(5);

            async Task testAsync(Func<Task> cancel, CancellationTokenSource tokenSource = default)
            {
                var cancelled = false;
                async Task action(CancellationToken token)
                {
                    await locker.RunWithHeartbeatAsync(
                        key,
                        expiration,
                        heartbeat,
                        _ =>
                        {
                            Assert.Fail("Action in progress");
                            return Task.CompletedTask;
                        });

                    try
                    {
                        await Task.Delay(delay, token);
                    }
                    catch
                    {
                        cancelled = true;
                    }
                }

                var task = locker.RunWithHeartbeatAsync(key, expiration, heartbeat, action, tokenSource);
                (await locker.IsTaskRunningAsync(key)).Should().BeTrue();
                await cancel();
                await task;
                cancelled.Should().BeTrue();
                (await locker.IsTaskRunningAsync(key)).Should().BeFalse();
            }

            await testAsync(() => locker.CancelTaskAsync(key, expiration));
            var cancellationTokenSource = new CancellationTokenSource();
            await testAsync(() => Task.Run(cancellationTokenSource.Cancel), cancellationTokenSource);
        }

        private static Task FailingAction()
        {
            throw new ApplicationException("This action is supposed to fail");
        }
    }
}
