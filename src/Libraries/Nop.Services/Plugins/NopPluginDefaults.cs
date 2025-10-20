﻿using Nop.Core.Caching;

namespace Nop.Services.Plugins;

/// <summary>
/// Represents default values related to plugins
/// </summary>
public static partial class NopPluginDefaults
{
    /// <summary>
    /// Gets the path to file that contained (in previous versions) installed plugin system names
    /// </summary>
    public static string ObsoleteInstalledPluginsFilePath => "~/App_Data/InstalledPlugins.txt";

    /// <summary>
    /// Gets the path to file that contains installed plugin system names
    /// </summary>
    public static string InstalledPluginsFilePath => "~/App_Data/installedPlugins.json";

    /// <summary>
    /// Gets the path to file that contains installed plugin system names
    /// </summary>
    public static string PluginsInfoFilePath => "~/App_Data/plugins.json";

    /// <summary>
    /// Gets the path to plugins folder
    /// </summary>
    public static string Path => "~/Plugins";

    /// <summary>
    /// Gets the path to plugins folder
    /// </summary>
    public static string UploadedPath => "~/Plugins/Uploaded";

    /// <summary>
    /// Gets the plugins folder name
    /// </summary>
    public static string PathName => "Plugins";

    /// <summary>
    /// Gets the path to plugins refs folder
    /// </summary>
    public static string RefsPathName => "refs";

    /// <summary>
    /// Gets the name of the plugin description file
    /// </summary>
    public static string DescriptionFileName => "plugin.json";

    /// <summary>
    /// Gets the plugins logo filename
    /// </summary>
    public static string LogoFileName => "logo";

    /// <summary>
    /// Gets supported extensions of logo file
    /// </summary>
    public static List<string> SupportedLogoImageExtensions => ["jpg", "png", "gif"];

    /// <summary>
    /// Gets the path to temp directory with uploads
    /// </summary>
    public static string UploadsTempPath => "~/App_Data/TempUploads";

    /// <summary>
    /// Gets the name of the file containing information about the uploaded items
    /// </summary>
    public static string UploadedItemsFileName => "uploadedItems.json";

    /// <summary>
    /// Gets the path to themes folder
    /// </summary>
    public static string ThemesPath => "~/Themes";

    /// <summary>
    /// Gets the name of the theme description file
    /// </summary>
    public static string ThemeDescriptionFileName => "theme.json";

    /// <summary>
    /// Gets a key for caching plugins for admin navigation
    /// </summary>
    /// <remarks>
    /// {0} : customer identifier
    /// </remarks>
    public static CacheKey AdminNavigationPluginsCacheKey => new("Nop.plugins.adminnavigation.{0}");

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string AdminNavigationPluginsPrefix => "Nop.plugins.adminnavigation.";
}