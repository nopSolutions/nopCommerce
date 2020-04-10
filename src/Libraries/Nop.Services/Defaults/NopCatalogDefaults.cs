namespace Nop.Services.Defaults
{
    /// <summary>
    /// Represents default values related to catalog services
    /// </summary>
    public static partial class NopCatalogDefaults
    {
        #region Products

        /// <summary>
        /// Gets a template of product name on copying
        /// </summary>
        /// <remarks>
        /// {0} : product name
        /// </remarks>
        public static string ProductCopyNameTemplate => "Copy of {0}";

        #endregion
    }
}