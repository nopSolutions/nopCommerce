using System.Collections.Generic;

namespace Nop.Services.PromotionFeed
{
    /// <summary>
    /// Promotion feed service interface
    /// </summary>
    public partial interface IPromotionFeedService
    {
        /// <summary>
        /// Load promotion feed by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found promotion feed</returns>
        IPromotionFeed LoadPromotionFeedBySystemName(string systemName);

        /// <summary>
        /// Load all promotion feeds
        /// </summary>
        /// <returns>Promotion feeds</returns>
        IList<IPromotionFeed> LoadAllPromotionFeeds();
    }
}
