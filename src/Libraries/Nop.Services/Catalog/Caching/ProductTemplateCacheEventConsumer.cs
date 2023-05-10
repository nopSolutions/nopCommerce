using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product template cache event consumer
    /// </summary>
    public partial class ProductTemplateCacheEventConsumer : CacheEventConsumer<ProductTemplate>
    {
    }
}
