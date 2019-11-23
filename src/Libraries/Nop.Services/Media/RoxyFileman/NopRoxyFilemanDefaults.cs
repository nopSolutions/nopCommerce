namespace Nop.Services.Media.RoxyFileman
{
    /// <summary>
    /// Represents default values related to roxyFileman
    /// </summary>
    public static partial class NopRoxyFilemanDefaults
    {
        /// <summary>
        /// Default path to root directory of uploaded files (if appropriate settings are not specified)
        /// </summary>
        public static string DefaultRootDirectory = "/images/uploaded";

        /// <summary>
        /// Path to configuration file
        /// </summary>
        public static string ConfigurationFile = "/lib/Roxy_Fileman/conf.json";

        /// <summary>
        /// Path to directory of language files
        /// </summary>
        public static string LanguageDirectory = "/lib/Roxy_Fileman/lang";
    }
}
