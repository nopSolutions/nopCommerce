using System;
using System.Runtime.Serialization;
using System.Threading;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents a serializable cache message.
    /// </summary>
    [Serializable]
    [DataContract]
    public class CacheMessage
    {
        private static long _operationCounter = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheMessage"/> class.
        /// </summary>
        public CacheMessage()
        {
            OperationId = Interlocked.Increment(ref _operationCounter);
        }

        /// <summary>
        /// The Id of the operation. Ever increasing number to order operations.
        /// </summary>
        [DataMember]
        public long OperationId { get; set; }

        /// <summary>
        /// The operation to perform.
        /// </summary>
        [DataMember]
        public string Operation { get; set; }

        /// <summary>
        /// The key of the cache entry.
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// The cache entry itself.
        /// </summary>
        [DataMember]
        public CacheEntry Entry { get; set; }
    }

    /// <summary>
    /// Represents a cache entry.
    /// </summary>
    [Serializable]
    [DataContract]
    public struct CacheEntry
    {
        /// <summary>
        /// The value of the cahe entry.
        /// </summary>
        [DataMember]
        public object Value;

        /// <summary>
        /// The date time in UTC.
        /// </summary>
        [DataMember]
        public DateTime UtcDateTime;

        /// <summary>
        /// The time of the cache.
        /// </summary>
        [DataMember]
        public int CacheTime;

        /// <summary>
        /// Gets whether the Cache Entry is expired.
        /// </summary>
        public bool Expired => DateTime.UtcNow > UtcDateTime.AddMinutes(CacheTime);
    }
}
