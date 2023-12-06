using System.Net;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Redis connection wrapper
    /// </summary>
    /// <remarks>
    /// This class should be registered on IoC as singleton instance
    /// </remarks>
    public class RedisConnectionWrapper : IRedisConnectionWrapper
    {
        #region Fields

        protected readonly SemaphoreSlim _connectionLock = new(1, 1);
        protected volatile IConnectionMultiplexer _connection;
        protected readonly RedisCacheOptions _options;

        #endregion

        #region Ctor

        public RedisConnectionWrapper(IOptions<RedisCacheOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Create a new ConnectionMultiplexer instance
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<IConnectionMultiplexer> ConnectAsync()
        {
            IConnectionMultiplexer connection;

            if (_options.ConnectionMultiplexerFactory is null)
            {
                if (_options.ConfigurationOptions is not null)
                    connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions);
                else
                    connection = await ConnectionMultiplexer.ConnectAsync(_options.Configuration);
            }
            else
            {
                connection = await _options.ConnectionMultiplexerFactory();
            }

            if (_options.ProfilingSession != null)
                connection.RegisterProfiler(_options.ProfilingSession);

            return connection;
        }

        /// <summary>
        /// Create a new ConnectionMultiplexer instance
        /// </summary>
        /// <returns></returns>
        protected virtual IConnectionMultiplexer Connect()
        {
            IConnectionMultiplexer connection;

            if (_options.ConnectionMultiplexerFactory is null)
                connection = _options.ConfigurationOptions is not null ? ConnectionMultiplexer.Connect(_options.ConfigurationOptions) : ConnectionMultiplexer.Connect(_options.Configuration);
            else
                connection = _options.ConnectionMultiplexerFactory().GetAwaiter().GetResult();

            if (_options.ProfilingSession != null)
                connection.RegisterProfiler(_options.ProfilingSession);

            return connection;
        }

        /// <summary>
        /// Get connection to Redis servers, and reconnects if necessary
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<IConnectionMultiplexer> GetConnectionAsync()
        {
            if (_connection?.IsConnected == true)
                return _connection;

            await _connectionLock.WaitAsync();
            try
            {
                if (_connection?.IsConnected == true)
                    return _connection;

                //Connection disconnected. Disposing connection...
                _connection?.Dispose();

                //Creating new instance of Redis Connection
                _connection = await ConnectAsync();
            }
            finally
            {
                _connectionLock.Release();
            }

            return _connection;
        }

        /// <summary>
        /// Get connection to Redis servers, and reconnects if necessary
        /// </summary>
        /// <returns></returns>
        protected virtual IConnectionMultiplexer GetConnection()
        {
            if (_connection?.IsConnected == true)
                return _connection;

            _connectionLock.Wait();
            try
            {
                if (_connection?.IsConnected == true)
                    return _connection;

                //Connection disconnected. Disposing connection...
                _connection?.Dispose();

                //Creating new instance of Redis Connection
                _connection = Connect();
            }
            finally
            {
                _connectionLock.Release();
            }

            return _connection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Obtain an interactive connection to a database inside Redis
        /// </summary>
        /// <returns>Redis cache database</returns>
        public async Task<IDatabase> GetDatabaseAsync()
        {
            return (await GetConnectionAsync()).GetDatabase();
        }

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
        public async Task<IServer> GetServerAsync(EndPoint endPoint)
        {
            return (await GetConnectionAsync()).GetServer(endPoint);
        }

        /// <summary>
        /// Gets all endpoints defined on the server
        /// </summary>
        /// <returns>Array of endpoints</returns>
        public async Task<EndPoint[]> GetEndPointsAsync()
        {
            return (await GetConnectionAsync()).GetEndPoints();
        }

        /// <summary>
        /// Gets a subscriber for the server
        /// </summary>
        /// <returns>Array of endpoints</returns>
        public async Task<ISubscriber> GetSubscriberAsync()
        {
            return (await GetConnectionAsync()).GetSubscriber();
        }

        /// <summary>
        /// Gets a subscriber for the server
        /// </summary>
        /// <returns>Array of endpoints</returns>
        public ISubscriber GetSubscriber()
        {
            return GetConnection().GetSubscriber();
        }

        /// <summary>
        /// Delete all the keys of the database
        /// </summary>
        public async Task FlushDatabaseAsync()
        {
            var endPoints = await GetEndPointsAsync();
            await Task.WhenAll(endPoints.Select(async endPoint =>
            {
                var server = await GetServerAsync(endPoint);
                if (!server.IsReplica)
                {
                    await server.FlushDatabaseAsync();
                }
            }));
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

        #region Properties

        /// <summary>
        /// The Redis instance name
        /// </summary>
        public string Instance => _options.InstanceName ?? string.Empty;

        #endregion
    }
}
