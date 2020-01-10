using Nop.Core.Domain.Localization;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Localization
{
    public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
    {
        protected override void ClearCache(LocaleStringResource entity)
        {
            RemoveByPrefix(NopLocalizationCachingDefaults.LocaleStringResourcesPrefixCacheKey);
        }
    }
}
