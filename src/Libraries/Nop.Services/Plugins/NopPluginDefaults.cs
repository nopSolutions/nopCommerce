using System.Collections.Generic;

namespace Nop.Services.Plugins
{
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
        /// Gets the plugins folder name
        /// </summary>
        public static string PathName => "Plugins";

        /// <summary>
        /// Gets the path to plugins shadow copies folder
        /// </summary>
        public static string ShadowCopyPath => "~/Plugins/bin";

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
        /// Gets the name of reserve folder for plugins shadow copies
        /// </summary>
        public static string ReserveShadowCopyPathName => "reserve_bin_";

        /// <summary>
        /// Gets the name pattern of reserve folder for plugins shadow copies
        /// </summary>
        public static string ReserveShadowCopyPathNamePattern => "reserve_bin_*";

        /// <summary>
        /// Gets supported extensions of logo file
        /// </summary>
        public static List<string> SupportedLogoImageExtensions => new List<string> { "jpg", "png", "gif" };

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
        public static string AdminNavigationPluginsCacheKey => "Nop.plugins.adminnavigation-{0}";

        /// <summary>
        /// Gets a key to clear cache
        /// </summary>
        public static string AdminNavigationPluginsPrefixCacheKey => "Nop.plugins.adminnavigation";
    }
}