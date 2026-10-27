namespace Nop.Services.Themes;

/// <summary>
/// Represents default values related to themes
/// </summary>
public static partial class NopThemeDefaults
{
    /// <summary>
    /// Gets the path to themes folder
    /// </summary>
    public static string ThemesPath => "~/Themes";

    /// <summary>
    /// Gets the name of the theme description file
    /// </summary>
    public static string ThemeDescriptionFileName => "theme.json";

    /// <summary>
    /// Gets the theme cache key used to store the theme name in the HTTP context items collection
    /// </summary>
    public static string HttpContextThemeCacheKey => "http-context-theme-cache-key";

    /// <summary>
    /// Gets the key for store the name of the theme in the <see cref="Microsoft.AspNetCore.Mvc.Razor.ViewLocationExpanderContext"/>
    /// </summary>
    public static string ThemeKey => "nop.themename";
}