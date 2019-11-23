using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

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

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="routeName">Route name</param>
        /// <param name="routeTemplate">Route template</param>
        /// <param name="defaults">Defaults</param>
        /// <param name="constraints">Constraints</param>
        /// <param name="dataTokens">Data tokens</param>
        /// <param name="inlineConstraintResolver">Inline constraint resolver</param>
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

            if (!DataSettingsManager.DatabaseIsInstalled || !SeoFriendlyUrlsForLanguagesEnabled)
                return data;

            //add language code to page URL in case if it's localized URL
            var path = context.HttpContext.Request.Path.Value;
            if (path.IsLocalizedUrl(context.HttpContext.Request.PathBase, false, out Language language))
                data.VirtualPath = $"/{language.UniqueSeoCode}{data.VirtualPath}";

            return data;
        }

        /// <summary>
        /// Route request to the particular action
        /// </summary>
        /// <param name="context">A route context object</param>
        /// <returns>Task of the routing</returns>
        public override Task RouteAsync(RouteContext context)
        {
            if (!DataSettingsManager.DatabaseIsInstalled || !SeoFriendlyUrlsForLanguagesEnabled)
                return base.RouteAsync(context);

            //if path isn't localized, no special action required
            var path = context.HttpContext.Request.Path.Value;
            if (!path.IsLocalizedUrl(context.HttpContext.Request.PathBase, false, out Language _))
                return base.RouteAsync(context);

            //remove language code and application path from the path
            var newPath = path.RemoveLanguageSeoCodeFromUrl(context.HttpContext.Request.PathBase, false);

            //set new request path and try to get route handler
            context.HttpContext.Request.Path = newPath;
            base.RouteAsync(context).Wait();

            //then return the original request path
            context.HttpContext.Request.Path = path;
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