using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Catalog;
using Nop.Services.Seo;
using Nop.Services.Catalog;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;

namespace Nop.Web.Framework.Mvc.Routing
{
    public static class UrlHelperExtensions
    {
        public static string SeName = "SeName";
        public static string CategorySeName = "CategorySeName";
        public static string ProductBreadcrumbRoute = "ProductBreadcrumb";
        public static string ProductRoute = "Product";

        private static (string routeName, object values) GetProductRouteData(object values)
        {
            if (EngineContext.Current.Resolve<CatalogSettings>().EnableProductUrlWithCategoryBreadcrumb)
            {
                var expando = new ExpandoObject();
                var dictionary = (IDictionary<string, object>)expando;
                foreach (var property in values.GetType().GetProperties())
                    dictionary.Add(property.Name, property.GetValue(values));

                if (dictionary.TryGetValue(SeName, out var slugValue))
                {
                    var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
                    var urlRecord = urlRecordService.GetBySlugAsync(slugValue as string).Result;

                    if (urlRecord != null && urlRecord.EntityName.Equals("product", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var categoryService = EngineContext.Current.Resolve<ICategoryService>();
                        var productCategory = categoryService.GetProductCategoriesByProductIdAsync(urlRecord.EntityId).Result.LastOrDefault();

                        if (productCategory != null)
                        {
                            var category = categoryService.GetCategoryByIdAsync(productCategory.CategoryId).Result;
                            var categorySlug = urlRecordService.GetSeNameAsync(category).Result;
                            if (dictionary.ContainsKey(CategorySeName))
                                dictionary.Remove(CategorySeName);

                            dictionary[CategorySeName] = categorySlug;
                            return (ProductBreadcrumbRoute, dictionary);
                        }
                    }
                }
            }

            return (ProductRoute, values);
        }

        public static string ProductRouteUrl(this IUrlHelper helper, object values)
        {
            var routeData = GetProductRouteData(values);
            return helper.RouteUrl(routeData.routeName, routeData.values);
        }

        public static string ProductRouteUrl(this IUrlHelper helper, object values, string protocol)
        {
            var routeData = GetProductRouteData(values);
            return helper.RouteUrl(routeData.routeName, routeData.values, protocol);
        }

        public static string ProductRouteUrl(this IUrlHelper helper, object values, string protocol, string host)
        {
            var routeData = GetProductRouteData(values);
            return helper.RouteUrl(routeData.routeName, routeData.values, protocol, host);
        }

        public static string ProductRouteUrl(this IUrlHelper helper, object values, string protocol, string host, string fragment)
        {
            var routeData = GetProductRouteData(values);
            return helper.RouteUrl(routeData.routeName, routeData.values, protocol, host, fragment);
        }
    }
}
