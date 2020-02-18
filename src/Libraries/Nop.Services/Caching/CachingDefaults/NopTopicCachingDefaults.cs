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
        /// {3} : customer role IDs
        /// </remarks>
        public static string TopicsAllWithACLCacheKey => "Nop.topics.acl-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic ID
        /// </remarks>
        public static string TopicsByIdCacheKey => "Nop.topics.id-{0}";

        /// <summary>
        /// Gets a pattern to clear cache
        /// </summary>
        public static string TopicsPrefixCacheKey => "Nop.topics.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string TopicTemplatesAllCacheKey => "Nop.topictemplates.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string TopicTemplatesPrefixCacheKey => "Nop.topictemplates.";
    }
}