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
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Language entity)
        {
            await RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity);
            await RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity);
            await RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity);
            await RemoveByPrefixAsync(NopLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity);
        }
    }
}