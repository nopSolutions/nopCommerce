using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents custom overridden redirect result executor
/// </summary>
public partial class NopRedirectResultExecutor : RedirectResultExecutor
{
    #region Fields

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly SecuritySettings _securitySettings;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public NopRedirectResultExecutor(IActionContextAccessor actionContextAccessor,
        ILoggerFactory loggerFactory,
        IUrlHelperFactory urlHelperFactory,
        SecuritySettings securitySettings,
        IWebHelper webHelper) : base(loggerFactory, urlHelperFactory)
    {
        _actionContextAccessor = actionContextAccessor;
        _urlHelperFactory = urlHelperFactory;
        _securitySettings = securitySettings;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute passed redirect result
    /// </summary>
    /// <param name="context">Action context</param>
    /// <param name="result">Redirect result</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override Task ExecuteAsync(ActionContext context, RedirectResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (_securitySettings.AllowNonAsciiCharactersInHeaders)
        {
            //passed redirect URL may contain non-ASCII characters, that are not allowed now (see https://github.com/aspnet/KestrelHttpServer/issues/1144)
            //so we force to encode this URL before processing
            var url = WebUtility.UrlDecode(result.Url);
            var urlHelper = result.UrlHelper ?? _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var isLocalUrl = urlHelper.IsLocalUrl(url);

            var uriStr = url;
            if (isLocalUrl)
            {
                var pathBase = context.HttpContext.Request.PathBase;
                uriStr = $"{_webHelper.GetStoreLocation().TrimEnd('/')}{(url.StartsWith(pathBase) && !string.IsNullOrEmpty(pathBase) ? url.Replace(pathBase, "") : url)}";
            }
            var uri = new Uri(uriStr, UriKind.Absolute);

            //Allowlist redirect URI schemes to http and https
            if ((uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) && urlHelper.IsLocalUrl(uri.AbsolutePath))
                result.Url = isLocalUrl ? uri.PathAndQuery : $"{uri.GetLeftPart(UriPartial.Query)}{uri.Fragment}";
            else
                result.Url = urlHelper.RouteUrl("Homepage");
        }

        return base.ExecuteAsync(context, result);
    }

    #endregion
}