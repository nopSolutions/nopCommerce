using Nop.Core.Domain.Localization;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Localization.Caching
{
    /// <summary>
    /// Represents a locale string resource cache event consumer
    /// </summary>
    public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(LocaleStringResource entity)
        {
            await Remove(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity.LanguageId);
            await Remove(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity.LanguageId);
            await Remove(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity.LanguageId);
            await RemoveByPrefix(NopLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity.LanguageId);
        }
    }
}
