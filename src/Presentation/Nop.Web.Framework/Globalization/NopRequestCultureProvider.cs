using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Globalization
{
    /// <summary>
    /// Determines the culture information for a request
    /// </summary>
    public class NopRequestCultureProvider : RequestCultureProvider
    {
        public NopRequestCultureProvider(RequestLocalizationOptions options)
        {
            Options = options;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            //set working language culture
            var culture = (await EngineContext.Current.Resolve<IWorkContext>().GetWorkingLanguageAsync()).LanguageCulture;

            return new ProviderCultureResult(culture, culture);
        }
    }
}
