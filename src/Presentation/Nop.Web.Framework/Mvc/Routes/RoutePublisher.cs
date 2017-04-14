using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Extensions;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;

namespace Nop.Web.Framework.Mvc.Routes
{
    /// <summary>
    /// Represents implementation of route publisher
    /// </summary>
    public class RoutePublisher : IRoutePublisher
    {
        #region Fields

        protected readonly ITypeFinder typeFinder;

        #endregion

        #region Ctor

        public RoutePublisher(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Find a plugin descriptor by some type which is located into its assembly
        /// </summary>
        /// <param name="providerType">Provider type</param>
        /// <returns>Plugin descriptor</returns>
        protected virtual PluginDescriptor FindPlugin(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");

            return PluginManager.ReferencedPlugins.FirstOrDefault(plugin => plugin.ReferencedAssembly != null 
                && plugin.ReferencedAssembly.FullName.Equals(providerType.Assembly.FullName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public virtual void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //find route providers provided by other assemblies
            var routeProviders = typeFinder.FindClassesOfType<IRouteProvider>();

            //create and sort instances of route providers
            var instances = routeProviders
                .Where(routeProvider => FindPlugin(routeProvider).Return(provider => provider.Installed, false)) //ignore not installed plugins
                .Select(routeProvider => (IRouteProvider)Activator.CreateInstance(routeProvider))
                .OrderByDescending(routeProvider => routeProvider.Priority);

            //register all provided routes
            foreach (var routeProvider in instances)
                routeProvider.RegisterRoutes(routeBuilder);
        }

        #endregion
    }
}
