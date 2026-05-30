using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Security;
using Nop.Core.Http;
using Nop.Services.Helpers;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents custom overridden redirect result executor
/// </summary>
public partial class NopRedirectResultExecutor : RedirectResultExecutor
{
    #region Fields

    protected readonly IWebHelper _webHelper;
    protected readonly LinkGenerator _linkGenerator;
    protected readonly SecuritySettings _securitySettings;

    #endregion

    #region Ctor

    public NopRedirectResultExecutor(ILoggerFactory loggerFactory,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper,
        LinkGenerator linkGenerator,
        SecuritySettings securitySettings) : base(loggerFactory, urlHelperFactory)
    {
        _webHelper = webHelper;
        _linkGenerator = linkGenerator;
        _securitySettings = securitySettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute passed redirect result
    /// </summary>
    /// <param name="context">Action context</param>
    /// <param name="result">Redirect result</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task ExecuteAsync(ActionContext context, RedirectResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (_securitySettings.AllowNonAsciiCharactersInHeaders)
        {
            //passed redirect URL may contain non-ASCII characters, that are not allowed now (see https://github.com/aspnet/KestrelHttpServer/issues/1144)
            //so we force to encode this URL before processing
            var url = WebUtility.UrlDecode(result.Url);
            var isLocalUrl = _webHelper.CheckIsLocalUrl(url);

            var uriStr = url;
            if (isLocalUrl)
            {
                var pathBase = context.HttpContext.Request.PathBase;
                uriStr = $"{_webHelper.GetStoreLocation().TrimEnd('/')}{(url.StartsWith(pathBase) && !string.IsNullOrEmpty(pathBase) ? url.Replace(pathBase, "") : url)}";
            }
            var uri = new Uri(uriStr, UriKind.Absolute);

            //Allowlist redirect URI schemes to http and https
            if ((uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) && _webHelper.CheckIsLocalUrl(uri.AbsolutePath))
                result.Url = isLocalUrl ? uri.PathAndQuery : $"{uri.GetLeftPart(UriPartial.Query)}{uri.Fragment}";
            else
                result.Url = _linkGenerator.GetPathByRouteValues(context.HttpContext, NopRouteNames.General.HOMEPAGE);
        }

        await base.ExecuteAsync(context, result);
    }

    #endregion
}