using System.Net;
using StackExchange.Redis;

namespace Nop.Services.Caching;

/// <summary>
/// Represents Redis connection wrapper
/// </summary>
public partial interface IRedisConnectionWrapper
{
    /// <summary>
    /// Obtain an interactive connection to a database inside Redis
    /// </summary>
    /// <returns>Redis cache database</returns>
    Task<IDatabase> GetDatabaseAsync();

    /// <summary>
    /// Obtain an interactive connection to a database inside Redis
    /// </summary>
    /// <returns>Redis cache database</returns>
    IDatabase GetDatabase();

    /// <summary>
    /// Obtain a configuration API for an individual server
    /// </summary>
    /// <param name="endPoint">The network endpoint</param>
    /// <returns>Redis server</returns>
    Task<IServer> GetServerAsync(EndPoint endPoint);

    /// <summary>
    /// Gets all endpoints defined on the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    Task<EndPoint[]> GetEndPointsAsync();

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    Task<ISubscriber> GetSubscriberAsync();

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    ISubscriber GetSubscriber();

    /// <summary>
    /// Delete all the keys of the database
    /// </summary>
    Task FlushDatabaseAsync();

    /// <summary>
    /// The Redis instance name
    /// </summary>
    string Instance { get; }
}