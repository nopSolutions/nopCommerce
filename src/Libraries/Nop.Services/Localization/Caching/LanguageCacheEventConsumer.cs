using Nop.Core.Domain.Localization;
using Nop.Services.Caching;
using System.Threading.Tasks;

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
        protected override async Task ClearCache(Language entity)
        {
            await Remove(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity);
            await Remove(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity);
            await Remove(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity);
            await RemoveByPrefix(NopLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity);
        }
    }
}