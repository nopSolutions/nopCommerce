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
            Remove(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey.FillCacheKey(entity));
            Remove(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey.FillCacheKey(entity));
            Remove(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey.FillCacheKey(entity));

            var prefix = NopLocalizationDefaults.LocaleStringResourcesByResourceNamePrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            RemoveByPrefix(NopLocalizationDefaults.LanguagesAllPrefixCacheKey);
        }
    }
}