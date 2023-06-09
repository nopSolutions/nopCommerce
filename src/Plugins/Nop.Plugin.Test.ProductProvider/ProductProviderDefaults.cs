using Nop.Core;

namespace Nop.Plugin.Test.ProductProvider
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class ProductProviderDefaults
    {
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "Test.ProductProvider";

        /// <summary>
        /// Gets a user agent used to request Sendinblue services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets a name of the synchronization schedule task
        /// </summary>
        public static string SynchronizationTaskName => "Product Synchronization (ProductProvider plugin)";

        /// <summary>
        /// Gets a type of the synchronization schedule task
        /// </summary>
        public static string SynchronizationTask => "Nop.Plugin.Test.ProductProvider.Services.SyncDataTask";

        /// <summary>
        /// Gets a default synchronization period in seconds
        /// </summary>
        public static int DefaultSynchronizationPeriod => 5;
    }
}