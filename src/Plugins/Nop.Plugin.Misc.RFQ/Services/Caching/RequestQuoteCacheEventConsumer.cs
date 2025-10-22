using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Services.Caching;

namespace Nop.Plugin.Misc.RFQ.Services.Caching;

/// <summary>
/// Represents a request a quote cache event consumer
/// </summary>
public class RequestQuoteCacheEventConsumer : CacheEventConsumer<RequestQuote>;