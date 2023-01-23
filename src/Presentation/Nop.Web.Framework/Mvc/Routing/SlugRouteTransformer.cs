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
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Events;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents slug route transformer
    /// </summary>
    public partial class SlugRouteTransformer : DynamicRouteValueTransformer
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryService _categoryService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public SlugRouteTransformer(CatalogSettings catalogSettings,
            ICategoryService categoryService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            IManufacturerService manufacturerService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            LocalizationSettings localizationSettings)
        {
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _manufacturerService = manufacturerService;
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
        /// <param name="catalogPath">URL catalog path</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SingleSlugRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord, string catalogPath)
        {
            //if URL record is not active let's find the latest one
            var slug = urlRecord.IsActive
                ? urlRecord.Slug
                : await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
            if (string.IsNullOrEmpty(slug))
                return;

            if (!urlRecord.IsActive || !string.IsNullOrEmpty(catalogPath))
            {
                //permanent redirect to new URL with active single slug
                InternalRedirect(httpContext, values, $"/{slug}", true);
                return;
            }

            //Ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when customers choose a new language but a slug stays the same
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && values.TryGetValue(NopRoutingDefaults.RouteValue.Language, out var langValue))
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
                var language = languages
                    .FirstOrDefault(lang => lang.Published && lang.UniqueSeoCode.Equals(langValue?.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    ?? languages.FirstOrDefault();

                var slugLocalized = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id);
                if (!string.IsNullOrEmpty(slugLocalized) && !slugLocalized.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                {
                    //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)

                    //redirect to the page for current language
                    InternalRedirect(httpContext, values, $"/{language.UniqueSeoCode}/{slugLocalized}", false);
                    return;
                }
            }

            //since we are here, all is ok with the slug, so process URL
            switch (urlRecord.EntityName)
            {
                case var name when name.Equals(nameof(Product), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Product", "ProductDetails", slug, (NopRoutingDefaults.RouteValue.ProductId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(ProductTag), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Catalog", "ProductsByTag", slug, (NopRoutingDefaults.RouteValue.ProductTagId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(Category), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Catalog", "Category", slug, (NopRoutingDefaults.RouteValue.CategoryId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(Manufacturer), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Catalog", "Manufacturer", slug, (NopRoutingDefaults.RouteValue.ManufacturerId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(Vendor), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Catalog", "Vendor", slug, (NopRoutingDefaults.RouteValue.VendorId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(NewsItem), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "News", "NewsItem", slug, (NopRoutingDefaults.RouteValue.NewsItemId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(BlogPost), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Blog", "BlogPost", slug, (NopRoutingDefaults.RouteValue.BlogPostId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(Topic), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Topic", "TopicDetails", slug, (NopRoutingDefaults.RouteValue.TopicId, urlRecord.EntityId));
                    return;
            }
        }

        /// <summary>
        /// Try transforming the route values, assuming the passed URL record is of a product type
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="urlRecord">Record found by the URL slug</param>
        /// <param name="catalogPath">URL catalog path</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value whether the route values were processed
        /// </returns>
        protected virtual async Task<bool> TryProductCatalogRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord, string catalogPath)
        {
            //ensure it's a product URL record
            if (!urlRecord.EntityName.Equals(nameof(Product), StringComparison.InvariantCultureIgnoreCase))
                return false;

            //if the product URL structure type is product seName only, it will be processed later by a single slug
            if (_catalogSettings.ProductUrlStructureTypeId == (int)ProductUrlStructureType.Product)
                return false;

            //get active slug for the product
            var slug = urlRecord.IsActive
                ? urlRecord.Slug
                : await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
            if (string.IsNullOrEmpty(slug))
                return false;

            //try to get active catalog (e.g. category or manufacturer) seName for the product
            var catalogSeName = string.Empty;
            var isCategoryProductUrl = _catalogSettings.ProductUrlStructureTypeId == (int)ProductUrlStructureType.CategoryProduct;
            if (isCategoryProductUrl)
            {
                var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(urlRecord.EntityId)).FirstOrDefault();
                var category = await _categoryService.GetCategoryByIdAsync(productCategory?.CategoryId ?? 0);
                catalogSeName = category is not null ? await _urlRecordService.GetSeNameAsync(category) : string.Empty;
            }
            var isManufacturerProductUrl = _catalogSettings.ProductUrlStructureTypeId == (int)ProductUrlStructureType.ManufacturerProduct;
            if (isManufacturerProductUrl)
            {
                var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(urlRecord.EntityId)).FirstOrDefault();
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(productManufacturer?.ManufacturerId ?? 0);
                catalogSeName = manufacturer is not null ? await _urlRecordService.GetSeNameAsync(manufacturer) : string.Empty;
            }
            if (string.IsNullOrEmpty(catalogSeName))
                return false;

            //get URL record by the specified catalog path
            var catalogUrlRecord = await _urlRecordService.GetBySlugAsync(catalogPath);
            if (catalogUrlRecord is null ||
                (isCategoryProductUrl && !catalogUrlRecord.EntityName.Equals(nameof(Category), StringComparison.InvariantCultureIgnoreCase)) ||
                (isManufacturerProductUrl && !catalogUrlRecord.EntityName.Equals(nameof(Manufacturer), StringComparison.InvariantCultureIgnoreCase)) ||
                !urlRecord.IsActive)
            {
                //permanent redirect to new URL with active catalog seName and active slug
                InternalRedirect(httpContext, values, $"/{catalogSeName}/{slug}", true);
                return true;
            }

            //ensure the catalog seName and slug are the same for the current language
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && values.TryGetValue(NopRoutingDefaults.RouteValue.Language, out var langValue))
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
                var language = languages
                    .FirstOrDefault(lang => lang.Published && lang.UniqueSeoCode.Equals(langValue?.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    ?? languages.FirstOrDefault();

                var slugLocalized = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id);
                var catalogSlugLocalized = await _urlRecordService.GetActiveSlugAsync(catalogUrlRecord.EntityId, catalogUrlRecord.EntityName, language.Id);
                if ((!string.IsNullOrEmpty(slugLocalized) && !slugLocalized.Equals(slug, StringComparison.InvariantCultureIgnoreCase)) ||
                    (!string.IsNullOrEmpty(catalogSlugLocalized) && !catalogSlugLocalized.Equals(catalogUrlRecord.Slug, StringComparison.InvariantCultureIgnoreCase)))
                {
                    //redirect to localized URL for the current language
                    var activeSlug = !string.IsNullOrEmpty(slugLocalized) ? slugLocalized : slug;
                    var activeCatalogSlug = !string.IsNullOrEmpty(catalogSlugLocalized) ? catalogSlugLocalized : catalogUrlRecord.Slug;
                    InternalRedirect(httpContext, values, $"/{language.UniqueSeoCode}/{activeCatalogSlug}/{activeSlug}", false);
                    return true;
                }
            }

            //ensure the specified catalog path is equal to the active catalog seName
            //we do it here after localization check to avoid double redirect
            if (!catalogSeName.Equals(catalogUrlRecord.Slug, StringComparison.InvariantCultureIgnoreCase))
            {
                //permanent redirect to new URL with active catalog seName and active slug
                InternalRedirect(httpContext, values, $"/{catalogSeName}/{slug}", true);
                return true;
            }

            //all is ok, so select the appropriate action
            RouteToAction(values, "Product", "ProductDetails", slug,
                (NopRoutingDefaults.RouteValue.ProductId, urlRecord.EntityId), (NopRoutingDefaults.RouteValue.CatalogSeName, catalogSeName));
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
            httpContext.Items[NopHttpDefaults.GenericRouteInternalRedirect] = true;
        }

        /// <summary>
        /// Transform route values to set controller, action and action parameters
        /// </summary>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="controller">Controller name</param>
        /// <param name="action">Action name</param>
        /// <param name="slug">URL slug</param>
        /// <param name="parameters">Action parameters</param>
        protected virtual void RouteToAction(RouteValueDictionary values, string controller, string action, string slug, params (string Key, object Value)[] parameters)
        {
            values[NopRoutingDefaults.RouteValue.Controller] = controller;
            values[NopRoutingDefaults.RouteValue.Action] = action;
            values[NopRoutingDefaults.RouteValue.SeName] = slug;
            foreach (var (key, value) in parameters)
            {
                values[key] = value;
            }
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
            //get values to transform for action selection
            var values = new RouteValueDictionary(routeValues);
            if (values is null)
                return values;

            if (!values.TryGetValue(NopRoutingDefaults.RouteValue.SeName, out var slug))
                return values;

            //find record by the URL slug
            if (await _urlRecordService.GetBySlugAsync(slug.ToString()) is not UrlRecord urlRecord)
                return values;

            //allow third-party handlers to select an action by the found URL record
            var routingEvent = new GenericRoutingEvent(httpContext, values, urlRecord);
            await _eventPublisher.PublishAsync(routingEvent);
            if (routingEvent.Handled)
                return values;

            //then try to select an action by the found URL record and the catalog path
            var catalogPath = values.TryGetValue(NopRoutingDefaults.RouteValue.CatalogSeName, out var catalogPathValue)
                ? catalogPathValue.ToString()
                : string.Empty;
            if (await TryProductCatalogRoutingAsync(httpContext, values, urlRecord, catalogPath))
                return values;

            //finally, select an action by the URL record only
            await SingleSlugRoutingAsync(httpContext, values, urlRecord, catalogPath);

            return values;
        }

        #endregion
    }
}