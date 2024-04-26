using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Core.Configuration;

/// <summary>
/// Represents distributed cache configuration parameters
/// </summary>
public partial class DistributedCacheConfig : IConfig
{
    /// <summary>
    /// Gets or sets a distributed cache type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public DistributedCacheType DistributedCacheType { get; protected set; } = DistributedCacheType.RedisSynchronizedMemory;

    /// <summary>
    /// Gets or sets a value indicating whether we should use distributed cache
    /// </summary>
    public bool Enabled { get; protected set; } = false;

    /// <summary>
    /// Gets or sets connection string. Used when distributed cache is enabled
    /// </summary>
    public string ConnectionString { get; protected set; } = "127.0.0.1:6379,ssl=False";

    /// <summary>
    /// Gets or sets schema name. Used when distributed cache is enabled and DistributedCacheType property is set as SqlServer
    /// </summary>
    public string SchemaName { get; protected set; } = "dbo";

    /// <summary>
    /// Gets or sets table name. Used when distributed cache is enabled and DistributedCacheType property is set as SqlServer
    /// </summary>
    public string TableName { get; protected set; } = "DistributedCache";

    /// <summary>
    /// Gets or sets instance name. Used when distributed cache is enabled and DistributedCacheType property is set as Redis or RedisSynchronizedMemory.
    /// Useful when one wants to partition a single Redis server for use with multiple apps, e.g. by setting InstanceName to "development" and "production".
    /// </summary>
    public string InstanceName { get; protected set; } = "nopCommerce";

    /// <summary>
    /// Gets or sets the Redis event publish interval in milliseconds.
    /// Used when distributed cache is enabled and DistributedCacheType property is set as RedisSynchronizedMemory.
    /// If greater than zero, events will be buffered for this long before being published in batch, in order to reduce server load.
    /// If zero, events are published when they are raised, without buffering.
    /// </summary>
    public int PublishIntervalMs { get; protected set; } = 500;
}