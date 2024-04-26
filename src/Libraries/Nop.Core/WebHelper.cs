using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Nop.Core.Http;

namespace Nop.Core;

/// <summary>
/// Represents a web helper
/// </summary>
public partial class WebHelper : IWebHelper
{
    #region Fields  

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IHostApplicationLifetime _hostApplicationLifetime;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly Lazy<IStoreContext> _storeContext;

    #endregion

    #region Ctor

    public WebHelper(IActionContextAccessor actionContextAccessor,
        IHostApplicationLifetime hostApplicationLifetime,
        IHttpContextAccessor httpContextAccessor,
        IUrlHelperFactory urlHelperFactory,
        Lazy<IStoreContext> storeContext)
    {
        _actionContextAccessor = actionContextAccessor;
        _hostApplicationLifetime = hostApplicationLifetime;
        _httpContextAccessor = httpContextAccessor;
        _urlHelperFactory = urlHelperFactory;
        _storeContext = storeContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Check whether current HTTP request is available
    /// </summary>
    /// <returns>True if available; otherwise false</returns>
    protected virtual bool IsRequestAvailable()
    {
        if (_httpContextAccessor?.HttpContext == null)
            return false;

        try
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return false;
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Is IP address specified
    /// </summary>
    /// <param name="address">IP address</param>
    /// <returns>Result</returns>
    protected virtual bool IsIpAddressSet(IPAddress address)
    {
        var rez = address != null && address.ToString() != IPAddress.IPv6Loopback.ToString();

        return rez;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get URL referrer if exists
    /// </summary>
    /// <returns>URL referrer</returns>
    public virtual string GetUrlReferrer()
    {
        if (!IsRequestAvailable())
            return string.Empty;

        //URL referrer is null in some case (for example, in IE 8)
        return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
    }

    /// <summary>
    /// Get IP address from HTTP context
    /// </summary>
    /// <returns>String of IP address</returns>
    public virtual string GetCurrentIpAddress()
    {
        if (!IsRequestAvailable() || _httpContextAccessor.HttpContext!.Connection.RemoteIpAddress is not { } remoteIp)
            return string.Empty;

        return (remoteIp.Equals(IPAddress.IPv6Loopback) ? IPAddress.Loopback : remoteIp).ToString();
    }

    /// <summary>
    /// Gets this page URL
    /// </summary>
    /// <param name="includeQueryString">Value indicating whether to include query strings</param>
    /// <param name="useSsl">Value indicating whether to get SSL secured page URL. Pass null to determine automatically</param>
    /// <param name="lowercaseUrl">Value indicating whether to lowercase URL</param>
    /// <returns>Page URL</returns>
    public virtual string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false)
    {
        if (!IsRequestAvailable())
            return string.Empty;

        //get store location
        var storeLocation = GetStoreLocation(useSsl ?? IsCurrentConnectionSecured());

        //add local path to the URL
        var pageUrl = $"{storeLocation.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.Path}";

        //add query string to the URL
        if (includeQueryString)
            pageUrl = $"{pageUrl}{_httpContextAccessor.HttpContext.Request.QueryString}";

        //whether to convert the URL to lower case
        if (lowercaseUrl)
            pageUrl = pageUrl.ToLowerInvariant();

        return pageUrl;
    }

    /// <summary>
    /// Gets a value indicating whether current connection is secured
    /// </summary>
    /// <returns>True if it's secured, otherwise false</returns>
    public virtual bool IsCurrentConnectionSecured()
    {
        if (!IsRequestAvailable())
            return false;

        return _httpContextAccessor.HttpContext.Request.IsHttps;
    }

    /// <summary>
    /// Gets store host location
    /// </summary>
    /// <param name="useSsl">Whether to get SSL secured URL</param>
    /// <returns>Store host location</returns>
    public virtual string GetStoreHost(bool useSsl)
    {
        if (!IsRequestAvailable())
            return string.Empty;

        //try to get host from the request HOST header
        var hostHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
        if (StringValues.IsNullOrEmpty(hostHeader))
            return string.Empty;

        //add scheme to the URL
        var storeHost = $"{(useSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp)}{Uri.SchemeDelimiter}{hostHeader.FirstOrDefault()}";

        //ensure that host is ended with slash
        storeHost = $"{storeHost.TrimEnd('/')}/";

        return storeHost;
    }

    /// <summary>
    /// Gets store location
    /// </summary>
    /// <param name="useSsl">Whether to get SSL secured URL; pass null to determine automatically</param>
    /// <returns>Store location</returns>
    public virtual string GetStoreLocation(bool? useSsl = null)
    {
        var storeLocation = string.Empty;

        //get store host
        var storeHost = GetStoreHost(useSsl ?? IsCurrentConnectionSecured());
        if (!string.IsNullOrEmpty(storeHost))
        {
            //add application path base if exists
            storeLocation = IsRequestAvailable() ? $"{storeHost.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.PathBase}" : storeHost;
        }

        //if host is empty (it is possible only when HttpContext is not available), use URL of a store entity configured in admin area
        if (string.IsNullOrEmpty(storeHost))
            storeLocation = _storeContext.Value.GetCurrentStore()?.Url
                            ?? throw new Exception("Current store cannot be loaded");

        //ensure that URL is ended with slash
        storeLocation = $"{storeLocation.TrimEnd('/')}/";

        return storeLocation;
    }

    /// <summary>
    /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
    /// </summary>
    /// <returns>True if the request targets a static resource file.</returns>
    public virtual bool IsStaticResource()
    {
        if (!IsRequestAvailable())
            return false;

        string path = _httpContextAccessor.HttpContext.Request.Path;

        //a little workaround. FileExtensionContentTypeProvider contains most of static file extensions. So we can use it
        //source: https://github.com/aspnet/StaticFiles/blob/dev/src/Microsoft.AspNetCore.StaticFiles/FileExtensionContentTypeProvider.cs
        //if it can return content type, then it's a static file
        var contentTypeProvider = new FileExtensionContentTypeProvider();
        return contentTypeProvider.TryGetContentType(path, out var _);
    }

    /// <summary>
    /// Modify query string of the URL
    /// </summary>
    /// <param name="url">Url to modify</param>
    /// <param name="key">Query parameter key to add</param>
    /// <param name="values">Query parameter values to add</param>
    /// <returns>New URL with passed query parameter</returns>
    public virtual string ModifyQueryString(string url, string key, params string[] values)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        if (string.IsNullOrEmpty(key))
            return url;

        //prepare URI object
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var isLocalUrl = urlHelper.IsLocalUrl(url);

        var uriStr = url;
        if (isLocalUrl)
        {
            var pathBase = _httpContextAccessor.HttpContext.Request.PathBase;
            uriStr = $"{GetStoreLocation().TrimEnd('/')}{(url.StartsWith(pathBase) ? url.Replace(pathBase, "") : url)}";
        }

        var uri = new Uri(uriStr, UriKind.Absolute);

        //get current query parameters
        var queryParameters = QueryHelpers.ParseQuery(uri.Query);

        //and add passed one
        queryParameters[key] = string.Join(",", values);

        //add only first value
        //two the same query parameters? theoretically it's not possible.
        //but MVC has some ugly implementation for checkboxes and we can have two values
        //find more info here: http://www.mindstorminteractive.com/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
        //we do this validation just to ensure that the first one is not overridden
        var queryBuilder = new QueryBuilder(queryParameters
            .ToDictionary(parameter => parameter.Key, parameter => parameter.Value.FirstOrDefault()?.ToString() ?? string.Empty));

        //create new URL with passed query parameters
        url = $"{(isLocalUrl ? uri.LocalPath : uri.GetLeftPart(UriPartial.Path))}{queryBuilder.ToQueryString()}{uri.Fragment}";

        return url;
    }

    /// <summary>
    /// Remove query parameter from the URL
    /// </summary>
    /// <param name="url">Url to modify</param>
    /// <param name="key">Query parameter key to remove</param>
    /// <param name="value">Query parameter value to remove; pass null to remove all query parameters with the specified key</param>
    /// <returns>New URL without passed query parameter</returns>
    public virtual string RemoveQueryString(string url, string key, string value = null)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        if (string.IsNullOrEmpty(key))
            return url;

        //prepare URI object
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var isLocalUrl = urlHelper.IsLocalUrl(url);
        var uri = new Uri(isLocalUrl ? $"{GetStoreLocation().TrimEnd('/')}{url}" : url, UriKind.Absolute);

        //get current query parameters
        var queryParameters = QueryHelpers.ParseQuery(uri.Query)
            .SelectMany(parameter => parameter.Value, (parameter, queryValue) => new KeyValuePair<string, string>(parameter.Key, queryValue))
            .ToList();

        if (!string.IsNullOrEmpty(value))
        {
            //remove a specific query parameter value if it's passed
            queryParameters.RemoveAll(parameter => parameter.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                                                   && parameter.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase));
        }
        else
        {
            //or remove query parameter by the key
            queryParameters.RemoveAll(parameter => parameter.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        }

        var queryBuilder = new QueryBuilder(queryParameters);

        //create new URL without passed query parameters
        url = $"{(isLocalUrl ? uri.LocalPath : uri.GetLeftPart(UriPartial.Path))}{queryBuilder.ToQueryString()}{uri.Fragment}";

        return url;
    }

