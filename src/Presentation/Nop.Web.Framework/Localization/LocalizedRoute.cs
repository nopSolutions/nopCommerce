using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
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
            if (data == null)
                return null;

            if (!DataSettingsHelper.DatabaseIsInstalled() || !SeoFriendlyUrlsForLanguagesEnabled)
                return data;

            //get requested page URL
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            var pageUrl = webHelper.GetRawUrl(context.HttpContext.Request);
            var applicationPath = context.HttpContext.Request.PathBase.HasValue ?
                context.HttpContext.Request.PathBase.Value : "/";

            //add language code to page URL in case if it's localized URL
            if (pageUrl.IsLocalizedUrl(applicationPath, true))
            {
                var seoCode = pageUrl.GetLanguageSeoCodeFromUrl(applicationPath, true);
                data.VirtualPath = string.Format("/{0}{1}", seoCode, data.VirtualPath);
            }

            return data;
        }

        /// <summary>
        /// Route request to the particular action
        /// </summary>
        /// <param name="context">A route context object</param>
        /// <returns>Task of the routing</returns>
        public override Task RouteAsync(RouteContext context)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled() || !SeoFriendlyUrlsForLanguagesEnabled)
                return base.RouteAsync(context);

            //get request path
            var applicationPath = context.HttpContext.Request.PathBase.HasValue ?
                context.HttpContext.Request.PathBase.Value : "/";
            var path = context.HttpContext.Request.PathBase.Value + context.HttpContext.Request.Path.Value;

            //path isn't localized, so no special action required
            if (!path.IsLocalizedUrl(applicationPath, true))
                return base.RouteAsync(context);

            //remove language code and application path from the path
            var newPath = path.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
            if (string.IsNullOrEmpty(newPath))
                newPath = "/";
            newPath = newPath.RemoveApplicationPathFromRawUrl(applicationPath);

            //set new request path and try to get route handler
            var originalPath = context.HttpContext.Request.Path;
            context.HttpContext.Request.Path = newPath;
            base.RouteAsync(context).Wait();

            //then return the original request path
            context.HttpContext.Request.Path = originalPath;
            return _target.RouteAsync(context);
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