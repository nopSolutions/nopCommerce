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
            await Remove(_cacheKeyService.PrepareKey(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity));
            await Remove(_cacheKeyService.PrepareKey(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity));
            await Remove(_cacheKeyService.PrepareKey(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity));

            var prefix = _cacheKeyService.PrepareKeyPrefix(NopLocalizationDefaults.LocaleStringResourcesByResourceNamePrefixCacheKey, entity);
            await RemoveByPrefix(prefix);

            await RemoveByPrefix(NopLocalizationDefaults.LanguagesAllPrefixCacheKey);
        }
    }
}