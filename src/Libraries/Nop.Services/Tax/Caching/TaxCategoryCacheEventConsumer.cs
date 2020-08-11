using Nop.Core.Domain.Tax;
using Nop.Services.Caching;

namespace Nop.Services.Tax.Caching
{
    /// <summary>
    /// Represents a TAX category cache event consumer
    /// </summary>
    public partial class TaxCategoryCacheEventConsumer : CacheEventConsumer<TaxCategory>
    {
    }
}
