using Mapster;
using Nop.Core.Configuration;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure.Mapper;
using Nop.Data.Configuration;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Cms;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Areas.Admin.Models.MultiFactorAuthentication;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.WebOptimizer;

namespace Nop.Web.Areas.Admin.Infrastructure.Mapper;

/// <summary>
/// Mapster configuration for admin area models
/// </summary>
public partial class AdminMapperConfiguration : IOrderedMapperProfile
{
    #region Methods

    /// <summary>
    /// Configure mappings
    /// </summary>
    /// <param name="config">Type adapter configuration</param>
    public void Configure(TypeAdapterConfig config)
    {
        //create specific maps
        CreateConfigMaps(config);
        CreateAffiliatesMaps(config);
        CreateAuthenticationMaps(config);
        CreateMultiFactorAuthenticationMaps(config);
        CreateBlogsMaps(config);
        CreateCatalogMaps(config);
        CreateCmsMaps(config);
        CreateCommonMaps(config);
        CreateCustomersMaps(config);
        CreateDirectoryMaps(config);
        CreateDiscountsMaps(config);
        CreateForumsMaps(config);
        CreateGdprMaps(config);
        CreateLocalizationMaps(config);
        CreateLoggingMaps(config);
        CreateMediaMaps(config);
        CreateMessagesMaps(config);
        CreateNewsMaps(config);
        CreateOrdersMaps(config);
        CreatePaymentsMaps(config);
        CreatePluginsMaps(config);
        CreatePollsMaps(config);
        CreateSecurityMaps(config);
        CreateSeoMaps(config);
        CreateShippingMaps(config);
        CreateStoresMaps(config);
        CreateTasksMaps(config);
        CreateTaxMaps(config);
        CreateTopicsMaps(config);
        CreateVendorsMaps(config);
        CreateWarehouseMaps(config);

        //add some generic mapping rules
        ConfigureGlobalSettings(config);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Configure global mapping settings
    /// </summary>
    /// <param name="config">Type adapter configuration</param>
    protected virtual void ConfigureGlobalSettings(TypeAdapterConfig config)
    {
        // BaseNopModel
        config.When((s, d, m) => typeof(BaseNopModel).IsAssignableFrom(d))
              .Ignore(nameof(BaseNopModel.CustomProperties));

        // ISettingsModel
        config.When((s, d, m) => typeof(ISettingsModel).IsAssignableFrom(d))
              .Ignore(nameof(ISettingsModel.ActiveStoreScopeConfiguration));

        // IConfig
        config.When((s, d, m) => typeof(IConfig).IsAssignableFrom(d))
              .Ignore(nameof(IConfig.Name));

        // ILocalizedModel
        config.When((s, d, m) => typeof(ILocalizedModel).IsAssignableFrom(d))
              .Ignore(nameof(ILocalizedModel<ILocalizedModel>.Locales));

        // IStoreMappingSupported
        config.When((s, d, m) => typeof(IStoreMappingSupported).IsAssignableFrom(d))
              .Ignore(nameof(IStoreMappingSupported.LimitedToStores));

        // IStoreMappingSupportedModel
        config.When((s, d, m) => typeof(IStoreMappingSupportedModel).IsAssignableFrom(d))
              .Ignore(nameof(IStoreMappingSupportedModel.AvailableStores))
              .Ignore(nameof(IStoreMappingSupportedModel.SelectedStoreIds));

        // IAclSupported
        config.When((s, d, m) => typeof(IAclSupported).IsAssignableFrom(d))
              .Ignore(nameof(IAclSupported.SubjectToAcl));

        // IAclSupportedModel
        config.When((s, d, m) => typeof(IAclSupportedModel).IsAssignableFrom(d))
              .Ignore(nameof(IAclSupportedModel.AvailableCustomerRoles))
              .Ignore(nameof(IAclSupportedModel.SelectedCustomerRoleIds));

        // IDiscountSupportedModel
        config.When((s, d, m) => typeof(IDiscountSupportedModel).IsAssignableFrom(d))
              .Ignore(nameof(IDiscountSupportedModel.AvailableDiscounts))
              .Ignore(nameof(IDiscountSupportedModel.SelectedDiscountIds));

        // ITranslationSupportedModel
        config.When((s, d, m) => typeof(ITranslationSupportedModel).IsAssignableFrom(d))
              .Ignore(nameof(ITranslationSupportedModel.PreTranslationAvailable));

        // IPluginModel common ignores
        config.When((s, d, m) => typeof(IPluginModel).IsAssignableFrom(d))
              .Ignore(nameof(IPluginModel.ConfigurationUrl))
              .Ignore(nameof(IPluginModel.IsActive))
              .Ignore(nameof(IPluginModel.LogoUrl));

        
        // Base rule: any IPlugin -> any IPluginModel
        config.NewConfig<IPlugin, IPluginModel>()
            .AfterMapping((src, dest) =>
            {
                dest.DisplayOrder = src.PluginDescriptor.DisplayOrder;
                dest.FriendlyName = src.PluginDescriptor.FriendlyName;
                dest.SystemName   = src.PluginDescriptor.SystemName;
            });
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Create configuration maps 
    /// </summary>
    protected virtual void CreateConfigMaps(TypeAdapterConfig config)
    {
        config.NewConfig<CacheConfig, CacheConfigModel>();
        config.NewConfig<CacheConfigModel, CacheConfig>();

        config.NewConfig<HostingConfig, HostingConfigModel>();
        config.NewConfig<HostingConfigModel, HostingConfig>();

        config.NewConfig<DistributedCacheConfig, DistributedCacheConfigModel>()
              .Ignore(model => model.DistributedCacheTypeValues);
        config.NewConfig<DistributedCacheConfigModel, DistributedCacheConfig>();

        config.NewConfig<InstallationConfig, InstallationConfigModel>();
        config.NewConfig<InstallationConfigModel, InstallationConfig>();

        config.NewConfig<PluginConfig, PluginConfigModel>();
        config.NewConfig<PluginConfigModel, PluginConfig>();

        config.NewConfig<CommonConfig, CommonConfigModel>();
        config.NewConfig<CommonConfigModel, CommonConfig>();

        config.NewConfig<DataConfig, DataConfigModel>()
              .Ignore(model => model.DataProviderTypeValues);
        config.NewConfig<DataConfigModel, DataConfig>();

        config.NewConfig<WebOptimizerConfig, WebOptimizerConfigModel>();
        config.NewConfig<WebOptimizerConfigModel, WebOptimizerConfig>()
              .Ignore(entity => entity.CdnUrl)
              .Ignore(entity => entity.AllowEmptyBundle)
              .Ignore(entity => entity.HttpsCompression)
              .Ignore(entity => entity.EnableTagHelperBundling)
              .Ignore(entity => entity.EnableCaching)
              .Ignore(entity => entity.EnableMemoryCache);
    }

    /// <summary>
    /// Create affiliates maps 
    /// </summary>
    protected virtual void CreateAffiliatesMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Affiliate, AffiliateModel>()
              .Ignore(model => model.Address)
              .Ignore(model => model.AffiliatedCustomerSearchModel)
              .Ignore(model => model.AffiliatedOrderSearchModel)
              .Ignore(model => model.Url);
        config.NewConfig<AffiliateModel, Affiliate>()
              .Ignore(entity => entity.Deleted);

        config.NewConfig<Order, AffiliatedOrderModel>()
              .Ignore(model => model.OrderStatus)
              .Ignore(model => model.PaymentStatus)
              .Ignore(model => model.ShippingStatus)
              .Ignore(model => model.OrderTotal)
              .Ignore(model => model.CreatedOn);

        config.NewConfig<Customer, AffiliatedCustomerModel>()
              .Ignore(model => model.Name);
    }

    /// <summary>
    /// Create authentication maps 
    /// </summary>
    protected virtual void CreateAuthenticationMaps(TypeAdapterConfig config)
    {
        config.NewConfig<IExternalAuthenticationMethod, ExternalAuthenticationMethodModel>();
    }

    /// <summary>
    /// Create multi-factor authentication maps 
    /// </summary>
    protected virtual void CreateMultiFactorAuthenticationMaps(TypeAdapterConfig config)
    {
        config.NewConfig<IMultiFactorAuthenticationMethod, MultiFactorAuthenticationMethodModel>();
    }

    /// <summary>
    /// Create blogs maps 
    /// </summary>
    protected virtual void CreateBlogsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<BlogComment, BlogCommentModel>()
            .Ignore(d => d.BlogPostTitle)
            .Ignore(d => d.Comment)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.CustomerInfo)
            .Ignore(d => d.StoreName);

