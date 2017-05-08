using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Localization
{
    /// <summary>
    /// Provides properties and methods for defining a localized route, and for getting information about the localized route.
    /// </summary>
    public class LocalizedRoute : Route
    {
        #region Fields

        private readonly IRouter _target;
        private bool? _seoFriendlyUrlsForLanguagesEnabled;

        #endregion

        #region Ctor

        public LocalizedRoute(IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, 
            IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns information about the URL that is associated with the route
        /// </summary>
        /// <param name="context">A context for virtual path generation operations</param>
        /// <returns>Information about the route and virtual path</returns>
        public override VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            //get base virtual path
            var data = base.GetVirtualPath(context);
            
            if (data != null && DataSettingsHelper.DatabaseIsInstalled() && SeoFriendlyUrlsForLanguagesEnabled)
            {
                //get request path
                var path = context.HttpContext.Request.Path.Value;

                //get application path
#if NET451
                var applicationPath = context.HttpContext.Request.ApplicationPath;
#else
                var applicationPath = "/";
#endif

                //add language code to path in case if it's localized URL
                if (path.IsLocalizedUrl(applicationPath, true))
                    data.VirtualPath = string.Format("{0}/{1}", path.GetLanguageSeoCodeFromUrl(applicationPath, true), data.VirtualPath);
            }

            return data;
        }

        /// <summary>
        /// Route request to the particular action
        /// </summary>
        /// <param name="context">A route context object</param>
        /// <returns>Task of the routing</returns>
        public override async Task RouteAsync(RouteContext context)
        {
            if (DataSettingsHelper.DatabaseIsInstalled() && SeoFriendlyUrlsForLanguagesEnabled)
            {
                //get request path
                var path = context.HttpContext.Request.Path.Value;

                //get application path
#if NET451
                var applicationPath = context.HttpContext.Request.ApplicationPath;
#else
                var applicationPath = "/";
#endif

                //path isn't localized, so no special action required
                if (path.IsLocalizedUrl(applicationPath, false))
                {
                    //remove language code from the path
                    var newVirtualPath = path.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
                    if (string.IsNullOrEmpty(newVirtualPath))
                        newVirtualPath = "/";

                    //and application path
                    newVirtualPath = newVirtualPath.RemoveApplicationPathFromRawUrl(applicationPath);

                    //get path segments
                    //TODO test (and do not use "return")
                    var pathSegments = newVirtualPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (pathSegments == null || pathSegments.Length < 2)
                        return;

                    //create new route data
                    var newRouteData = new RouteData(context.RouteData);
                    newRouteData.Values["controller"] = pathSegments[0];
                    newRouteData.Values["action"] = pathSegments[1];
                    context.RouteData = newRouteData;
                }
            }
            //route request
            await _target.RouteAsync(context);
        }

        /// <summary>
        /// Clear _seoFriendlyUrlsForLanguagesEnabled cached value
        /// </summary>
        public virtual void ClearSeoFriendlyUrlsCachedValue()
        {
            _seoFriendlyUrlsForLanguagesEnabled = null;
        }

#endregion

#region Properties

        /// <summary>
        /// Gets value of _seoFriendlyUrlsForLanguagesEnabled settings
        /// </summary>
        protected bool SeoFriendlyUrlsForLanguagesEnabled
        {
            get
            {
                if (!_seoFriendlyUrlsForLanguagesEnabled.HasValue)
                    _seoFriendlyUrlsForLanguagesEnabled = EngineContext.Current.Resolve<LocalizationSettings>().SeoFriendlyUrlsForLanguagesEnabled;

                return _seoFriendlyUrlsForLanguagesEnabled.Value;
            }
        }

#endregion
    }
}