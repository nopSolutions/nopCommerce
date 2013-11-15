using Nop.Core.Configuration;

namespace Nop.Core.Domain.Vendors
{
    /// <summary>
    /// Vendor settings
    /// </summary>
    public class VendorSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }
        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        public string PageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets the value indicating how many vendors to display in vendors block
        /// </summary>
        public int VendorsBlockItemsToDisplay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display vendor name on the product details page
        /// </summary>
        public bool ShowVendorOnProductDetailsPage { get; set; }
    }
}
