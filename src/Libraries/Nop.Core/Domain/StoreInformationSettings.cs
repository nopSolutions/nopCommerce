using Nop.Core.Configuration;

namespace Nop.Core.Domain
{
    public class StoreInformationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a store name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets a store URL
        /// </summary>
        public string StoreUrl { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether store is closed
        /// </summary>
        public bool StoreClosed { get; set; }

        /// <summary>
        /// Gets or sets a default store theme
        /// </summary>
        public string DefaultStoreTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select a theme
        /// </summary>
        public bool AllowCustomerToSelectTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed in public store (used for debugging)
        /// </summary>
        public bool DisplayMiniProfilerInPublicStore { get; set; }
    }
}
