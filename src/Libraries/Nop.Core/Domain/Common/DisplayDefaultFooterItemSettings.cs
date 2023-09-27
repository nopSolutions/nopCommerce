using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    /// <summary>
    /// Display default menu item settings
    /// </summary>
    public partial class DisplayDefaultFooterItemSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display "sitemap" footer item
        /// </summary>
        public bool DisplaySitemapFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "contact us" footer item
        /// </summary>
        public bool DisplayContactUsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "product search" footer item
        /// </summary>
        public bool DisplayProductSearchFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "news" footer item
        /// </summary>
        public bool DisplayNewsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "blog" footer item
        /// </summary>
        public bool DisplayBlogFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "forums" footer item
        /// </summary>
        public bool DisplayForumsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "recently viewed products" footer item
        /// </summary>
        public bool DisplayRecentlyViewedProductsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "compare products" footer item
        /// </summary>
        public bool DisplayCompareProductsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "new products" footer item
        /// </summary>
        public bool DisplayNewProductsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "customer info" footer item
        /// </summary>
        public bool DisplayCustomerInfoFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "customer orders" footer item
        /// </summary>
        public bool DisplayCustomerOrdersFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "customer addresses" footer item
        /// </summary>
        public bool DisplayCustomerAddressesFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "shopping cart" footer item
        /// </summary>
        public bool DisplayShoppingCartFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "wishlist" footer item
        /// </summary>
        public bool DisplayWishlistFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "apply vendor account" footer item
        /// </summary>
        public bool DisplayApplyVendorAccountFooterItem { get; set; }
    }
}
