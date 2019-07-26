using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Framework.Seo
{
    /// <summary>
    /// Provides properties and methods for defining a SEO friendly route, and for getting information about the route.
    /// </summary>
    public class GenericPathRoute : LocalizedRoute
    {

        #region Fields

        private readonly IRouter _target;

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
        public GenericPathRoute(IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults,
            IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get route values for current route
        /// </summary>
        /// <param name="context">Route context</param>
        /// <returns>Route values</returns>
        protected RouteValueDictionary GetRouteValues(RouteContext context)
        {
            //remove language code from the path if it's localized URL
            var path = context.HttpContext.Request.Path.Value;
            if (SeoFriendlyUrlsForLanguagesEnabled && path.IsLocalizedUrl(context.HttpContext.Request.PathBase, false, out Language _))
                path = path.RemoveLanguageSeoCodeFromUrl(context.HttpContext.Request.PathBase, false);

            //parse route data
            var routeValues = new RouteValueDictionary(ParsedTemplate.Parameters
                .Where(parameter => parameter.DefaultValue != null)
                .ToDictionary(parameter => parameter.Name, parameter => parameter.DefaultValue));
            var matcher = new TemplateMatcher(ParsedTemplate, routeValues);
            matcher.TryMatch(path, routeValues);

            return routeValues;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Route request to the particular action
        /// </summary>
        /// <param name="context">A route context object</param>
        /// <returns>Task of the routing</returns>
        public override Task RouteAsync(RouteContext context)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return Task.CompletedTask;

            //try to get slug from the route data
            var routeValues = GetRouteValues(context);
            if (!routeValues.TryGetValue("GenericSeName", out object slugValue) || string.IsNullOrEmpty(slugValue as string))
                return Task.CompletedTask;

            var slug = slugValue as string;

            //performance optimization, we load a cached verion here. It reduces number of SQL requests for each page load
            var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
            var urlRecord = urlRecordService.GetBySlugCached(slug);
            //comment the line above and uncomment the line below in order to disable this performance "workaround"
            //var urlRecord = urlRecordService.GetBySlug(slug);

            //no URL record found
            if (urlRecord == null)
                return Task.CompletedTask;

            //virtual directory path
            var pathBase = context.HttpContext.Request.PathBase;

            //if URL record is not active let's find the latest one
            if (!urlRecord.IsActive)
            {
                var activeSlug = urlRecordService.GetActiveSlug(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
                if (string.IsNullOrEmpty(activeSlug))
                    return Task.CompletedTask;

                //redirect to active slug if found
                var redirectionRouteData = new RouteData(context.RouteData);
                redirectionRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                redirectionRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                redirectionRouteData.Values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{activeSlug}{context.HttpContext.Request.QueryString}";
                redirectionRouteData.Values[NopPathRouteDefaults.PermanentRedirectFieldKey] = true;
                context.HttpContext.Items["nop.RedirectFromGenericPathRoute"] = true;
                context.RouteData = redirectionRouteData;
                return _target.RouteAsync(context);
            }

            //ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when customers choose a new language but a slug stays the same
            var slugForCurrentLanguage = urlRecordService.GetSeName(urlRecord.EntityId, urlRecord.EntityName);
            if (!string.IsNullOrEmpty(slugForCurrentLanguage) && !slugForCurrentLanguage.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
            {
                //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)

                //redirect to the page for current language
                var redirectionRouteData = new RouteData(context.RouteData);
                redirectionRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                redirectionRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                redirectionRouteData.Values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{slugForCurrentLanguage}{context.HttpContext.Request.QueryString}";
                redirectionRouteData.Values[NopPathRouteDefaults.PermanentRedirectFieldKey] = false;
                context.HttpContext.Items["nop.RedirectFromGenericPathRoute"] = true;
                context.RouteData = redirectionRouteData;
                return _target.RouteAsync(context);
            }

            //since we are here, all is ok with the slug, so process URL
            var currentRouteData = new RouteData(context.RouteData);
            switch (urlRecord.EntityName.ToLowerInvariant())
            {
                case "product":
                    currentRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "Product";
                    currentRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "ProductDetails";
                    currentRouteData.Values[NopPathRouteDefaults.ProductIdFieldKey] = urlRecord.EntityId;
                    currentRouteData.Values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "producttag":
                    currentRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    currentRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "ProductsByTag";
                    currentRouteData.Values[NopPathRouteDefaults.ProducttagIdFieldKey] = urlRecord.EntityId;
                    currentRouteData.Values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "category":
                    currentRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    currentRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "Category";
                    currentRouteData.Values[NopPathRouteDefaults.CategoryIdFieldKey] = urlRecord.EntityId;
                    currentRouteData.Values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "manufacturer":
                    currentRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    currentRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "Manufacturer";
                    currentRouteData.Values[NopPathRouteDefaults.ManufacturerIdFieldKey] = urlRecord.EntityId;
                    currentRouteData.Values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "vendor":
                    currentRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    currentRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "Vendor";
                    currentRouteData.Values[NopPathRouteDefaults.VendorIdFieldKey] = urlRecord.EntityId;
                    currentRouteData.Values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "newsitem":
                    currentRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "News";
                    currentRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "NewsItem";
                    currentRouteData.Values[NopPathRouteDefaults.NewsItemIdFieldKey] = urlRecord.EntityId;
                    currentRouteData.Values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "blogpost":
                    currentRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "Blog";
                    currentRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "BlogPost";
                    currentRouteData.Values[NopPathRouteDefaults.BlogPostIdFieldKey] = urlRecord.EntityId;
                    currentRouteData.Values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "topic":
                    currentRouteData.Values[NopPathRouteDefaults.ControllerFieldKey] = "Topic";
                    currentRouteData.Values[NopPathRouteDefaults.ActionFieldKey] = "TopicDetails";
                    currentRouteData.Values[NopPathRouteDefaults.TopicIdFieldKey] = urlRecord.EntityId;
                    currentRouteData.Values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                default:
                    //no record found, thus generate an event this way developers could insert their own types
                    EngineContext.Current.Resolve<IEventPublisher>()
                        ?.Publish(new CustomUrlRecordEntityNameRequestedEvent(currentRouteData, urlRecord));
                    break;
            }
            context.RouteData = currentRouteData;

            //route request
            return _target.RouteAsync(context);
        }

        #endregion
    }
}