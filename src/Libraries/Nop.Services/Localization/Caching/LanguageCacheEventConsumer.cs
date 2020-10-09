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
            Remove(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity);
            Remove(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity);
            Remove(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity);
            RemoveByPrefix(NopLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity);
        }
    }
}