        config.NewConfig<BlogCommentModel, BlogComment>()
            .Ignore(d => d.CommentText)
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.BlogPostId)
            .Ignore(d => d.CustomerId)
            .Ignore(d => d.StoreId);

        config.NewConfig<BlogPost, BlogPostModel>()
            .Ignore(d => d.ApprovedComments)
            .Ignore(d => d.AvailableLanguages)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.LanguageName)
            .Ignore(d => d.NotApprovedComments)
            .Ignore(d => d.SeName)
            .Ignore(d => d.InitialBlogTags);

        config.NewConfig<BlogPostModel, BlogPost>()
            .Ignore(d => d.CreatedOnUtc);

        config.NewConfig<BlogSettings, BlogSettingsModel>()
            .Ignore(d => d.AllowNotRegisteredUsersToLeaveComments_OverrideForStore)
            .Ignore(d => d.BlogCommentsMustBeApproved_OverrideForStore)
            .Ignore(d => d.Enabled_OverrideForStore)
            .Ignore(d => d.NotifyAboutNewBlogComments_OverrideForStore)
            .Ignore(d => d.NumberOfTags_OverrideForStore)
            .Ignore(d => d.PostsPageSize_OverrideForStore)
            .Ignore(d => d.ShowHeaderRssUrl_OverrideForStore);

        config.NewConfig<BlogSettingsModel, BlogSettings>();
    }

    /// <summary>
    /// Create catalog maps 
    /// </summary>
    protected virtual void CreateCatalogMaps(TypeAdapterConfig config)
    {
        config.NewConfig<CatalogSettings, CatalogSettingsModel>()
            .Ignore(d => d.AllowAnonymousUsersToEmailAFriend_OverrideForStore)
            .Ignore(d => d.AllowAnonymousUsersToReviewProduct_OverrideForStore)
            .Ignore(d => d.AllowProductSorting_OverrideForStore)
            .Ignore(d => d.AllowProductViewModeChanging_OverrideForStore)
            .Ignore(d => d.AllowViewUnpublishedProductPage_OverrideForStore)
            .Ignore(d => d.AvailableViewModes)
            .Ignore(d => d.CategoryBreadcrumbEnabled_OverrideForStore)
            .Ignore(d => d.CompareProductsEnabled_OverrideForStore)
            .Ignore(d => d.DefaultViewMode_OverrideForStore)
            .Ignore(d => d.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore)
            .Ignore(d => d.DisplayTaxShippingInfoFooter_OverrideForStore)
            .Ignore(d => d.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore)
            .Ignore(d => d.DisplayTaxShippingInfoProductBoxes_OverrideForStore)
            .Ignore(d => d.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore)
            .Ignore(d => d.DisplayTaxShippingInfoShoppingCart_OverrideForStore)
            .Ignore(d => d.DisplayTaxShippingInfoWishlist_OverrideForStore)
            .Ignore(d => d.EmailAFriendEnabled_OverrideForStore)
            .Ignore(d => d.ExportImportAllowDownloadImages_OverrideForStore)
            .Ignore(d => d.ExportImportCategoriesUsingCategoryName_OverrideForStore)
            .Ignore(d => d.ExportImportProductAttributes_OverrideForStore)
            .Ignore(d => d.ExportImportProductCategoryBreadcrumb_OverrideForStore)
            .Ignore(d => d.ExportImportProductSpecificationAttributes_OverrideForStore)
            .Ignore(d => d.ExportImportTierPrices_OverrideForStore)
            .Ignore(d => d.ExportImportRelatedEntitiesByName_OverrideForStore)
            .Ignore(d => d.ExportImportProductUseLimitedToStores_OverrideForStore)
            .Ignore(d => d.ExportImportCategoryUseLimitedToStores_OverrideForStore)
            .Ignore(d => d.ExportImportSplitProductsFile_OverrideForStore)
            .Ignore(d => d.IncludeFullDescriptionInCompareProducts_OverrideForStore)
            .Ignore(d => d.IncludeShortDescriptionInCompareProducts_OverrideForStore)
            .Ignore(d => d.ManufacturersBlockItemsToDisplay_OverrideForStore)
            .Ignore(d => d.NewProductsEnabled_OverrideForStore)
            .Ignore(d => d.NewProductsPageSize_OverrideForStore)
            .Ignore(d => d.NewProductsAllowCustomersToSelectPageSize_OverrideForStore)
            .Ignore(d => d.NewProductsPageSizeOptions_OverrideForStore)
            .Ignore(d => d.NotifyCustomerAboutProductReviewReply_OverrideForStore)
            .Ignore(d => d.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore)
            .Ignore(d => d.NumberOfBestsellersOnHomepage_OverrideForStore)
            .Ignore(d => d.NumberOfProductTags_OverrideForStore)
            .Ignore(d => d.PageShareCode_OverrideForStore)
            .Ignore(d => d.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore)
            .Ignore(d => d.ProductReviewsMustBeApproved_OverrideForStore)
            .Ignore(d => d.OneReviewPerProductFromCustomer_OverrideForStore)
            .Ignore(d => d.ProductReviewsPageSizeOnAccountPage_OverrideForStore)
            .Ignore(d => d.ProductReviewsSortByCreatedDateAscending_OverrideForStore)
            .Ignore(d => d.ProductsAlsoPurchasedEnabled_OverrideForStore)
            .Ignore(d => d.ProductsAlsoPurchasedNumber_OverrideForStore)
            .Ignore(d => d.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore)
            .Ignore(d => d.ProductsByTagPageSizeOptions_OverrideForStore)
            .Ignore(d => d.ProductsByTagPageSize_OverrideForStore)
            .Ignore(d => d.ProductSearchAutoCompleteEnabled_OverrideForStore)
            .Ignore(d => d.ProductSearchEnabled_OverrideForStore)
            .Ignore(d => d.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore)
            .Ignore(d => d.ProductSearchTermMinimumLength_OverrideForStore)
            .Ignore(d => d.RecentlyViewedProductsEnabled_OverrideForStore)
            .Ignore(d => d.RecentlyViewedProductsNumber_OverrideForStore)
            .Ignore(d => d.RemoveRequiredProducts_OverrideForStore)
            .Ignore(d => d.SearchPageAllowCustomersToSelectPageSize_OverrideForStore)
            .Ignore(d => d.SearchPagePageSizeOptions_OverrideForStore)
            .Ignore(d => d.SearchPageProductsPerPage_OverrideForStore)
            .Ignore(d => d.ShowBestsellersOnHomepage_OverrideForStore)
            .Ignore(d => d.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore)
            .Ignore(d => d.ShowCategoryProductNumber_OverrideForStore)
            .Ignore(d => d.ShowFreeShippingNotification_OverrideForStore)
            .Ignore(d => d.ShowShortDescriptionOnCatalogPages_OverrideForStore)
            .Ignore(d => d.ShowGtin_OverrideForStore)
            .Ignore(d => d.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore)
            .Ignore(d => d.ShowManufacturerPartNumber_OverrideForStore)
            .Ignore(d => d.ShowProductImagesInSearchAutoComplete_OverrideForStore)
            .Ignore(d => d.ShowProductReviewsOnAccountPage_OverrideForStore)
            .Ignore(d => d.ShowProductReviewsPerStore_OverrideForStore)
            .Ignore(d => d.ShowProductsFromSubcategories_OverrideForStore)
            .Ignore(d => d.ShowShareButton_OverrideForStore)
            .Ignore(d => d.ShowSkuOnCatalogPages_OverrideForStore)
            .Ignore(d => d.ShowSkuOnProductDetailsPage_OverrideForStore)
            .Ignore(d => d.DisplayDatePreOrderAvailability_OverrideForStore)
            .Ignore(d => d.UseAjaxCatalogProductsLoading_OverrideForStore)
            .Ignore(d => d.SearchPagePriceRangeFiltering_OverrideForStore)
            .Ignore(d => d.SearchPagePriceFrom_OverrideForStore)
            .Ignore(d => d.SearchPagePriceTo_OverrideForStore)
            .Ignore(d => d.SearchPageManuallyPriceRange_OverrideForStore)
            .Ignore(d => d.ProductsByTagPriceRangeFiltering_OverrideForStore)
            .Ignore(d => d.ProductsByTagPriceFrom_OverrideForStore)
            .Ignore(d => d.ProductsByTagPriceTo_OverrideForStore)
            .Ignore(d => d.ProductsByTagManuallyPriceRange_OverrideForStore)
            .Ignore(d => d.EnableManufacturerFiltering_OverrideForStore)
            .Ignore(d => d.EnablePriceRangeFiltering_OverrideForStore)
            .Ignore(d => d.EnableSpecificationAttributeFiltering_OverrideForStore)
            .Ignore(d => d.DisplayFromPrices_OverrideForStore)
            .Ignore(d => d.AttributeValueOutOfStockDisplayTypes)
            .Ignore(d => d.AttributeValueOutOfStockDisplayType_OverrideForStore)
            .Ignore(d => d.SortOptionSearchModel)
            .Ignore(d => d.ReviewTypeSearchModel)
            .Ignore(d => d.PrimaryStoreCurrencyCode)
            .Ignore(d => d.AllowCustomersToSearchWithManufacturerName_OverrideForStore)
            .Ignore(d => d.AllowCustomersToSearchWithCategoryName_OverrideForStore)
            .Ignore(d => d.DisplayAllPicturesOnCatalogPages_OverrideForStore)
            .Ignore(d => d.ProductUrlStructureTypeId_OverrideForStore)
            .Ignore(d => d.ProductUrlStructureTypes)
            .Ignore(d => d.ShowSearchBoxCategories_OverrideForStore)
            .Ignore(d => d.ArtificialIntelligenceSettingsModel);
        config.NewConfig<CatalogSettingsModel, CatalogSettings>()
            .Ignore(d => d.AjaxProcessAttributeChange)
            .Ignore(d => d.CompareProductsNumber)
            .Ignore(d => d.CountDisplayedYearsDatePicker)
            .Ignore(d => d.DefaultCategoryPageSize)
            .Ignore(d => d.DefaultCategoryPageSizeOptions)
            .Ignore(d => d.DefaultManufacturerPageSize)
            .Ignore(d => d.DefaultManufacturerPageSizeOptions)
            .Ignore(d => d.DefaultProductRatingValue)
            .Ignore(d => d.DisplayTierPricesWithDiscounts)
            .Ignore(d => d.ExportImportProductsCountInOneFile)
            .Ignore(d => d.ExportImportUseDropdownlistsForAssociatedEntities)
            .Ignore(d => d.IncludeFeaturedProductsInNormalLists)
            .Ignore(d => d.MaximumBackInStockSubscriptions)
            .Ignore(d => d.ProductSortingEnumDisabled)
            .Ignore(d => d.ProductSortingEnumDisplayOrder)
            .Ignore(d => d.PublishBackProductWhenCancellingOrders)
            .Ignore(d => d.UseAjaxLoadMenu)
            .Ignore(d => d.UseLinksInRequiredProductWarnings)
            .Ignore(d => d.UseStandardSearchWhenSearchProviderThrowsException)
            .Ignore(d => d.ActiveSearchProviderSystemName)
            .Ignore(d => d.VendorProductReviewsPageSize);

        config.NewConfig<ProductCategory, CategoryProductModel>()
            .Ignore(d => d.ProductName);
        config.NewConfig<CategoryProductModel, ProductCategory>()
            .Ignore(d => d.CategoryId)
            .Ignore(d => d.ProductId);

        config.NewConfig<Category, CategoryModel>()
            .Ignore(d => d.AvailableCategories)
            .Ignore(d => d.AvailableCategoryTemplates)
            .Ignore(d => d.Breadcrumb)
            .Ignore(d => d.CategoryProductSearchModel)
            .Ignore(d => d.SeName)
            .Ignore(d => d.PrimaryStoreCurrencyCode);
        config.NewConfig<CategoryModel, Category>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.Deleted)
            .Ignore(d => d.UpdatedOnUtc);

        config.NewConfig<CategoryTemplate, CategoryTemplateModel>();
        config.NewConfig<CategoryTemplateModel, CategoryTemplate>();

        config.NewConfig<ProductManufacturer, ManufacturerProductModel>()
            .Ignore(d => d.ProductName);
        config.NewConfig<ManufacturerProductModel, ProductManufacturer>()
            .Ignore(d => d.ManufacturerId)
            .Ignore(d => d.ProductId);

        config.NewConfig<Manufacturer, ManufacturerModel>()
            .Ignore(d => d.AvailableManufacturerTemplates)
            .Ignore(d => d.ManufacturerProductSearchModel)
            .Ignore(d => d.SeName)
            .Ignore(d => d.PrimaryStoreCurrencyCode);
        config.NewConfig<ManufacturerModel, Manufacturer>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.Deleted)
            .Ignore(d => d.UpdatedOnUtc);

        config.NewConfig<ManufacturerTemplate, ManufacturerTemplateModel>();
        config.NewConfig<ManufacturerTemplateModel, ManufacturerTemplate>();

        //Review type
        config.NewConfig<ReviewType, ReviewTypeModel>();
        config.NewConfig<ReviewTypeModel, ReviewType>();

        //product review
        config.NewConfig<ProductReview, ProductReviewModel>()
            .Ignore(d => d.CustomerInfo)
            .Ignore(d => d.IsLoggedInAsVendor)
            .Ignore(d => d.ProductReviewReviewTypeMappingSearchModel)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.StoreName)
            .Ignore(d => d.ShowStoreName)
            .Ignore(d => d.ProductName);

        //product review type mapping
        config.NewConfig<ProductReviewReviewTypeMapping, ProductReviewReviewTypeMappingModel>()
            .Ignore(d => d.Name)
            .Ignore(d => d.Description)
            .Ignore(d => d.VisibleToAllCustomers);

        //products
        config.NewConfig<Product, ProductModel>()
            .Ignore(d => d.AddPictureModel)
            .Ignore(d => d.AssociatedProductSearchModel)
            .Ignore(d => d.AssociatedToProductId)
            .Ignore(d => d.AssociatedToProductName)
            .Ignore(d => d.AvailableBasepriceBaseUnits)
            .Ignore(d => d.AvailableBasepriceUnits)
            .Ignore(d => d.AvailableCategories)
            .Ignore(d => d.AvailableDeliveryDates)
            .Ignore(d => d.AvailableManufacturers)
            .Ignore(d => d.AvailableProductAvailabilityRanges)
            .Ignore(d => d.AvailableProductTemplates)
            .Ignore(d => d.AvailableTaxCategories)
            .Ignore(d => d.AvailableVendors)
            .Ignore(d => d.AvailableWarehouses)
            .Ignore(d => d.BaseDimensionIn)
            .Ignore(d => d.BaseWeightIn)
            .Ignore(d => d.CopyProductModel)
            .Ignore(d => d.CrossSellProductSearchModel)
            .Ignore(d => d.HasAvailableSpecificationAttributes)
            .Ignore(d => d.IsLoggedInAsVendor)
            .Ignore(d => d.LastStockQuantity)
            .Ignore(d => d.PictureThumbnailUrl)
            .Ignore(d => d.PrimaryStoreCurrencyCode)
            .Ignore(d => d.ProductAttributeCombinationSearchModel)
            .Ignore(d => d.ProductAttributeMappingSearchModel)
            .Ignore(d => d.ProductAttributesExist)
            .Ignore(d => d.CanCreateCombinations)
            .Ignore(d => d.ProductEditorSettingsModel)
            .Ignore(d => d.ProductOrderSearchModel)
            .Ignore(d => d.ProductPictureModels)
            .Ignore(d => d.ProductPictureSearchModel)
            .Ignore(d => d.ProductVideoModels)
            .Ignore(d => d.ProductVideoSearchModel)
            .Ignore(d => d.AddVideoModel)
            .Ignore(d => d.ProductSpecificationAttributeSearchModel)
            .Ignore(d => d.ProductsTypesSupportedByProductTemplates)
            .Ignore(d => d.AvailableProductTags)
            .Ignore(d => d.ProductTypeName)
            .Ignore(d => d.ProductWarehouseInventoryModels)
            .Ignore(d => d.RelatedProductSearchModel)
            .Ignore(d => d.SelectedCategoryIds)
            .Ignore(d => d.SelectedManufacturerIds)
            .Ignore(d => d.SeName)
            .Ignore(d => d.StockQuantityHistory)
            .Ignore(d => d.StockQuantityHistorySearchModel)
            .Ignore(d => d.StockQuantityStr)
            .Ignore(d => d.TierPriceSearchModel)
            .Ignore(d => d.SelectedProductTags)
            .Ignore(d => d.FormattedPrice);

        config.NewConfig<ProductModel, Product>()
            .Ignore(d => d.ApprovedRatingSum)
            .Ignore(d => d.ApprovedTotalReviews)
            .Ignore(d => d.BackorderMode)
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.Deleted)
            .Ignore(d => d.DownloadActivationType)
            .Ignore(d => d.GiftCardType)
            .Ignore(d => d.LowStockActivity)
            .Ignore(d => d.ManageInventoryMethod)
            .Ignore(d => d.NotApprovedRatingSum)
            .Ignore(d => d.NotApprovedTotalReviews)
            .Ignore(d => d.ParentGroupedProductId)
            .Ignore(d => d.ProductType)
            .Ignore(d => d.RecurringCyclePeriod)
            .Ignore(d => d.RentalPricePeriod)
            .Ignore(d => d.UpdatedOnUtc);

        config.NewConfig<Product, DiscountProductModel>()
            .Ignore(d => d.ProductId)
            .Ignore(d => d.ProductName)
            .Ignore(d => d.VendorId);

        config.NewConfig<Product, AssociatedProductModel>()
            .Ignore(d => d.ProductName);

        config.NewConfig<ProductAttributeCombination, ProductAttributeCombinationModel>()
            .Ignore(d => d.AttributesXml)
            .Ignore(d => d.ProductAttributes)
            .Ignore(d => d.ProductPictureModels)
            .Ignore(d => d.PictureThumbnailUrl)
            .Ignore(d => d.Warnings)
            .Ignore(d => d.PictureIds);

        config.NewConfig<ProductAttributeCombinationModel, ProductAttributeCombination>()
            .Ignore(d => d.AttributesXml)
