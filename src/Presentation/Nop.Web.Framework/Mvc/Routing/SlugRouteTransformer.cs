using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Localization;
using Nop.Services.Events;
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
        private readonly IUrlRecordService _urlRecordService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public SlugRouteTransformer(IEventPublisher eventPublisher,
            ILanguageService languageService,
            IUrlRecordService urlRecordService,
            LocalizationSettings localizationSettings)
        {
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _urlRecordService = urlRecordService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (values == null)
                return new ValueTask<RouteValueDictionary>(values);

            if (!values.TryGetValue("SeName", out var slugValue) || string.IsNullOrEmpty(slugValue as string))
                return new ValueTask<RouteValueDictionary>(values);

            var slug = slugValue as string;
            var urlRecord = _urlRecordService.GetBySlug(slug);

            //no URL record found
            if (urlRecord == null)
                return new ValueTask<RouteValueDictionary>(values);

            //virtual directory path
            var pathBase = httpContext.Request.PathBase;

            //if URL record is not active let's find the latest one
            if (!urlRecord.IsActive)
            {
                var activeSlug = _urlRecordService.GetActiveSlug(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
                if (string.IsNullOrEmpty(activeSlug))
                    return new ValueTask<RouteValueDictionary>(values);

                //redirect to active slug if found
                values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{activeSlug}{httpContext.Request.QueryString}";
                values[NopPathRouteDefaults.PermanentRedirectFieldKey] = true;
                httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;

                return new ValueTask<RouteValueDictionary>(values);
            }

            //Ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when customers choose a new language but a slug stays the same
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var urllanguage = values["language"];
                if (urllanguage != null && !string.IsNullOrEmpty(urllanguage.ToString()))
                {
                    var language = _languageService.GetAllLanguages().FirstOrDefault(x => x.UniqueSeoCode.ToLowerInvariant() == urllanguage.ToString().ToLowerInvariant());
                    if (language == null)
                        language = _languageService.GetAllLanguages().FirstOrDefault();

                    var slugForCurrentLanguage = _urlRecordService.GetActiveSlug(urlRecord.EntityId, urlRecord.EntityName, language.Id);
                    if (!string.IsNullOrEmpty(slugForCurrentLanguage) && !slugForCurrentLanguage.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)

                        //redirect to the page for current language
                        values[NopPathRouteDefaults.ControllerFieldKey] = "Common";
                        values[NopPathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                        values[NopPathRouteDefaults.UrlFieldKey] = $"{pathBase}/{language.UniqueSeoCode}/{slugForCurrentLanguage}{httpContext.Request.QueryString}";
                        values[NopPathRouteDefaults.PermanentRedirectFieldKey] = false;
                        httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;

                        return new ValueTask<RouteValueDictionary>(values);
                    }
                }
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
                    _eventPublisher.Publish(new GenericRoutingEvent(values, urlRecord));
                    break;
            }

            return new ValueTask<RouteValueDictionary>(values);
        }

        #endregion
    }
}