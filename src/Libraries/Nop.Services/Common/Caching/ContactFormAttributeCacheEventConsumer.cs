using Nop.Core.Domain.Common;
using Nop.Services.Attributes;
using Nop.Services.Caching;

namespace Nop.Services.Common.Caching;

/// <summary>
/// Represents a contact form attribute cache event consumer
/// </summary>
public partial class ContactFormAttributeCacheEventConsumer : CacheEventConsumer<ContactFormAttribute>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ContactFormAttribute entity)
    {
        await RemoveAsync(NopAttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(ContactFormAttribute), entity);
    }
}