#pragma warning disable CS0618
            .Ignore(d => d.PictureId);
#pragma warning restore CS0618

        config.NewConfig<ProductAttribute, ProductAttributeModel>()
            .Ignore(d => d.PredefinedProductAttributeValueSearchModel)
            .Ignore(d => d.ProductAttributeProductSearchModel);
        config.NewConfig<ProductAttributeModel, ProductAttribute>();

        config.NewConfig<Product, ProductAttributeProductModel>()
            .Ignore(d => d.ProductName);

        config.NewConfig<PredefinedProductAttributeValue, PredefinedProductAttributeValueModel>()
            .Ignore(d => d.WeightAdjustmentStr)
            .Ignore(d => d.PriceAdjustmentStr);

        config.NewConfig<PredefinedProductAttributeValueModel, PredefinedProductAttributeValue>();

        config.NewConfig<ProductAttributeMapping, ProductAttributeMappingModel>()
            .Ignore(d => d.ValidationRulesString)
            .Ignore(d => d.AttributeControlType)
            .Ignore(d => d.ConditionString)
            .Ignore(d => d.ProductAttribute)
            .Ignore(d => d.AvailableProductAttributes)
            .Ignore(d => d.ConditionAllowed)
            .Ignore(d => d.ConditionModel)
            .Ignore(d => d.ProductAttributeValueSearchModel);

        config.NewConfig<ProductAttributeMappingModel, ProductAttributeMapping>()
            .Ignore(d => d.ConditionAttributeXml)
            .Ignore(d => d.AttributeControlType);

        config.NewConfig<ProductAttributeValue, ProductAttributeValueModel>()
            .Ignore(d => d.AttributeValueTypeName)
            .Ignore(d => d.Name)
            .Ignore(d => d.PriceAdjustmentStr)
            .Ignore(d => d.AssociatedProductName)
            .Ignore(d => d.PictureThumbnailUrl)
            .Ignore(d => d.WeightAdjustmentStr)
            .Ignore(d => d.DisplayColorSquaresRgb)
            .Ignore(d => d.DisplayImageSquaresPicture)
            .Ignore(d => d.ProductPictureModels)
            .Ignore(d => d.PictureIds);
        config.NewConfig<ProductAttributeValueModel, ProductAttributeValue>()
            .Ignore(d => d.AttributeValueType)
            .Ignore(d => d.Quantity)
#pragma warning disable CS0618
            .Ignore(d => d.PictureId);
