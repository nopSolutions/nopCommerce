using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Themes;

/// <summary>
/// Specifies the contracts for a view location expander that is used by Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine instances to determine search paths for a view.
/// </summary>
public partial class ThemeableViewLocationExpander : IViewLocationExpander
{
    protected const string THEME_KEY = "nop.themename";
    protected const string HTTP_CONTEXT_THEME_CACHE_KEY = "http-context-theme-cache-key";

    /// <summary>
    /// Invoked by a Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine to determine the
    /// values that would be consumed by this instance of Microsoft.AspNetCore.Mvc.Razor.IViewLocationExpander.
    /// The calculated values are used to determine if the view location has changed since the last time it was located.
    /// </summary>
    /// <param name="context">Context</param>
    public virtual void PopulateValues(ViewLocationExpanderContext context)
    {
        //no need to add the themeable view locations at all as the administration should not be themeable anyway
        if (context.AreaName?.Equals(AreaNames.ADMIN) ?? false)
            return;

        var httpContext = context.ActionContext.HttpContext;
        if (!httpContext.Items.TryGetValue(HTTP_CONTEXT_THEME_CACHE_KEY, out var cachedThemeName))
        {
            cachedThemeName = EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync().Result;
            httpContext.Items[HTTP_CONTEXT_THEME_CACHE_KEY] = cachedThemeName;
        }

        context.Values[THEME_KEY] = (string)cachedThemeName;
    }

    /// <summary>
    /// Invoked by a Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine to determine potential locations for a view.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="viewLocations">View locations</param>
    /// <returns>View locations</returns>
    public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.Values.TryGetValue(THEME_KEY, out string theme))
        {
            viewLocations = new[] {
                    $"/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                    $"/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                }
                .Concat(viewLocations);
        }

        return viewLocations;
    }
}