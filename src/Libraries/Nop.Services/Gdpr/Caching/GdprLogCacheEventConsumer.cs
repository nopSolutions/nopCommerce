using Nop.Core.Domain.Gdpr;
using Nop.Services.Caching;

namespace Nop.Services.Gdpr.Caching;

/// <summary>
/// Represents a GDPR log cache event consumer
/// </summary>
public partial class GdprLogCacheEventConsumer : CacheEventConsumer<GdprLog>
{
}