using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Services.Caching;

namespace Nop.Plugin.Misc.RFQ.Services.Caching;

/// <summary>
/// Represents an quote item cache event consumer
/// </summary>
public class QuoteItemCacheEventConsumer : CacheEventConsumer<QuoteItem>;