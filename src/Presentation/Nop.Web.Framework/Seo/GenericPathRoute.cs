using System.Web;
using System.Web.Routing;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Seo;

namespace Nop.Web.Framework.Seo
{
    /// <summary>
    /// Provides properties and methods for defining a SEO friendly route, and for getting information about the route.
    /// </summary>
    public class GenericPathRoute : Route
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern and handler class.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public GenericPathRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class and default parameter values.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public GenericPathRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values and constraints.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public GenericPathRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values, 
        /// constraints,and custom values.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="dataTokens">Custom values that are passed to the route handler, but which are not used to determine whether the route matches a specific URL pattern. The route handler might need these values to process the request.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public GenericPathRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns information about the requested route.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition.
        /// </returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData data = base.GetRouteData(httpContext);
            if (data != null && DataSettingsHelper.DatabaseIsInstalled())
            {
                var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
                var slug = data.Values["generic_se_name"] as string;
                var urlRecord = urlRecordService.GetBySlug(slug);
                if (urlRecord == null)
                {
                    //no record found
                    //data.Values["controller"] = "Home";
                    //data.Values["action"] = "Index";
                    return data;
                }

                //process URL
                switch (urlRecord.EntityName.ToLowerInvariant())
                {
                    case "product":
                        {
                            data.Values["controller"] = "Catalog";
                            data.Values["action"] = "Product";
                            data.Values["productid"] = urlRecord.EntityId;
                            data.Values["SeName"] = urlRecord.Slug;
                        }
                        break;
                    case "category":
                        {
                            data.Values["controller"] = "Catalog";
                            data.Values["action"] = "Category";
                            data.Values["categoryid"] = urlRecord.EntityId;
                            data.Values["SeName"] = urlRecord.Slug;
                        }
                        break;
                    case "manufacturer":
                        {
                            data.Values["controller"] = "Catalog";
                            data.Values["action"] = "Manufacturer";
                            data.Values["manufacturerid"] = urlRecord.EntityId;
                            data.Values["SeName"] = urlRecord.Slug;
                        }
                        break;
                    default:
                        {
                            //no record found
                            //data.Values["controller"] = "Home";
                            //data.Values["action"] = "Index";
                            return data;
                        }
                }
            }
            return data;
        }

        #endregion
    }
}