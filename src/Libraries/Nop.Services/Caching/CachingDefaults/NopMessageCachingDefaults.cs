using Nop.Core.Caching;

namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to messages services
    /// </summary>
    public static partial class NopMessageCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        public static CacheKey MessageTemplatesAllCacheKey => new CacheKey("Nop.messagetemplate.all-{0}", MessageTemplatesAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string MessageTemplatesAllPrefixCacheKey => "Nop.messagetemplate.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : store ID
        /// </remarks>
        public static CacheKey MessageTemplatesByNameCacheKey => new CacheKey("Nop.messagetemplate.name-{0}-{1}", MessageTemplatesByNamePrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// </remarks>
        public static string MessageTemplatesByNamePrefixCacheKey => "Nop.messagetemplate.name-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        public static CacheKey EmailAccountsAllCacheKey => new CacheKey("Nop.emailaccounts.all");
    }
}