using Microsoft.AspNetCore.Http;
using Nop.Data;

namespace Nop.Services.Themes;

/// <summary>
/// Represents middleware that enables themes
/// </summary>
public partial class ThemesMiddleware
{
    #region Fields
    protected readonly RequestDelegate _next;

    #endregion

    #region Ctor

    public ThemesMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke middleware actions
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="themeContext">The theme context</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InvokeAsync(HttpContext context, IThemeContext themeContext)
    {
        if (DataSettingsManager.IsDatabaseInstalled() && !context.Items.TryGetValue(NopThemeDefaults.HttpContextThemeCacheKey, out var cachedThemeName))
        {
            cachedThemeName = await themeContext.GetWorkingThemeNameAsync();
            context.Items[NopThemeDefaults.HttpContextThemeCacheKey] = cachedThemeName;
        }

        await _next(context);
    }

    #endregion
}