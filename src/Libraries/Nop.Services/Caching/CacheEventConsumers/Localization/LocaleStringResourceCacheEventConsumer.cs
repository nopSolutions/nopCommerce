using Nop.Core.Domain.Localization;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Localization
{
    /// <summary>
    /// Represents a locale string resource cache event consumer
    /// </summary>
    public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(LocaleStringResource entity)
        {
            RemoveByPrefix(NopLocalizationCachingDefaults.LocaleStringResourcesPrefixCacheKey);
        }
    }
}
