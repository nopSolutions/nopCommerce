using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Events;
using Nop.Services.Seo;

namespace Nop.Web.Framework.Seo
{
    /// <summary>
    /// Provides properties and methods for defining a SEO friendly route, and for getting information about the route.
    /// </summary>
    public class GenericPathRoute : Route
    {
        #region Fields

        private readonly IRouter _target;

        #endregion

        #region Ctor

        public GenericPathRoute(IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, 
            IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Route request to the particular action
        /// </summary>
        /// <param name="context">A route context object</param>
        /// <returns>Task of the routing</returns>
        public override async Task RouteAsync(RouteContext context)
        {
            //get current route data
            var currentRouteData = new RouteData(context.RouteData);
            if (currentRouteData == null || !DataSettingsHelper.DatabaseIsInstalled())
                return;

            //get registered UrlRecordService
            var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();

            //get slug from path
            var slug = currentRouteData.Values["generic_se_name"] as string;

            //performance optimization
            //we load a cached verion here. It reduces number of SQL requests for each page load
            var urlRecord = urlRecordService.GetBySlugCached(slug);

            //comment the line above and uncomment the line below in order to disable this performance "workaround"
            //var urlRecord = urlRecordService.GetBySlug(slug);

            if (urlRecord == null)
            {
                //no URL record found
                currentRouteData.Values["controller"] = "Common";
                currentRouteData.Values["action"] = "PageNotFound";
                context.RouteData = currentRouteData;

                //route request
                await _target.RouteAsync(context);
                return;
            }

            if (!urlRecord.IsActive)
            {
                //URL record is not active. let's find the latest one
                var activeSlug = urlRecordService.GetActiveSlug(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
                if (string.IsNullOrEmpty(activeSlug))
                {
                    //no active slug found
                    currentRouteData.Values["controller"] = "Common";
                    currentRouteData.Values["action"] = "PageNotFound";
                    context.RouteData = currentRouteData;

                    //route request
                    await _target.RouteAsync(context);
                    return;
                }

                //the active one is found
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                var location = string.Format("{0}{1}", webHelper.GetStoreLocation(), activeSlug);
                context.HttpContext.Response.Redirect(location, true);
                return;
            }

            //ensure that the slug is the same for the current language
            //otherwise, it can cause some issues when customers choose a new language but a slug stays the same
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var slugForCurrentLanguage = SeoExtensions.GetSeName(urlRecord.EntityId, urlRecord.EntityName, workContext.WorkingLanguage.Id);
            if (!string.IsNullOrEmpty(slugForCurrentLanguage) && !slugForCurrentLanguage.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
            {
                //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                var location = string.Format("{0}{1}", webHelper.GetStoreLocation(), slugForCurrentLanguage);
                context.HttpContext.Response.Redirect(location, false);
                return;
            }

            //process URL
            switch (urlRecord.EntityName.ToLowerInvariant())
            {
                case "product":
                    currentRouteData.Values["controller"] = "Product";
                    currentRouteData.Values["action"] = "ProductDetails";
                    currentRouteData.Values["productid"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "category":
                    currentRouteData.Values["controller"] = "Catalog";
                    currentRouteData.Values["action"] = "Category";
                    currentRouteData.Values["categoryid"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "manufacturer":
                    currentRouteData.Values["controller"] = "Catalog";
                    currentRouteData.Values["action"] = "Manufacturer";
                    currentRouteData.Values["manufacturerid"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "vendor":
                    currentRouteData.Values["controller"] = "Catalog";
                    currentRouteData.Values["action"] = "Vendor";
                    currentRouteData.Values["vendorid"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "newsitem":
                    currentRouteData.Values["controller"] = "News";
                    currentRouteData.Values["action"] = "NewsItem";
                    currentRouteData.Values["newsItemId"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "blogpost":
                    currentRouteData.Values["controller"] = "Blog";
                    currentRouteData.Values["action"] = "BlogPost";
                    currentRouteData.Values["blogPostId"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "topic":
                    currentRouteData.Values["controller"] = "Topic";
                    currentRouteData.Values["action"] = "TopicDetails";
                    currentRouteData.Values["topicId"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                default:
                    //no record found
                    //thus generate an event this way developers could insert their own types
                    EngineContext.Current.Resolve<IEventPublisher>().Publish(new CustomUrlRecordEntityNameRequested(currentRouteData, urlRecord));
                    break;
            }
            context.RouteData = currentRouteData;

            //route request
            await _target.RouteAsync(context);
        }

        #endregion
    }
}