namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Represents default values related to Export/Import features
    /// </summary>
    public static partial class ExportImportDefaults
    {
        /// <summary>
        /// Gets the name of the default hash algorithm
        /// </summary>
        /// <returns>
        /// SHA512 - it's quite fast hash (to cheaply distinguish between objects)
        /// </returns>
        public static string ImageHashAlgorithm => "SHA512";

        /// <summary>
        /// Gets the path to temporary files
        /// </summary>
        public static string UploadsTempPath => "~/App_Data/TempUploads";
    }
}
