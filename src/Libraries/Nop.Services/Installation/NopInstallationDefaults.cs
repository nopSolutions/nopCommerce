namespace Nop.Services.Installation
{
    /// <summary>
    /// Represents default values related to installation services
    /// </summary>
    public static partial class NopInstallationDefaults
    {
        /// <summary>
        /// Gets a path to the localization resources file
        /// </summary>
        public static string LocalizationResourcesPath => "~/App_Data/Localization/";

        /// <summary>
        /// Gets a localization resources file extension
        /// </summary>
        public static string LocalizationResourcesFileExtension => "nopres.xml";

        /// <summary>
        /// Gets a path to the installation required data file
        /// </summary>
        public static string RequiredDataPath => "~/App_Data/Install/Fast/create_required_data.sql";

        /// <summary>
        /// Gets a path to the installation sample data file
        /// </summary>
        public static string SampleDataPath => "~/App_Data/Install/Fast/create_sample_data.sql";

        /// <summary>
        /// Gets a path to the installation sample images
        /// </summary>
        public static string SampleImagesPath => "images\\samples\\";
    }
}