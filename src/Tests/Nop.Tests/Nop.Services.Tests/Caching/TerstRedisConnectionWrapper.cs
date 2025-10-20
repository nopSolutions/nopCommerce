using System.Net;
using Moq;
using Nop.Services.Caching;
using StackExchange.Redis;

namespace Nop.Tests.Nop.Services.Tests.Caching;

internal class TestRedisConnectionWrapper : IRedisConnectionWrapper
{
    protected static int _npp = 1;

    protected readonly ISubscriber _subscriber = new TestSubscriber();
    protected readonly IDatabase _database;
    protected readonly IServer _server;

    protected string _instance;

    public TestRedisConnectionWrapper()
    {
        _instance = $"test_redis_connection_wrapper_{_npp}";
        var database = new Mock<IDatabase>();
        database.Setup(d => d.Database).Returns(_npp);
        _database = database.Object;
        var server = new Mock<IServer>();
        _server = server.Object;

        _npp += 1;
    }

    /// <summary>
    /// Obtain an interactive connection to a database inside Redis
    /// </summary>
    /// <returns>Redis cache database</returns>
    public Task<IDatabase> GetDatabaseAsync()
    {
        return Task.FromResult(GetDatabase());
    }

    /// <summary>
    /// Obtain an interactive connection to a database inside Redis
    /// </summary>
    /// <returns>Redis cache database</returns>
    public IDatabase GetDatabase()
    {
        return _database;
    }

    /// <summary>
    /// Obtain a configuration API for an individual server
    /// </summary>
    /// <param name="endPoint">The network endpoint</param>
    /// <returns>Redis server</returns>
    public Task<IServer> GetServerAsync(EndPoint endPoint)
    {
        return Task.FromResult(_server);
    }

    /// <summary>
    /// Gets all endpoints defined on the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public Task<EndPoint[]> GetEndPointsAsync()
    {
        return Task.FromResult(new[] { _server.EndPoint });
    }

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public Task<ISubscriber> GetSubscriberAsync()
    {
        return Task.FromResult(GetSubscriber());
    }

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public ISubscriber GetSubscriber()
    {
        return _subscriber;
    }

    /// <summary>
    /// Delete all the keys of the database
    /// </summary>
    public async Task FlushDatabaseAsync()
    {
        await _server.FlushDatabaseAsync(_database.Database);
    }

    /// <summary>
    /// The Redis instance name
    /// </summary>
    public string Instance => _instance;
}