    /// <summary>
    /// Gets query string value by name
    /// </summary>
    /// <typeparam name="T">Returned value type</typeparam>
    /// <param name="name">Query parameter name</param>
    /// <returns>Query string value</returns>
    public virtual T QueryString<T>(string name)
    {
        if (!IsRequestAvailable())
            return default;

        if (StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Query[name]))
            return default;

        return CommonHelper.To<T>(_httpContextAccessor.HttpContext.Request.Query[name].ToString());
    }

    /// <summary>
    /// Restart application domain
    /// </summary>
    public virtual void RestartAppDomain()
    {
        _hostApplicationLifetime.StopApplication();
    }

    /// <summary>
    /// Gets a value that indicates whether the client is being redirected to a new location
    /// </summary>
    public virtual bool IsRequestBeingRedirected
    {
        get
        {
            var response = _httpContextAccessor.HttpContext.Response;
            //ASP.NET 4 style - return response.IsRequestBeingRedirected;
            int[] redirectionStatusCodes = [StatusCodes.Status301MovedPermanently, StatusCodes.Status302Found];

            return redirectionStatusCodes.Contains(response.StatusCode);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
    /// </summary>
    public virtual bool IsPostBeingDone
    {
        get
        {
            if (_httpContextAccessor.HttpContext.Items[NopHttpDefaults.IsPostBeingDoneRequestItem] == null)
                return false;

            return Convert.ToBoolean(_httpContextAccessor.HttpContext.Items[NopHttpDefaults.IsPostBeingDoneRequestItem]);
        }

        set => _httpContextAccessor.HttpContext.Items[NopHttpDefaults.IsPostBeingDoneRequestItem] = value;
    }

    /// <summary>
    /// Gets current HTTP request protocol
    /// </summary>
    public virtual string GetCurrentRequestProtocol()
    {
        return IsCurrentConnectionSecured() ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
    }

    /// <summary>
    /// Gets whether the specified HTTP request URI references the local host.
    /// </summary>
    /// <param name="req">HTTP request</param>
    /// <returns>True, if HTTP request URI references to the local host</returns>
    public virtual bool IsLocalRequest(HttpRequest req)
    {
        //source: https://stackoverflow.com/a/41242493/7860424
        var connection = req.HttpContext.Connection;
        if (IsIpAddressSet(connection.RemoteIpAddress))
        {
            //We have a remote address set up
            return IsIpAddressSet(connection.LocalIpAddress)
                //Is local is same as remote, then we are local
                ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                //else we are remote if the remote IP address is not a loopback address
                : IPAddress.IsLoopback(connection.RemoteIpAddress);
        }

        return true;
    }

    /// <summary>
    /// Get the raw path and full query of request
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>Raw URL</returns>
    public virtual string GetRawUrl(HttpRequest request)
    {
        //first try to get the raw target from request feature
        //note: value has not been UrlDecoded
        var rawUrl = request.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;

        //or compose raw URL manually
        if (string.IsNullOrEmpty(rawUrl))
            rawUrl = $"{request.PathBase}{request.Path}{request.QueryString}";

        return rawUrl;
    }

    /// <summary>
    /// Gets whether the request is made with AJAX 
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>Result</returns>
    public virtual bool IsAjaxRequest(HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Headers == null)
            return false;

        return request.Headers.XRequestedWith == "XMLHttpRequest";
    }

    #endregion
}