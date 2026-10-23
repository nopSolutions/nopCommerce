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
    protected readonly IRazorViewEngine _viewEngine;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IRazorPageActivator _pageActivator;
    protected readonly HtmlEncoder _htmlEncoder;
    protected readonly DiagnosticListener _diagnosticListener;

    public ThemeableRazorViewEngine(IRazorViewEngine viewEngine,
        IHttpContextAccessor httpContextAccessor,
        IRazorPageActivator pageActivator,
        HtmlEncoder htmlEncoder,
        DiagnosticListener diagnosticListener)
    {
        _viewEngine = viewEngine;
        _httpContextAccessor = httpContextAccessor;
        _pageActivator = pageActivator;
        _htmlEncoder = htmlEncoder;
        _diagnosticListener = diagnosticListener;
    }

    /// <summary>
    /// Gets the mirrored path for a plugin view. Administration and non-plugin paths are unchanged.
    /// </summary>
    protected virtual (string ThemePath, string PluginPath) GetViewPaths(string executingFilePath, string viewPath)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null ||
            string.Equals(httpContext.Request.RouteValues["area"]?.ToString(), AreaNames.ADMIN, StringComparison.OrdinalIgnoreCase) ||
            !httpContext.Items.TryGetValue(NopThemeDefaults.HttpContextThemeCacheKey, out var cachedThemeName) ||
            cachedThemeName is not string themeName || string.IsNullOrWhiteSpace(themeName))
            return default;

        //The theme name is one directory, never a path supplied to the file system.
        if (themeName is "." or ".." || themeName.IndexOfAny(['/', '\\']) >= 0)
            return default;

        var absolutePath = _viewEngine.GetAbsolutePath(executingFilePath, viewPath)?.Replace('\\', '/');
        if (absolutePath?.StartsWith("~/", StringComparison.Ordinal) == true)
            absolutePath = absolutePath[1..];

        //Relative siblings of an override must still fall back to the original plugin directory.
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
    protected virtual ViewEngineResult PrepareView(ViewEngineResult result)
    {
        if (!result.Success || result.View is not RazorView razorView)
            return result;

        return ViewEngineResult.Found(result.ViewName,
            new RazorView(this, _pageActivator, razorView.ViewStartPages, razorView.RazorPage,
                _htmlEncoder, _diagnosticListener));
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public virtual ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        => PrepareView(_viewEngine.FindView(context, viewName, isMainPage));

    /// <inheritdoc />
    public virtual RazorPageResult FindPage(ActionContext context, string pageName)
        => _viewEngine.FindPage(context, pageName);

    /// <inheritdoc />
    public virtual string GetAbsolutePath(string executingFilePath, string pagePath)
        => _viewEngine.GetAbsolutePath(executingFilePath, pagePath);
}