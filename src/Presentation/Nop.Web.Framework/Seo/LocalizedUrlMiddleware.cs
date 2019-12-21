using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Seo
{
    public class LocalizedUrlMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizedUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var seoFriendlyUrlsForLanguagesEnabled = EngineContext.Current.Resolve<LocalizationSettings>().SeoFriendlyUrlsForLanguagesEnabled;
            if (!DataSettingsManager.DatabaseIsInstalled || !seoFriendlyUrlsForLanguagesEnabled)
                return _next(context);

            //if path isn't localized, no special action required
            var path = context.Request.Path.Value;
            if (!path.IsLocalizedUrl(context.Request.PathBase, false, out _))
                return _next(context);

            //remove language code and application path from the path
            var newPath = path.RemoveLanguageSeoCodeFromUrl(context.Request.PathBase, false);

            //set new request path and try to get route handler
            context.Request.Path = newPath;
            context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
            context.Response.Headers[HeaderNames.Location] = newPath;

            return Task.CompletedTask;
        }
    }

    public static class LocalizedUrlMiddlewareExtensions
    {
        public static IApplicationBuilder UseUrlCultureRedirection(this IApplicationBuilder app)
        {
            app.UseMiddleware<LocalizedUrlMiddleware>();
            return app;
        }
    }
}
