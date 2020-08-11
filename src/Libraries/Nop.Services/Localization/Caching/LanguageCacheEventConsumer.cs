using Nop.Core.Domain.Localization;
using Nop.Services.Caching;

namespace Nop.Services.Localization.Caching
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
            Remove(_staticCacheManager.PrepareKey(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity));
            Remove(_staticCacheManager.PrepareKey(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity));
            Remove(_staticCacheManager.PrepareKey(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity));
            RemoveByPrefix(_staticCacheManager.PrepareKeyPrefix(NopLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity));
        }
    }
}