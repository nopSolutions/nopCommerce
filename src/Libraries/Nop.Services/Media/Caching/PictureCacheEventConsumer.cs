using Nop.Core.Domain.Media;
using Nop.Services.Caching;

namespace Nop.Services.Media.Caching
{
    /// <summary>
    /// Represents a picture cache event consumer
    /// </summary>
    public partial class PictureCacheEventConsumer : CacheEventConsumer<Picture>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Picture entity)
        {
            RemoveByPrefix(NopMediaDefaults.ThumbsExistsPrefixCacheKey);
        }
    }
}
