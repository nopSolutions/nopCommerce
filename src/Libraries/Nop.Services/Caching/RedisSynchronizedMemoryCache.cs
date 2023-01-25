using System;
using Microsoft.Extensions.Caching.Memory;
using Nop.Core.Caching;
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
        private bool _disposed;

        /// <summary>
        /// Holds the keys known by this nopCommerce instance
        /// </summary>
        private readonly ICacheKeyManager _keyManager;
        private readonly IMemoryCache _memoryCache;
        private readonly IRedisConnectionWrapper _connection;

        #endregion

        #region Ctor

        public RedisSynchronizedMemoryCache(IMemoryCache memoryCache,
            IRedisConnectionWrapper connectionWrapper,
            ICacheKeyManager cacheKeyManager)
        {
            _processId = $"{Guid.NewGuid()}:{Environment.ProcessId}";

            _memoryCache = memoryCache;
            _keyManager = cacheKeyManager;
            _connection = connectionWrapper;

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
                if (value == _processId) 
                    return;

                var key = ((string)redisChannel).Replace(channel, "");
                _memoryCache.Remove(key);
                _keyManager.RemoveKey(key);
            });
        }

        /// <summary>
        /// Unique channel prefix
        /// </summary>
        protected string ChannelPrefix => $"__change@{_connection.GetDatabase().Database}__{_connection.Instance}__:";

        /// <summary>
        /// Publish change event on key channel
        /// </summary>
        /// <param name="key"></param>
        protected void PublishChangeEvent(object key)
        {
            var subscriber = _connection.GetSubscriber();

            var stringKey = key.ToString();
            subscriber.Publish($"{ChannelPrefix}{stringKey}", _processId, CommandFlags.FireAndForget);
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
                var subscriber = _connection.GetSubscriber();
                subscriber.Unsubscribe(ChannelPrefix + "*");

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
