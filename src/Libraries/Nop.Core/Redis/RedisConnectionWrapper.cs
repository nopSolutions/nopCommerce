using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Nop.Core.Redis
{
    /// <summary>
    /// Represents Redis connection wrapper implementation
    /// </summary>
    public class RedisConnectionWrapper : IRedisConnectionWrapper, ILocker
    {
        #region Fields

        private bool _disposed;

        private readonly Lazy<string> _connectionString;
        private volatile ConnectionMultiplexer _connection;
        private volatile RedLockFactory _redisLockFactory;

        private readonly AppSettings _appSettings;

        #endregion

        #region Ctor

        public RedisConnectionWrapper(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _connectionString = new Lazy<string>(GetConnectionString);
            _redisLockFactory = CreateRedisLockFactory();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get connection string to Redis cache from configuration
        /// </summary>
        /// <returns></returns>
        protected string GetConnectionString()
        {
            return _appSettings.RedisConfig.ConnectionString;
        }

        /// <summary>
        /// Gets all endpoints defined on the server
        /// </summary>
        /// <returns>Array of endpoints</returns>
        protected EndPoint[] GetEndPoints()
        {
            var connection = GetConnection();

            return connection.GetEndPoints();
        }

        /// <summary>
        /// Get connection to Redis servers
        /// </summary>
        /// <returns></returns>
        protected async Task<ConnectionMultiplexer> GetConnectionAsync()
        {
            if (_connection != null && _connection.IsConnected)
                return _connection;

            //Connection disconnected. Disposing connection...
            _connection?.Dispose();

            //Creating new instance of Redis Connection
            _connection = await ConnectionMultiplexer.ConnectAsync(_connectionString.Value);

            return _connection;
        }

        /// <summary>
        /// Get connection to Redis servers
        /// </summary>
        /// <returns></returns>
        protected ConnectionMultiplexer GetConnection()
        {
            if (_connection != null && _connection.IsConnected)
                return _connection;

            //Connection disconnected. Disposing connection...
            _connection?.Dispose();

            //Creating new instance of Redis Connection
            _connection = ConnectionMultiplexer.Connect(_connectionString.Value);

            return _connection;
        }

        /// <summary>
        /// Create instance of RedLock factory
        /// </summary>
        /// <returns>RedLock factory</returns>
        protected RedLockFactory CreateRedisLockFactory()
        {
            //get RedLock endpoints
            var configurationOptions = ConfigurationOptions.Parse(_connectionString.Value);
            var redLockEndPoints = GetEndPoints().Select(endPoint => new RedLockEndPoint
            {
                EndPoint = endPoint,
                Password = configurationOptions.Password,
                Ssl = configurationOptions.Ssl,
                RedisDatabase = configurationOptions.DefaultDatabase,
                ConfigCheckSeconds = configurationOptions.ConfigCheckSeconds,
                ConnectionTimeout = configurationOptions.ConnectTimeout,
                SyncTimeout = configurationOptions.SyncTimeout
            }).ToList();

            //create RedLock factory to use RedLock distributed lock algorithm
            return RedLockFactory.Create(redLockEndPoints);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Obtain an interactive connection to a database inside Redis
        /// </summary>
        /// <param name="db">Database number</param>
        /// <returns>Redis cache database</returns>
        public async Task<IDatabase> GetDatabaseAsync(int db)
        {
            var connection = await GetConnectionAsync();

            return connection.GetDatabase(db);
        }

        /// <summary>
        /// Obtain an interactive connection to a database inside Redis
        /// </summary>
        /// <param name="db">Database number</param>
        /// <returns>Redis cache database</returns>
        public IDatabase GetDatabase(int db)
        {
            var connection = GetConnection();

            return connection.GetDatabase(db);
        }

        /// <summary>
        /// Obtain a configuration API for an individual server
        /// </summary>
        /// <param name="endPoint">The network endpoint</param>
        /// <returns>Redis server</returns>
        public async Task<IServer> GetServerAsync(EndPoint endPoint)
        {
            var connection = await GetConnectionAsync();

            return connection.GetServer(endPoint);
        }

        /// <summary>
        /// Gets all endpoints defined on the server
        /// </summary>
        /// <returns>Array of endpoints</returns>
        public async Task<EndPoint[]> GetEndPointsAsync()
        {
            var connection = await GetConnectionAsync();

            return connection.GetEndPoints();
        }
        
        /// <summary>
        /// Perform some action with Redis distributed lock
        /// </summary>
        /// <param name="resource">The thing we are locking on</param>
        /// <param name="expirationTime">The time after which the lock will automatically be expired by Redis</param>
        /// <param name="action">Action to be performed with locking</param>
        /// <returns>True if lock was acquired and action was performed; otherwise false</returns>
        public bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action)
        {
            //use RedLock library
            using var redisLock = _redisLockFactory.CreateLock(resource, expirationTime);
            //ensure that lock is acquired
            if (!redisLock.IsAcquired)
                return false;

            //perform action
            action();

            return true;
        }

        /// <summary>
        /// Release all resources associated with this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //dispose ConnectionMultiplexer
                _connection?.Dispose();

                //dispose RedLock factory
                _redisLockFactory?.Dispose();
            }
            _disposed = true;
        }

        #endregion
    }
}