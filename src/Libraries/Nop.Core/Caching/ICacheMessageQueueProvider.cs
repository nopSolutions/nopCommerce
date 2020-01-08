using System.Collections.Concurrent;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Cache message queue provider interface.
    /// </summary>
    public interface ICacheMessageQueueProvider
    {

        /// <summary>
        /// Gets a queue with cache messages to process.
        /// </summary>
        ConcurrentQueue<CacheMessage> CacheMessageQueue { get; }
    }
}
