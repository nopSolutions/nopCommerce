using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using StackExchange.Redis;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Represents a local in-memory cache with distributed synchronization by Redis
    /// </summary>
    /// <remarks>
    /// This class should be registered on IoC as singleton instance
    /// </remarks>
    public class RedisSynchronizedMemoryCache : ISynchronizedMemoryCache
    {
        #region Fields

        protected readonly string _processId;
        protected bool _disposed;

        /// <summary>
        /// Holds the keys known by this nopCommerce instance
        /// </summary>
        protected readonly ICacheKeyManager _keyManager;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IRedisConnectionWrapper _connection;
        protected readonly ConcurrentQueue<string> _messageQueue = new();
        protected readonly Timer _timer;

        #endregion

        #region Ctor

        public RedisSynchronizedMemoryCache(IMemoryCache memoryCache,
            IRedisConnectionWrapper connectionWrapper,
            ICacheKeyManager cacheKeyManager,
            AppSettings appSettings)
        {
            _processId = $"{Guid.NewGuid()}:{Environment.ProcessId}";

            _memoryCache = memoryCache;
            _keyManager = cacheKeyManager;
            _connection = connectionWrapper;

            var publishIntervalMs = appSettings.Get<DistributedCacheConfig>().PublishIntervalMs;
            if (publishIntervalMs > 0)
            {
                var timeSpan = TimeSpan.FromMilliseconds(publishIntervalMs);
                _timer = new(_ => PublishQueuedChangeEvents(), null, timeSpan, timeSpan);
            }

            Subscribe();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Subscribe to perform some operation when a message to the preferred/active node is broadcast
        /// </summary>
        protected void Subscribe()
        {
            var channel = ChannelPrefix;
            var subscriber = _connection.GetSubscriber();
            subscriber.Subscribe(channel + "*", (redisChannel, value) =>
            {
                var publisher = ((string)redisChannel).Replace(channel, "");
                if (publisher == _processId)
                    return;

                var keys = JsonConvert.DeserializeObject<string[]>(value);

                if (keys == null)
                    return;

                foreach (var key in keys)
                {
                    _memoryCache.Remove(key);
                    _keyManager.RemoveKey(key);
                }
            });
        }

        /// <summary>
        /// Unique channel prefix
        /// </summary>
        protected string ChannelPrefix => $"__change@{_connection.GetDatabase().Database}__{_connection.Instance}__:";

        /// <summary>
        /// Enqueue or publish change event
        /// </summary>
        /// <param name="key">The evicted cache key to be published</param>
        protected void PublishChangeEvent(object key)
        {
            var stringKey = key.ToString();

            if (_timer == null)
                BatchPublishChangeEvents(stringKey);
            else
                _messageQueue.Enqueue(stringKey);
        }

        /// <summary>
        /// Publish accumulated change events on key channel
        /// </summary>
        protected void PublishQueuedChangeEvents()
        {
            IEnumerable<string> getKeys()
            {
                while (_messageQueue.TryDequeue(out var key))
                    yield return key;
            }

            BatchPublishChangeEvents(getKeys().Distinct().ToArray());
        }

        /// <summary>
        /// Publish change events on key channel
        /// <param name="keys">The evicted entries to publish on the key channel.</param>
        /// </summary>
        protected void BatchPublishChangeEvents(params string[] keys)
        {
            if (keys.Length == 0)
                return;

            var subscriber = _connection.GetSubscriber();
            subscriber.Publish(
                $"{ChannelPrefix}{_processId}",
                JsonConvert.SerializeObject(keys),
                CommandFlags.FireAndForget);
        }

        /// <summary>
        /// The callback method to run after the entry is evicted
        /// </summary>
        /// <param name="key">The key of the entry being evicted.</param>
        /// <param name="value">The value of the entry being evicted.</param>
        /// <param name="reason">The <see cref="EvictionReason"/>.</param>
        /// <param name="state">The information that was passed when registering the callback.</param>
        protected void OnEviction(object key, object value, EvictionReason reason, object state)
        {
            switch (reason)
            {
                case EvictionReason.Replaced:
                case EvictionReason.TokenExpired: // e.g. clear cache event
                    PublishChangeEvent(key);
                    break;
                // don't publish here on removed, as it could be triggered by a redis event itself
                default:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <returns>The newly created <see cref="T:Microsoft.Extensions.Caching.Memory.ICacheEntry" /> instance.</returns>
        public ICacheEntry CreateEntry(object key)
        {
            return _memoryCache.CreateEntry(key).RegisterPostEvictionCallback(OnEviction);
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove(object key)
        {
            _memoryCache.Remove(key);

            //publish event manually instead of through eviction callback to avoid feedback loops
            PublishChangeEvent(key);
        }

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The located value or null.</param>
        /// <returns>True if the key was found.</returns>
        public bool TryGetValue(object key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _timer?.Dispose();
                var subscriber = _connection.GetSubscriber();
                subscriber.Unsubscribe(ChannelPrefix + "*");

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
