using Nop.Core.Domain.Directory;
using Nop.Services.Caching;

namespace Nop.Services.Directory.Caching
{
    /// <summary>
    /// Represents a country cache event consumer
    /// </summary>
    public partial class CountryCacheEventConsumer : CacheEventConsumer<Country>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Country entity)
        {
            RemoveByPrefix(NopDirectoryDefaults.CountriesPrefixCacheKey);
        }
    }
}