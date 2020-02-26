namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to topic services
    /// </summary>
    public static partial class NopTopicCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// </remarks>
        public static string TopicsAllCacheKey => "Nop.topics.all-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// {3} : customer role IDs hash
        /// </remarks>
        public static string TopicsAllWithACLCacheKey => "Nop.topics.all.acl-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a pattern to clear cache
        /// </summary>
        public static string TopicsAllPrefixCacheKey => "Nop.topics.all";
        
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : store id
        /// {2} : customer roles Ids hash
        /// </remarks>
        public static string TopicBySystemNameCacheKey => "Nop.topics.systemName-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// </remarks>
        public static string TopicBySystemNamePrefixCacheKey => "Nop.topics.systemName-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string TopicTemplatesAllCacheKey => "Nop.topictemplates.all";
    }
}