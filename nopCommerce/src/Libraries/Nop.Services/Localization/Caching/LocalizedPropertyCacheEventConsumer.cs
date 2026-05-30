using Nop.Core.Domain.Localization;
using Nop.Services.Caching;

namespace Nop.Services.Localization.Caching;

/// <summary>
/// Represents a localized property cache event consumer
/// </summary>
public partial class LocalizedPropertyCacheEventConsumer : CacheEventConsumer<LocalizedProperty>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(LocalizedProperty entity)
    {
        await RemoveAsync(NopLocalizationDefaults.LocalizedPropertyCacheKey, entity.LanguageId, entity.EntityId, entity.LocaleKeyGroup, entity.LocaleKey);
        await RemoveAsync(NopLocalizationDefaults.LocalizedPropertiesCacheKey, entity.EntityId, entity.LocaleKeyGroup, entity.LocaleKey);
        await RemoveAsync(NopLocalizationDefaults.LocalizedPropertyLookupCacheKey, entity.LanguageId);
    }
}