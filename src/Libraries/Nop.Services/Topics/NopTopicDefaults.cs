using Nop.Core.Caching;

namespace Nop.Services.Topics;

/// <summary>
/// Represents default values related to topic services
/// </summary>
public static partial class NopTopicDefaults
{
    #region Caching defaults

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : store ID
    /// {1} : show hidden?
    /// </remarks>
    public static CacheKey TopicsAllCacheKey => new("Nop.topic.all.{0}-{1}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : store ID
    /// {1} : show hidden?
    /// {2} : customer role IDs hash
    /// </remarks>
    public static CacheKey TopicsAllWithACLCacheKey => new("Nop.topic.all.withacl.{0}-{1}-{2}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : topic system name
    /// {1} : store id
    /// {2} : customer roles Ids hash
    /// </remarks>
    public static CacheKey TopicBySystemNameCacheKey => new("Nop.topic.bysystemname.{0}-{1}-{2}");

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : topic system name
    /// </remarks>
    public static string TopicBySystemNamePrefix => "Nop.topic.bysystemname.{0}";

    #endregion
}