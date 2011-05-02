
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Catalog
{
    public class CatalogSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to hide prices for non-registered customers
        /// </summary>
        public bool HidePricesForNonRegistered { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether product sorting is enabled
        /// </summary>
        public bool AllowProductSorting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change product view mode
        /// </summary>
        public bool AllowProductViewModeChanging { get; set; }
    }
}