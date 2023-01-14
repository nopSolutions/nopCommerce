using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Nop.Core.Caching
{
    /// <summary>
    /// A distributed cache manager that locks the acquisition task
    /// </summary>
    public partial class MemoryCacheLocker : ILocker
    {
        #region Fields

        private readonly IMemoryCache _memoryCache;

        #endregion

        #region Ctor

        public MemoryCacheLocker(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #endregion

        #region Utilities

        private async Task<bool> RunAsync(string key, TimeSpan? expirationTime, Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource = default)
        {
            var started = false;
            try
            {
                var tokenSource = _memoryCache.GetOrCreate(key, entry => new Lazy<CancellationTokenSource>(() =>
                {
                    entry.AbsoluteExpirationRelativeToNow = expirationTime;
                    entry.SetPriority(CacheItemPriority.NeverRemove);
                    started = true;
                    return cancellationTokenSource ?? new();
                }, true)).Value;

                if (started)
                    await action(tokenSource.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (started)
                    _memoryCache.Remove(key);
            }
            return started;
        }

        #endregion

        #region Methods

        public async Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action)
        {
            return await RunAsync(resource, expirationTime, _ => action());
        }

        public async Task RunWithHeartbeatAsync(string key, TimeSpan expirationTime, TimeSpan heartbeatInterval, Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource = default)
        {
            // We ignore expirationTime and heartbeatInterval here, as the cache is not shared with other instances,
            // and will be cleared on system failure anyway. The task is guaranteed to still be running as long as it is in the cache.
            await RunAsync(key, null, action, cancellationTokenSource);
        }

        public Task CancelTaskAsync(string key, TimeSpan expirationTime)
        {
            if (_memoryCache.TryGetValue(key, out Lazy<CancellationTokenSource> tokenSource))
                tokenSource.Value.Cancel();
            return Task.CompletedTask;
        }

        public Task<bool> IsTaskRunningAsync(string key)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        #endregion
    }
}
