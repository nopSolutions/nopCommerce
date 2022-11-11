namespace Nop.Services.Installation
{
    /// <summary>
    /// Represents default values related to installation services
    /// </summary>
    public static partial class NopInstallationDefaults
    {
        /// <summary>
        /// Gets a request path to the install URL
        /// </summary>
        public static string InstallPath => "install";

        /// <summary>
        /// Gets a path to the localization resources file
        /// </summary>
        public static string LocalizationResourcesPath => "~/App_Data/Localization/";

        /// <summary>
        /// Gets a localization resources file extension
        /// </summary>
        public static string LocalizationResourcesFileExtension => "nopres.xml";

        /// <summary>
        /// Gets a path to the installation sample images
        /// </summary>
        public static string SampleImagesPath => "images\\samples\\";
    }
}