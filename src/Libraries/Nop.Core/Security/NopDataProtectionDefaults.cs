namespace Nop.Core.Security
{
    /// <summary>
    /// Represents default values related to data protection
    /// </summary>
    public static partial class NopDataProtectionDefaults
    {
        /// <summary>
        /// Gets the name of the key file used to store the protection key list to Azure (used with the UseAzureBlobStorageToStoreDataProtectionKeys option enabled)
        /// </summary>
        public static string AzureDataProtectionKeyFile => "DataProtectionKeys.xml";

        /// <summary>
        /// Gets the name of the key path used to store the protection key list to local file system (used when UseAzureBlobStorageToStoreDataProtectionKeys option not enabled)
        /// </summary>
        public static string DataProtectionKeysPath => "~/App_Data/DataProtectionKeys";
    }
}
