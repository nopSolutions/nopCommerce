using Nop.Core.Domain.Localization;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Localization
{
    /// <summary>
    /// Represents a language cache event consumer
    /// </summary>
    public partial class LanguageCacheEventConsumer : CacheEventConsumer<Language>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Language entity)
        {
            Remove(NopLocalizationCachingDefaults.LocaleStringResourcesAllPublicCacheKey.ToCacheKey(entity));
            Remove(NopLocalizationCachingDefaults.LocaleStringResourcesAllAdminCacheKey.ToCacheKey(entity));
            Remove(NopLocalizationCachingDefaults.LocaleStringResourcesAllCacheKey.ToCacheKey(entity));

            var prefix = NopLocalizationCachingDefaults.LocaleStringResourcesByResourceNamePrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            RemoveByPrefix(NopLocalizationCachingDefaults.LanguagesAllPrefixCacheKey);
        }
    }
}