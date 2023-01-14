using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Nop.Services.Caching
{
    public class RedisSynchronizedMemoryCache : IMemoryCache
    {
        private static readonly string _processId;

        private RedisConnectionWrapper _connection;

        private bool _disposed;
        private readonly IMemoryCache _memoryCache;
        private readonly string _ignorePrefix;


        static RedisSynchronizedMemoryCache()
        {
            var machineId = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            if (string.IsNullOrEmpty(machineId))
                machineId = Environment.MachineName;

            _processId = machineId + Environment.ProcessId.ToString();
        }


        public RedisSynchronizedMemoryCache(IMemoryCache memoryCache, RedisConnectionWrapper connectionWrapper, string ignorePrefix = null)
        {
            _memoryCache = memoryCache;
            _ignorePrefix = ignorePrefix;
            _connection = connectionWrapper;
            SubscribeAsync().Wait();
        }

        private async Task<string> GetChannelAsync()
        {
            var db = await _connection.GetDatabaseAsync();
            return $"__change@{db.Database}__{_connection.Instance}__:";
        }

        private async Task SubscribeAsync()
        {
            var channel = await GetChannelAsync();
            (await _connection.GetSubscriberAsync()).Subscribe(channel + "*", (redisChannel, value) =>
            {
                if (value != _processId)
                    _memoryCache.Remove(((string)redisChannel).Replace(channel, ""));
            });
        }

        private async Task PublishChangeEventAsync(object key)
        {
            var channel = await GetChannelAsync();
            var stringKey = key.ToString();
            if (string.IsNullOrEmpty(_ignorePrefix) || !stringKey.StartsWith(_ignorePrefix))
                await (await _connection.GetSubscriberAsync()).PublishAsync($"{channel}{stringKey}", _processId, CommandFlags.FireAndForget);
        }

        private void OnEviction(object key, object value, EvictionReason reason, object state)
        {
            switch (reason)
            {
                case EvictionReason.Replaced:
                case EvictionReason.TokenExpired: // e.g. clear cache event
                    _ = PublishChangeEventAsync(key);
                    break;
                // don't publish here on removed, as it could be triggered by a redis event itself
                default:
                    break;
            }
        }

        public ICacheEntry CreateEntry(object key)
        {
            return _memoryCache.CreateEntry(key).RegisterPostEvictionCallback(OnEviction);
        }

        public void Remove(object key)
        {
            _memoryCache.Remove(key);
            // publish event manually instead of through eviction callback to avoid feedback loops
            _ = PublishChangeEventAsync(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    var channel = await GetChannelAsync();
                    (await _connection.GetSubscriberAsync()).Unsubscribe(channel + "*");
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            DisposeAsync(disposing: true).Wait();
            GC.SuppressFinalize(this);
        }
    }
}
