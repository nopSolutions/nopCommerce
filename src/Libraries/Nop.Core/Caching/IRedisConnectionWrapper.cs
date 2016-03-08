using System;
using System.Net;
using StackExchange.Redis;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Redis connection wrapper
    /// </summary>
    public interface IRedisConnectionWrapper : IDisposable
    {
        IDatabase Database(int? db = null);
        IServer Server(EndPoint endPoint);
        EndPoint[] GetEndpoints();
        void FlushDb(int? db = null);
    }
}
