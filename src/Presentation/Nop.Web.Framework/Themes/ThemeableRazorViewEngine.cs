using System.Diagnostics;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Nop.Services.Themes;

namespace Nop.Web.Framework.Themes;

/// <summary>
/// Resolves explicit plugin view paths against the working theme before using the original view.
/// </summary>
public partial class ThemeableRazorViewEngine : IRazorViewEngine
{
    #region Fields

    protected readonly DiagnosticListener _diagnosticListener;
    protected readonly HtmlEncoder _htmlEncoder;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IRazorPageActivator _pageActivator;
    protected readonly IRazorViewEngine _viewEngine;

    #endregion

    #region Ctor

    public ThemeableRazorViewEngine(DiagnosticListener diagnosticListener,
        HtmlEncoder htmlEncoder,
        IHttpContextAccessor httpContextAccessor,
        IRazorPageActivator pageActivator,
        IRazorViewEngine viewEngine)
    {
        _diagnosticListener = diagnosticListener;
        _htmlEncoder = htmlEncoder;
        _httpContextAccessor = httpContextAccessor;
        _pageActivator = pageActivator;
        _viewEngine = viewEngine;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the mirrored path for a plugin view. Administration and non-plugin paths are unchanged.
    /// </summary>
    /// <param name="executingFilePath">The executing file path</param>
    /// <param name="viewPath">The view path</param>
    /// <returns>A tuple containing the theme path and plugin path</returns>
    protected virtual (string ThemePath, string PluginPath) GetViewPaths(string executingFilePath, string viewPath)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null ||
            string.Equals(httpContext.Request.RouteValues["area"]?.ToString(), AreaNames.ADMIN, StringComparison.OrdinalIgnoreCase) ||
            !httpContext.Items.TryGetValue(NopThemeDefaults.HttpContextThemeCacheKey, out var cachedThemeName) ||
            cachedThemeName is not string themeName || string.IsNullOrWhiteSpace(themeName))
            return default;

        //the theme name is one directory, never a path supplied to the file system.
        if (themeName is "." or ".." || themeName.IndexOfAny(['/', '\\']) >= 0)
            return default;

        var absolutePath = _viewEngine.GetAbsolutePath(executingFilePath, viewPath)?.Replace('\\', '/');
        if (absolutePath?.StartsWith("~/", StringComparison.Ordinal) == true)
            absolutePath = absolutePath[1..];

        //relative siblings of an override must still fall back to the original plugin directory.
        var themePrefix = $"/Themes/{themeName}";
        if (!viewPath.StartsWith('/') && !viewPath.StartsWith("~/", StringComparison.Ordinal) &&
            absolutePath?.StartsWith($"{themePrefix}/Plugins/", StringComparison.OrdinalIgnoreCase) == true)
            absolutePath = absolutePath[themePrefix.Length..];

        if (absolutePath?.StartsWith("/Plugins/", StringComparison.OrdinalIgnoreCase) != true ||
            !absolutePath.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
            return default;

        var segments = absolutePath.Split('/');
        if (segments.Length < 4 || segments.Any(segment => segment is "." or "..") ||
            segments.Any(segment => string.Equals(segment, AreaNames.ADMIN, StringComparison.OrdinalIgnoreCase)))
            return default;

        return ($"{themePrefix}{absolutePath}", absolutePath);
    }

