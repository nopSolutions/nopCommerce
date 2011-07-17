using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Plugins;

namespace Nop.Services.PromotionFeed
{
    /// <summary>
    /// Promotion feed service
    /// </summary>
    public partial class PromotionFeedService : IPromotionFeedService
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pluginFinder">Plugin finder</param>
        public PromotionFeedService(IPluginFinder pluginFinder)
        {
            this._pluginFinder = pluginFinder;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load promotion feed by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found promotion feed</returns>
        public virtual IPromotionFeed LoadPromotionFeedBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IPromotionFeed>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IPromotionFeed>();

            return null;
        }

        /// <summary>
        /// Load all promotion feeds
        /// </summary>
        /// <returns>Promotion feeds</returns>
        public virtual IList<IPromotionFeed> LoadAllPromotionFeeds()
        {
            var providers = _pluginFinder.GetPlugins<IPromotionFeed>();
            return providers.ToList();
        }

        #endregion
    }
}
