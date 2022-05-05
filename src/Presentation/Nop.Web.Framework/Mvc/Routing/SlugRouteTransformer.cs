using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Events;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents slug route transformer
    /// </summary>
    public class SlugRouteTransformer : DynamicRouteValueTransformer
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public SlugRouteTransformer(IEventPublisher eventPublisher,
            ILanguageService languageService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            LocalizationSettings localizationSettings)
        {
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Transform route values according to the passed URL record
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="urlRecord">Record found by the URL slug</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SingleSlugRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord)
        {
            //if URL record is not active let's find the latest one
            if (!urlRecord.IsActive)
            {
                var activeSlug = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
                if (string.IsNullOrEmpty(activeSlug))
                    return;

                //redirect to active slug if found
                InternalRedirect(httpContext, values, $"/{activeSlug}", true);
                return;
            }

            //Ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when customers choose a new language but a slug stays the same
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                if (values.TryGetValue(NopRoutingDefaults.RouteValue.Language, out var languageValue))
                {
                    var code = languageValue?.ToString();
                    var store = await _storeContext.GetCurrentStoreAsync();
                    var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
                    var language = languages
                        .FirstOrDefault(lang => lang.Published && lang.UniqueSeoCode.Equals(code, StringComparison.InvariantCultureIgnoreCase))
                        ?? languages.FirstOrDefault();

                    var slugForCurrentLanguage = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id);
                    if (!string.IsNullOrEmpty(slugForCurrentLanguage) && !slugForCurrentLanguage.Equals(urlRecord.Slug, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)

                        //redirect to the page for current language
                        InternalRedirect(httpContext, values, $"/{language.UniqueSeoCode}/{slugForCurrentLanguage}", false);
                        return;
                    }
                }
            }

            //since we are here, all is ok with the slug, so process URL
            switch (urlRecord.EntityName)
            {
                case var name when name.Equals(nameof(Product), StringComparison.InvariantCultureIgnoreCase):
                    values[NopRoutingDefaults.RouteValue.Controller] = "Product";
                    values[NopRoutingDefaults.RouteValue.Action] = "ProductDetails";
                    values[NopRoutingDefaults.RouteValue.ProductId] = urlRecord.EntityId;
                    values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(ProductTag), StringComparison.InvariantCultureIgnoreCase):
                    values[NopRoutingDefaults.RouteValue.Controller] = "Catalog";
                    values[NopRoutingDefaults.RouteValue.Action] = "ProductsByTag";
                    values[NopRoutingDefaults.RouteValue.ProductTagId] = urlRecord.EntityId;
                    values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(Category), StringComparison.InvariantCultureIgnoreCase):
                    values[NopRoutingDefaults.RouteValue.Controller] = "Catalog";
                    values[NopRoutingDefaults.RouteValue.Action] = "Category";
                    values[NopRoutingDefaults.RouteValue.CategoryId] = urlRecord.EntityId;
                    values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(Manufacturer), StringComparison.InvariantCultureIgnoreCase):
                    values[NopRoutingDefaults.RouteValue.Controller] = "Catalog";
                    values[NopRoutingDefaults.RouteValue.Action] = "Manufacturer";
                    values[NopRoutingDefaults.RouteValue.ManufacturerId] = urlRecord.EntityId;
                    values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(Vendor), StringComparison.InvariantCultureIgnoreCase):
                    values[NopRoutingDefaults.RouteValue.Controller] = "Catalog";
                    values[NopRoutingDefaults.RouteValue.Action] = "Vendor";
                    values[NopRoutingDefaults.RouteValue.VendorId] = urlRecord.EntityId;
                    values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(NewsItem), StringComparison.InvariantCultureIgnoreCase):
                    values[NopRoutingDefaults.RouteValue.Controller] = "News";
                    values[NopRoutingDefaults.RouteValue.Action] = "NewsItem";
                    values[NopRoutingDefaults.RouteValue.NewsItemId] = urlRecord.EntityId;
                    values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(BlogPost), StringComparison.InvariantCultureIgnoreCase):
                    values[NopRoutingDefaults.RouteValue.Controller] = "Blog";
                    values[NopRoutingDefaults.RouteValue.Action] = "BlogPost";
                    values[NopRoutingDefaults.RouteValue.BlogPostId] = urlRecord.EntityId;
                    values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(Topic), StringComparison.InvariantCultureIgnoreCase):
                    values[NopRoutingDefaults.RouteValue.Controller] = "Topic";
                    values[NopRoutingDefaults.RouteValue.Action] = "TopicDetails";
                    values[NopRoutingDefaults.RouteValue.TopicId] = urlRecord.EntityId;
                    values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;
                    break;
            }
        }

        /// <summary>
        /// Transform route values according to the passed URL record and URL catalog path
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="urlRecord">Record found by the URL slug</param>
        /// <param name="catalogPath">URL catalog path</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task CatalogRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord, string catalogPath)
        {
            if (await ProductCatalogRoutingAsync(httpContext, values, urlRecord, catalogPath))
                return;
        }

        /// <summary>
        /// Transform route values according to the passed URL record and URL catalog path
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="urlRecord">Record found by the URL slug</param>
        /// <param name="catalogPath">URL catalog path</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the set of values
        /// </returns>
        protected virtual async Task<bool> ProductCatalogRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord, string catalogPath)
        {
            //no product URL record found
            if (!urlRecord.EntityName.Equals("product", StringComparison.InvariantCultureIgnoreCase))
                return false;

            values[NopRoutingDefaults.RouteValue.Controller] = "Product";
            values[NopRoutingDefaults.RouteValue.Action] = "ProductDetails";
            values[NopRoutingDefaults.RouteValue.ProductId] = urlRecord.EntityId;
            values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;

            return true;
        }

        /// <summary>
        /// Transform route values to redirect the request
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="path">Path</param>
        /// <param name="permanent">Whether the redirect should be permanent</param>
        protected virtual void InternalRedirect(HttpContext httpContext, RouteValueDictionary values, string path, bool permanent)
        {
            values[NopRoutingDefaults.RouteValue.Controller] = "Common";
            values[NopRoutingDefaults.RouteValue.Action] = "InternalRedirect";
            values[NopRoutingDefaults.RouteValue.Url] = $"{httpContext.Request.PathBase}{path}{httpContext.Request.QueryString}";
            values[NopRoutingDefaults.RouteValue.PermanentRedirect] = permanent;
            httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a set of transformed route values that will be used to select an action
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="routeValues">The route values associated with the current match</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the set of values
        /// </returns>
        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary routeValues)
        {
            var values = new RouteValueDictionary(routeValues);
            if (values is null)
                return values;

            if (!values.TryGetValue(NopRoutingDefaults.RouteValue.SeName, out var slug) || await _urlRecordService.GetBySlugAsync(slug.ToString()) is not UrlRecord urlRecord)
                return values;

            //give the ability to transform values to third-party handlers
            var routingEvent = new GenericRoutingEvent(httpContext, values, urlRecord);
            await _eventPublisher.PublishAsync(routingEvent);
            if (routingEvent.Handled)
                return values;

            if (values.TryGetValue(NopRoutingDefaults.RouteValue.CatalogSeName, out var catalogPathValue) && catalogPathValue is string catalogPath)
            {
                await CatalogRoutingAsync(httpContext, values, urlRecord, catalogPath);
                return values;
            }

            await SingleSlugRoutingAsync(httpContext, values, urlRecord);

            return values;
        }

        #endregion
    }
}