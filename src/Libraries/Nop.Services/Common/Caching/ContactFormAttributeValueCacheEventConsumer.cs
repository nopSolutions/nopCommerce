using Nop.Core.Domain.Common;
using Nop.Services.Attributes;
using Nop.Services.Caching;

namespace Nop.Services.Common.Caching;

/// <summary>
/// Represents a contact form attribute value cache event consumer
/// </summary>
public partial class ContactFormAttributeValueCacheEventConsumer : CacheEventConsumer<ContactFormAttributeValue>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ContactFormAttributeValue entity)
    {
        await RemoveAsync(NopAttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(ContactFormAttribute), entity.AttributeId);
    }
}