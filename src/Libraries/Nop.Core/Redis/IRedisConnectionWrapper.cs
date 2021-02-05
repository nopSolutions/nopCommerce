using System;
using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Nop.Core.Redis
{
    /// <summary>
    /// Represents Redis connection wrapper
    /// </summary>
    public interface IRedisConnectionWrapper : IDisposable
    {
        /// <summary>
        /// Obtain an interactive connection to a database inside Redis
        /// </summary>
        /// <param name="db">Database number</param>
        /// <returns>Redis cache database</returns>
        Task<IDatabase> GetDatabaseAsync(int db);

        /// <summary>
        /// Obtain an interactive connection to a database inside Redis
        /// </summary>
        /// <param name="db">Database number</param>
        /// <returns>Redis cache database</returns>
        IDatabase GetDatabase(int db);

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
    }
}
