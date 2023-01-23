using AutoMapper;
using AutoMapper.Internal;
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
using Nop.Web.Framework.WebOptimizer;

namespace Nop.Web.Areas.Admin.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration for admin area models
    /// </summary>
    public partial class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public AdminMapperConfiguration()
        {
            //create specific maps
            CreateConfigMaps();
            CreateAffiliatesMaps();
            CreateAuthenticationMaps();
            CreateMultiFactorAuthenticationMaps();
            CreateBlogsMaps();
            CreateCatalogMaps();
            CreateCmsMaps();
            CreateCommonMaps();
            CreateCustomersMaps();
            CreateDirectoryMaps();
            CreateDiscountsMaps();
            CreateForumsMaps();
            CreateGdprMaps();
            CreateLocalizationMaps();
            CreateLoggingMaps();
            CreateMediaMaps();
            CreateMessagesMaps();
            CreateNewsMaps();
            CreateOrdersMaps();
            CreatePaymentsMaps();
            CreatePluginsMaps();
            CreatePollsMaps();
            CreateSecurityMaps();
            CreateSeoMaps();
            CreateShippingMaps();
            CreateStoresMaps();
            CreateTasksMaps();
            CreateTaxMaps();
            CreateTopicsMaps();
            CreateVendorsMaps();
            CreateWarehouseMaps();

            //add some generic mapping rules
            this.Internal().ForAllMaps((mapConfiguration, map) =>
            {
                //exclude Form and CustomProperties from mapping BaseNopModel
                if (typeof(BaseNopModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    //map.ForMember(nameof(BaseNopModel.Form), options => options.Ignore());
                    map.ForMember(nameof(BaseNopModel.CustomProperties), options => options.Ignore());
                }

                //exclude ActiveStoreScopeConfiguration from mapping ISettingsModel
                if (typeof(ISettingsModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ISettingsModel.ActiveStoreScopeConfiguration), options => options.Ignore());

                //exclude some properties from mapping configuration and models
                if (typeof(IConfig).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IConfig.Name), options => options.Ignore());

                //exclude Locales from mapping ILocalizedModel
                if (typeof(ILocalizedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ILocalizedModel<ILocalizedModel>.Locales), options => options.Ignore());

                //exclude some properties from mapping store mapping supported entities and models
                if (typeof(IStoreMappingSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IStoreMappingSupported.LimitedToStores), options => options.Ignore());
                if (typeof(IStoreMappingSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IStoreMappingSupportedModel.AvailableStores), options => options.Ignore());
                    map.ForMember(nameof(IStoreMappingSupportedModel.SelectedStoreIds), options => options.Ignore());
                }

                //exclude some properties from mapping ACL supported entities and models
                if (typeof(IAclSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IAclSupported.SubjectToAcl), options => options.Ignore());
                if (typeof(IAclSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IAclSupportedModel.AvailableCustomerRoles), options => options.Ignore());
                    map.ForMember(nameof(IAclSupportedModel.SelectedCustomerRoleIds), options => options.Ignore());
                }

                //exclude some properties from mapping discount supported entities and models
                if (typeof(IDiscountSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IDiscountSupportedModel.AvailableDiscounts), options => options.Ignore());
                    map.ForMember(nameof(IDiscountSupportedModel.SelectedDiscountIds), options => options.Ignore());
                }

                if (typeof(IPluginModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    //exclude some properties from mapping plugin models
                    map.ForMember(nameof(IPluginModel.ConfigurationUrl), options => options.Ignore());
                    map.ForMember(nameof(IPluginModel.IsActive), options => options.Ignore());
                    map.ForMember(nameof(IPluginModel.LogoUrl), options => options.Ignore());

                    //define specific rules for mapping plugin models
                    if (typeof(IPlugin).IsAssignableFrom(mapConfiguration.SourceType))
                    {
                        map.ForMember(nameof(IPluginModel.DisplayOrder), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.DisplayOrder));
                        map.ForMember(nameof(IPluginModel.FriendlyName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.FriendlyName));
                        map.ForMember(nameof(IPluginModel.SystemName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.SystemName));
                    }
                }
            });
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Create configuration maps 
        /// </summary>
        protected virtual void CreateConfigMaps()
        {
            CreateMap<CacheConfig, CacheConfigModel>();
            CreateMap<CacheConfigModel, CacheConfig>();

            CreateMap<HostingConfig, HostingConfigModel>();
            CreateMap<HostingConfigModel, HostingConfig>();

            CreateMap<DistributedCacheConfig, DistributedCacheConfigModel>()
                .ForMember(model => model.DistributedCacheTypeValues, options => options.Ignore());
            CreateMap<DistributedCacheConfigModel, DistributedCacheConfig>();

            CreateMap<AzureBlobConfig, AzureBlobConfigModel>();
            CreateMap<AzureBlobConfigModel, AzureBlobConfig>()
                .ForMember(entity => entity.Enabled, options => options.Ignore())
                .ForMember(entity => entity.DataProtectionKeysEncryptWithVault, options => options.Ignore());

            CreateMap<InstallationConfig, InstallationConfigModel>();
            CreateMap<InstallationConfigModel, InstallationConfig>();

            CreateMap<PluginConfig, PluginConfigModel>();
            CreateMap<PluginConfigModel, PluginConfig>();

            CreateMap<CommonConfig, CommonConfigModel>();
            CreateMap<CommonConfigModel, CommonConfig>();

            CreateMap<DataConfig, DataConfigModel>()
                .ForMember(model => model.DataProviderTypeValues, options => options.Ignore());
            CreateMap<DataConfigModel, DataConfig>();

            CreateMap<WebOptimizerConfig, WebOptimizerConfigModel>();
            CreateMap<WebOptimizerConfigModel, WebOptimizerConfig>()
                .ForMember(entity => entity.CdnUrl, options => options.Ignore())
                .ForMember(entity => entity.AllowEmptyBundle, options => options.Ignore())
                .ForMember(entity => entity.HttpsCompression, options => options.Ignore())
                .ForMember(entity => entity.EnableTagHelperBundling, options => options.Ignore())
                .ForMember(entity => entity.EnableCaching, options => options.Ignore())
                .ForMember(entity => entity.EnableMemoryCache, options => options.Ignore());
        }

        /// <summary>
        /// Create affiliates maps 
        /// </summary>
        protected virtual void CreateAffiliatesMaps()
        {
            CreateMap<Affiliate, AffiliateModel>()
                .ForMember(model => model.Address, options => options.Ignore())
                .ForMember(model => model.AffiliatedCustomerSearchModel, options => options.Ignore())
                .ForMember(model => model.AffiliatedOrderSearchModel, options => options.Ignore())
                .ForMember(model => model.Url, options => options.Ignore());
            CreateMap<AffiliateModel, Affiliate>()
                .ForMember(entity => entity.Deleted, options => options.Ignore());

            CreateMap<Order, AffiliatedOrderModel>()
                .ForMember(model => model.OrderStatus, options => options.Ignore())
                .ForMember(model => model.PaymentStatus, options => options.Ignore())
                .ForMember(model => model.ShippingStatus, options => options.Ignore())
                .ForMember(model => model.OrderTotal, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore());

            CreateMap<Customer, AffiliatedCustomerModel>()
                .ForMember(model => model.Name, options => options.Ignore());

        }

        /// <summary>
        /// Create authentication maps 
        /// </summary>
        protected virtual void CreateAuthenticationMaps()
        {
            CreateMap<IExternalAuthenticationMethod, ExternalAuthenticationMethodModel>();
        }

        /// <summary>
        /// Create multi-factor authentication maps 
        /// </summary>
        protected virtual void CreateMultiFactorAuthenticationMaps()
        {
            CreateMap<IMultiFactorAuthenticationMethod, MultiFactorAuthenticationMethodModel>();
        }

        /// <summary>
        /// Create blogs maps 
        /// </summary>new
        protected virtual void CreateBlogsMaps()
        {
            CreateMap<BlogComment, BlogCommentModel>()
                .ForMember(model => model.BlogPostTitle, options => options.Ignore())
                .ForMember(model => model.Comment, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CustomerInfo, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());

            CreateMap<BlogCommentModel, BlogComment>()
                .ForMember(entity => entity.CommentText, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.BlogPostId, options => options.Ignore())
                .ForMember(entity => entity.CustomerId, options => options.Ignore())
                .ForMember(entity => entity.StoreId, options => options.Ignore());

            CreateMap<BlogPost, BlogPostModel>()
                .ForMember(model => model.ApprovedComments, options => options.Ignore())
                .ForMember(model => model.AvailableLanguages, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LanguageName, options => options.Ignore())
                .ForMember(model => model.NotApprovedComments, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.InitialBlogTags, options => options.Ignore());
            CreateMap<BlogPostModel, BlogPost>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

            CreateMap<BlogSettings, BlogSettingsModel>()
                .ForMember(model => model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.BlogCommentsMustBeApproved_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyAboutNewBlogComments_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfTags_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PostsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowHeaderRssUrl_OverrideForStore, options => options.Ignore());
            CreateMap<BlogSettingsModel, BlogSettings>();
        }

        /// <summary>
        /// Create catalog maps 
        /// </summary>
        protected virtual void CreateCatalogMaps()
        {
            CreateMap<CatalogSettings, CatalogSettingsModel>()
                .ForMember(model => model.AllowAnonymousUsersToEmailAFriend_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowAnonymousUsersToReviewProduct_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowProductSorting_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowProductViewModeChanging_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowViewUnpublishedProductPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AvailableViewModes, options => options.Ignore())
                .ForMember(model => model.CategoryBreadcrumbEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CompareProductsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultViewMode_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoFooter_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoProductBoxes_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoShoppingCart_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoWishlist_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EmailAFriendEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportAllowDownloadImages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportCategoriesUsingCategoryName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportProductAttributes_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportProductCategoryBreadcrumb_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportProductSpecificationAttributes_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportRelatedEntitiesByName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportProductUseLimitedToStores_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportSplitProductsFile_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IncludeFullDescriptionInCompareProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IncludeShortDescriptionInCompareProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ManufacturersBlockItemsToDisplay_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsAllowCustomersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsPageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyCustomerAboutProductReviewReply_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfBestsellersOnHomepage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfProductTags_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PageShareCode_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewsMustBeApproved_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OneReviewPerProductFromCustomer_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewsPageSizeOnAccountPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewsSortByCreatedDateAscending_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsAlsoPurchasedEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsAlsoPurchasedNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsByTagPageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsByTagPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductSearchAutoCompleteEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductSearchEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductSearchTermMinimumLength_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RecentlyViewedProductsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RecentlyViewedProductsNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RemoveRequiredProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchPagePageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchPageProductsPerPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowBestsellersOnHomepage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowCategoryProductNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowFreeShippingNotification_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowShortDescriptionOnCatalogPages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowGtin_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowManufacturerPartNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductImagesInSearchAutoComplete_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductReviewsOnAccountPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductReviewsPerStore_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductsFromSubcategories_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowShareButton_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowSkuOnCatalogPages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowSkuOnProductDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayDatePreOrderAvailability_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.UseAjaxCatalogProductsLoading_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPagePriceRangeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPagePriceFrom_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPagePriceTo_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPageManuallyPriceRange_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductsByTagPriceRangeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductsByTagPriceFrom_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductsByTagPriceTo_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductsByTagManuallyPriceRange_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.EnableManufacturerFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.EnablePriceRangeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.EnableSpecificationAttributeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.DisplayFromPrices_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.AttributeValueOutOfStockDisplayTypes, mo => mo.Ignore())
                .ForMember(model => model.AttributeValueOutOfStockDisplayType_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SortOptionSearchModel, options => options.Ignore())
                .ForMember(model => model.ReviewTypeSearchModel, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.AllowCustomersToSearchWithManufacturerName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowCustomersToSearchWithCategoryName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayAllPicturesOnCatalogPages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductUrlStructureTypeId_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductUrlStructureTypes, mo => mo.Ignore());
            CreateMap<CatalogSettingsModel, CatalogSettings>()
                .ForMember(settings => settings.AjaxProcessAttributeChange, options => options.Ignore())
                .ForMember(settings => settings.CompareProductsNumber, options => options.Ignore())
                .ForMember(settings => settings.CountDisplayedYearsDatePicker, options => options.Ignore())
                .ForMember(settings => settings.DefaultCategoryPageSize, options => options.Ignore())
                .ForMember(settings => settings.DefaultCategoryPageSizeOptions, options => options.Ignore())
                .ForMember(settings => settings.DefaultManufacturerPageSize, options => options.Ignore())
                .ForMember(settings => settings.DefaultManufacturerPageSizeOptions, options => options.Ignore())
                .ForMember(settings => settings.DefaultProductRatingValue, options => options.Ignore())
                .ForMember(settings => settings.DisplayTierPricesWithDiscounts, options => options.Ignore())
                .ForMember(settings => settings.ExportImportProductsCountInOneFile, options => options.Ignore())
                .ForMember(settings => settings.ExportImportUseDropdownlistsForAssociatedEntities, options => options.Ignore())
                .ForMember(settings => settings.IncludeFeaturedProductsInNormalLists, options => options.Ignore())
                .ForMember(settings => settings.MaximumBackInStockSubscriptions, options => options.Ignore())
                .ForMember(settings => settings.ProductSortingEnumDisabled, options => options.Ignore())
                .ForMember(settings => settings.ProductSortingEnumDisplayOrder, options => options.Ignore())
                .ForMember(settings => settings.PublishBackProductWhenCancellingOrders, options => options.Ignore())
                .ForMember(settings => settings.UseAjaxLoadMenu, options => options.Ignore())
                .ForMember(settings => settings.UseLinksInRequiredProductWarnings, options => options.Ignore())
                .ForMember(settings => settings.ActiveSearchProviderSystemName, options => options.Ignore());

            CreateMap<ProductCategory, CategoryProductModel>()
                .ForMember(model => model.ProductName, options => options.Ignore());
            CreateMap<CategoryProductModel, ProductCategory>()
                .ForMember(entity => entity.CategoryId, options => options.Ignore())
                .ForMember(entity => entity.ProductId, options => options.Ignore());

            CreateMap<Category, CategoryModel>()
                .ForMember(model => model.AvailableCategories, options => options.Ignore())
                .ForMember(model => model.AvailableCategoryTemplates, options => options.Ignore())
                .ForMember(model => model.Breadcrumb, options => options.Ignore())
                .ForMember(model => model.CategoryProductSearchModel, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<CategoryModel, Category>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<CategoryTemplate, CategoryTemplateModel>();
            CreateMap<CategoryTemplateModel, CategoryTemplate>();

            CreateMap<ProductManufacturer, ManufacturerProductModel>()
                .ForMember(model => model.ProductName, options => options.Ignore());
            CreateMap<ManufacturerProductModel, ProductManufacturer>()
                .ForMember(entity => entity.ManufacturerId, options => options.Ignore())
                .ForMember(entity => entity.ProductId, options => options.Ignore());

            CreateMap<Manufacturer, ManufacturerModel>()
                .ForMember(model => model.AvailableManufacturerTemplates, options => options.Ignore())
                .ForMember(model => model.ManufacturerProductSearchModel, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<ManufacturerModel, Manufacturer>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ManufacturerTemplate, ManufacturerTemplateModel>();
            CreateMap<ManufacturerTemplateModel, ManufacturerTemplate>();

            //Review type
            CreateMap<ReviewType, ReviewTypeModel>();
            CreateMap<ReviewTypeModel, ReviewType>();

            //product review
            CreateMap<ProductReview, ProductReviewModel>()
                .ForMember(model => model.CustomerInfo, mo => mo.Ignore())
                .ForMember(model => model.IsLoggedInAsVendor, mo => mo.Ignore())
                .ForMember(model => model.ProductReviewReviewTypeMappingSearchModel, mo => mo.Ignore())
                .ForMember(model => model.CreatedOn, mo => mo.Ignore())
                .ForMember(model => model.StoreName, mo => mo.Ignore())
                .ForMember(model => model.ShowStoreName, mo => mo.Ignore())
                .ForMember(model => model.ProductName, mo => mo.Ignore());

            //product review type mapping
            CreateMap<ProductReviewReviewTypeMapping, ProductReviewReviewTypeMappingModel>()
                .ForMember(model => model.Name, mo => mo.Ignore())
                .ForMember(model => model.Description, mo => mo.Ignore())
                .ForMember(model => model.VisibleToAllCustomers, mo => mo.Ignore());

            //products
            CreateMap<Product, ProductModel>()
                .ForMember(model => model.AddPictureModel, options => options.Ignore())
                .ForMember(model => model.AssociatedProductSearchModel, options => options.Ignore())
                .ForMember(model => model.AssociatedToProductId, options => options.Ignore())
                .ForMember(model => model.AssociatedToProductName, options => options.Ignore())
                .ForMember(model => model.AvailableBasepriceBaseUnits, options => options.Ignore())
                .ForMember(model => model.AvailableBasepriceUnits, options => options.Ignore())
                .ForMember(model => model.AvailableCategories, options => options.Ignore())
                .ForMember(model => model.AvailableDeliveryDates, options => options.Ignore())
                .ForMember(model => model.AvailableManufacturers, options => options.Ignore())
                .ForMember(model => model.AvailableProductAvailabilityRanges, options => options.Ignore())
                .ForMember(model => model.AvailableProductTemplates, options => options.Ignore())
                .ForMember(model => model.AvailableTaxCategories, options => options.Ignore())
                .ForMember(model => model.AvailableVendors, options => options.Ignore())
                .ForMember(model => model.AvailableWarehouses, options => options.Ignore())
                .ForMember(model => model.BaseDimensionIn, options => options.Ignore())
                .ForMember(model => model.BaseWeightIn, options => options.Ignore())
                .ForMember(model => model.CopyProductModel, options => options.Ignore())
                .ForMember(model => model.CrossSellProductSearchModel, options => options.Ignore())
                .ForMember(model => model.HasAvailableSpecificationAttributes, options => options.Ignore())
                .ForMember(model => model.IsLoggedInAsVendor, options => options.Ignore())
                .ForMember(model => model.LastStockQuantity, options => options.Ignore())
                .ForMember(model => model.PictureThumbnailUrl, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.ProductAttributeCombinationSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductAttributeMappingSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductAttributesExist, options => options.Ignore())
                .ForMember(model => model.CanCreateCombinations, options => options.Ignore())
                .ForMember(model => model.ProductEditorSettingsModel, options => options.Ignore())
                .ForMember(model => model.ProductOrderSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductPictureModels, options => options.Ignore())
                .ForMember(model => model.ProductPictureSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductVideoModels, options => options.Ignore())
                .ForMember(model => model.ProductVideoSearchModel, options => options.Ignore())
                .ForMember(model => model.AddVideoModel, options => options.Ignore())
                .ForMember(model => model.ProductSpecificationAttributeSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductsTypesSupportedByProductTemplates, options => options.Ignore())
                .ForMember(model => model.ProductTags, options => options.Ignore())
                .ForMember(model => model.ProductTypeName, options => options.Ignore())
                .ForMember(model => model.ProductWarehouseInventoryModels, options => options.Ignore())
                .ForMember(model => model.RelatedProductSearchModel, options => options.Ignore())
                .ForMember(model => model.SelectedCategoryIds, options => options.Ignore())
                .ForMember(model => model.SelectedManufacturerIds, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.StockQuantityHistory, options => options.Ignore())
                .ForMember(model => model.StockQuantityHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.StockQuantityStr, options => options.Ignore())
                .ForMember(model => model.TierPriceSearchModel, options => options.Ignore())
                .ForMember(model => model.InitialProductTags, options => options.Ignore());
            CreateMap<ProductModel, Product>()
                .ForMember(entity => entity.ApprovedRatingSum, options => options.Ignore())
                .ForMember(entity => entity.ApprovedTotalReviews, options => options.Ignore())
                .ForMember(entity => entity.BackorderMode, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.DownloadActivationType, options => options.Ignore())
                .ForMember(entity => entity.GiftCardType, options => options.Ignore())
                .ForMember(entity => entity.HasDiscountsApplied, options => options.Ignore())
                .ForMember(entity => entity.HasTierPrices, options => options.Ignore())
                .ForMember(entity => entity.LowStockActivity, options => options.Ignore())
                .ForMember(entity => entity.ManageInventoryMethod, options => options.Ignore())
                .ForMember(entity => entity.NotApprovedRatingSum, options => options.Ignore())
                .ForMember(entity => entity.NotApprovedTotalReviews, options => options.Ignore())
                .ForMember(entity => entity.ParentGroupedProductId, options => options.Ignore())
                .ForMember(entity => entity.ProductType, options => options.Ignore())
                .ForMember(entity => entity.RecurringCyclePeriod, options => options.Ignore())
                .ForMember(entity => entity.RentalPricePeriod, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<Product, DiscountProductModel>()
                .ForMember(model => model.ProductId, options => options.Ignore())
                .ForMember(model => model.ProductName, options => options.Ignore());

            CreateMap<Product, AssociatedProductModel>()
                .ForMember(model => model.ProductName, options => options.Ignore());

            CreateMap<ProductAttributeCombination, ProductAttributeCombinationModel>()
               .ForMember(model => model.AttributesXml, options => options.Ignore())
               .ForMember(model => model.ProductAttributes, options => options.Ignore())
               .ForMember(model => model.ProductPictureModels, options => options.Ignore())
               .ForMember(model => model.PictureThumbnailUrl, options => options.Ignore())
               .ForMember(model => model.Warnings, options => options.Ignore());
            CreateMap<ProductAttributeCombinationModel, ProductAttributeCombination>()
               .ForMember(entity => entity.AttributesXml, options => options.Ignore());

            CreateMap<ProductAttribute, ProductAttributeModel>()
                .ForMember(model => model.PredefinedProductAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductAttributeProductSearchModel, options => options.Ignore());
            CreateMap<ProductAttributeModel, ProductAttribute>();

            CreateMap<Product, ProductAttributeProductModel>()
                .ForMember(model => model.ProductName, options => options.Ignore());

            CreateMap<PredefinedProductAttributeValue, PredefinedProductAttributeValueModel>()
                .ForMember(model => model.WeightAdjustmentStr, options => options.Ignore())
                .ForMember(model => model.PriceAdjustmentStr, options => options.Ignore());
            CreateMap<PredefinedProductAttributeValueModel, PredefinedProductAttributeValue>();

            CreateMap<ProductAttributeMapping, ProductAttributeMappingModel>()
                .ForMember(model => model.ValidationRulesString, options => options.Ignore())
                .ForMember(model => model.AttributeControlType, options => options.Ignore())
                .ForMember(model => model.ConditionString, options => options.Ignore())
                .ForMember(model => model.ProductAttribute, options => options.Ignore())
                .ForMember(model => model.AvailableProductAttributes, options => options.Ignore())
                .ForMember(model => model.ConditionAllowed, options => options.Ignore())
                .ForMember(model => model.ConditionModel, options => options.Ignore())
                .ForMember(model => model.ProductAttributeValueSearchModel, options => options.Ignore());
            CreateMap<ProductAttributeMappingModel, ProductAttributeMapping>()
                .ForMember(entity => entity.ConditionAttributeXml, options => options.Ignore())
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

            CreateMap<ProductAttributeValue, ProductAttributeValueModel>()
                .ForMember(model => model.AttributeValueTypeName, options => options.Ignore())
                .ForMember(model => model.Name, options => options.Ignore())
                .ForMember(model => model.PriceAdjustmentStr, options => options.Ignore())
                .ForMember(model => model.AssociatedProductName, options => options.Ignore())
                .ForMember(model => model.PictureThumbnailUrl, options => options.Ignore())
                .ForMember(model => model.WeightAdjustmentStr, options => options.Ignore())
                .ForMember(model => model.DisplayColorSquaresRgb, options => options.Ignore())
                .ForMember(model => model.DisplayImageSquaresPicture, options => options.Ignore())
                .ForMember(model => model.ProductPictureModels, options => options.Ignore());
            CreateMap<ProductAttributeValueModel, ProductAttributeValue>()
               .ForMember(entity => entity.AttributeValueType, options => options.Ignore())
               .ForMember(entity => entity.Quantity, options => options.Ignore());

            CreateMap<ProductEditorSettings, ProductEditorSettingsModel>();
            CreateMap<ProductEditorSettingsModel, ProductEditorSettings>();

            CreateMap<ProductPicture, ProductPictureModel>()
                .ForMember(model => model.OverrideAltAttribute, options => options.Ignore())
                .ForMember(model => model.OverrideTitleAttribute, options => options.Ignore())
                .ForMember(model => model.PictureUrl, options => options.Ignore());

            CreateMap<ProductVideo, ProductVideoModel>()
               .ForMember(model => model.VideoUrl, options => options.Ignore());

            CreateMap<Product, SpecificationAttributeProductModel>()
                .ForMember(model => model.SpecificationAttributeId, options => options.Ignore())
                .ForMember(model => model.ProductId, options => options.Ignore())
                .ForMember(model => model.ProductName, options => options.Ignore());

            CreateMap<ProductSpecificationAttribute, ProductSpecificationAttributeModel>()
                .ForMember(model => model.AttributeTypeName, options => options.Ignore())
                .ForMember(model => model.ValueRaw, options => options.Ignore())
                .ForMember(model => model.AttributeId, options => options.Ignore())
                .ForMember(model => model.AttributeName, options => options.Ignore())
                .ForMember(model => model.SpecificationAttributeOptionId, options => options.Ignore());

            CreateMap<ProductSpecificationAttribute, AddSpecificationAttributeModel>()
                .ForMember(entity => entity.SpecificationId, options => options.Ignore())
                .ForMember(entity => entity.AttributeTypeName, options => options.Ignore())
                .ForMember(entity => entity.AttributeId, options => options.Ignore())
                .ForMember(entity => entity.AttributeName, options => options.Ignore())
                .ForMember(entity => entity.ValueRaw, options => options.Ignore())
                .ForMember(entity => entity.Value, options => options.Ignore())
                .ForMember(entity => entity.AvailableOptions, options => options.Ignore())
                .ForMember(entity => entity.AvailableAttributes, options => options.Ignore());

            CreateMap<AddSpecificationAttributeModel, ProductSpecificationAttribute>()
                .ForMember(model => model.CustomValue, options => options.Ignore())
                .ForMember(model => model.AttributeType, options => options.Ignore());

            CreateMap<ProductTag, ProductTagModel>()
               .ForMember(model => model.ProductCount, options => options.Ignore());

            CreateMap<ProductTemplate, ProductTemplateModel>();
            CreateMap<ProductTemplateModel, ProductTemplate>();

            CreateMap<RelatedProduct, RelatedProductModel>()
               .ForMember(model => model.Product2Name, options => options.Ignore());

            CreateMap<SpecificationAttribute, SpecificationAttributeModel>()
                .ForMember(model => model.SpecificationAttributeOptionSearchModel, options => options.Ignore())
                .ForMember(model => model.SpecificationAttributeProductSearchModel, options => options.Ignore())
                .ForMember(model => model.AvailableGroups, options => options.Ignore());
            CreateMap<SpecificationAttributeModel, SpecificationAttribute>();

            CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionModel>()
                .ForMember(model => model.EnableColorSquaresRgb, options => options.Ignore())
                .ForMember(model => model.NumberOfAssociatedProducts, options => options.Ignore());
            CreateMap<SpecificationAttributeOptionModel, SpecificationAttributeOption>();

            CreateMap<SpecificationAttributeGroup, SpecificationAttributeGroupModel>();
            CreateMap<SpecificationAttributeGroupModel, SpecificationAttributeGroup>();

            CreateMap<StockQuantityHistory, StockQuantityHistoryModel>()
                .ForMember(model => model.WarehouseName, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.AttributeCombination, options => options.Ignore());

            CreateMap<TierPrice, TierPriceModel>()
                .ForMember(model => model.Store, options => options.Ignore())
                .ForMember(model => model.AvailableCustomerRoles, options => options.Ignore())
                .ForMember(model => model.AvailableStores, options => options.Ignore())
                .ForMember(model => model.CustomerRole, options => options.Ignore());
            CreateMap<TierPriceModel, TierPrice>()
                .ForMember(entity => entity.CustomerRoleId, options => options.Ignore())
                .ForMember(entity => entity.ProductId, options => options.Ignore());
        }

        /// <summary>
        /// Create CMS maps 
        /// </summary>
        protected virtual void CreateCmsMaps()
        {
            CreateMap<IWidgetPlugin, WidgetModel>()
                .ForMember(model => model.WidgetViewComponentArguments, options => options.Ignore())
                .ForMember(model => model.WidgetViewComponentName, options => options.Ignore());
        }

        /// <summary>
        /// Create common maps 
        /// </summary>
        protected virtual void CreateCommonMaps()
        {
            CreateMap<Address, AddressModel>()
                .ForMember(model => model.AddressHtml, options => options.Ignore())
                .ForMember(model => model.AvailableCountries, options => options.Ignore())
                .ForMember(model => model.AvailableStates, options => options.Ignore())
                .ForMember(model => model.CountryName, options => options.Ignore())
                .ForMember(model => model.CustomAddressAttributes, options => options.Ignore())
                .ForMember(model => model.FormattedCustomAddressAttributes, options => options.Ignore())
                .ForMember(model => model.StateProvinceName, options => options.Ignore())
                .ForMember(model => model.CityRequired, options => options.Ignore())
                .ForMember(model => model.CompanyRequired, options => options.Ignore())
                .ForMember(model => model.CountryRequired, options => options.Ignore())
                .ForMember(model => model.CountyRequired, options => options.Ignore())
                .ForMember(model => model.EmailRequired, options => options.Ignore())
                .ForMember(model => model.FaxRequired, options => options.Ignore())
                .ForMember(model => model.FirstNameRequired, options => options.Ignore())
                .ForMember(model => model.LastNameRequired, options => options.Ignore())
                .ForMember(model => model.PhoneRequired, options => options.Ignore())
                .ForMember(model => model.StateProvinceName, options => options.Ignore())
                .ForMember(model => model.StreetAddress2Required, options => options.Ignore())
                .ForMember(model => model.StreetAddressRequired, options => options.Ignore())
                .ForMember(model => model.ZipPostalCodeRequired, options => options.Ignore());
            CreateMap<AddressModel, Address>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CustomAttributes, options => options.Ignore());

            CreateMap<AddressAttribute, AddressAttributeModel>()
                .ForMember(model => model.AddressAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore());
            CreateMap<AddressAttributeModel, AddressAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

            CreateMap<AddressAttributeValue, AddressAttributeValueModel>();
            CreateMap<AddressAttributeValueModel, AddressAttributeValue>();

            CreateMap<AddressSettings, AddressSettingsModel>();
            CreateMap<AddressSettingsModel, AddressSettings>()
                .ForMember(settings => settings.PreselectCountryIfOnlyOne, options => options.Ignore());

            CreateMap<Setting, SettingModel>()
                .ForMember(setting => setting.AvailableStores, options => options.Ignore())
                .ForMember(setting => setting.Store, options => options.Ignore());
        }

        /// <summary>
        /// Create customers maps 
        /// </summary>
        protected virtual void CreateCustomersMaps()
        {
            CreateMap<CustomerAttribute, CustomerAttributeModel>()
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
                .ForMember(model => model.CustomerAttributeValueSearchModel, options => options.Ignore());
            CreateMap<CustomerAttributeModel, CustomerAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

            CreateMap<CustomerAttributeValue, CustomerAttributeValueModel>();
            CreateMap<CustomerAttributeValueModel, CustomerAttributeValue>();

            CreateMap<CustomerRole, CustomerRoleModel>()
                .ForMember(model => model.PurchasedWithProductName, options => options.Ignore())
                .ForMember(model => model.TaxDisplayTypeValues, options => options.Ignore());
            CreateMap<CustomerRoleModel, CustomerRole>();

            CreateMap<CustomerSettings, CustomerSettingsModel>();
            CreateMap<CustomerSettingsModel, CustomerSettings>()
                .ForMember(settings => settings.AvatarMaximumSizeBytes, options => options.Ignore())
                .ForMember(settings => settings.DeleteGuestTaskOlderThanMinutes, options => options.Ignore())
                .ForMember(settings => settings.DownloadableProductsValidateUser, options => options.Ignore())
                .ForMember(settings => settings.HashedPasswordFormat, options => options.Ignore())
                .ForMember(settings => settings.OnlineCustomerMinutes, options => options.Ignore())
                .ForMember(settings => settings.SuffixDeletedCustomers, options => options.Ignore())
                .ForMember(settings => settings.LastActivityMinutes, options => options.Ignore());

            CreateMap<MultiFactorAuthenticationSettings, MultiFactorAuthenticationSettingsModel>();
            CreateMap<MultiFactorAuthenticationSettingsModel, MultiFactorAuthenticationSettings>()
                .ForMember(settings => settings.ActiveAuthenticationMethodSystemNames, option => option.Ignore());

            CreateMap<RewardPointsSettings, RewardPointsSettingsModel>()
                .ForMember(model => model.ActivatePointsImmediately, options => options.Ignore())
                .ForMember(model => model.ActivationDelay_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayHowMuchWillBeEarned_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExchangeRate_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumRewardPointsToUsePerOrder_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinimumRewardPointsToUse_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinOrderTotalToAwardPoints_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PointsForPurchases_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumRedeemedRate_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PointsForRegistration_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.PurchasesPointsValidity_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RegistrationPointsValidity_OverrideForStore, options => options.Ignore());
            CreateMap<RewardPointsSettingsModel, RewardPointsSettings>();

            CreateMap<RewardPointsHistory, CustomerRewardPointsModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.PointsBalance, options => options.Ignore())
                .ForMember(model => model.EndDate, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());

            CreateMap<ActivityLog, CustomerActivityLogModel>()
               .ForMember(model => model.CreatedOn, options => options.Ignore())
               .ForMember(model => model.ActivityLogTypeName, options => options.Ignore());

            CreateMap<Customer, CustomerModel>()
                .ForMember(model => model.Email, options => options.Ignore())
                .ForMember(model => model.FullName, options => options.Ignore())
                .ForMember(model => model.Company, options => options.Ignore())
                .ForMember(model => model.Phone, options => options.Ignore())
                .ForMember(model => model.ZipPostalCode, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.CustomerRoleNames, options => options.Ignore())
                .ForMember(model => model.AvatarUrl, options => options.Ignore())
                .ForMember(model => model.UsernamesEnabled, options => options.Ignore())
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.AvailableVendors, options => options.Ignore())
                .ForMember(model => model.GenderEnabled, options => options.Ignore())
                .ForMember(model => model.Gender, options => options.Ignore())
                .ForMember(model => model.FirstNameEnabled, options => options.Ignore())
                .ForMember(model => model.FirstName, options => options.Ignore())
                .ForMember(model => model.LastNameEnabled, options => options.Ignore())
                .ForMember(model => model.LastName, options => options.Ignore())
                .ForMember(model => model.DateOfBirthEnabled, options => options.Ignore())
                .ForMember(model => model.DateOfBirth, options => options.Ignore())
                .ForMember(model => model.CompanyEnabled, options => options.Ignore())
                .ForMember(model => model.StreetAddressEnabled, options => options.Ignore())
                .ForMember(model => model.StreetAddress, options => options.Ignore())
                .ForMember(model => model.StreetAddress2Enabled, options => options.Ignore())
                .ForMember(model => model.StreetAddress2, options => options.Ignore())
                .ForMember(model => model.ZipPostalCodeEnabled, options => options.Ignore())
                .ForMember(model => model.CityEnabled, options => options.Ignore())
                .ForMember(model => model.City, options => options.Ignore())
                .ForMember(model => model.CountyEnabled, options => options.Ignore())
                .ForMember(model => model.County, options => options.Ignore())
                .ForMember(model => model.CountryEnabled, options => options.Ignore())
                .ForMember(model => model.CountryId, options => options.Ignore())
                .ForMember(model => model.AvailableCountries, options => options.Ignore())
                .ForMember(model => model.StateProvinceEnabled, options => options.Ignore())
                .ForMember(model => model.StateProvinceId, options => options.Ignore())
                .ForMember(model => model.AvailableStates, options => options.Ignore())
                .ForMember(model => model.PhoneEnabled, options => options.Ignore())
                .ForMember(model => model.FaxEnabled, options => options.Ignore())
                .ForMember(model => model.Fax, options => options.Ignore())
                .ForMember(model => model.CustomerAttributes, options => options.Ignore())
                .ForMember(model => model.RegisteredInStore, options => options.Ignore())
                .ForMember(model => model.DisplayRegisteredInStore, options => options.Ignore())
                .ForMember(model => model.AffiliateName, options => options.Ignore())
                .ForMember(model => model.TimeZoneId, options => options.Ignore())
                .ForMember(model => model.AllowCustomersToSetTimeZone, options => options.Ignore())
                .ForMember(model => model.AvailableTimeZones, options => options.Ignore())
                .ForMember(model => model.VatNumber, options => options.Ignore())
                .ForMember(model => model.VatNumberStatusNote, options => options.Ignore())
                .ForMember(model => model.DisplayVatNumber, options => options.Ignore())
                .ForMember(model => model.LastVisitedPage, options => options.Ignore())
                .ForMember(model => model.AvailableNewsletterSubscriptionStores, options => options.Ignore())
                .ForMember(model => model.SelectedNewsletterSubscriptionStoreIds, options => options.Ignore())
                .ForMember(model => model.DisplayRewardPointsHistory, options => options.Ignore())
                .ForMember(model => model.AddRewardPoints, options => options.Ignore())
                .ForMember(model => model.CustomerRewardPointsSearchModel, options => options.Ignore())
                .ForMember(model => model.SendEmail, options => options.Ignore())
                .ForMember(model => model.SendPm, options => options.Ignore())
                .ForMember(model => model.AllowSendingOfPrivateMessage, options => options.Ignore())
                .ForMember(model => model.AllowSendingOfWelcomeMessage, options => options.Ignore())
                .ForMember(model => model.AllowReSendingOfActivationMessage, options => options.Ignore())
                .ForMember(model => model.GdprEnabled, options => options.Ignore())
                .ForMember(model => model.MultiFactorAuthenticationProvider, options => options.Ignore())
                .ForMember(model => model.CustomerAssociatedExternalAuthRecordsSearchModel, options => options.Ignore())
                .ForMember(model => model.CustomerAddressSearchModel, options => options.Ignore())
                .ForMember(model => model.CustomerOrderSearchModel, options => options.Ignore())
                .ForMember(model => model.CustomerShoppingCartSearchModel, options => options.Ignore())
                .ForMember(model => model.CustomerActivityLogSearchModel, options => options.Ignore())
                .ForMember(model => model.CustomerBackInStockSubscriptionSearchModel, options => options.Ignore());

            CreateMap<CustomerModel, Customer>()
                .ForMember(entity => entity.CustomerGuid, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LastActivityDateUtc, options => options.Ignore())
                .ForMember(entity => entity.EmailToRevalidate, options => options.Ignore())
                .ForMember(entity => entity.HasShoppingCartItems, options => options.Ignore())
                .ForMember(entity => entity.RequireReLogin, options => options.Ignore())
                .ForMember(entity => entity.FailedLoginAttempts, options => options.Ignore())
                .ForMember(entity => entity.CannotLoginUntilDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.IsSystemAccount, options => options.Ignore())
                .ForMember(entity => entity.SystemName, options => options.Ignore())
                .ForMember(entity => entity.LastLoginDateUtc, options => options.Ignore())
                .ForMember(entity => entity.BillingAddressId, options => options.Ignore())
                .ForMember(entity => entity.ShippingAddressId, options => options.Ignore())
                .ForMember(entity => entity.VatNumberStatusId, options => options.Ignore())
                .ForMember(entity => entity.CustomCustomerAttributesXML, options => options.Ignore())
                .ForMember(entity => entity.CurrencyId, options => options.Ignore())
                .ForMember(entity => entity.LanguageId, options => options.Ignore())
                .ForMember(entity => entity.TaxDisplayTypeId, options => options.Ignore())
                .ForMember(entity => entity.VatNumberStatus, options => options.Ignore())
                .ForMember(entity => entity.TaxDisplayType, options => options.Ignore())
                .ForMember(entity => entity.RegisteredInStoreId, options => options.Ignore());

            CreateMap<Customer, OnlineCustomerModel>()
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.CustomerInfo, options => options.Ignore())
                .ForMember(model => model.LastIpAddress, options => options.Ignore())
                .ForMember(model => model.Location, options => options.Ignore())
                .ForMember(model => model.LastVisitedPage, options => options.Ignore());

            CreateMap<BackInStockSubscription, CustomerBackInStockSubscriptionModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore())
                .ForMember(model => model.ProductName, options => options.Ignore());
        }

        /// <summary>
        /// Create directory maps 
        /// </summary>
        protected virtual void CreateDirectoryMaps()
        {
            CreateMap<Country, CountryModel>()
                .ForMember(model => model.NumberOfStates, options => options.Ignore())
                .ForMember(model => model.StateProvinceSearchModel, options => options.Ignore());
            CreateMap<CountryModel, Country>();

            CreateMap<Currency, CurrencyModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.IsPrimaryExchangeRateCurrency, options => options.Ignore())
                .ForMember(model => model.IsPrimaryStoreCurrency, options => options.Ignore());
            CreateMap<CurrencyModel, Currency>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.RoundingType, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<MeasureDimension, MeasureDimensionModel>()
                .ForMember(model => model.IsPrimaryDimension, options => options.Ignore());
            CreateMap<MeasureDimensionModel, MeasureDimension>();

            CreateMap<MeasureWeight, MeasureWeightModel>()
                .ForMember(model => model.IsPrimaryWeight, options => options.Ignore());
            CreateMap<MeasureWeightModel, MeasureWeight>();

            CreateMap<StateProvince, StateProvinceModel>();
            CreateMap<StateProvinceModel, StateProvince>();
        }

        /// <summary>
        /// Create discounts maps 
        /// </summary>
        protected virtual void CreateDiscountsMaps()
        {
            CreateMap<Discount, DiscountModel>()
                .ForMember(model => model.AddDiscountRequirement, options => options.Ignore())
                .ForMember(model => model.AvailableDiscountRequirementRules, options => options.Ignore())
                .ForMember(model => model.AvailableRequirementGroups, options => options.Ignore())
                .ForMember(model => model.DiscountCategorySearchModel, options => options.Ignore())
                .ForMember(model => model.DiscountManufacturerSearchModel, options => options.Ignore())
                .ForMember(model => model.DiscountProductSearchModel, options => options.Ignore())
                .ForMember(model => model.DiscountTypeName, options => options.Ignore())
                .ForMember(model => model.DiscountUrl, options => options.Ignore())
                .ForMember(model => model.DiscountUsageHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.GroupName, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.RequirementGroupId, options => options.Ignore())
                .ForMember(model => model.TimesUsed, options => options.Ignore());
            CreateMap<DiscountModel, Discount>()
                .ForMember(entity => entity.DiscountLimitation, options => options.Ignore())
                .ForMember(entity => entity.DiscountType, options => options.Ignore());

            CreateMap<DiscountUsageHistory, DiscountUsageHistoryModel>()
                .ForMember(entity => entity.CreatedOn, options => options.Ignore())
                .ForMember(entity => entity.OrderTotal, options => options.Ignore())
                .ForMember(entity => entity.CustomOrderNumber, options => options.Ignore());

            CreateMap<Category, DiscountCategoryModel>()
                .ForMember(entity => entity.CategoryName, options => options.Ignore())
                .ForMember(entity => entity.CategoryId, options => options.Ignore());

            CreateMap<Manufacturer, DiscountManufacturerModel>()
                .ForMember(entity => entity.ManufacturerId, options => options.Ignore())
                .ForMember(entity => entity.ManufacturerName, options => options.Ignore());
        }

        /// <summary>
        /// Create forums maps 
        /// </summary>
        protected virtual void CreateForumsMaps()
        {
            CreateMap<Forum, ForumModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.ForumGroups, options => options.Ignore());
            CreateMap<ForumModel, Forum>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LastPostCustomerId, options => options.Ignore())
                .ForMember(entity => entity.LastPostId, options => options.Ignore())
                .ForMember(entity => entity.LastPostTime, options => options.Ignore())
                .ForMember(entity => entity.LastTopicId, options => options.Ignore())
                .ForMember(entity => entity.NumPosts, options => options.Ignore())
                .ForMember(entity => entity.NumTopics, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ForumGroup, ForumGroupModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore());
            CreateMap<ForumGroupModel, ForumGroup>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ForumSettings, ForumSettingsModel>()
                .ForMember(model => model.ActiveDiscussionsFeedCount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ActiveDiscussionsFeedEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ActiveDiscussionsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowCustomersToDeletePosts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowCustomersToEditPosts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowCustomersToManageSubscriptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowGuestsToCreatePosts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowGuestsToCreateTopics_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowPostVoting_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowPrivateMessages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForumEditorValues, options => options.Ignore())
                .ForMember(model => model.ForumEditor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForumFeedCount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForumFeedsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForumsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaxVotesPerDay_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyAboutPrivateMessages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PostsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RelativeDateTimeFormattingEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchResultsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowAlertForPM_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowCustomersPostCount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SignaturesEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TopicsPageSize_OverrideForStore, options => options.Ignore());
            CreateMap<ForumSettingsModel, ForumSettings>()
                .ForMember(settings => settings.ForumSearchTermMinimumLength, options => options.Ignore())
                .ForMember(settings => settings.ForumSubscriptionsPageSize, options => options.Ignore())
                .ForMember(settings => settings.HomepageActiveDiscussionsTopicCount, options => options.Ignore())
                .ForMember(settings => settings.LatestCustomerPostsPageSize, options => options.Ignore())
                .ForMember(settings => settings.PMSubjectMaxLength, options => options.Ignore())
                .ForMember(settings => settings.PMTextMaxLength, options => options.Ignore())
                .ForMember(settings => settings.PostMaxLength, options => options.Ignore())
                .ForMember(settings => settings.PrivateMessagesPageSize, options => options.Ignore())
                .ForMember(settings => settings.StrippedTopicMaxLength, options => options.Ignore())
                .ForMember(settings => settings.TopicSubjectMaxLength, options => options.Ignore());
        }

        /// <summary>
        /// Create GDPR maps 
        /// </summary>
        protected virtual void CreateGdprMaps()
        {
            CreateMap<GdprSettings, GdprSettingsModel>()
                .ForMember(model => model.GdprConsentSearchModel, options => options.Ignore())
                .ForMember(model => model.GdprEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LogNewsletterConsent_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LogPrivacyPolicyConsent_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LogUserProfileChanges_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DeleteInactiveCustomersAfterMonths_OverrideForStore, options => options.Ignore());
            CreateMap<GdprSettingsModel, GdprSettings>();

            CreateMap<GdprConsent, GdprConsentModel>();
            CreateMap<GdprConsentModel, GdprConsent>();

            CreateMap<GdprLog, GdprLogModel>()
                .ForMember(model => model.CustomerInfo, options => options.Ignore())
                .ForMember(model => model.RequestType, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore());
        }

        /// <summary>
        /// Create localization maps 
        /// </summary>
        protected virtual void CreateLocalizationMaps()
        {
            CreateMap<Language, LanguageModel>()
                .ForMember(model => model.AvailableCurrencies, options => options.Ignore())
                .ForMember(model => model.LocaleResourceSearchModel, options => options.Ignore());
            CreateMap<LanguageModel, Language>();

            CreateMap<LocaleResourceModel, LocaleStringResource>()
                .ForMember(entity => entity.LanguageId, options => options.Ignore());
        }

        /// <summary>
        /// Create logging maps 
        /// </summary>
        protected virtual void CreateLoggingMaps()
        {
            CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(model => model.ActivityLogTypeName, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CustomerEmail, options => options.Ignore());
            CreateMap<ActivityLogModel, ActivityLog>()
                .ForMember(entity => entity.ActivityLogTypeId, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.EntityId, options => options.Ignore())
                .ForMember(entity => entity.EntityName, options => options.Ignore());

            CreateMap<ActivityLogType, ActivityLogTypeModel>();
            CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(entity => entity.SystemKeyword, options => options.Ignore());

            CreateMap<Log, LogModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.FullMessage, options => options.Ignore())
                .ForMember(model => model.CustomerEmail, options => options.Ignore());
            CreateMap<LogModel, Log>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LogLevelId, options => options.Ignore());
        }

        /// <summary>
        /// Create media maps 
        /// </summary>
        protected virtual void CreateMediaMaps()
        {
            CreateMap<MediaSettings, MediaSettingsModel>()
                .ForMember(model => model.AssociatedProductPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AvatarPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CartThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OrderThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CategoryThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultImageQuality_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultPictureZoomEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ImportProductImagesUsingHash_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ManufacturerThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumImageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MiniCartThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MultipleThumbDirectories_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PicturesStoredIntoDatabase, options => options.Ignore())
                .ForMember(model => model.ProductDetailsPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.VendorThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductDefaultImageId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowSVGUploads_OverrideForStore, options => options.Ignore());
            CreateMap<MediaSettingsModel, MediaSettings>()
                .ForMember(settings => settings.AutoCompleteSearchThumbPictureSize, options => options.Ignore())
                .ForMember(settings => settings.AzureCacheControlHeader, options => options.Ignore())
                .ForMember(settings => settings.UseAbsoluteImagePath, options => options.Ignore())
                .ForMember(settings => settings.ImageSquarePictureSize, options => options.Ignore())
                .ForMember(settings => settings.VideoIframeAllow, options => options.Ignore())
                .ForMember(settings => settings.VideoIframeHeight, options => options.Ignore())
                .ForMember(settings => settings.VideoIframeWidth, options => options.Ignore());
        }

        /// <summary>
        /// Create messages maps 
        /// </summary>
        protected virtual void CreateMessagesMaps()
        {
            CreateMap<Campaign, CampaignModel>()
                .ForMember(model => model.AllowedTokens, options => options.Ignore())
                .ForMember(model => model.AvailableCustomerRoles, options => options.Ignore())
                .ForMember(model => model.AvailableEmailAccounts, options => options.Ignore())
                .ForMember(model => model.AvailableStores, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.DontSendBeforeDate, options => options.Ignore())
                .ForMember(model => model.EmailAccountId, options => options.Ignore())
                .ForMember(model => model.TestEmail, options => options.Ignore());
            CreateMap<CampaignModel, Campaign>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore());

            CreateMap<EmailAccount, EmailAccountModel>()
                .ForMember(model => model.IsDefaultEmailAccount, options => options.Ignore())
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.SendTestEmailTo, options => options.Ignore());
            CreateMap<EmailAccountModel, EmailAccount>()
                .ForMember(entity => entity.Password, options => options.Ignore());

            CreateMap<MessageTemplate, MessageTemplateModel>()
                .ForMember(model => model.AllowedTokens, options => options.Ignore())
                .ForMember(model => model.AvailableEmailAccounts, options => options.Ignore())
                .ForMember(model => model.HasAttachedDownload, options => options.Ignore())
                .ForMember(model => model.ListOfStores, options => options.Ignore())
                .ForMember(model => model.SendImmediately, options => options.Ignore());
            CreateMap<MessageTemplateModel, MessageTemplate>()
                .ForMember(entity => entity.DelayPeriod, options => options.Ignore());

            CreateMap<NewsLetterSubscription, NewsletterSubscriptionModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());
            CreateMap<NewsletterSubscriptionModel, NewsLetterSubscription>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.NewsLetterSubscriptionGuid, options => options.Ignore())
                .ForMember(entity => entity.StoreId, options => options.Ignore());

            CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.DontSendBeforeDate, options => options.Ignore())
                .ForMember(model => model.EmailAccountName, options => options.Ignore())
                .ForMember(model => model.PriorityName, options => options.Ignore())
                .ForMember(model => model.SendImmediately, options => options.Ignore())
                .ForMember(model => model.SentOn, options => options.Ignore());
            CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(entity => entity.AttachmentFileName, options => options.Ignore())
                .ForMember(entity => entity.AttachmentFilePath, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore())
                .ForMember(entity => entity.EmailAccountId, options => options.Ignore())
                .ForMember(entity => entity.Priority, options => options.Ignore())
                .ForMember(entity => entity.PriorityId, options => options.Ignore())
                .ForMember(entity => entity.SentOnUtc, options => options.Ignore());
        }

        /// <summary>
        /// Create news maps 
        /// </summary>
        protected virtual void CreateNewsMaps()
        {
            CreateMap<NewsComment, NewsCommentModel>()
                .ForMember(model => model.CustomerInfo, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CommentText, options => options.Ignore())
                .ForMember(model => model.NewsItemTitle, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());
            CreateMap<NewsCommentModel, NewsComment>()
                .ForMember(entity => entity.CommentTitle, options => options.Ignore())
                .ForMember(entity => entity.CommentText, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.NewsItemId, options => options.Ignore())
                .ForMember(entity => entity.CustomerId, options => options.Ignore())
                .ForMember(entity => entity.StoreId, options => options.Ignore());

            CreateMap<NewsItem, NewsItemModel>()
                .ForMember(model => model.ApprovedComments, options => options.Ignore())
                .ForMember(model => model.AvailableLanguages, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LanguageName, options => options.Ignore())
                .ForMember(model => model.NotApprovedComments, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore());
            CreateMap<NewsItemModel, NewsItem>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

            CreateMap<NewsSettings, NewsSettingsModel>()
                .ForMember(model => model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MainPageNewsCount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewsArchivePageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewsCommentsMustBeApproved_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyAboutNewNewsComments_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowHeaderRssUrl_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowNewsOnMainPage_OverrideForStore, options => options.Ignore());
            CreateMap<NewsSettingsModel, NewsSettings>();
        }

        /// <summary>
        /// Create orders maps 
        /// </summary>
        protected virtual void CreateOrdersMaps()
        {
            CreateMap<Order, CustomerOrderModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.OrderStatus, options => options.Ignore())
                .ForMember(model => model.PaymentStatus, options => options.Ignore())
                .ForMember(model => model.ShippingStatus, options => options.Ignore())
                .ForMember(model => model.OrderTotal, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());

            CreateMap<OrderNote, OrderNoteModel>()
                .ForMember(model => model.DownloadGuid, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore());

            CreateMap<CheckoutAttribute, CheckoutAttributeModel>()
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
                .ForMember(model => model.AvailableTaxCategories, options => options.Ignore())
                .ForMember(model => model.CheckoutAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.ConditionAllowed, options => options.Ignore())
                .ForMember(model => model.ConditionModel, options => options.Ignore());
            CreateMap<CheckoutAttributeModel, CheckoutAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore())
                .ForMember(entity => entity.ConditionAttributeXml, options => options.Ignore());

            CreateMap<CheckoutAttributeValue, CheckoutAttributeValueModel>()
                .ForMember(model => model.BaseWeightIn, options => options.Ignore())
                .ForMember(model => model.DisplayColorSquaresRgb, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<CheckoutAttributeValueModel, CheckoutAttributeValue>();

            CreateMap<GiftCard, GiftCardModel>()
                .ForMember(model => model.AmountStr, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.GiftCardUsageHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.PurchasedWithOrderId, options => options.Ignore())
                .ForMember(model => model.PurchasedWithOrderNumber, options => options.Ignore())
                .ForMember(model => model.RemainingAmountStr, options => options.Ignore());
            CreateMap<GiftCardModel, GiftCard>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.GiftCardType, options => options.Ignore())
                .ForMember(entity => entity.IsRecipientNotified, options => options.Ignore())
                .ForMember(entity => entity.PurchasedWithOrderItemId, options => options.Ignore());

            CreateMap<GiftCardUsageHistory, GiftCardUsageHistoryModel>()
                .ForMember(model => model.OrderId, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumber, options => options.Ignore())
                .ForMember(entity => entity.CreatedOn, options => options.Ignore())
                .ForMember(entity => entity.UsedValue, options => options.Ignore());

            CreateMap<OrderSettings, OrderSettingsModel>()
                .ForMember(model => model.AllowAdminsToBuyCallForPriceProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductThumbnailInOrderDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AnonymousCheckoutAllowed_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderProcessingEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CheckoutDisabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumberMask_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DeleteGiftCardUsageHistory_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisableBillingAddressCheckoutStep_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisableOrderCompletedPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayPickupInStoreOnShippingMethodPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportWithProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IsReOrderAllowed_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinOrderSubtotalAmountIncludingTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinOrderSubtotalAmount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinOrderTotalAmount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfDaysReturnRequestAvailable_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OnePageCheckoutEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OrderIdent, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.ReturnRequestActionSearchModel, options => options.Ignore())
                .ForMember(model => model.ReturnRequestNumberMask_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReturnRequestReasonSearchModel, options => options.Ignore())
                .ForMember(model => model.ReturnRequestsAllowFiles_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReturnRequestsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TermsOfServiceOnOrderConfirmPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TermsOfServiceOnShoppingCartPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<OrderSettingsModel, OrderSettings>()
                .ForMember(settings => settings.GeneratePdfInvoiceInCustomerLanguage, options => options.Ignore())
                .ForMember(settings => settings.MinimumOrderPlacementInterval, options => options.Ignore())
                .ForMember(settings => settings.DisplayCustomerCurrencyOnOrders, options => options.Ignore())
                .ForMember(settings => settings.ReturnRequestsFileMaximumSize, options => options.Ignore())
                .ForMember(settings => settings.DisplayOrderSummary, options => options.Ignore());

            CreateMap<ReturnRequestAction, ReturnRequestActionModel>();
            CreateMap<ReturnRequestActionModel, ReturnRequestAction>();

            CreateMap<ReturnRequestReason, ReturnRequestReasonModel>();
            CreateMap<ReturnRequestReasonModel, ReturnRequestReason>();

            CreateMap<ReturnRequest, ReturnRequestModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CustomerInfo, options => options.Ignore())
                .ForMember(model => model.ReturnRequestStatusStr, options => options.Ignore())
                .ForMember(model => model.ProductId, options => options.Ignore())
                .ForMember(model => model.ProductName, options => options.Ignore())
                .ForMember(model => model.OrderId, options => options.Ignore())
                .ForMember(model => model.AttributeInfo, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumber, options => options.Ignore())
                .ForMember(model => model.UploadedFileGuid, options => options.Ignore())
                .ForMember(model => model.ReturnRequestStatusStr, options => options.Ignore());
            CreateMap<ReturnRequestModel, ReturnRequest>()
                 .ForMember(entity => entity.CustomNumber, options => options.Ignore())
                 .ForMember(entity => entity.StoreId, options => options.Ignore())
                 .ForMember(entity => entity.OrderItemId, options => options.Ignore())
                 .ForMember(entity => entity.UploadedFileId, options => options.Ignore())
                 .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                 .ForMember(entity => entity.ReturnRequestStatus, options => options.Ignore())
                 .ForMember(entity => entity.CustomerId, options => options.Ignore())
                 .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ShoppingCartSettings, ShoppingCartSettingsModel>()
                .ForMember(model => model.AllowAnonymousUsersToEmailWishlist_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowCartItemEditing_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CartsSharedBetweenStores_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CrossSellsNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.GroupTierPricesForDistinctShoppingCartItems_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayCartAfterAddingProduct_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayWishlistAfterAddingProduct_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EmailWishlistEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumShoppingCartItems_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumWishlistItems_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MiniShoppingCartEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MiniShoppingCartProductNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MoveItemsFromWishlistToCart_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowDiscountBox_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowGiftCardBox_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductImagesInMiniShoppingCart_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductImagesOnShoppingCart_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductImagesOnWishList_OverrideForStore, options => options.Ignore());
            CreateMap<ShoppingCartSettingsModel, ShoppingCartSettings>()
                .ForMember(settings => settings.RenderAssociatedAttributeValueQuantity, options => options.Ignore())
                .ForMember(settings => settings.RoundPricesDuringCalculation, options => options.Ignore());

            CreateMap<ShoppingCartItem, ShoppingCartItemModel>()
                .ForMember(model => model.Store, options => options.Ignore())
                .ForMember(model => model.AttributeInfo, options => options.Ignore())
                .ForMember(model => model.UnitPrice, options => options.Ignore())
                .ForMember(model => model.UnitPriceValue, options => options.Ignore())
                .ForMember(model => model.UpdatedOn, options => options.Ignore())
                .ForMember(model => model.ProductName, options => options.Ignore())
                .ForMember(model => model.Total, options => options.Ignore())
                .ForMember(model => model.TotalValue, options => options.Ignore());
        }

        /// <summary>
        /// Create payments maps 
        /// </summary>
        protected virtual void CreatePaymentsMaps()
        {
            CreateMap<IPaymentMethod, PaymentMethodModel>()
                .ForMember(model => model.RecurringPaymentType, options => options.Ignore());

            CreateMap<RecurringPayment, RecurringPaymentModel>()
                .ForMember(model => model.CustomerId, options => options.Ignore())
                .ForMember(model => model.InitialOrderId, options => options.Ignore())
                .ForMember(model => model.NextPaymentDate, options => options.Ignore())
                .ForMember(model => model.StartDate, options => options.Ignore())
                .ForMember(model => model.CyclePeriodStr, options => options.Ignore())
                .ForMember(model => model.PaymentType, options => options.Ignore())
                .ForMember(model => model.CanCancelRecurringPayment, options => options.Ignore())
                .ForMember(model => model.CustomerEmail, options => options.Ignore())
                .ForMember(model => model.RecurringPaymentHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.CyclesRemaining, options => options.Ignore());

            CreateMap<RecurringPaymentModel, RecurringPayment>()
                .ForMember(entity => entity.StartDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CyclePeriod, options => options.Ignore())
                .ForMember(entity => entity.InitialOrderId, options => options.Ignore());

            CreateMap<RecurringPaymentHistory, RecurringPaymentHistoryModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.OrderStatus, options => options.Ignore())
                .ForMember(model => model.PaymentStatus, options => options.Ignore())
                .ForMember(model => model.ShippingStatus, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumber, options => options.Ignore());
        }

        /// <summary>
        /// Create plugins maps 
        /// </summary>
        protected virtual void CreatePluginsMaps()
        {
            CreateMap<PluginDescriptor, PluginModel>()
                .ForMember(model => model.CanChangeEnabled, options => options.Ignore())
                .ForMember(model => model.IsEnabled, options => options.Ignore());
        }

        /// <summary>
        /// Create polls maps 
        /// </summary>
        protected virtual void CreatePollsMaps()
        {
            CreateMap<PollAnswer, PollAnswerModel>();
            CreateMap<PollAnswerModel, PollAnswer>();

            CreateMap<Poll, PollModel>()
                .ForMember(model => model.AvailableLanguages, options => options.Ignore())
                .ForMember(model => model.PollAnswerSearchModel, options => options.Ignore())
                .ForMember(model => model.LanguageName, options => options.Ignore());
            CreateMap<PollModel, Poll>();
        }

        /// <summary>
        /// Create security maps 
        /// </summary>
        protected virtual void CreateSecurityMaps()
        {
            CreateMap<CaptchaSettings, CaptchaSettingsModel>()
                .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReCaptchaPrivateKey_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReCaptchaPublicKey_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnApplyVendorPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnBlogCommentPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnContactUsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnEmailProductToFriendPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnEmailWishlistToFriendPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnLoginPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnNewsCommentPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnProductReviewPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnRegistrationPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnForgotPasswordPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnForum_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnCheckoutPageForGuests_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CaptchaType_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReCaptchaV3ScoreThreshold_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CaptchaTypeValues, options => options.Ignore());
            CreateMap<CaptchaSettingsModel, CaptchaSettings>()
                .ForMember(settings => settings.AutomaticallyChooseLanguage, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaDefaultLanguage, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaRequestTimeout, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaTheme, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaApiUrl, options => options.Ignore());
        }

        /// <summary>
        /// Create SEO maps 
        /// </summary>
        protected virtual void CreateSeoMaps()
        {
            CreateMap<UrlRecord, UrlRecordModel>()
                .ForMember(model => model.DetailsUrl, options => options.Ignore())
                .ForMember(model => model.Language, options => options.Ignore())
                .ForMember(model => model.Name, options => options.Ignore());
            CreateMap<UrlRecordModel, UrlRecord>()
                .ForMember(entity => entity.LanguageId, options => options.Ignore())
                .ForMember(entity => entity.Slug, options => options.Ignore());
        }

        /// <summary>
        /// Create shipping maps 
        /// </summary>
        protected virtual void CreateShippingMaps()
        {
            CreateMap<DeliveryDate, DeliveryDateModel>();
            CreateMap<DeliveryDateModel, DeliveryDate>();

            CreateMap<IPickupPointProvider, PickupPointProviderModel>();

            CreateMap<ProductAvailabilityRange, ProductAvailabilityRangeModel>();
            CreateMap<ProductAvailabilityRangeModel, ProductAvailabilityRange>();

            CreateMap<ShippingMethod, ShippingMethodModel>();
            CreateMap<ShippingMethodModel, ShippingMethod>();

            CreateMap<IShippingRateComputationMethod, ShippingProviderModel>();

            CreateMap<Shipment, ShipmentModel>()
                .ForMember(model => model.ShippedDate, options => options.Ignore())
                .ForMember(model => model.ReadyForPickupDate, options => options.Ignore())
                .ForMember(model => model.DeliveryDate, options => options.Ignore())
                .ForMember(model => model.TotalWeight, options => options.Ignore())
                .ForMember(model => model.TrackingNumberUrl, options => options.Ignore())
                .ForMember(model => model.Items, options => options.Ignore())
                .ForMember(model => model.ShipmentStatusEvents, options => options.Ignore())
                .ForMember(model => model.PickupInStore, options => options.Ignore())
                .ForMember(model => model.CanShip, options => options.Ignore())
                .ForMember(model => model.CanMarkAsReadyForPickup, options => options.Ignore())
                .ForMember(model => model.CanDeliver, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumber, options => options.Ignore());

            CreateMap<ShippingSettings, ShippingSettingsModel>()
                .ForMember(model => model.AllowPickupInStore_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ConsiderAssociatedProductsDimensions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SortShippingValues, options => options.Ignore())
                .ForMember(model => model.ShippingSorting_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayPickupPointsOnMap_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayShipmentEventsToCustomers_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayShipmentEventsToStoreOwner_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EstimateShippingCartPageEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EstimateShippingProductPageEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EstimateShippingCityNameEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FreeShippingOverXEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FreeShippingOverXIncludingTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FreeShippingOverXValue_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.GoogleMapsApiKey_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.HideShippingTotal_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.ShippingOriginAddress, options => options.Ignore())
                .ForMember(model => model.ShippingOriginAddress_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShipToSameAddress_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.UseWarehouseLocation_OverrideForStore, options => options.Ignore());
            CreateMap<ShippingSettingsModel, ShippingSettings>()
                .ForMember(settings => settings.ActivePickupPointProviderSystemNames, options => options.Ignore())
                .ForMember(settings => settings.ActiveShippingRateComputationMethodSystemNames, options => options.Ignore())
                .ForMember(settings => settings.ReturnValidOptionsIfThereAreAny, options => options.Ignore())
                .ForMember(settings => settings.ShipSeparatelyOneItemEach, options => options.Ignore())
                .ForMember(settings => settings.UseCubeRootMethod, options => options.Ignore())
                .ForMember(settings => settings.RequestDelay, options => options.Ignore());
        }

        /// <summary>
        /// Create stores maps 
        /// </summary>
        protected virtual void CreateStoresMaps()
        {
            CreateMap<Store, StoreModel>()
                .ForMember(model => model.AvailableLanguages, options => options.Ignore());
            CreateMap<StoreModel, Store>()
                .ForMember(entity => entity.SslEnabled, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore());
        }

        /// <summary>
        /// Create tasks maps 
        /// </summary>
        protected virtual void CreateTasksMaps()
        {
            CreateMap<ScheduleTask, ScheduleTaskModel>();
            CreateMap<ScheduleTaskModel, ScheduleTask>()
                .ForMember(entity => entity.Type, options => options.Ignore())
                .ForMember(entity => entity.LastStartUtc, options => options.Ignore())
                .ForMember(entity => entity.LastEndUtc, options => options.Ignore())
                .ForMember(entity => entity.LastSuccessUtc, options => options.Ignore())
                .ForMember(entity => entity.LastEnabledUtc, options => options.Ignore());
        }

        /// <summary>
        /// Create tax maps 
        /// </summary>
        protected virtual void CreateTaxMaps()
        {
            CreateMap<TaxCategory, TaxCategoryModel>();
            CreateMap<TaxCategoryModel, TaxCategory>();

            CreateMap<ITaxProvider, TaxProviderModel>()
                .ForMember(model => model.IsPrimaryTaxProvider, options => options.Ignore());

            CreateMap<TaxSettings, TaxSettingsModel>()
                .ForMember(model => model.AllowCustomersToSelectTaxDisplayType_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultTaxAddress, options => options.Ignore())
                .ForMember(model => model.DefaultTaxAddress_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultTaxCategoryId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxRates_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxSuffix_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatAllowVatExemption_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatAssumeValid_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatEnabledForGuests_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatShopCountries, options => options.Ignore())
                .ForMember(model => model.EuVatShopCountryId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatUseWebService_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.HideTaxInOrderSummary_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.HideZeroTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PricesIncludeTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShippingIsTaxable_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShippingPriceIncludesTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShippingTaxClassId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TaxBasedOnPickupPointAddress_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TaxBasedOnValues, options => options.Ignore())
                .ForMember(model => model.TaxBasedOn_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TaxCategories, options => options.Ignore())
                .ForMember(model => model.TaxDisplayTypeValues, options => options.Ignore())
                .ForMember(model => model.TaxDisplayType_OverrideForStore, options => options.Ignore());
            CreateMap<TaxSettingsModel, TaxSettings>()
                .ForMember(settings => settings.ActiveTaxProviderSystemName, options => options.Ignore())
                .ForMember(settings => settings.LogErrors, options => options.Ignore());
        }

        /// <summary>
        /// Create topics maps 
        /// </summary>
        protected virtual void CreateTopicsMaps()
        {
            CreateMap<Topic, TopicModel>()
                .ForMember(model => model.AvailableTopicTemplates, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.TopicName, options => options.Ignore())
                .ForMember(model => model.Url, options => options.Ignore());
            CreateMap<TopicModel, Topic>();

            CreateMap<TopicTemplate, TopicTemplateModel>();
            CreateMap<TopicTemplateModel, TopicTemplate>();
        }

        /// <summary>
        /// Create vendors maps 
        /// </summary>
        protected virtual void CreateVendorsMaps()
        {
            CreateMap<Vendor, VendorModel>()
                .ForMember(model => model.Address, options => options.Ignore())
                .ForMember(model => model.AddVendorNoteMessage, options => options.Ignore())
                .ForMember(model => model.AssociatedCustomers, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.VendorAttributes, options => options.Ignore())
                .ForMember(model => model.VendorNoteSearchModel, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<VendorModel, Vendor>()
                .ForMember(entity => entity.Deleted, options => options.Ignore());

            CreateMap<VendorNote, VendorNoteModel>()
               .ForMember(model => model.CreatedOn, options => options.Ignore())
               .ForMember(model => model.Note, options => options.Ignore());

            CreateMap<VendorAttribute, VendorAttributeModel>()
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
                .ForMember(model => model.VendorAttributeValueSearchModel, options => options.Ignore());
            CreateMap<VendorAttributeModel, VendorAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

            CreateMap<VendorAttributeValue, VendorAttributeValueModel>();
            CreateMap<VendorAttributeValueModel, VendorAttributeValue>();

            CreateMap<VendorSettings, VendorSettingsModel>()
                .ForMember(model => model.AllowCustomersToApplyForVendorAccount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowCustomersToContactVendors_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowSearchByVendor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowVendorsToEditInfo_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowVendorsToImportProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumProductNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowVendorOnOrderDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowVendorOnProductDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TermsOfServiceEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.VendorAttributeSearchModel, options => options.Ignore())
                .ForMember(model => model.VendorsBlockItemsToDisplay_OverrideForStore, options => options.Ignore());
            CreateMap<VendorSettingsModel, VendorSettings>()
                .ForMember(settings => settings.DefaultVendorPageSizeOptions, options => options.Ignore());
        }

        /// <summary>
        /// Create warehouse maps 
        /// </summary>
        protected virtual void CreateWarehouseMaps()
        {
            CreateMap<Warehouse, WarehouseModel>()
                .ForMember(entity => entity.Address, options => options.Ignore());
            CreateMap<WarehouseModel, Warehouse>()
                .ForMember(entity => entity.AddressId, options => options.Ignore());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

        #endregion
    }
}