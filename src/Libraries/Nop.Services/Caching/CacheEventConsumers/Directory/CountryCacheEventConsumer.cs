using Nop.Core.Domain.Directory;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Directory
{
    public partial class CountryCacheEventConsumer : CacheEventConsumer<Country>
    {
        protected override void ClearCache(Country entity)
        {
            RemoveByPrefix(NopDirectoryCachingDefaults.CountriesPrefixCacheKey);
        }
    }
}