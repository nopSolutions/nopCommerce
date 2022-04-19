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
        /// Create a set of transformed route values to redirect the request to the passed URL
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="url"></param>
        /// <param name="permanent">Whether the redirect should be permanent</param>
        /// <returns>Set of values</returns>
        protected RouteValueDictionary InternalRedirectValues(HttpContext httpContext, RouteValueDictionary values, string url, bool permanent)
        {
            values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
            values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
            values[NopPathRouteDefaults.UrlFieldKey] = url;
            values[NopPathRouteDefaults.PermanentRedirectFieldKey] = permanent;
            httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;

            return values;
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

            if (!values.TryGetValue("SeName", out var slugValue) || slugValue is not string slug)
                return values;

            var urlRecord = await _urlRecordService.GetBySlugAsync(slug);
            if (urlRecord is null)
                return values;

            //if URL record is not active let's find the latest one
            if (!urlRecord.IsActive)
            {
                var activeSlug = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
                if (string.IsNullOrEmpty(activeSlug))
                    return values;

                //redirect to active slug if found
                var url = $"{httpContext.Request.PathBase}/{activeSlug}{httpContext.Request.QueryString}";
                return InternalRedirectValues(httpContext, values, url, true);
            }

            //Ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when customers choose a new language but a slug stays the same
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                if (values.TryGetValue(NopPathRouteDefaults.LanguageRouteValue, out var languageValue))
                {
                    var code = languageValue?.ToString();
                    var store = await _storeContext.GetCurrentStoreAsync();
                    var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
                    var language = languages
                        .FirstOrDefault(lang => lang.Published && lang.UniqueSeoCode.Equals(code, StringComparison.InvariantCultureIgnoreCase))
                        ?? languages.FirstOrDefault();

                    var slugForCurrentLanguage = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id);
                    if (!string.IsNullOrEmpty(slugForCurrentLanguage) && !slugForCurrentLanguage.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)

                        //redirect to the page for current language
                        var pathBase = httpContext.Request.PathBase;
                        var url = $"{pathBase}/{language.UniqueSeoCode}/{slugForCurrentLanguage}{httpContext.Request.QueryString}";
                        return InternalRedirectValues(httpContext, values, url, false);
                    }
                }
            }

            //since we are here, all is ok with the slug, so process URL
            switch (urlRecord.EntityName)
            {
                case var name when name.Equals(nameof(Product), StringComparison.InvariantCultureIgnoreCase):
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Product";
                    values[NopPathRouteDefaults.ActionFieldKey] = "ProductDetails";
                    values[NopPathRouteDefaults.ProductIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(ProductTag), StringComparison.InvariantCultureIgnoreCase):
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "ProductsByTag";
                    values[NopPathRouteDefaults.ProducttagIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(Category), StringComparison.InvariantCultureIgnoreCase):
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "Category";
                    values[NopPathRouteDefaults.CategoryIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(Manufacturer), StringComparison.InvariantCultureIgnoreCase):
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "Manufacturer";
                    values[NopPathRouteDefaults.ManufacturerIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(Vendor), StringComparison.InvariantCultureIgnoreCase):
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Catalog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "Vendor";
                    values[NopPathRouteDefaults.VendorIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(NewsItem), StringComparison.InvariantCultureIgnoreCase):
                    values[NopPathRouteDefaults.ControllerFieldKey] = "News";
                    values[NopPathRouteDefaults.ActionFieldKey] = "NewsItem";
                    values[NopPathRouteDefaults.NewsItemIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(BlogPost), StringComparison.InvariantCultureIgnoreCase):
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Blog";
                    values[NopPathRouteDefaults.ActionFieldKey] = "BlogPost";
                    values[NopPathRouteDefaults.BlogPostIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;

                case var name when name.Equals(nameof(Topic), StringComparison.InvariantCultureIgnoreCase):
                    values[NopPathRouteDefaults.ControllerFieldKey] = "Topic";
                    values[NopPathRouteDefaults.ActionFieldKey] = "TopicDetails";
                    values[NopPathRouteDefaults.TopicIdFieldKey] = urlRecord.EntityId;
                    values[NopPathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                    break;

                default:
                    //no record found, thus generate an event this way developers could insert their own types
                    await _eventPublisher.PublishAsync(new GenericRoutingEvent(values, urlRecord));
                    break;
            }

            return values;
        }

        #endregion
    }
}