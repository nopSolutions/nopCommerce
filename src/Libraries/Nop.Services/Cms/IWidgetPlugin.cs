using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Plugins;

namespace Nop.Services.Cms
{
    /// <summary>
    /// Provides an interface for creating widgets
    /// </summary>
    public partial interface IWidgetPlugin : IPlugin
    {
        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        IList<string> GetWidgetZones();

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        /// <param name="viewComponentArguments">View component arguments</param>
        void GetDisplayWidgetRoute(out string viewComponentName, out RouteValueDictionary viewComponentArguments);
    }
}
