using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Events;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents product route transformer
    /// </summary>
    public class ProductRouteTransformer : DynamicRouteValueTransformer
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryService _categoryService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public ProductRouteTransformer(CatalogSettings catalogSettings,
            ICategoryService categoryService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            IUrlRecordService urlRecordService,
            LocalizationSettings localizationSettings)
        {
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _urlRecordService = urlRecordService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (!_catalogSettings.EnableProductUrlWithCategoryBreadcrumb)
                return values;

            if (values == null)
                return values;

            if (!values.TryGetValue("CategorySeName", out var categorySlugValue) || string.IsNullOrEmpty(categorySlugValue as string))
                return values;

            if (!values.TryGetValue("SeName", out var slugValue) || string.IsNullOrEmpty(slugValue as string))
                return values;

            var slug = slugValue as string;
            var productUrlRecord = await _urlRecordService.GetBySlugAsync(slug);

            //no product URL record found
            if (productUrlRecord == null || !productUrlRecord.EntityName.Equals("product", StringComparison.InvariantCultureIgnoreCase))
                return values;

            var slug1 = categorySlugValue as string;
            var categoryUrlRecord = await _urlRecordService.GetBySlugAsync(slug1);

            //no category URL record found
            if (categoryUrlRecord == null || !categoryUrlRecord.EntityName.Equals("category", StringComparison.InvariantCultureIgnoreCase))
                return values;

            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productUrlRecord.EntityId);
            var productCategory = productCategories.FirstOrDefault(pc => pc.CategoryId == categoryUrlRecord.EntityId);
            if(productCategory == null)
                return values;

            //virtual directory path
            var pathBase = httpContext.Request.PathBase;

            //if product URL record is not active let's find the latest one
            if (!productUrlRecord.IsActive)
            {
                var activeSlug = await _urlRecordService.GetActiveSlugAsync(productUrlRecord.EntityId, productUrlRecord.EntityName, productUrlRecord.LanguageId);
                if (string.IsNullOrEmpty(activeSlug))
                    return values;

                //redirect to active slug if found
                values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{categoryUrlRecord.Slug}/{activeSlug}{httpContext.Request.QueryString}";
                values[NopPathRouteDefaults.PermanentRedirectFieldKey] = true;
                httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;

                return values;
            }

            //if category URL record is not active let's find the latest one
            if (!categoryUrlRecord.IsActive)
            {
                var activeSlug = await _urlRecordService.GetActiveSlugAsync(categoryUrlRecord.EntityId, categoryUrlRecord.EntityName, categoryUrlRecord.LanguageId);
                if (string.IsNullOrEmpty(activeSlug))
                    return values;

                //redirect to active slug if found
                values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{activeSlug}/{productUrlRecord.Slug}{httpContext.Request.QueryString}";
                values[NopPathRouteDefaults.PermanentRedirectFieldKey] = true;
                httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;

                return values;
            }

            //Ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when customers choose a new language but a slug stays the same
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var urllanguage = values["language"];
                if (urllanguage != null && !string.IsNullOrEmpty(urllanguage.ToString()))
                {
                    var languages = await _languageService.GetAllLanguagesAsync();
                    var language = languages.FirstOrDefault(x => x.UniqueSeoCode.ToLowerInvariant() == urllanguage.ToString().ToLowerInvariant())
                        ?? languages.FirstOrDefault();

                    var productSlugLocalized = await _urlRecordService.GetActiveSlugAsync(productUrlRecord.EntityId, productUrlRecord.EntityName, language.Id);
                    if (!string.IsNullOrEmpty(productSlugLocalized) && !productSlugLocalized.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //redirect to the page for current language
                        values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                        values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                        values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{language.UniqueSeoCode}/{categoryUrlRecord.Slug}/{productSlugLocalized}{httpContext.Request.QueryString}";
                        values[NopPathRouteDefaults.PermanentRedirectFieldKey] = false;
                        httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;

                        return values;
                    }

                    var categorySlugLocalized = await _urlRecordService.GetActiveSlugAsync(categoryUrlRecord.EntityId, categoryUrlRecord.EntityName, language.Id);
                    if (!string.IsNullOrEmpty(categorySlugLocalized) && !categorySlugLocalized.Equals(slug1, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //redirect to the page for current language
                        values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                        values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                        values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{language.UniqueSeoCode}/{categorySlugLocalized}/{productUrlRecord.Slug}{httpContext.Request.QueryString}";
                        values[NopPathRouteDefaults.PermanentRedirectFieldKey] = false;
                        httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;

                        return values;
                    }
                }
            }

            values[NopPathRouteDefaults.ControllerFieldKey] = "Product";
            values[NopPathRouteDefaults.ActionFieldKey] = "ProductDetails";
            values[NopPathRouteDefaults.ProductIdFieldKey] = productUrlRecord.EntityId;
            values[NopPathRouteDefaults.SeNameFieldKey] = productUrlRecord.Slug;

            return values;
        }

        #endregion
    }
}