    /// <summary>
    /// Uses this engine for subsequent layout lookups too, including layouts of fallback plugin views.
    /// </summary>
    /// <param name="result">The view engine result</param>
    /// <returns>The prepared view engine result</returns>
    protected virtual ViewEngineResult PrepareView(ViewEngineResult result)
    {
        if (!result.Success || result.View is not RazorView razorView)
            return result;

        return ViewEngineResult.Found(result.ViewName,
            new RazorView(this, _pageActivator, razorView.ViewStartPages, razorView.RazorPage,
                _htmlEncoder, _diagnosticListener));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the view with the given <paramref name="viewPath" />, relative to <paramref name="executingFilePath" />
    /// unless <paramref name="viewPath" /> is already absolute.
    /// </summary>
    /// <param name="executingFilePath">The absolute path to the currently-executing view, if any.</param>
    /// <param name="viewPath">The path to the view.</param>
    /// <param name="isMainPage">Determines if the page being found is the main page for an action.</param>
    /// <returns>The <see cref="T:Microsoft.AspNetCore.Mvc.ViewEngines.ViewEngineResult" /> of locating the view.</returns>
    public virtual ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
    {
        ArgumentException.ThrowIfNullOrEmpty(viewPath);

        var (themePath, pluginPath) = GetViewPaths(executingFilePath, viewPath);
        
        if (themePath != null)
        {
            //The real theme path is Razor's cache key, including for unsuccessful lookups.
            var result = _viewEngine.GetView(null, themePath, isMainPage);
            if (result.Success)
                return PrepareView(result);
        }

        return PrepareView(pluginPath != null
            ? _viewEngine.GetView(null, pluginPath, isMainPage)
            : _viewEngine.GetView(executingFilePath, viewPath, isMainPage));
    }

    /// <summary>
    /// Gets the page with the given <paramref name="pagePath" />, relative to <paramref name="executingFilePath" />
    /// unless <paramref name="pagePath" /> is already absolute.
    /// </summary>
    /// <param name="executingFilePath">The absolute path to the currently-executing page, if any.</param>
    /// <param name="pagePath">The path to the page.</param>
    /// <returns>The <see cref="T:Microsoft.AspNetCore.Mvc.Razor.RazorPageResult" /> of locating the page.</returns>
    /// <remarks><seealso cref="M:Microsoft.AspNetCore.Mvc.ViewEngines.IViewEngine.GetView(System.String,System.String,System.Boolean)" />.</remarks>
    public virtual RazorPageResult GetPage(string executingFilePath, string pagePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(pagePath);

        var (themePath, pluginPath) = GetViewPaths(executingFilePath, pagePath);
        
        if (themePath != null)
        {
            var result = _viewEngine.GetPage(null, themePath);
           
            if (result.Page != null)
                return result;
        }

        return pluginPath != null
            ? _viewEngine.GetPage(null, pluginPath)
            : _viewEngine.GetPage(executingFilePath, pagePath);
    }


    /// <summary>
    /// Finds the view with the given <paramref name="viewName" /> using view locations and information from the
    /// <paramref name="context" />.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.ActionContext" />.</param>
    /// <param name="viewName">The name or path of the view that is rendered to the response.</param>
    /// <param name="isMainPage">Determines if the page being found is the main page for an action.</param>
    /// <returns>The <see cref="T:Microsoft.AspNetCore.Mvc.ViewEngines.ViewEngineResult" /> of locating the view.</returns>
    /// <remarks>Use <see cref="M:Microsoft.AspNetCore.Mvc.ViewEngines.IViewEngine.GetView(System.String,System.String,System.Boolean)" /> when the absolute or relative
    /// path of the view is known.</remarks>
    public virtual ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
    {
        return PrepareView(_viewEngine.FindView(context, viewName, isMainPage));
    }

    /// <summary>
    /// Finds the page with the given <paramref name="pageName" /> using view locations and information from the
    /// <paramref name="context" />.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.ActionContext" />.</param>
    /// <param name="pageName">The name or path of the page.</param>
    /// <returns>The <see cref="T:Microsoft.AspNetCore.Mvc.Razor.RazorPageResult" /> of locating the page.</returns>
    /// <remarks>
    /// Use <see cref="M:Microsoft.AspNetCore.Mvc.Razor.IRazorViewEngine.GetPage(System.String,System.String)" /> when the absolute or relative path of the page is known.
    /// <seealso cref="M:Microsoft.AspNetCore.Mvc.ViewEngines.IViewEngine.FindView(Microsoft.AspNetCore.Mvc.ActionContext,System.String,System.Boolean)" />.
    /// </remarks>
    public virtual RazorPageResult FindPage(ActionContext context, string pageName)
    {
        return _viewEngine.FindPage(context, pageName);
    }

    /// <summary>
    /// Converts the given <paramref name="pagePath" /> to be absolute, relative to
    /// <paramref name="executingFilePath" /> unless <paramref name="pagePath" /> is already absolute.
    /// </summary>
    /// <param name="executingFilePath">The absolute path to the currently-executing page, if any.</param>
    /// <param name="pagePath">The path to the page.</param>
    /// <returns>
    /// The combination of <paramref name="executingFilePath" /> and <paramref name="pagePath" /> if
    /// <paramref name="pagePath" /> is a relative path. The <paramref name="pagePath" /> value (unchanged)
    /// otherwise.
    /// </returns>
    public virtual string GetAbsolutePath(string executingFilePath, string pagePath)
    {
        return _viewEngine.GetAbsolutePath(executingFilePath, pagePath);
    }

    #endregion
}