#pragma warning restore CS0618

        config.NewConfig<ProductEditorSettings, ProductEditorSettingsModel>();
        config.NewConfig<ProductEditorSettingsModel, ProductEditorSettings>();

        config.NewConfig<ProductPicture, ProductPictureModel>()
            .Ignore(d => d.OverrideAltAttribute)
            .Ignore(d => d.OverrideTitleAttribute)
            .Ignore(d => d.PictureUrl);

        config.NewConfig<ProductVideo, ProductVideoModel>()
            .Ignore(d => d.VideoUrl);

        config.NewConfig<Product, SpecificationAttributeProductModel>()
            .Ignore(d => d.SpecificationAttributeId)
            .Ignore(d => d.ProductId)
            .Ignore(d => d.ProductName);

        config.NewConfig<ProductSpecificationAttribute, ProductSpecificationAttributeModel>()
            .Ignore(d => d.AttributeTypeName)
            .Ignore(d => d.ValueRaw)
            .Ignore(d => d.AttributeId)
            .Ignore(d => d.AttributeName)
            .Ignore(d => d.SpecificationAttributeOptionId);

        config.NewConfig<ProductSpecificationAttribute, AddSpecificationAttributeModel>()
            .Ignore(d => d.SpecificationId)
            .Ignore(d => d.AttributeTypeName)
            .Ignore(d => d.AttributeId)
            .Ignore(d => d.AttributeName)
            .Ignore(d => d.ValueRaw)
            .Ignore(d => d.Value)
            .Ignore(d => d.AvailableOptions)
            .Ignore(d => d.AvailableAttributes);

        config.NewConfig<AddSpecificationAttributeModel, ProductSpecificationAttribute>()
            .Ignore(d => d.CustomValue)
            .Ignore(d => d.AttributeType);

        config.NewConfig<ProductTag, ProductTagModel>()
            .Ignore(d => d.ProductCount)
            .Ignore(d => d.ProductTagProductSearchModel);

        config.NewConfig<ProductTemplate, ProductTemplateModel>();
        config.NewConfig<ProductTemplateModel, ProductTemplate>();

        config.NewConfig<RelatedProduct, RelatedProductModel>()
            .Ignore(d => d.Product2Name);

        config.NewConfig<SpecificationAttribute, SpecificationAttributeModel>()
            .Ignore(d => d.SpecificationAttributeOptionSearchModel)
            .Ignore(d => d.SpecificationAttributeProductSearchModel)
            .Ignore(d => d.AvailableGroups);
        config.NewConfig<SpecificationAttributeModel, SpecificationAttribute>();

        config.NewConfig<SpecificationAttributeOption, SpecificationAttributeOptionModel>()
            .Ignore(d => d.EnableColorSquaresRgb)
            .Ignore(d => d.NumberOfAssociatedProducts);
        config.NewConfig<SpecificationAttributeOptionModel, SpecificationAttributeOption>();

        config.NewConfig<SpecificationAttributeGroup, SpecificationAttributeGroupModel>();
        config.NewConfig<SpecificationAttributeGroupModel, SpecificationAttributeGroup>();

        config.NewConfig<StockQuantityHistory, StockQuantityHistoryModel>()
            .Ignore(d => d.WarehouseName)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.AttributeCombination);

        config.NewConfig<TierPrice, TierPriceModel>()
            .Ignore(d => d.Store)
            .Ignore(d => d.AvailableCustomerRoles)
            .Ignore(d => d.AvailableStores)
            .Ignore(d => d.CustomerRole)
            .Ignore(d => d.FormattedPrice)
            .Ignore(d => d.PrimaryStoreCurrencyCode);
        config.NewConfig<TierPriceModel, TierPrice>()
            .Ignore(d => d.CustomerRoleId)
            .Ignore(d => d.ProductId);
    }

    /// <summary>
    /// Create CMS maps
    /// </summary>
    protected virtual void CreateCmsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<IWidgetPlugin, WidgetModel>()
            .Ignore(d => d.WidgetViewComponentArguments)
            .Ignore(d => d.WidgetViewComponentName);
    }

    /// <summary>
    /// Create common maps
    /// </summary>
    protected virtual void CreateCommonMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Address, AddressModel>()
            .Ignore(d => d.AddressHtml)
            .Ignore(d => d.AvailableCountries)
            .Ignore(d => d.AvailableStates)
            .Ignore(d => d.CountryName)
            .Ignore(d => d.CustomAddressAttributes)
            .Ignore(d => d.FormattedCustomAddressAttributes)
            .Ignore(d => d.StateProvinceName)
            .Ignore(d => d.CityRequired)
            .Ignore(d => d.CompanyRequired)
            .Ignore(d => d.CountryRequired)
            .Ignore(d => d.CountyRequired)
            .Ignore(d => d.EmailRequired)
            .Ignore(d => d.FaxRequired)
            .Ignore(d => d.FirstNameRequired)
            .Ignore(d => d.LastNameRequired)
            .Ignore(d => d.PhoneRequired)
            .Ignore(d => d.StreetAddress2Required)
            .Ignore(d => d.StreetAddressRequired)
            .Ignore(d => d.ZipPostalCodeRequired);

        config.NewConfig<AddressModel, Address>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.CustomAttributes);
        config.NewConfig<AddressAttribute, AddressAttributeModel>()
            .Ignore(d => d.AddressAttributeValueSearchModel)
            .Ignore(d => d.AttributeControlTypeName);
        config.NewConfig<AddressAttributeModel, AddressAttribute>()
            .Ignore(d => d.AttributeControlType);
        config.NewConfig<AddressAttributeValue, AddressAttributeValueModel>();
        config.NewConfig<AddressAttributeValueModel, AddressAttributeValue>();

        config.NewConfig<AddressSettings, AddressSettingsModel>()
            .Ignore(d => d.AvailableCountries);

        config.NewConfig<AddressSettingsModel, AddressSettings>()
            .Ignore(d => d.PreselectCountryIfOnlyOne)
            .Ignore(d => d.PrePopulateCountryByCustomer);

        config.NewConfig<Setting, SettingModel>()
            .Ignore(d => d.AvailableStores)
            .Ignore(d => d.Store);
    }

    /// <summary>
    /// Create customers maps
    /// </summary>
    protected virtual void CreateCustomersMaps(TypeAdapterConfig config)
    {
        config.NewConfig<CustomerAttribute, CustomerAttributeModel>()
            .Ignore(d => d.AttributeControlTypeName)
            .Ignore(d => d.CustomerAttributeValueSearchModel);
        config.NewConfig<CustomerAttributeModel, CustomerAttribute>()
            .Ignore(d => d.AttributeControlType);

        config.NewConfig<CustomerAttributeValue, CustomerAttributeValueModel>();
        config.NewConfig<CustomerAttributeValueModel, CustomerAttributeValue>();

        config.NewConfig<CustomerRole, CustomerRoleModel>()
            .Ignore(d => d.PurchasedWithProductName)
            .Ignore(d => d.TaxDisplayTypeValues);
        config.NewConfig<CustomerRoleModel, CustomerRole>();

        config.NewConfig<CustomerSettings, CustomerSettingsModel>()
            .Ignore(d => d.AvailableCountries);
        config.NewConfig<CustomerSettingsModel, CustomerSettings>()
            .Ignore(d => d.AvatarMaximumSizeBytes)
            .Ignore(d => d.DeleteGuestTaskOlderThanMinutes)
            .Ignore(d => d.DownloadableProductsValidateUser)
            .Ignore(d => d.HashedPasswordFormat)
            .Ignore(d => d.OnlineCustomerMinutes)
            .Ignore(d => d.SuffixDeletedCustomers)
            .Ignore(d => d.LastActivityMinutes)
            .Ignore(d => d.RequiredReLoginAfterPasswordChange);

        config.NewConfig<MultiFactorAuthenticationSettings, MultiFactorAuthenticationSettingsModel>();
        config.NewConfig<MultiFactorAuthenticationSettingsModel, MultiFactorAuthenticationSettings>()
            .Ignore(d => d.ActiveAuthenticationMethodSystemNames);

        config.NewConfig<RewardPointsSettings, RewardPointsSettingsModel>()
            .Ignore(d => d.ActivatePointsImmediately)
            .Ignore(d => d.ActivationDelay_OverrideForStore)
            .Ignore(d => d.DisplayHowMuchWillBeEarned_OverrideForStore)
            .Ignore(d => d.Enabled_OverrideForStore)
            .Ignore(d => d.ExchangeRate_OverrideForStore)
            .Ignore(d => d.MaximumRewardPointsToUsePerOrder_OverrideForStore)
            .Ignore(d => d.MinimumRewardPointsToUse_OverrideForStore)
            .Ignore(d => d.MinOrderTotalToAwardPoints_OverrideForStore)
            .Ignore(d => d.PageSize_OverrideForStore)
            .Ignore(d => d.PointsForPurchases_OverrideForStore)
            .Ignore(d => d.MaximumRedeemedRate_OverrideForStore)
            .Ignore(d => d.PointsForRegistration_OverrideForStore)
            .Ignore(d => d.PrimaryStoreCurrencyCode)
            .Ignore(d => d.PurchasesPointsValidity_OverrideForStore)
            .Ignore(d => d.RegistrationPointsValidity_OverrideForStore);
        config.NewConfig<RewardPointsSettingsModel, RewardPointsSettings>();

        config.NewConfig<RewardPointsHistory, CustomerRewardPointsModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.PointsBalance)
            .Ignore(d => d.EndDate)
            .Ignore(d => d.StoreName);

        config.NewConfig<ActivityLog, CustomerActivityLogModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.ActivityLogTypeName);

        config.NewConfig<Customer, CustomerModel>()
            .Ignore(d => d.Email)
            .Ignore(d => d.FullName)
            .Ignore(d => d.Company)
            .Ignore(d => d.Phone)
            .Ignore(d => d.ZipPostalCode)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.LastActivityDate)
            .Ignore(d => d.CustomerRoleNames)
            .Ignore(d => d.AvatarUrl)
            .Ignore(d => d.UsernamesEnabled)
            .Ignore(d => d.Password)
            .Ignore(d => d.AvailableVendors)
            .Ignore(d => d.GenderEnabled)
            .Ignore(d => d.NeutralGenderEnabled)
            .Ignore(d => d.Gender)
            .Ignore(d => d.FirstNameEnabled)
            .Ignore(d => d.FirstName)
            .Ignore(d => d.LastNameEnabled)
            .Ignore(d => d.LastName)
            .Ignore(d => d.DateOfBirthEnabled)
            .Ignore(d => d.DateOfBirth)
            .Ignore(d => d.CompanyEnabled)
            .Ignore(d => d.StreetAddressEnabled)
            .Ignore(d => d.StreetAddress)
            .Ignore(d => d.StreetAddress2Enabled)
            .Ignore(d => d.StreetAddress2)
            .Ignore(d => d.ZipPostalCodeEnabled)
            .Ignore(d => d.CityEnabled)
            .Ignore(d => d.City)
            .Ignore(d => d.CountyEnabled)
            .Ignore(d => d.County)
            .Ignore(d => d.CountryEnabled)
            .Ignore(d => d.CountryId)
            .Ignore(d => d.AvailableCountries)
            .Ignore(d => d.StateProvinceEnabled)
            .Ignore(d => d.StateProvinceId)
            .Ignore(d => d.AvailableStates)
            .Ignore(d => d.PhoneEnabled)
            .Ignore(d => d.FaxEnabled)
            .Ignore(d => d.Fax)
            .Ignore(d => d.CustomerAttributes)
            .Ignore(d => d.RegisteredInStore)
            .Ignore(d => d.DisplayRegisteredInStore)
            .Ignore(d => d.AffiliateName)
            .Ignore(d => d.TimeZoneId)
            .Ignore(d => d.AllowCustomersToSetTimeZone)
            .Ignore(d => d.AvailableTimeZones)
            .Ignore(d => d.VatNumber)
            .Ignore(d => d.VatNumberStatusNote)
            .Ignore(d => d.DisplayVatNumber)
            .Ignore(d => d.LastVisitedPage)
            .Ignore(d => d.DisplayRewardPointsHistory)
            .Ignore(d => d.AddRewardPoints)
            .Ignore(d => d.CustomerRewardPointsSearchModel)
            .Ignore(d => d.SendEmail)
            .Ignore(d => d.SendPm)
            .Ignore(d => d.AllowSendingOfPrivateMessage)
            .Ignore(d => d.AllowSendingOfWelcomeMessage)
            .Ignore(d => d.AllowReSendingOfActivationMessage)
            .Ignore(d => d.GdprEnabled)
            .Ignore(d => d.MultiFactorAuthenticationProvider)
            .Ignore(d => d.CustomerAssociatedExternalAuthRecordsSearchModel)
            .Ignore(d => d.CustomerAddressSearchModel)
            .Ignore(d => d.CustomerOrderSearchModel)
            .Ignore(d => d.CustomerShoppingCartSearchModel)
            .Ignore(d => d.CustomerActivityLogSearchModel)
            .Ignore(d => d.CustomerBackInStockSubscriptionSearchModel);

        config.NewConfig<CustomerModel, Customer>()
            .Ignore(d => d.CustomerGuid)
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.LastActivityDateUtc)
            .Ignore(d => d.EmailToRevalidate)
            .Ignore(d => d.HasShoppingCartItems)
            .Ignore(d => d.RequireReLogin)
            .Ignore(d => d.FailedLoginAttempts)
            .Ignore(d => d.CannotLoginUntilDateUtc)
            .Ignore(d => d.Deleted)
            .Ignore(d => d.IsSystemAccount)
            .Ignore(d => d.SystemName)
            .Ignore(d => d.LastLoginDateUtc)
            .Ignore(d => d.BillingAddressId)
            .Ignore(d => d.ShippingAddressId)
            .Ignore(d => d.VatNumberStatusId)
            .Ignore(d => d.CustomCustomerAttributesXML)
            .Ignore(d => d.CurrencyId)
            .Ignore(d => d.LanguageId)
            .Ignore(d => d.TaxDisplayTypeId)
            .Ignore(d => d.VatNumberStatus)
            .Ignore(d => d.TaxDisplayType)
            .Ignore(d => d.RegisteredInStoreId);

        config.NewConfig<Customer, OnlineCustomerModel>()
            .Ignore(d => d.LastActivityDate)
            .Ignore(d => d.CustomerInfo)
            .Ignore(d => d.LastIpAddress)
            .Ignore(d => d.Location)
            .Ignore(d => d.LastVisitedPage);

        config.NewConfig<BackInStockSubscription, CustomerBackInStockSubscriptionModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.StoreName)
            .Ignore(d => d.ProductName);
    }

    /// <summary>
    /// Create directory maps 
    /// </summary>
    protected virtual void CreateDirectoryMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Country, CountryModel>()
            .Ignore(d => d.NumberOfStates)
            .Ignore(d => d.StateProvinceSearchModel);
        config.NewConfig<CountryModel, Country>();

        config.NewConfig<Currency, CurrencyModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.IsPrimaryExchangeRateCurrency)
            .Ignore(d => d.IsPrimaryStoreCurrency);
        config.NewConfig<CurrencyModel, Currency>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.RoundingType)
            .Ignore(d => d.UpdatedOnUtc);

        config.NewConfig<MeasureDimension, MeasureDimensionModel>()
            .Ignore(d => d.IsPrimaryDimension);
        config.NewConfig<MeasureDimensionModel, MeasureDimension>();

        config.NewConfig<MeasureWeight, MeasureWeightModel>()
            .Ignore(d => d.IsPrimaryWeight);
        config.NewConfig<MeasureWeightModel, MeasureWeight>();

        config.NewConfig<StateProvince, StateProvinceModel>();
        config.NewConfig<StateProvinceModel, StateProvince>();
    }

    /// <summary>
    /// Create discounts maps
    /// </summary>
    protected virtual void CreateDiscountsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Discount, DiscountModel>()
            .Ignore(d => d.AddDiscountRequirement)
            .Ignore(d => d.AvailableDiscountRequirementRules)
            .Ignore(d => d.AvailableRequirementGroups)
            .Ignore(d => d.AvailableVendors)
            .Ignore(d => d.DiscountCategorySearchModel)
            .Ignore(d => d.DiscountManufacturerSearchModel)
            .Ignore(d => d.DiscountProductSearchModel)
            .Ignore(d => d.DiscountTypeName)
            .Ignore(d => d.DiscountUrl)
            .Ignore(d => d.DiscountUsageHistorySearchModel)
            .Ignore(d => d.GroupName)
            .Ignore(d => d.PrimaryStoreCurrencyCode)
            .Ignore(d => d.RequirementGroupId)
            .Ignore(d => d.TimesUsed)
            .Ignore(d => d.IsLoggedInAsVendor);

        config.NewConfig<DiscountModel, Discount>()
            .Ignore(d => d.DiscountLimitation)
            .Ignore(d => d.DiscountType);

        config.NewConfig<DiscountUsageHistory, DiscountUsageHistoryModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.OrderTotal)
            .Ignore(d => d.CustomOrderNumber);

        config.NewConfig<Category, DiscountCategoryModel>()
            .Ignore(d => d.CategoryName)
            .Ignore(d => d.CategoryId);

        config.NewConfig<Manufacturer, DiscountManufacturerModel>()
            .Ignore(d => d.ManufacturerId)
            .Ignore(d => d.ManufacturerName);
    }

    /// <summary>
    /// Create forums maps
    /// </summary>
    protected virtual void CreateForumsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Forum, ForumModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.ForumGroups);
        config.NewConfig<ForumModel, Forum>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.LastPostCustomerId)
            .Ignore(d => d.LastPostId)
            .Ignore(d => d.LastPostTime)
            .Ignore(d => d.LastTopicId)
            .Ignore(d => d.NumPosts)
            .Ignore(d => d.NumTopics)
            .Ignore(d => d.UpdatedOnUtc);

        config.NewConfig<ForumGroup, ForumGroupModel>()
            .Ignore(d => d.CreatedOn);
        config.NewConfig<ForumGroupModel, ForumGroup>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.UpdatedOnUtc);

        config.NewConfig<ForumSettings, ForumSettingsModel>()
            .Ignore(d => d.ActiveDiscussionsFeedCount_OverrideForStore)
            .Ignore(d => d.ActiveDiscussionsFeedEnabled_OverrideForStore)
            .Ignore(d => d.ActiveDiscussionsPageSize_OverrideForStore)
            .Ignore(d => d.AllowCustomersToDeletePosts_OverrideForStore)
            .Ignore(d => d.AllowCustomersToEditPosts_OverrideForStore)
            .Ignore(d => d.AllowCustomersToManageSubscriptions_OverrideForStore)
            .Ignore(d => d.AllowGuestsToCreatePosts_OverrideForStore)
            .Ignore(d => d.AllowGuestsToCreateTopics_OverrideForStore)
            .Ignore(d => d.AllowPostVoting_OverrideForStore)
            .Ignore(d => d.AllowPrivateMessages_OverrideForStore)
            .Ignore(d => d.ForumEditorValues)
            .Ignore(d => d.ForumEditor_OverrideForStore)
            .Ignore(d => d.ForumFeedCount_OverrideForStore)
            .Ignore(d => d.ForumFeedsEnabled_OverrideForStore)
            .Ignore(d => d.ForumsEnabled_OverrideForStore)
            .Ignore(d => d.MaxVotesPerDay_OverrideForStore)
            .Ignore(d => d.NotifyAboutPrivateMessages_OverrideForStore)
            .Ignore(d => d.PostsPageSize_OverrideForStore)
            .Ignore(d => d.RelativeDateTimeFormattingEnabled_OverrideForStore)
            .Ignore(d => d.SearchResultsPageSize_OverrideForStore)
            .Ignore(d => d.ShowAlertForPM_OverrideForStore)
            .Ignore(d => d.ShowCustomersPostCount_OverrideForStore)
            .Ignore(d => d.SignaturesEnabled_OverrideForStore)
            .Ignore(d => d.TopicsPageSize_OverrideForStore);
        config.NewConfig<ForumSettingsModel, ForumSettings>()
            .Ignore(d => d.ForumSearchTermMinimumLength)
            .Ignore(d => d.ForumSubscriptionsPageSize)
            .Ignore(d => d.HomepageActiveDiscussionsTopicCount)
            .Ignore(d => d.LatestCustomerPostsPageSize)
            .Ignore(d => d.PMSubjectMaxLength)
            .Ignore(d => d.PMTextMaxLength)
            .Ignore(d => d.PostMaxLength)
            .Ignore(d => d.PrivateMessagesPageSize)
            .Ignore(d => d.StrippedTopicMaxLength)
            .Ignore(d => d.TopicSubjectMaxLength)
            .Ignore(d => d.TopicMetaDescriptionLength);
    }

    /// <summary>
    /// Create GDPR maps 
    /// </summary>
    protected virtual void CreateGdprMaps(TypeAdapterConfig config)
    {
        config.NewConfig<GdprSettings, GdprSettingsModel>()
            .Ignore(d => d.GdprConsentSearchModel)
            .Ignore(d => d.GdprEnabled_OverrideForStore)
            .Ignore(d => d.LogNewsLetterConsent_OverrideForStore)
            .Ignore(d => d.LogPrivacyPolicyConsent_OverrideForStore)
            .Ignore(d => d.LogUserProfileChanges_OverrideForStore)
            .Ignore(d => d.DeleteInactiveCustomersAfterMonths_OverrideForStore);
        config.NewConfig<GdprSettingsModel, GdprSettings>();

        config.NewConfig<GdprConsent, GdprConsentModel>();
        config.NewConfig<GdprConsentModel, GdprConsent>();

        config.NewConfig<GdprLog, GdprLogModel>()
            .Ignore(d => d.CustomerInfo)
            .Ignore(d => d.RequestType)
            .Ignore(d => d.CreatedOn);
    }

    /// <summary>
    /// Create localization maps 
    /// </summary>
    protected virtual void CreateLocalizationMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Language, LanguageModel>()
            .Ignore(d => d.AvailableCurrencies)
            .Ignore(d => d.LocaleResourceSearchModel)
            .Ignore(d => d.AvailableFlagImages);
        config.NewConfig<LanguageModel, Language>();

        config.NewConfig<LocaleResourceModel, LocaleStringResource>()
            .Ignore(d => d.LanguageId);
    }

    /// <summary>
    /// Create logging maps
    /// </summary>
    protected virtual void CreateLoggingMaps(TypeAdapterConfig config)
    {
        config.NewConfig<ActivityLog, ActivityLogModel>()
            .Ignore(d => d.ActivityLogTypeName)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.CustomerEmail);
        config.NewConfig<ActivityLogModel, ActivityLog>()
            .Ignore(d => d.ActivityLogTypeId)
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.EntityId)
            .Ignore(d => d.EntityName);

        config.NewConfig<ActivityLogType, ActivityLogTypeModel>();
        config.NewConfig<ActivityLogTypeModel, ActivityLogType>()
            .Ignore(d => d.SystemKeyword);

        config.NewConfig<Log, LogModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.FullMessage)
            .Ignore(d => d.CustomerEmail);
        config.NewConfig<LogModel, Log>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.LogLevelId);
    }

    /// <summary>
    /// Create media maps
    /// </summary>
    protected virtual void CreateMediaMaps(TypeAdapterConfig config)
    {
        config.NewConfig<MediaSettings, MediaSettingsModel>()
            .Ignore(d => d.AssociatedProductPictureSize_OverrideForStore)
            .Ignore(d => d.AvatarPictureSize_OverrideForStore)
            .Ignore(d => d.CartThumbPictureSize_OverrideForStore)
            .Ignore(d => d.OrderThumbPictureSize_OverrideForStore)
            .Ignore(d => d.CategoryThumbPictureSize_OverrideForStore)
            .Ignore(d => d.DefaultImageQuality_OverrideForStore)
            .Ignore(d => d.DefaultPictureZoomEnabled_OverrideForStore)
            .Ignore(d => d.ImportProductImagesUsingHash_OverrideForStore)
            .Ignore(d => d.ManufacturerThumbPictureSize_OverrideForStore)
            .Ignore(d => d.MaximumImageSize_OverrideForStore)
            .Ignore(d => d.MiniCartThumbPictureSize_OverrideForStore)
            .Ignore(d => d.MultipleThumbDirectories_OverrideForStore)
            .Ignore(d => d.PicturesStoredIntoDatabase)
            .Ignore(d => d.ProductDetailsPictureSize_OverrideForStore)
            .Ignore(d => d.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore)
            .Ignore(d => d.ProductThumbPictureSize_OverrideForStore)
            .Ignore(d => d.VendorThumbPictureSize_OverrideForStore)
            .Ignore(d => d.ProductDefaultImageId_OverrideForStore)
            .Ignore(d => d.AllowSvgUploads_OverrideForStore);
        config.NewConfig<MediaSettingsModel, MediaSettings>()
            .Ignore(d => d.AutoCompleteSearchThumbPictureSize)
            .Ignore(d => d.UseAbsoluteImagePath)
            .Ignore(d => d.AutoOrientImage)
            .Ignore(d => d.ImageSquarePictureSize)
            .Ignore(d => d.VideoIframeAllow)
            .Ignore(d => d.VideoIframeHeight)
            .Ignore(d => d.VideoIframeWidth)
            .Ignore(d => d.PicturePath);
    }

    /// <summary>
    /// Create messages maps
    /// </summary>
    protected virtual void CreateMessagesMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Campaign, CampaignModel>()
            .Ignore(d => d.AllowedTokens)
            .Ignore(d => d.AvailableCustomerRoles)
            .Ignore(d => d.AvailableNewsLetterSubscriptionTypes)
            .Ignore(d => d.AvailableEmailAccounts)
            .Ignore(d => d.AvailableStores)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.DontSendBeforeDate)
            .Ignore(d => d.EmailAccountId)
            .Ignore(d => d.TestEmail)
            .Ignore(d => d.CopyCampaignModel);
        config.NewConfig<CampaignModel, Campaign>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.DontSendBeforeDateUtc);
        config.NewConfig<EmailAccount, EmailAccountModel>()
            .Ignore(d => d.IsDefaultEmailAccount)
            .Ignore(d => d.SendTestEmailTo)
            .Ignore(d => d.AvailableEmailAuthenticationMethods)
            .Ignore(d => d.AuthUrl);
        config.NewConfig<EmailAccountModel, EmailAccount>()
            .Ignore(d => d.Password)
            .Ignore(d => d.ClientSecret)
            .Ignore(d => d.EmailAuthenticationMethodId);

        config.NewConfig<MessageTemplate, MessageTemplateModel>()
            .Ignore(d => d.AllowedTokens)
            .Ignore(d => d.AvailableEmailAccounts)
            .Ignore(d => d.HasAttachedDownload)
            .Ignore(d => d.ListOfStores)
            .Ignore(d => d.SendImmediately);
        config.NewConfig<MessageTemplateModel, MessageTemplate>()
            .Ignore(d => d.DelayPeriod);

        config.NewConfig<NewsLetterSubscription, NewsLetterSubscriptionModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.SubscriptionTypeName)
            .Ignore(d => d.AvailableNewsLetterSubscriptionTypes)
            .Ignore(d => d.AvailableNewsLetterSubscriptionStores)
            .Ignore(d => d.AvailableNewsLetterSubscriptionLanguages)
            .Ignore(d => d.SelectedNewsLetterSubscriptionTypeId)
            .Ignore(d => d.SelectedNewsLetterSubscriptionStoreId)
            .Ignore(d => d.SelectedNewsLetterSubscriptionLanguageId)
            .Ignore(d => d.LanguageName)
            .Ignore(d => d.StoreName);
        config.NewConfig<NewsLetterSubscriptionModel, NewsLetterSubscription>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.TypeId)
            .Ignore(d => d.NewsLetterSubscriptionGuid)
            .Ignore(d => d.LanguageId)
            .Ignore(d => d.StoreId);

        //Newsletter subscription type
        config.NewConfig<NewsLetterSubscriptionType, NewsLetterSubscriptionTypeModel>();
        config.NewConfig<NewsLetterSubscriptionTypeModel, NewsLetterSubscriptionType>();

        config.NewConfig<QueuedEmail, QueuedEmailModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.DontSendBeforeDate)
            .Ignore(d => d.EmailAccountName)
            .Ignore(d => d.PriorityName)
            .Ignore(d => d.SendImmediately)
            .Ignore(d => d.SentOn);
        config.NewConfig<QueuedEmailModel, QueuedEmail>()
            .Ignore(d => d.AttachmentFileName)
            .Ignore(d => d.AttachmentFilePath)
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.DontSendBeforeDateUtc)
            .Ignore(d => d.EmailAccountId)
            .Ignore(d => d.Priority)
            .Ignore(d => d.PriorityId)
            .Ignore(d => d.SentOnUtc);
    }

    /// <summary>
    /// Create news maps
    /// </summary>
    protected virtual void CreateNewsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<NewsComment, NewsCommentModel>()
            .Ignore(d => d.CustomerInfo)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.CommentText)
            .Ignore(d => d.NewsItemTitle)
            .Ignore(d => d.StoreName);
        config.NewConfig<NewsCommentModel, NewsComment>()
            .Ignore(d => d.CommentTitle)
            .Ignore(d => d.CommentText)
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.NewsItemId)
            .Ignore(d => d.CustomerId)
            .Ignore(d => d.StoreId);

        config.NewConfig<NewsItem, NewsItemModel>()
            .Ignore(d => d.ApprovedComments)
            .Ignore(d => d.AvailableLanguages)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.LanguageName)
            .Ignore(d => d.NotApprovedComments)
            .Ignore(d => d.SeName);
        config.NewConfig<NewsItemModel, NewsItem>()
            .Ignore(d => d.CreatedOnUtc);

        config.NewConfig<NewsSettings, NewsSettingsModel>()
            .Ignore(d => d.AllowNotRegisteredUsersToLeaveComments_OverrideForStore)
            .Ignore(d => d.Enabled_OverrideForStore)
            .Ignore(d => d.MainPageNewsCount_OverrideForStore)
            .Ignore(d => d.NewsArchivePageSize_OverrideForStore)
            .Ignore(d => d.NewsCommentsMustBeApproved_OverrideForStore)
            .Ignore(d => d.NotifyAboutNewNewsComments_OverrideForStore)
            .Ignore(d => d.ShowHeaderRssUrl_OverrideForStore)
            .Ignore(d => d.ShowNewsOnMainPage_OverrideForStore);
        config.NewConfig<NewsSettingsModel, NewsSettings>();
    }

    /// <summary>
    /// Create orders maps 
    /// </summary>
    protected virtual void CreateOrdersMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Order, CustomerOrderModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.OrderStatus)
            .Ignore(d => d.PaymentStatus)
            .Ignore(d => d.ShippingStatus)
            .Ignore(d => d.OrderTotal)
            .Ignore(d => d.StoreName);

        config.NewConfig<OrderNote, OrderNoteModel>()
            .Ignore(d => d.DownloadGuid)
            .Ignore(d => d.CreatedOn);

        config.NewConfig<CheckoutAttribute, CheckoutAttributeModel>()
            .Ignore(d => d.AttributeControlTypeName)
            .Ignore(d => d.AvailableTaxCategories)
            .Ignore(d => d.CheckoutAttributeValueSearchModel)
            .Ignore(d => d.ConditionAllowed)
            .Ignore(d => d.ConditionModel);
        config.NewConfig<CheckoutAttributeModel, CheckoutAttribute>()
            .Ignore(d => d.AttributeControlType)
            .Ignore(d => d.ConditionAttributeXml);

        config.NewConfig<CheckoutAttributeValue, CheckoutAttributeValueModel>()
            .Ignore(d => d.BaseWeightIn)
            .Ignore(d => d.DisplayColorSquaresRgb)
            .Ignore(d => d.PrimaryStoreCurrencyCode);
        config.NewConfig<CheckoutAttributeValueModel, CheckoutAttributeValue>();

        config.NewConfig<GiftCard, GiftCardModel>()
            .Ignore(d => d.AmountStr)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.GiftCardUsageHistorySearchModel)
            .Ignore(d => d.PrimaryStoreCurrencyCode)
            .Ignore(d => d.PurchasedWithOrderId)
            .Ignore(d => d.PurchasedWithOrderNumber)
            .Ignore(d => d.RemainingAmountStr);
        config.NewConfig<GiftCardModel, GiftCard>()
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.GiftCardType)
            .Ignore(d => d.IsRecipientNotified)
            .Ignore(d => d.PurchasedWithOrderItemId);

        config.NewConfig<GiftCardUsageHistory, GiftCardUsageHistoryModel>()
            .Ignore(d => d.OrderId)
            .Ignore(d => d.CustomOrderNumber)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.UsedValue);

        config.NewConfig<OrderSettings, OrderSettingsModel>()
            .Ignore(d => d.AllowAdminsToBuyCallForPriceProducts_OverrideForStore)
            .Ignore(d => d.ShowProductThumbnailInOrderDetailsPage_OverrideForStore)
            .Ignore(d => d.AnonymousCheckoutAllowed_OverrideForStore)
            .Ignore(d => d.AttachPdfInvoiceToOrderProcessingEmail_OverrideForStore)
            .Ignore(d => d.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore)
            .Ignore(d => d.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore)
            .Ignore(d => d.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore)
            .Ignore(d => d.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore)
            .Ignore(d => d.CheckoutDisabled_OverrideForStore)
            .Ignore(d => d.CustomOrderNumberMask_OverrideForStore)
            .Ignore(d => d.DeleteGiftCardUsageHistory_OverrideForStore)
            .Ignore(d => d.DisableBillingAddressCheckoutStep_OverrideForStore)
            .Ignore(d => d.DisableOrderCompletedPage_OverrideForStore)
            .Ignore(d => d.DisplayPickupInStoreOnShippingMethodPage_OverrideForStore)
            .Ignore(d => d.ExportWithProducts_OverrideForStore)
            .Ignore(d => d.IsReOrderAllowed_OverrideForStore)
            .Ignore(d => d.MinOrderSubtotalAmountIncludingTax_OverrideForStore)
            .Ignore(d => d.MinOrderSubtotalAmount_OverrideForStore)
            .Ignore(d => d.MinOrderTotalAmount_OverrideForStore)
            .Ignore(d => d.NumberOfDaysReturnRequestAvailable_OverrideForStore)
            .Ignore(d => d.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore)
            .Ignore(d => d.OnePageCheckoutEnabled_OverrideForStore)
            .Ignore(d => d.OrderIdent)
            .Ignore(d => d.PrimaryStoreCurrencyCode)
            .Ignore(d => d.ReturnRequestActionSearchModel)
            .Ignore(d => d.ReturnRequestNumberMask_OverrideForStore)
            .Ignore(d => d.ReturnRequestReasonSearchModel)
            .Ignore(d => d.ReturnRequestsAllowFiles_OverrideForStore)
            .Ignore(d => d.ReturnRequestsEnabled_OverrideForStore)
            .Ignore(d => d.TermsOfServiceOnOrderConfirmPage_OverrideForStore)
            .Ignore(d => d.TermsOfServiceOnShoppingCartPage_OverrideForStore);
        config.NewConfig<OrderSettingsModel, OrderSettings>()
            .Ignore(d => d.GeneratePdfInvoiceInCustomerLanguage)
            .Ignore(d => d.MinimumOrderPlacementInterval)
            .Ignore(d => d.DisplayCustomerCurrencyOnOrders)
            .Ignore(d => d.ReturnRequestsFileMaximumSize)
            .Ignore(d => d.DisplayOrderSummary)
            .Ignore(d => d.PlaceOrderWithLock)
            .Ignore(d => d.CustomerOrdersPageSize);

        config.NewConfig<ReturnRequestAction, ReturnRequestActionModel>();
        config.NewConfig<ReturnRequestActionModel, ReturnRequestAction>();

        config.NewConfig<ReturnRequestReason, ReturnRequestReasonModel>();
        config.NewConfig<ReturnRequestReasonModel, ReturnRequestReason>();

        config.NewConfig<ReturnRequest, ReturnRequestModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.CustomerInfo)
            .Ignore(d => d.ReturnRequestStatusStr)
            .Ignore(d => d.ProductId)
            .Ignore(d => d.ProductName)
            .Ignore(d => d.OrderId)
            .Ignore(d => d.AttributeInfo)
            .Ignore(d => d.CustomOrderNumber)
            .Ignore(d => d.UploadedFileGuid);
        config.NewConfig<ReturnRequestModel, ReturnRequest>()
            .Ignore(d => d.CustomNumber)
            .Ignore(d => d.StoreId)
            .Ignore(d => d.OrderItemId)
            .Ignore(d => d.UploadedFileId)
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.ReturnRequestStatus)
            .Ignore(d => d.CustomerId)
            .Ignore(d => d.UpdatedOnUtc);

        config.NewConfig<ShoppingCartSettings, ShoppingCartSettingsModel>()
            .Ignore(d => d.AllowAnonymousUsersToEmailWishlist_OverrideForStore)
            .Ignore(d => d.AllowCartItemEditing_OverrideForStore)
            .Ignore(d => d.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore)
            .Ignore(d => d.AllowMultipleWishlist_OverrideForStore)
            .Ignore(d => d.MaximumNumberOfCustomWishlist_OverrideForStore)
            .Ignore(d => d.CartsSharedBetweenStores_OverrideForStore)
            .Ignore(d => d.CrossSellsNumber_OverrideForStore)
            .Ignore(d => d.GroupTierPricesForDistinctShoppingCartItems_OverrideForStore)
            .Ignore(d => d.DisplayCartAfterAddingProduct_OverrideForStore)
            .Ignore(d => d.DisplayWishlistAfterAddingProduct_OverrideForStore)
            .Ignore(d => d.EmailWishlistEnabled_OverrideForStore)
            .Ignore(d => d.MaximumShoppingCartItems_OverrideForStore)
            .Ignore(d => d.MaximumWishlistItems_OverrideForStore)
            .Ignore(d => d.MiniShoppingCartEnabled_OverrideForStore)
            .Ignore(d => d.MiniShoppingCartProductNumber_OverrideForStore)
            .Ignore(d => d.MoveItemsFromWishlistToCart_OverrideForStore)
            .Ignore(d => d.ShowDiscountBox_OverrideForStore)
            .Ignore(d => d.ShowGiftCardBox_OverrideForStore)
            .Ignore(d => d.ShowProductImagesInMiniShoppingCart_OverrideForStore)
            .Ignore(d => d.ShowProductImagesOnShoppingCart_OverrideForStore)
            .Ignore(d => d.ShowProductImagesOnWishList_OverrideForStore);
        config.NewConfig<ShoppingCartSettingsModel, ShoppingCartSettings>()
            .Ignore(d => d.RenderAssociatedAttributeValueQuantity)
            .Ignore(d => d.RoundPricesDuringCalculation);

        config.NewConfig<ShoppingCartItem, ShoppingCartItemModel>()
            .Ignore(d => d.Store)
            .Ignore(d => d.CustomWishlistName)
            .Ignore(d => d.AttributeInfo)
            .Ignore(d => d.UnitPrice)
            .Ignore(d => d.UnitPriceValue)
            .Ignore(d => d.UpdatedOn)
            .Ignore(d => d.ProductName)
            .Ignore(d => d.Total)
            .Ignore(d => d.TotalValue);
    }

    /// <summary>
    /// Create payments maps 
    /// </summary>
    protected virtual void CreatePaymentsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<IPaymentMethod, PaymentMethodModel>()
            .Ignore(d => d.RecurringPaymentType);

        config.NewConfig<RecurringPayment, RecurringPaymentModel>()
            .Ignore(d => d.CustomerId)
            .Ignore(d => d.InitialOrderId)
            .Ignore(d => d.NextPaymentDate)
            .Ignore(d => d.StartDate)
            .Ignore(d => d.CyclePeriodStr)
            .Ignore(d => d.PaymentType)
            .Ignore(d => d.CanCancelRecurringPayment)
            .Ignore(d => d.CustomerEmail)
            .Ignore(d => d.RecurringPaymentHistorySearchModel)
            .Ignore(d => d.CyclesRemaining);

        config.NewConfig<RecurringPaymentModel, RecurringPayment>()
            .Ignore(d => d.StartDateUtc)
            .Ignore(d => d.Deleted)
            .Ignore(d => d.CreatedOnUtc)
            .Ignore(d => d.CyclePeriod)
            .Ignore(d => d.InitialOrderId);

        config.NewConfig<RecurringPaymentHistory, RecurringPaymentHistoryModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.OrderStatus)
            .Ignore(d => d.PaymentStatus)
            .Ignore(d => d.ShippingStatus)
            .Ignore(d => d.CustomOrderNumber);
    }

    /// <summary>
    /// Create plugins maps 
    /// </summary>
    protected virtual void CreatePluginsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<PluginDescriptor, PluginModel>()
            .Ignore(d => d.CanChangeEnabled)
            .Ignore(d => d.IsEnabled);
    }
    /// <summary>
    /// Create polls maps 
    /// </summary>
    protected virtual void CreatePollsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<PollAnswer, PollAnswerModel>();
        config.NewConfig<PollAnswerModel, PollAnswer>();

        config.NewConfig<Poll, PollModel>()
            .Ignore(d => d.AvailableLanguages)
            .Ignore(d => d.PollAnswerSearchModel)
            .Ignore(d => d.LanguageName);
        config.NewConfig<PollModel, Poll>();
    }

    /// <summary>
    /// Create security maps 
    /// </summary>
    protected virtual void CreateSecurityMaps(TypeAdapterConfig config)
    {
        config.NewConfig<CaptchaSettings, CaptchaSettingsModel>()
            .Ignore(d => d.Enabled_OverrideForStore)
            .Ignore(d => d.ReCaptchaPrivateKey_OverrideForStore)
            .Ignore(d => d.ReCaptchaPublicKey_OverrideForStore)
            .Ignore(d => d.ShowOnApplyVendorPage_OverrideForStore)
            .Ignore(d => d.ShowOnBlogCommentPage_OverrideForStore)
            .Ignore(d => d.ShowOnContactUsPage_OverrideForStore)
            .Ignore(d => d.ShowOnEmailProductToFriendPage_OverrideForStore)
            .Ignore(d => d.ShowOnEmailWishlistToFriendPage_OverrideForStore)
            .Ignore(d => d.ShowOnLoginPage_OverrideForStore)
            .Ignore(d => d.ShowOnNewsCommentPage_OverrideForStore)
            .Ignore(d => d.ShowOnNewsLetterPage_OverrideForStore)
            .Ignore(d => d.ShowOnProductReviewPage_OverrideForStore)
            .Ignore(d => d.ShowOnRegistrationPage_OverrideForStore)
            .Ignore(d => d.ShowOnForgotPasswordPage_OverrideForStore)
            .Ignore(d => d.ShowOnForum_OverrideForStore)
            .Ignore(d => d.ShowOnCheckoutPageForGuests_OverrideForStore)
            .Ignore(d => d.ShowOnCheckGiftCardBalance_OverrideForStore)
            .Ignore(d => d.CaptchaType_OverrideForStore)
            .Ignore(d => d.ReCaptchaV3ScoreThreshold_OverrideForStore)
            .Ignore(d => d.CaptchaTypeValues);
        config.NewConfig<CaptchaSettingsModel, CaptchaSettings>()
            .Ignore(d => d.AutomaticallyChooseLanguage)
            .Ignore(d => d.ReCaptchaDefaultLanguage)
            .Ignore(d => d.ReCaptchaRequestTimeout)
            .Ignore(d => d.ReCaptchaTheme)
            .Ignore(d => d.ReCaptchaApiUrl);
    }

    /// <summary>
    /// Create SEO maps 
    /// </summary>
    protected virtual void CreateSeoMaps(TypeAdapterConfig config)
    {
        config.NewConfig<UrlRecord, UrlRecordModel>()
            .Ignore(d => d.DetailsUrl)
            .Ignore(d => d.Language)
            .Ignore(d => d.Name);

        config.NewConfig<UrlRecordModel, UrlRecord>()
            .Ignore(d => d.LanguageId)
            .Ignore(d => d.Slug);
    }

    /// <summary>
    /// Create shipping maps 
    /// </summary>
    protected virtual void CreateShippingMaps(TypeAdapterConfig config)
    {
        config.NewConfig<DeliveryDate, DeliveryDateModel>();
        config.NewConfig<DeliveryDateModel, DeliveryDate>();

        config.NewConfig<IPickupPointProvider, PickupPointProviderModel>();
        config.NewConfig<IShippingRateComputationMethod, ShippingProviderModel>();

        config.NewConfig<ProductAvailabilityRange, ProductAvailabilityRangeModel>();
        config.NewConfig<ProductAvailabilityRangeModel, ProductAvailabilityRange>();

        config.NewConfig<ShippingMethod, ShippingMethodModel>();
        config.NewConfig<ShippingMethodModel, ShippingMethod>();

        config.NewConfig<Shipment, ShipmentModel>()
            .Ignore(d => d.ShippedDate)
            .Ignore(d => d.ReadyForPickupDate)
            .Ignore(d => d.DeliveryDate)
            .Ignore(d => d.TotalWeight)
            .Ignore(d => d.TrackingNumberUrl)
            .Ignore(d => d.Items)
            .Ignore(d => d.ShipmentStatusEvents)
            .Ignore(d => d.PickupInStore)
            .Ignore(d => d.CanShip)
            .Ignore(d => d.CanMarkAsReadyForPickup)
            .Ignore(d => d.CanDeliver)
            .Ignore(d => d.CustomOrderNumber);

        config.NewConfig<ShippingSettings, ShippingSettingsModel>()
            .Ignore(d => d.AllowPickupInStore_OverrideForStore)
            .Ignore(d => d.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore)
            .Ignore(d => d.ConsiderAssociatedProductsDimensions_OverrideForStore)
            .Ignore(d => d.SortShippingValues)
            .Ignore(d => d.ShippingSorting_OverrideForStore)
            .Ignore(d => d.DisplayPickupPointsOnMap_OverrideForStore)
            .Ignore(d => d.IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore)
            .Ignore(d => d.DisplayShipmentEventsToCustomers_OverrideForStore)
            .Ignore(d => d.DisplayShipmentEventsToStoreOwner_OverrideForStore)
            .Ignore(d => d.EstimateShippingCartPageEnabled_OverrideForStore)
            .Ignore(d => d.EstimateShippingProductPageEnabled_OverrideForStore)
            .Ignore(d => d.EstimateShippingCityNameEnabled_OverrideForStore)
            .Ignore(d => d.FreeShippingOverXEnabled_OverrideForStore)
            .Ignore(d => d.FreeShippingOverXIncludingTax_OverrideForStore)
            .Ignore(d => d.FreeShippingOverXValue_OverrideForStore)
            .Ignore(d => d.GoogleMapsApiKey_OverrideForStore)
            .Ignore(d => d.HideShippingTotal_OverrideForStore)
            .Ignore(d => d.NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore)
            .Ignore(d => d.PrimaryStoreCurrencyCode)
            .Ignore(d => d.ShippingOriginAddress)
            .Ignore(d => d.ShippingOriginAddress_OverrideForStore)
            .Ignore(d => d.ShipToSameAddress_OverrideForStore)
            .Ignore(d => d.UseWarehouseLocation_OverrideForStore);

        config.NewConfig<ShippingSettingsModel, ShippingSettings>()
            .Ignore(d => d.ActivePickupPointProviderSystemNames)
            .Ignore(d => d.ActiveShippingRateComputationMethodSystemNames)
            .Ignore(d => d.ReturnValidOptionsIfThereAreAny)
            .Ignore(d => d.ShipSeparatelyOneItemEach)
            .Ignore(d => d.UseCubeRootMethod)
            .Ignore(d => d.RequestDelay);
    }

    /// <summary>
    /// Create stores maps 
    /// </summary>
    protected virtual void CreateStoresMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Store, StoreModel>()
            .Ignore(d => d.AvailableLanguages);

        config.NewConfig<StoreModel, Store>()
            .Ignore(d => d.SslEnabled)
            .Ignore(d => d.Deleted);
    }

    /// <summary>
    /// Create tasks maps 
    /// </summary>
    protected virtual void CreateTasksMaps(TypeAdapterConfig config)
    {
        config.NewConfig<ScheduleTask, ScheduleTaskModel>();
        config.NewConfig<ScheduleTaskModel, ScheduleTask>()
            .Ignore(d => d.Type)
            .Ignore(d => d.LastStartUtc)
            .Ignore(d => d.LastEndUtc)
            .Ignore(d => d.LastSuccessUtc)
            .Ignore(d => d.LastEnabledUtc);
    }

    /// <summary>
    /// Create tax maps 
    /// </summary>
    protected virtual void CreateTaxMaps(TypeAdapterConfig config)
    {
        config.NewConfig<TaxCategory, TaxCategoryModel>();
        config.NewConfig<TaxCategoryModel, TaxCategory>();

        config.NewConfig<ITaxProvider, TaxProviderModel>()
            .Ignore(d => d.IsPrimaryTaxProvider);

        config.NewConfig<TaxSettings, TaxSettingsModel>()
            .Ignore(d => d.AllowCustomersToSelectTaxDisplayType_OverrideForStore)
            .Ignore(d => d.AutomaticallyDetectCountry_OverrideForStore)
            .Ignore(d => d.DefaultTaxAddress)
            .Ignore(d => d.DefaultTaxAddress_OverrideForStore)
            .Ignore(d => d.DefaultTaxCategoryId_OverrideForStore)
            .Ignore(d => d.DisplayTaxRates_OverrideForStore)
            .Ignore(d => d.DisplayTaxSuffix_OverrideForStore)
            .Ignore(d => d.EuVatAllowVatExemption_OverrideForStore)
            .Ignore(d => d.EuVatAssumeValid_OverrideForStore)
            .Ignore(d => d.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore)
            .Ignore(d => d.EuVatEnabled_OverrideForStore)
            .Ignore(d => d.EuVatEnabledForGuests_OverrideForStore)
            .Ignore(d => d.EuVatRequired_OverrideForStore)
            .Ignore(d => d.EuVatShopCountries)
            .Ignore(d => d.EuVatShopCountryId_OverrideForStore)
            .Ignore(d => d.EuVatUseWebService_OverrideForStore)
            .Ignore(d => d.ForceTaxExclusionFromOrderSubtotal_OverrideForStore)
            .Ignore(d => d.HideTaxInOrderSummary_OverrideForStore)
            .Ignore(d => d.HideZeroTax_OverrideForStore)
            .Ignore(d => d.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore)
            .Ignore(d => d.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore)
            .Ignore(d => d.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore)
            .Ignore(d => d.PricesIncludeTax_OverrideForStore)
            .Ignore(d => d.ShippingIsTaxable_OverrideForStore)
            .Ignore(d => d.ShippingPriceIncludesTax_OverrideForStore)
            .Ignore(d => d.ShippingTaxClassId_OverrideForStore)
            .Ignore(d => d.TaxBasedOnPickupPointAddress_OverrideForStore)
            .Ignore(d => d.TaxBasedOnValues)
            .Ignore(d => d.TaxBasedOn_OverrideForStore)
            .Ignore(d => d.TaxCategories)
            .Ignore(d => d.TaxDisplayTypeValues)
            .Ignore(d => d.TaxDisplayType_OverrideForStore)
            .Ignore(d => d.HmrcApiUrl_OverrideForStore)
            .Ignore(d => d.HmrcClientId_OverrideForStore)
            .Ignore(d => d.HmrcClientSecret_OverrideForStore);

        config.NewConfig<TaxSettingsModel, TaxSettings>()
            .Ignore(d => d.ActiveTaxProviderSystemName)
            .Ignore(d => d.LogErrors);
    }

    /// <summary>
    /// Create topics maps 
    /// </summary>
    protected virtual void CreateTopicsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Topic, TopicModel>()
            .Ignore(d => d.AvailableTopicTemplates)
            .Ignore(d => d.SeName)
            .Ignore(d => d.TopicName)
            .Ignore(d => d.Url);

        config.NewConfig<TopicModel, Topic>();

        config.NewConfig<TopicTemplate, TopicTemplateModel>();
        config.NewConfig<TopicTemplateModel, TopicTemplate>();
    }

    /// <summary>
    /// Create vendors maps 
    /// </summary>
    protected virtual void CreateVendorsMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Vendor, VendorModel>()
            .Ignore(d => d.Address)
            .Ignore(d => d.AddVendorNoteMessage)
            .Ignore(d => d.AssociatedCustomers)
            .Ignore(d => d.SeName)
            .Ignore(d => d.VendorAttributes)
            .Ignore(d => d.VendorNoteSearchModel)
            .Ignore(d => d.PrimaryStoreCurrencyCode)
            .Ignore(d => d.PmCustomerInfo);
        config.NewConfig<VendorModel, Vendor>()
            .Ignore(d => d.Deleted);

        config.NewConfig<VendorNote, VendorNoteModel>()
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.Note);

        config.NewConfig<VendorAttribute, VendorAttributeModel>()
            .Ignore(d => d.AttributeControlTypeName)
            .Ignore(d => d.VendorAttributeValueSearchModel);
        config.NewConfig<VendorAttributeModel, VendorAttribute>()
            .Ignore(d => d.AttributeControlType);

        config.NewConfig<VendorAttributeValue, VendorAttributeValueModel>();
        config.NewConfig<VendorAttributeValueModel, VendorAttributeValue>();

        config.NewConfig<VendorSettings, VendorSettingsModel>()
            .Ignore(d => d.AllowCustomersToApplyForVendorAccount_OverrideForStore)
            .Ignore(d => d.AllowCustomersToContactVendors_OverrideForStore)
            .Ignore(d => d.AllowSearchByVendor_OverrideForStore)
            .Ignore(d => d.AllowVendorsToEditInfo_OverrideForStore)
            .Ignore(d => d.AllowVendorsToImportProducts_OverrideForStore)
            .Ignore(d => d.MaximumProductNumber_OverrideForStore)
            .Ignore(d => d.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore)
            .Ignore(d => d.ShowVendorOnOrderDetailsPage_OverrideForStore)
            .Ignore(d => d.ShowVendorOnProductDetailsPage_OverrideForStore)
            .Ignore(d => d.TermsOfServiceEnabled_OverrideForStore)
            .Ignore(d => d.VendorAttributeSearchModel)
            .Ignore(d => d.VendorsBlockItemsToDisplay_OverrideForStore);
        config.NewConfig<VendorSettingsModel, VendorSettings>()
            .Ignore(d => d.DefaultVendorPageSizeOptions)
            .Ignore(d => d.MaximumProductPicturesNumber);
    }

    /// <summary>
    /// Create warehouse maps 
    /// </summary>
    protected virtual void CreateWarehouseMaps(TypeAdapterConfig config)
    {
        config.NewConfig<Warehouse, WarehouseModel>()
            .Ignore(d => d.Address);

        config.NewConfig<WarehouseModel, Warehouse>()
            .Ignore(d => d.AddressId);
    }

    #endregion
    
    #region Properties

    /// <summary>
    /// Order of this mapper implementation
    /// </summary>
    public int Order => 0;

    #endregion
}