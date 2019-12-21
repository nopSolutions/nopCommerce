using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Events;
using Nop.Services.Seo;

namespace Nop.Web.Framework.Seo
{
    public class GenericRouteTransformer : DynamicRouteValueTransformer
    {
        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return new ValueTask<RouteValueDictionary>(values);

            if (httpContext is null || values is null)
                return new ValueTask<RouteValueDictionary>(values);

            if (!values.TryGetValue("GenericSeName", out var slugValue) || string.IsNullOrEmpty(slugValue as string))
                return new ValueTask<RouteValueDictionary>(values);

            var slug = slugValue as string;

            //performance optimization, we load a cached verion here. It reduces number of SQL requests for each page load
            var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
            var urlRecord = urlRecordService.GetBySlugCached(slug);
            //comment the line above and uncomment the line below in order to disable this performance "workaround"
            //var urlRecord = urlRecordService.GetBySlug(slug);

            //no URL record found
            if (urlRecord == null)
                return new ValueTask<RouteValueDictionary>(values);

            //virtual directory path
            var pathBase = httpContext.Request.PathBase;
            //if URL record is not active let's find the latest one
            if (!urlRecord.IsActive)
            {
                var activeSlug = urlRecordService.GetActiveSlug(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
                if (string.IsNullOrEmpty(activeSlug))
                    return new ValueTask<RouteValueDictionary>(values);

                //redirect to active slug if found
                values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{activeSlug}{httpContext.Request.QueryString}";
                values[NopPathRouteDefaults.PermanentRedirectFieldKey] = true;
                httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;

                return new ValueTask<RouteValueDictionary>(httpContext.Request.RouteValues);
            }

            //ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when customers choose a new language but a slug stays the same
            var slugForCurrentLanguage = urlRecordService.GetSeName(urlRecord.EntityId, urlRecord.EntityName);
            if (!string.IsNullOrEmpty(slugForCurrentLanguage) && !slugForCurrentLanguage.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
            {
                //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)

                //redirect to the page for current language
                values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{slugForCurrentLanguage}{httpContext.Request.QueryString}";
                values[NopPathRouteDefaults.PermanentRedirectFieldKey] = false;
                httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;

                return new ValueTask<RouteValueDictionary>(values);
            }

            //since we are here, all is ok with the slug, so process URL
            switch (urlRecord.EntityName.ToLowerInvariant())
            {
                case "product":
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Product";
                    values[NopPathRouteDefaults.ActionFieldKey] = "ProductDetails";
                    values[NopPathRouteDefaults.ProductIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "producttag":
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "ProductsByTag";
                    values[NopPathRouteDefaults.ProducttagIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "category":
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "Category";
                    values[NopPathRouteDefaults.CategoryIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "manufacturer":
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "Manufacturer";
                    values[NopPathRouteDefaults.ManufacturerIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "vendor":
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "Vendor";
                    values[NopPathRouteDefaults.VendorIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "newsitem":
                    values[NopPathRouteDefaults.ControllerFieldKey] = "News";
                    values[NopPathRouteDefaults.ActionFieldKey] = "NewsItem";
                    values[NopPathRouteDefaults.NewsItemIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "blogpost":
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Blog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "BlogPost";
                    values[NopPathRouteDefaults.BlogPostIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                case "topic":
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Topic";
                    values[NopPathRouteDefaults.ActionFieldKey] = "TopicDetails";
                    values[NopPathRouteDefaults.TopicIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;
                default:
                    //no record found, thus generate an event this way developers could insert their own types
                    EngineContext.Current.Resolve<IEventPublisher>()
                        ?.Publish(new CustomUrlRecordEntityNameRequestedEvent(httpContext.GetRouteData(), urlRecord));
                    break;
            }

            return new ValueTask<RouteValueDictionary>(values);
        }
    }
}