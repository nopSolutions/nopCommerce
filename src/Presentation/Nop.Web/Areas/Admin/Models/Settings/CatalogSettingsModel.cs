using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a catalog settings model
    /// </summary>
    public partial record CatalogSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Ctor

        public CatalogSettingsModel()
        {
            AvailableViewModes = new List<SelectListItem>();
            SortOptionSearchModel = new SortOptionSearchModel();
            ReviewTypeSearchModel = new ReviewTypeSearchModel();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowViewUnpublishedProductPage")]
        public bool AllowViewUnpublishedProductPage { get; set; }
        public bool AllowViewUnpublishedProductPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayDiscontinuedMessageForUnpublishedProducts")]
        public bool DisplayDiscontinuedMessageForUnpublishedProducts { get; set; }
        public bool DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowSkuOnProductDetailsPage")]
        public bool ShowSkuOnProductDetailsPage { get; set; }
        public bool ShowSkuOnProductDetailsPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowSkuOnCatalogPages")]
        public bool ShowSkuOnCatalogPages { get; set; }
        public bool ShowSkuOnCatalogPages_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowManufacturerPartNumber")]
        public bool ShowManufacturerPartNumber { get; set; }
        public bool ShowManufacturerPartNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowGtin")]
        public bool ShowGtin { get; set; }
        public bool ShowGtin_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowFreeShippingNotification")]
        public bool ShowFreeShippingNotification { get; set; }
        public bool ShowFreeShippingNotification_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowShortDescriptionOnCatalogPages")]
        public bool ShowShortDescriptionOnCatalogPages { get; set; }
        public bool ShowShortDescriptionOnCatalogPages_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowProductSorting")]
        public bool AllowProductSorting { get; set; }
        public bool AllowProductSorting_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowProductViewModeChanging")]
        public bool AllowProductViewModeChanging { get; set; }
        public bool AllowProductViewModeChanging_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DefaultViewMode")]
        public string DefaultViewMode { get; set; }
        public bool DefaultViewMode_OverrideForStore { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowProductsFromSubcategories")]
        public bool ShowProductsFromSubcategories { get; set; }
        public bool ShowProductsFromSubcategories_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryProductNumber")]
        public bool ShowCategoryProductNumber { get; set; }
        public bool ShowCategoryProductNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryProductNumberIncludingSubcategories")]
        public bool ShowCategoryProductNumberIncludingSubcategories { get; set; }
        public bool ShowCategoryProductNumberIncludingSubcategories_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled")]
        public bool CategoryBreadcrumbEnabled { get; set; }
        public bool CategoryBreadcrumbEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowShareButton")]
        public bool ShowShareButton { get; set; }
        public bool ShowShareButton_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PageShareCode")]
        public string PageShareCode { get; set; }
        public bool PageShareCode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductReviewsMustBeApproved")]
        public bool ProductReviewsMustBeApproved { get; set; }
        public bool ProductReviewsMustBeApproved_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.OneReviewPerProductFromCustomer")]
        public bool OneReviewPerProductFromCustomer { get; set; }
        public bool OneReviewPerProductFromCustomer_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToReviewProduct")]
        public bool AllowAnonymousUsersToReviewProduct { get; set; }
        public bool AllowAnonymousUsersToReviewProduct_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductReviewPossibleOnlyAfterPurchasing")]
        public bool ProductReviewPossibleOnlyAfterPurchasing { get; set; }
        public bool ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NotifyStoreOwnerAboutNewProductReviews")]
        public bool NotifyStoreOwnerAboutNewProductReviews { get; set; }
        public bool NotifyStoreOwnerAboutNewProductReviews_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NotifyCustomerAboutProductReviewReply")]
        public bool NotifyCustomerAboutProductReviewReply { get; set; }
        public bool NotifyCustomerAboutProductReviewReply_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowProductReviewsPerStore")]
        public bool ShowProductReviewsPerStore { get; set; }
        public bool ShowProductReviewsPerStore_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowProductReviewsTabOnAccountPage")]
        public bool ShowProductReviewsTabOnAccountPage { get; set; }
        public bool ShowProductReviewsOnAccountPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductReviewsPageSizeOnAccountPage")]
        public int ProductReviewsPageSizeOnAccountPage { get; set; }
        public bool ProductReviewsPageSizeOnAccountPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductReviewsSortByCreatedDateAscending")]
        public bool ProductReviewsSortByCreatedDateAscending { get; set; }
        public bool ProductReviewsSortByCreatedDateAscending_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.EmailAFriendEnabled")]
        public bool EmailAFriendEnabled { get; set; }
        public bool EmailAFriendEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToEmailAFriend")]
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }
        public bool AllowAnonymousUsersToEmailAFriend_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedProductsNumber")]
        public int RecentlyViewedProductsNumber { get; set; }
        public bool RecentlyViewedProductsNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedProductsEnabled")]
        public bool RecentlyViewedProductsEnabled { get; set; }
        public bool RecentlyViewedProductsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NewProductsEnabled")]
        public bool NewProductsEnabled { get; set; }
        public bool NewProductsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NewProductsPageSize")]
        public int NewProductsPageSize { get; set; }
        public bool NewProductsPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NewProductsAllowCustomersToSelectPageSize")]
        public bool NewProductsAllowCustomersToSelectPageSize { get; set; }
        public bool NewProductsAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NewProductsPageSizeOptions")]
        public string NewProductsPageSizeOptions { get; set; }
        public bool NewProductsPageSizeOptions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.CompareProductsEnabled")]
        public bool CompareProductsEnabled { get; set; }
        public bool CompareProductsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowBestsellersOnHomepage")]
        public bool ShowBestsellersOnHomepage { get; set; }
        public bool ShowBestsellersOnHomepage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfBestsellersOnHomepage")]
        public int NumberOfBestsellersOnHomepage { get; set; }
        public bool NumberOfBestsellersOnHomepage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageProductsPerPage")]
        public int SearchPageProductsPerPage { get; set; }
        public bool SearchPageProductsPerPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageAllowCustomersToSelectPageSize")]
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }
        public bool SearchPageAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions")]
        public string SearchPagePageSizeOptions { get; set; }
        public bool SearchPagePageSizeOptions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePriceRangeFiltering")]
        public bool SearchPagePriceRangeFiltering { get; set; }
        public bool SearchPagePriceRangeFiltering_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePriceFrom")]
        public decimal SearchPagePriceFrom { get; set; }
        public bool SearchPagePriceFrom_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePriceTo")]
        public decimal SearchPagePriceTo { get; set; }
        public bool SearchPagePriceTo_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageManuallyPriceRange")]
        public bool SearchPageManuallyPriceRange { get; set; }
        public bool SearchPageManuallyPriceRange_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductSearchAutoCompleteEnabled")]
        public bool ProductSearchAutoCompleteEnabled { get; set; }
        public bool ProductSearchAutoCompleteEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductSearchEnabled")]
        public bool ProductSearchEnabled { get; set; }
        public bool ProductSearchEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductSearchAutoCompleteNumberOfProducts")]
        public int ProductSearchAutoCompleteNumberOfProducts { get; set; }
        public bool ProductSearchAutoCompleteNumberOfProducts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowProductImagesInSearchAutoComplete")]
        public bool ShowProductImagesInSearchAutoComplete { get; set; }
        public bool ShowProductImagesInSearchAutoComplete_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowLinkToAllResultInSearchAutoComplete")]
        public bool ShowLinkToAllResultInSearchAutoComplete { get; set; }
        public bool ShowLinkToAllResultInSearchAutoComplete_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductSearchTermMinimumLength")]
        public int ProductSearchTermMinimumLength { get; set; }
        public bool ProductSearchTermMinimumLength_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsAlsoPurchasedEnabled")]
        public bool ProductsAlsoPurchasedEnabled { get; set; }
        public bool ProductsAlsoPurchasedEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsAlsoPurchasedNumber")]
        public int ProductsAlsoPurchasedNumber { get; set; }
        public bool ProductsAlsoPurchasedNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfProductTags")]
        public int NumberOfProductTags { get; set; }
        public bool NumberOfProductTags_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagPageSize")]
        public int ProductsByTagPageSize { get; set; }
        public bool ProductsByTagPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagAllowCustomersToSelectPageSize")]
        public bool ProductsByTagAllowCustomersToSelectPageSize { get; set; }
        public bool ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagPageSizeOptions")]
        public string ProductsByTagPageSizeOptions { get; set; }
        public bool ProductsByTagPageSizeOptions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagPriceRangeFiltering")]
        public bool ProductsByTagPriceRangeFiltering { get; set; }
        public bool ProductsByTagPriceRangeFiltering_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagPriceFrom")]
        public decimal ProductsByTagPriceFrom { get; set; }
        public bool ProductsByTagPriceFrom_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagPriceTo")]
        public decimal ProductsByTagPriceTo { get; set; }
        public bool ProductsByTagPriceTo_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagManuallyPriceRange")]
        public bool ProductsByTagManuallyPriceRange { get; set; }
        public bool ProductsByTagManuallyPriceRange_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeShortDescriptionInCompareProducts")]
        public bool IncludeShortDescriptionInCompareProducts { get; set; }
        public bool IncludeShortDescriptionInCompareProducts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeFullDescriptionInCompareProducts")]
        public bool IncludeFullDescriptionInCompareProducts { get; set; }
        public bool IncludeFullDescriptionInCompareProducts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ManufacturersBlockItemsToDisplay")]
        public int ManufacturersBlockItemsToDisplay { get; set; }
        public bool ManufacturersBlockItemsToDisplay_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoFooter")]
        public bool DisplayTaxShippingInfoFooter { get; set; }
        public bool DisplayTaxShippingInfoFooter_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoProductDetailsPage")]
        public bool DisplayTaxShippingInfoProductDetailsPage { get; set; }
        public bool DisplayTaxShippingInfoProductDetailsPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoProductBoxes")]
        public bool DisplayTaxShippingInfoProductBoxes { get; set; }
        public bool DisplayTaxShippingInfoProductBoxes_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoShoppingCart")]
        public bool DisplayTaxShippingInfoShoppingCart { get; set; }
        public bool DisplayTaxShippingInfoShoppingCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoWishlist")]
        public bool DisplayTaxShippingInfoWishlist { get; set; }
        public bool DisplayTaxShippingInfoWishlist_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoOrderDetailsPage")]
        public bool DisplayTaxShippingInfoOrderDetailsPage { get; set; }
        public bool DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportProductAttributes")]
        public bool ExportImportProductAttributes { get; set; }
        public bool ExportImportProductAttributes_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportProductSpecificationAttributes")]
        public bool ExportImportProductSpecificationAttributes { get; set; }
        public bool ExportImportProductSpecificationAttributes_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportProductCategoryBreadcrumb")]
        public bool ExportImportProductCategoryBreadcrumb { get; set; }
        public bool ExportImportProductCategoryBreadcrumb_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportCategoriesUsingCategoryName")]
        public bool ExportImportCategoriesUsingCategoryName { get; set; }
        public bool ExportImportCategoriesUsingCategoryName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages")]
        public bool ExportImportAllowDownloadImages { get; set; }
        public bool ExportImportAllowDownloadImages_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportSplitProductsFile")]
        public bool ExportImportSplitProductsFile { get; set; }
        public bool ExportImportSplitProductsFile_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.RemoveRequiredProducts")]
        public bool RemoveRequiredProducts { get; set; }
        public bool RemoveRequiredProducts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportRelatedEntitiesByName")]
        public bool ExportImportRelatedEntitiesByName { get; set; }
        public bool ExportImportRelatedEntitiesByName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportProductUseLimitedToStores")]
        public bool ExportImportProductUseLimitedToStores { get; set; }
        public bool ExportImportProductUseLimitedToStores_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreDiscounts")]
        public bool IgnoreDiscounts { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreFeaturedProducts")]
        public bool IgnoreFeaturedProducts { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreAcl")]
        public bool IgnoreAcl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreStoreLimitations")]
        public bool IgnoreStoreLimitations { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.CacheProductPrices")]
        public bool CacheProductPrices { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayDatePreOrderAvailability")]
        public bool DisplayDatePreOrderAvailability { get; set; }
        public bool DisplayDatePreOrderAvailability_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AttributeValueOutOfStockDisplayType")]
        public int AttributeValueOutOfStockDisplayType { get; set; }
        public bool AttributeValueOutOfStockDisplayType_OverrideForStore { get; set; }
        public SelectList AttributeValueOutOfStockDisplayTypes { get; set; }

        public SortOptionSearchModel SortOptionSearchModel { get; set; }

        public ReviewTypeSearchModel ReviewTypeSearchModel { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.UseAjaxCatalogProductsLoading")]
        public bool UseAjaxCatalogProductsLoading { get; set; }
        public bool UseAjaxCatalogProductsLoading_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.EnableManufacturerFiltering")]
        public bool EnableManufacturerFiltering { get; set; }
        public bool EnableManufacturerFiltering_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.EnablePriceRangeFiltering")]
        public bool EnablePriceRangeFiltering { get; set; }
        public bool EnablePriceRangeFiltering_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.EnableSpecificationAttributeFiltering")]
        public bool EnableSpecificationAttributeFiltering { get; set; }
        public bool EnableSpecificationAttributeFiltering_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayFromPrices")]
        public bool DisplayFromPrices { get; set; }
        public bool DisplayFromPrices_OverrideForStore { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithManufacturerName")]
        public bool AllowCustomersToSearchWithManufacturerName { get; set; }
        public bool AllowCustomersToSearchWithManufacturerName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithCategoryName")]
        public bool AllowCustomersToSearchWithCategoryName { get; set; }
        public bool AllowCustomersToSearchWithCategoryName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayAllPicturesOnCatalogPages")]
        public bool DisplayAllPicturesOnCatalogPages { get; set; }
        public bool DisplayAllPicturesOnCatalogPages_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductUrlStructureType")]
        public int ProductUrlStructureTypeId { get; set; }
        public bool ProductUrlStructureTypeId_OverrideForStore { get; set; }
        public SelectList ProductUrlStructureTypes { get; set; }

        #endregion
    }
}