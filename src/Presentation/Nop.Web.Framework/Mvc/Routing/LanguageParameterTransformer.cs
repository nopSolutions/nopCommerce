using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents class language parameter transformer
    /// </summary>
    public class LanguageParameterTransformer : IOutboundParameterTransformer
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public LanguageParameterTransformer(IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService)
        {
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Transforms the specified route value to a string for inclusion in a URI
        /// </summary>
        /// <param name="value">The route value to transform</param>
        /// <returns>The transformed value</returns>
        public string TransformOutbound(object value)
        {
            //first try to get a language code from the route values
            var routeValues = _httpContextAccessor.HttpContext.Request.RouteValues;
            if (routeValues.TryGetValue(NopPathRouteDefaults.LanguageRouteValue, out var routeValue))
            {
                //ensure this language is available
                var code = routeValue?.ToString();
                var storeContext = EngineContext.Current.Resolve<IStoreContext>();
                var store = storeContext.GetCurrentStore();
                var languages = _languageService.GetAllLanguagesAsync(storeId: store.Id).Result;
                var language = languages
                    .FirstOrDefault(lang => lang.Published && lang.UniqueSeoCode.Equals(code, StringComparison.InvariantCultureIgnoreCase));
                if (language is not null)
                    return language.UniqueSeoCode.ToLowerInvariant();
            }

            //if there is no code in the route values, check whether the value is passed
            if (value is null)
                return string.Empty;

            //or use the current language code
            //we don't use the passed value, since it's always either the same as the current one or it's the default value (en)
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var currentLanguage = workContext.GetWorkingLanguageAsync().Result;
            return currentLanguage.UniqueSeoCode.ToLowerInvariant();
        }

        #endregion
    }
}