using Nop.Core.Domain.Vendors;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a vendor review cache event consumer
/// </summary>
public partial class VendorReviewCacheEventConsumer : CacheEventConsumer<VendorReview>
{
}
