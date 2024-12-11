using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Globalization;
public class NopAcceptLanguageHeaderRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var acceptLanguageHeader = httpContext.Request.GetTypedHeaders().AcceptLanguage;

        if (acceptLanguageHeader == null || acceptLanguageHeader.Count == 0)
            return NullProviderCultureResult;

        var languageHeader = acceptLanguageHeader.FirstOrDefault();

        if (string.IsNullOrEmpty(languageHeader.Value.Value))
            return NullProviderCultureResult;

        try
        {
            var requestedCulture = new CultureInfo(languageHeader.Value.Value);

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            var language = languageService
                .GetAllLanguages()
                .FirstOrDefault(urlLanguage =>
                {
                    var storeCulture = new CultureInfo(urlLanguage.LanguageCulture);
                    return storeCulture.Name == requestedCulture.Name || storeCulture.Parent?.Name == requestedCulture.Name;
                });

            if (language == null)
                return new AcceptLanguageHeaderRequestCultureProvider().DetermineProviderCultureResult(httpContext);

            return Task.FromResult(new ProviderCultureResult(language.LanguageCulture));
        }
        catch (CultureNotFoundException)
        {
            return NullProviderCultureResult;
        }



    }
}
