using Nop.Core.Domain.Localization;
using Nop.Services.Caching;

namespace Nop.Services.Localization.Caching;

/// <summary>
/// Represents a locale string resource cache event consumer
/// </summary>
public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
{
    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(LocaleStringResource entity)
    {
        await RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity.LanguageId);
        await RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity.LanguageId);
        await RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity.LanguageId);
        await RemoveByPrefixAsync(NopLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity.LanguageId);
    }
}