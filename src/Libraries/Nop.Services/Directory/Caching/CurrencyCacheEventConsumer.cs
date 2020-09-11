using Nop.Core.Domain.Directory;
using Nop.Services.Caching;

namespace Nop.Services.Directory.Caching
{
    /// <summary>
    /// Represents a currency cache event consumer
    /// </summary>
    public partial class CurrencyCacheEventConsumer : CacheEventConsumer<Currency>
    {
    }
}
