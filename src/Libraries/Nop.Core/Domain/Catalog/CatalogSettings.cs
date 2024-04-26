using Nop.Core.Configuration;

namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Catalog settings
/// </summary>
public partial class CatalogSettings : ISettings
{
    public CatalogSettings()
    {
        ProductSortingEnumDisabled = new List<int>();
        ProductSortingEnumDisplayOrder = new Dictionary<int, int>();
    }

    /// <summary>
    /// Gets or sets a value indicating details pages of unpublished product details pages could be open (for SEO optimization)
    /// </summary>
    public bool AllowViewUnpublishedProductPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating customers should see "discontinued" message when visiting details pages of unpublished products (if "AllowViewUnpublishedProductPage" is "true)
    /// </summary>
    public bool DisplayDiscontinuedMessageForUnpublishedProducts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Published" or "Disable buy/wishlist buttons" flags should be updated after order cancellation (deletion).
    /// Of course, when qty > configured minimum stock level
    /// </summary>
    public bool PublishBackProductWhenCancellingOrders { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display product SKU on the product details page
    /// </summary>
    public bool ShowSkuOnProductDetailsPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display product SKU on catalog pages
    /// </summary>
    public bool ShowSkuOnCatalogPages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display manufacturer part number of a product
    /// </summary>
    public bool ShowManufacturerPartNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display GTIN of a product
    /// </summary>
    public bool ShowGtin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Free shipping" icon should be displayed for products
    /// </summary>
    public bool ShowFreeShippingNotification { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether short description should be displayed in product box
    /// </summary>
    public bool ShowShortDescriptionOnCatalogPages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether product sorting is enabled
    /// </summary>
    public bool AllowProductSorting { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers are allowed to change product view mode
    /// </summary>
    public bool AllowProductViewModeChanging { get; set; }

    /// <summary>
    /// Gets or sets a default view mode
    /// </summary>
    public string DefaultViewMode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a category details page should include products from subcategories
    /// </summary>
    public bool ShowProductsFromSubcategories { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether number of products should be displayed beside each category
    /// </summary>
    public bool ShowCategoryProductNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we include subcategories (when 'ShowCategoryProductNumber' is 'true')
    /// </summary>
    public bool ShowCategoryProductNumberIncludingSubcategories { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether category breadcrumb is enabled
    /// </summary>
    public bool CategoryBreadcrumbEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a 'Share button' is enabled
    /// </summary>
    public bool ShowShareButton { get; set; }

    /// <summary>
    /// Gets or sets a share code (e.g. ShareThis button code)
    /// </summary>
    public string PageShareCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating product reviews must be approved
    /// </summary>
    public bool ProductReviewsMustBeApproved { get; set; }

    /// <summary>
    /// Gets or sets a value indicating that customer can add only one review per product
    /// </summary>
    public bool OneReviewPerProductFromCustomer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the default rating value of the product reviews
    /// </summary>
    public int DefaultProductRatingValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to allow anonymous users write product reviews.
    /// </summary>
    public bool AllowAnonymousUsersToReviewProduct { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether product can be reviewed only by customer who have already ordered it
    /// </summary>
    public bool ProductReviewPossibleOnlyAfterPurchasing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether notification of a store owner about new product reviews is enabled
    /// </summary>
    public bool NotifyStoreOwnerAboutNewProductReviews { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customer notification about product review reply is enabled
    /// </summary>
    public bool NotifyCustomerAboutProductReviewReply { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product reviews will be filtered per store
    /// </summary>
    public bool ShowProductReviewsPerStore { get; set; }

    /// <summary>
    /// Gets or sets a show product reviews tab on account page
    /// </summary>
    public bool ShowProductReviewsTabOnAccountPage { get; set; }

    /// <summary>
    /// Gets or sets the page size for product reviews in account page
    /// </summary>
    public int ProductReviewsPageSizeOnAccountPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product reviews should be sorted by creation date as ascending
    /// </summary>
    public bool ProductReviewsSortByCreatedDateAscending { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether product 'Email a friend' feature is enabled
    /// </summary>
    public bool EmailAFriendEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
    /// </summary>
    public bool AllowAnonymousUsersToEmailAFriend { get; set; }

    /// <summary>
    /// Gets or sets a number of "Recently viewed products"
    /// </summary>
    public int RecentlyViewedProductsNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Recently viewed products" feature is enabled
    /// </summary>
    public bool RecentlyViewedProductsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "New products" page is enabled
    /// </summary>
    public bool NewProductsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a number of products on the "New products" page
    /// </summary>
    public int NewProductsPageSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers are allowed to select page size on the "New products" page
    /// </summary>
    public bool NewProductsAllowCustomersToSelectPageSize { get; set; }

    /// <summary>
    /// Gets or sets the available customer selectable page size options on the "New products" page
    /// </summary>
    public string NewProductsPageSizeOptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Compare products" feature is enabled
    /// </summary>
    public bool CompareProductsEnabled { get; set; }

    /// <summary>
    /// Gets or sets an allowed number of products to be compared
    /// </summary>
    public int CompareProductsNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether autocomplete is enabled
    /// </summary>
    public bool ProductSearchAutoCompleteEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the search box is displayed
    /// </summary>
    public bool ProductSearchEnabled { get; set; }

    /// <summary>
    /// Gets or sets a number of products to return when using "autocomplete" feature
    /// </summary>
    public int ProductSearchAutoCompleteNumberOfProducts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show product images in the auto complete search
    /// </summary>
    public bool ShowProductImagesInSearchAutoComplete { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show link to all result in the auto complete search
    /// </summary>
    public bool ShowLinkToAllResultInSearchAutoComplete { get; set; }

    /// <summary>
    /// Gets or sets a minimum search term length
    /// </summary>
    public int ProductSearchTermMinimumLength { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show bestsellers on home page
    /// </summary>
    public bool ShowBestsellersOnHomepage { get; set; }

    /// <summary>
    /// Gets or sets a number of bestsellers on home page
    /// </summary>
    public int NumberOfBestsellersOnHomepage { get; set; }

    /// <summary>
    /// Gets or sets a number of products per page on the search products page
    /// </summary>
    public int SearchPageProductsPerPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers are allowed to select page size on the search products page
    /// </summary>
    public bool SearchPageAllowCustomersToSelectPageSize { get; set; }

    /// <summary>
    /// Gets or sets the available customer selectable page size options on the search products page
    /// </summary>
    public string SearchPagePageSizeOptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the price range filtering is enabled on search page
    /// </summary>
    public bool SearchPagePriceRangeFiltering { get; set; }

    /// <summary>
    /// Gets or sets the "from" price on search page
    /// </summary>
    public decimal SearchPagePriceFrom { get; set; }

    /// <summary>
    /// Gets or sets the "to" price on search page
    /// </summary>
    public decimal SearchPagePriceTo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the price range should be entered manually on search page
    /// </summary>
    public bool SearchPageManuallyPriceRange { get; set; }

    /// <summary>
    /// Gets or sets "List of products purchased by other customers who purchased the above" option is enable
    /// </summary>
    public bool ProductsAlsoPurchasedEnabled { get; set; }

    /// <summary>
    /// Gets or sets a number of products also purchased by other customers to display
    /// </summary>
    public int ProductsAlsoPurchasedNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we should process attribute change using AJAX. It's used for dynamical attribute change, SKU/GTIN update of combinations, conditional attributes
    /// </summary>
    public bool AjaxProcessAttributeChange { get; set; }

    /// <summary>
    /// Gets or sets a number of product tags that appear in the tag cloud
    /// </summary>
    public int NumberOfProductTags { get; set; }

    /// <summary>
    /// Gets or sets a number of products per page on 'products by tag' page
    /// </summary>
    public int ProductsByTagPageSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers can select the page size for 'products by tag'
    /// </summary>
    public bool ProductsByTagAllowCustomersToSelectPageSize { get; set; }

    /// <summary>
    /// Gets or sets the available customer selectable page size options for 'products by tag'
    /// </summary>
    public string ProductsByTagPageSizeOptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the price range filtering is enabled on 'products by tag' page
    /// </summary>
    public bool ProductsByTagPriceRangeFiltering { get; set; }

    /// <summary>
    /// Gets or sets the "from" price on 'products by tag' page
    /// </summary>
    public decimal ProductsByTagPriceFrom { get; set; }

    /// <summary>
    /// Gets or sets the "to" price on 'products by tag' page
    /// </summary>
    public decimal ProductsByTagPriceTo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the price range should be entered manually on 'products by tag' page
    /// </summary>
    public bool ProductsByTagManuallyPriceRange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include "Short description" in compare products
    /// </summary>
    public bool IncludeShortDescriptionInCompareProducts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include "Full description" in compare products
    /// </summary>
    public bool IncludeFullDescriptionInCompareProducts { get; set; }

    /// <summary>
    /// An option indicating whether products on category and manufacturer pages should include featured products as well
    /// </summary>
    public bool IncludeFeaturedProductsInNormalLists { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to render link to required products in "Require other products added to the cart" warning
    /// </summary>
    public bool UseLinksInRequiredProductWarnings { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tier prices should be displayed with applied discounts (if available)
    /// </summary>
    public bool DisplayTierPricesWithDiscounts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore discounts (side-wide). It can significantly improve performance when enabled.
    /// </summary>
    public bool IgnoreDiscounts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore featured products (side-wide). It can significantly improve performance when enabled.
    /// </summary>
    public bool IgnoreFeaturedProducts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore ACL rules (side-wide). It can significantly improve performance when enabled.
    /// </summary>
    public bool IgnoreAcl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore "limit per store" rules (side-wide). It can significantly improve performance when enabled.
    /// </summary>
    public bool IgnoreStoreLimitations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to cache product prices. It can significantly improve performance when enabled.
    /// </summary>
    public bool CacheProductPrices { get; set; }

    /// <summary>
    /// Gets or sets a value indicating maximum number of 'back in stock' subscription
    /// </summary>
    public int MaximumBackInStockSubscriptions { get; set; }

    /// <summary>
    /// Gets or sets the value indicating how many manufacturers to display in manufacturers block
    /// </summary>
    public int ManufacturersBlockItemsToDisplay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display information about shipping and tax in the footer (used in Germany)
    /// </summary>
    public bool DisplayTaxShippingInfoFooter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display information about shipping and tax on product details pages (used in Germany)
    /// </summary>
    public bool DisplayTaxShippingInfoProductDetailsPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display information about shipping and tax in product boxes (used in Germany)
    /// </summary>
    public bool DisplayTaxShippingInfoProductBoxes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display information about shipping and tax on shopping cart page (used in Germany)
    /// </summary>
    public bool DisplayTaxShippingInfoShoppingCart { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display information about shipping and tax on wishlist page (used in Germany)
    /// </summary>
    public bool DisplayTaxShippingInfoWishlist { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display information about shipping and tax on order details page (used in Germany)
    /// </summary>
    public bool DisplayTaxShippingInfoOrderDetailsPage { get; set; }

    /// <summary>
    /// Gets or sets the default value to use for Category page size options (for new categories)
    /// </summary>
    public string DefaultCategoryPageSizeOptions { get; set; }

    /// <summary>
    /// Gets or sets the default value to use for Category page size (for new categories)
    /// </summary>
    public int DefaultCategoryPageSize { get; set; }

    /// <summary>
    /// Gets or sets the default value to use for Manufacturer page size options (for new manufacturers)
    /// </summary>
    public string DefaultManufacturerPageSizeOptions { get; set; }

    /// <summary>
    /// Gets or sets the default value to use for Manufacturer page size (for new manufacturers)
    /// </summary>
    public int DefaultManufacturerPageSize { get; set; }

    /// <summary>
    /// Gets or sets a list of disabled values of ProductSortingEnum
    /// </summary>
    public List<int> ProductSortingEnumDisabled { get; set; }

    /// <summary>
    /// Gets or sets a display order of ProductSortingEnum values 
    /// </summary>
    public Dictionary<int, int> ProductSortingEnumDisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the products need to be exported/imported with their attributes
    /// </summary>
    public bool ExportImportProductAttributes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether need to use "limited to stores" property for exported/imported products
    /// </summary>
    public bool ExportImportProductUseLimitedToStores { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the products need to be exported/imported with their specification attributes
    /// </summary>
    public bool ExportImportProductSpecificationAttributes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether need create dropdown list for export
    /// </summary>
    public bool ExportImportUseDropdownlistsForAssociatedEntities { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the products should be exported/imported with a full category name including names of all its parents
    /// </summary>
    public bool ExportImportProductCategoryBreadcrumb { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the categories need to be exported/imported using name of category
    /// </summary>
    public bool ExportImportCategoriesUsingCategoryName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the images can be downloaded from remote server
    /// </summary>
    public bool ExportImportAllowDownloadImages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether products must be importing by separated files
    /// </summary>
    public bool ExportImportSplitProductsFile { get; set; }

    /// <summary>
    /// Gets or sets a value of max products count in one file 
    /// </summary>
    public int ExportImportProductsCountInOneFile { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to remove required products from the cart if the main one is removed
    /// </summary>
    public bool RemoveRequiredProducts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the related entities need to be exported/imported using name
    /// </summary>
    public bool ExportImportRelatedEntitiesByName { get; set; }

    /// <summary>
    /// Gets or sets count of displayed years for datepicker
    /// </summary>
    public int CountDisplayedYearsDatePicker { get; set; }

    /// <summary>
    /// Get or set a value indicating whether it's necessary to show the date for pre-order availability in a public store
    /// </summary>
    public bool DisplayDatePreOrderAvailability { get; set; }

    /// <summary>
    /// Get or set a value indicating whether to use a standard menu in public store or use Ajax to load a menu
    /// </summary>
    public bool UseAjaxLoadMenu { get; set; }

    /// <summary>
    /// Get or set a value indicating whether to use standard or AJAX products loading (applicable to 'paging', 'filtering', 'view modes') in catalog
    /// </summary>
    public bool UseAjaxCatalogProductsLoading { get; set; }

    /// <summary>
    /// Get or set a value indicating whether the manufacturer filtering is enabled on catalog pages
    /// </summary>
    public bool EnableManufacturerFiltering { get; set; }

    /// <summary>
    /// Get or set a value indicating whether the price range filtering is enabled on catalog pages
    /// </summary>
    public bool EnablePriceRangeFiltering { get; set; }

    /// <summary>
    /// Get or set a value indicating whether the specification attribute filtering is enabled on catalog pages
    /// </summary>
    public bool EnableSpecificationAttributeFiltering { get; set; }

    /// <summary>
    /// Get or set a value indicating whether the "From" prices (based on price adjustments of combinations and attributes) are displayed on catalog pages
    /// </summary>
    public bool DisplayFromPrices { get; set; }

    /// <summary>
    /// Gets or sets the attribute value display type when out of stock
    /// </summary>
    public AttributeValueOutOfStockDisplayType AttributeValueOutOfStockDisplayType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customer can search with manufacturer name
    /// </summary>
    public bool AllowCustomersToSearchWithManufacturerName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customer can search with category name
    /// </summary>
    public bool AllowCustomersToSearchWithCategoryName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all pictures will be displayed on catalog pages
    /// </summary>
    public bool DisplayAllPicturesOnCatalogPages { get; set; }

    /// <summary>
    /// Gets or sets the identifier of product URL structure type (e.g. '/category-seo-name/product-seo-name' or '/product-seo-name')
    /// </summary>
    /// <remarks>We have ProductUrlStructureType enum, but we use int value here so that it can be overridden in third-party plugins</remarks>
    public int ProductUrlStructureTypeId { get; set; }

    /// <summary>
    /// Gets or sets an system name of active search provider
    /// </summary>
    public string ActiveSearchProviderSystemName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether standard search will be used when the search provider throws an exception
    /// </summary>
    public bool UseStandardSearchWhenSearchProviderThrowsException { get; set; }
}