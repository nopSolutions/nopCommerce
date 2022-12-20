using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using StackExchange.Redis;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Represents a redis distributed cache 
    /// </summary>
    public class RedisCacheManager : DistributedCacheManager
    {
        #region Fields

        private static RedisConnectionWrapper _connectionWrapper;
        private readonly IDatabase _db;

        #endregion

        #region Ctor

        public RedisCacheManager(AppSettings appSettings, IDistributedCache distributedCache) : base(appSettings,
            distributedCache)
        {
            _connectionWrapper ??=
                new RedisConnectionWrapper(appSettings.Get<DistributedCacheConfig>().ConnectionString);
            _db = _connectionWrapper.GetDatabase();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets the list of cache keys prefix
        /// </summary>
        /// <param name="endPoint">Network address</param>
        /// <param name="prefix">String key pattern</param>
        /// <returns>List of cache keys</returns>
        protected virtual IEnumerable<RedisKey> GetKeys(EndPoint endPoint, string prefix = null)
        {
            var server = _connectionWrapper.GetServer(endPoint);

            var keys = server.Keys(_db.Database, string.IsNullOrEmpty(prefix) ? null : $"{prefix}*");

            return keys;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Remove items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            prefix = PrepareKeyPrefix(prefix, prefixParameters);

            foreach (var endPoint in _connectionWrapper.GetEndPoints())
            {
                var keys = GetKeys(endPoint, prefix);

                _db.KeyDelete(keys.ToArray());
            }

            await RemoveByPrefixInstanceDataAsync(prefix);
        }

        /// <summary>
        /// Remove items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        public override void RemoveByPrefix(string prefix, params object[] prefixParameters)
        {
            prefix = PrepareKeyPrefix(prefix, prefixParameters);

            foreach (var endPoint in _connectionWrapper.GetEndPoints())
            {
                var keys = GetKeys(endPoint, prefix);

                _db.KeyDelete(keys.ToArray());
            }

            RemoveByPrefixInstanceData(prefix);
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task ClearAsync()
        {
            await _connectionWrapper.FlushDatabaseAsync();

            ClearInstanceData();
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// Represents Redis connection wrapper implementation
        /// </summary>
        protected class RedisConnectionWrapper
        {
            #region Fields

            private readonly object _lock = new();
            private volatile ConnectionMultiplexer _connection;
            private readonly Lazy<string> _connectionString;

            #endregion

            #region Ctor

            public RedisConnectionWrapper(string connectionString)
            {
                _connectionString = new Lazy<string>(connectionString);
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Get connection to Redis servers
            /// </summary>
            /// <returns></returns>
            protected ConnectionMultiplexer GetConnection()
            {
                if (_connection != null && _connection.IsConnected)
                    return _connection;

                lock (_lock)
                {
                    if (_connection != null && _connection.IsConnected)
                        return _connection;

                    //Connection disconnected. Disposing connection...
                    _connection?.Dispose();

                    //Creating new instance of Redis Connection
                    _connection = ConnectionMultiplexer.Connect(_connectionString.Value);
                }

                return _connection;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Obtain an interactive connection to a database inside Redis
            /// </summary>
            /// <returns>Redis cache database</returns>
            public IDatabase GetDatabase()
            {
                return GetConnection().GetDatabase();
            }

            /// <summary>
            /// Obtain a configuration API for an individual server
            /// </summary>
            /// <param name="endPoint">The network endpoint</param>
            /// <returns>Redis server</returns>
            public IServer GetServer(EndPoint endPoint)
            {
                return GetConnection().GetServer(endPoint);
            }

            /// <summary>
            /// Gets all endpoints defined on the server
            /// </summary>
            /// <returns>Array of endpoints</returns>
            public EndPoint[] GetEndPoints()
            {
                return GetConnection().GetEndPoints();
            }

            /// <summary>
            /// Delete all the keys of the database
            /// </summary>
            public async Task FlushDatabaseAsync()
            {
                var endPoints = GetEndPoints();

                foreach (var endPoint in endPoints)
                    await GetServer(endPoint).FlushDatabaseAsync();
            }


            /// <summary>
            /// Release all resources associated with this object
            /// </summary>
            public void Dispose()
            {
                //dispose ConnectionMultiplexer
                _connection?.Dispose();
            }

            #endregion
        }

        #endregion
    }
}