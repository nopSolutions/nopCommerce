using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
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
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public LanguageParameterTransformer(IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _storeContext = storeContext;
            _workContext = workContext;
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
            //try to get a language code from the route values or use the passed one
            _httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue(NopPathRouteDefaults.LanguageRouteValue, out var routeValue);
            var code = (routeValue ?? value)?.ToString();
            if (string.IsNullOrEmpty(code))
                return string.Empty;

            //check whether the language is available
            var store = _storeContext.GetCurrentStoreAsync().Result;
            var languages = _languageService.GetAllLanguagesAsync(storeId: store.Id).Result.Where(lang => lang.Published).ToList();
            var language = languages.FirstOrDefault(lang => lang.UniqueSeoCode.Equals(code, StringComparison.InvariantCultureIgnoreCase))
                ?? _workContext.GetWorkingLanguageAsync().Result;

            return language?.UniqueSeoCode.ToLowerInvariant();
        }

        #endregion
    }
}