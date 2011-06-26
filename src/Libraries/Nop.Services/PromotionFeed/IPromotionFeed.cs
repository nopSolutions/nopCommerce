using System.IO;
using System.Web.Routing;
using Nop.Core.Plugins;

namespace Nop.Services.PromotionFeed
{
    /// <summary>
    /// Provides an interface for creating a promotion feed
    /// </summary>
    public partial interface IPromotionFeed : IPlugin
    {
        #region Methods

        /// <summary>
        /// Generate a feed
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Generated feed</returns>
        void GenerateFeed(Stream stream);
        
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);

        #endregion
    }
}
