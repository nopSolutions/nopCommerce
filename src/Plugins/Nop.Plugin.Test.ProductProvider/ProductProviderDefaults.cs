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
        /// Gets a name of the synchronization schedule task
        /// </summary>
        public static string SynchronizationTaskName => "Product Synchronization (ProductProvider plugin)";

        /// <summary>
        /// Gets a type of the synchronization schedule task
        /// </summary>
        public static string SynchronizationTask => "Nop.Plugin.Test.ProductProvider.Services.SyncDataTask";

        /// <summary>
        /// Gets the type of authorization for external products
        /// </summary>
        public static string ApiKeyType => "Basic";

        /// <summary>
        /// Gets a default synchronization period in seconds
        /// </summary>
        public static int DefaultSynchronizationPeriod => 60 * 60 * 24; // 24h
    }
}