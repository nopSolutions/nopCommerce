using Nop.Core.Domain.Directory;
using Nop.Services.Caching;

namespace Nop.Services.Directory.Caching
{
    /// <summary>
    /// Represents a currency cache event consumer
    /// </summary>
    public partial class CurrencyCacheEventConsumer : CacheEventConsumer<Currency>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Currency entity)
        {
            RemoveByPrefix(NopDirectoryDefaults.CurrenciesAllPrefixCacheKey);
        }
    }
}
