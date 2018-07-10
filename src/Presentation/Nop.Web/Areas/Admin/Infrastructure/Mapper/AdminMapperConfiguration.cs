using AutoMapper;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
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
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Payments;
using Nop.Services.Seo;
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
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration for admin area models
    /// </summary>
    public class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public AdminMapperConfiguration()
        {
            //create specific maps
            CreateAffiliatesMaps();
            CreateAuthenticationMaps();
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

            //add some generic mapping rules
            ForAllMaps((mapConfiguration, map) =>
            {
                //exclude Form and CustomProperties from mapping BaseNopModel
                if (typeof(BaseNopModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(BaseNopModel.Form), options => options.Ignore());
                    map.ForMember(nameof(BaseNopModel.CustomProperties), options => options.Ignore());
                }

                //exclude ActiveStoreScopeConfiguration from mapping ISettingsModel
                if (typeof(ISettingsModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ISettingsModel.ActiveStoreScopeConfiguration), options => options.Ignore());

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
                if (typeof(IDiscountSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IDiscountSupported.AppliedDiscounts), options => options.Ignore());
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
                .ForMember(entity => entity.Address, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore());
        }

        /// <summary>
        /// Create authentication maps 
        /// </summary>
        protected virtual void CreateAuthenticationMaps()
        {
            CreateMap<IExternalAuthenticationMethod, ExternalAuthenticationMethodModel>();
        }

        /// <summary>
        /// Create blogs maps 
        /// </summary>
        protected virtual void CreateBlogsMaps()
        {
            CreateMap<BlogComment, BlogCommentModel>()
                .ForMember(model => model.Comment, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CustomerInfo, options => options.Ignore());
            CreateMap<BlogCommentModel, BlogComment>()
                .ForMember(entity => entity.BlogPost, options => options.Ignore())
                .ForMember(entity => entity.CommentText, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Customer, options => options.Ignore())
                .ForMember(entity => entity.Store, options => options.Ignore());

            CreateMap<BlogPost, BlogPostModel>()
                .ForMember(model => model.ApprovedComments, options => options.Ignore())
                .ForMember(model => model.AvailableLanguages, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.EndDate, options => options.Ignore())
                .ForMember(model => model.NotApprovedComments, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.MapFrom(entity => EngineContext.Current.Resolve<IUrlRecordService>().GetSeName(entity, entity.LanguageId, true, false)))
                .ForMember(model => model.StartDate, options => options.Ignore());
            CreateMap<BlogPostModel, BlogPost>()
                .ForMember(entity => entity.BlogComments, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.EndDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Language, options => options.Ignore())
                .ForMember(entity => entity.StartDateUtc, options => options.Ignore());

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
                .ForMember(model => model.ExportImportSplitProductsFile_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IncludeFullDescriptionInCompareProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IncludeShortDescriptionInCompareProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ManufacturersBlockItemsToDisplay_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyCustomerAboutProductReviewReply_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfBestsellersOnHomepage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfProductTags_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PageShareCode_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewsMustBeApproved_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewsPageSizeOnAccountPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewsSortByCreatedDateAscending_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsAlsoPurchasedEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsAlsoPurchasedNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsByTagPageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsByTagPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductSearchAutoCompleteEnabled_OverrideForStore, options => options.Ignore())
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
                .ForMember(model => model.SortOptionSearchModel, options => options.Ignore())
                .ForMember(model => model.ReviewTypeSearchModel, options => options.Ignore());
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
                .ForMember(settings => settings.UseLinksInRequiredProductWarnings, options => options.Ignore());

            CreateMap<Category, CategoryModel>()
                .ForMember(model => model.AvailableCategories, options => options.Ignore())
                .ForMember(model => model.AvailableCategoryTemplates, options => options.Ignore())
                .ForMember(model => model.Breadcrumb, options => options.Ignore())
                .ForMember(model => model.CategoryProductSearchModel, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.MapFrom(entity => EngineContext.Current.Resolve<IUrlRecordService>().GetSeName(entity, 0, true, false)));
            CreateMap<CategoryModel, Category>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.DiscountCategoryMappings, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<CategoryTemplate, CategoryTemplateModel>();
            CreateMap<CategoryTemplateModel, CategoryTemplate>();

            CreateMap<Manufacturer, ManufacturerModel>()
                .ForMember(model => model.AvailableManufacturerTemplates, options => options.Ignore())
                .ForMember(model => model.ManufacturerProductSearchModel, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.MapFrom(entity => EngineContext.Current.Resolve<IUrlRecordService>().GetSeName(entity, 0, true, false)));
            CreateMap<ManufacturerModel, Manufacturer>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.DiscountManufacturerMappings, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ManufacturerTemplate, ManufacturerTemplateModel>();
            CreateMap<ManufacturerTemplateModel, ManufacturerTemplate>();

            //Review type
            CreateMap<ReviewType, ReviewTypeModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                .ForMember(dest => dest.Form, mo => mo.Ignore());

            //products
            CreateMap<Product, ProductModel>()
                .ForMember(model => model.AddPictureModel, options => options.Ignore())
                .ForMember(model => model.AddSpecificationAttributeModel, options => options.Ignore())
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
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CrossSellProductSearchModel, options => options.Ignore())
                .ForMember(model => model.IsLoggedInAsVendor, options => options.Ignore())
                .ForMember(model => model.LastStockQuantity, options => options.Ignore())
                .ForMember(model => model.PictureThumbnailUrl, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.ProductAttributeCombinationSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductAttributeMappingSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductAttributesExist, options => options.Ignore())
                .ForMember(model => model.ProductEditorSettingsModel, options => options.Ignore())
                .ForMember(model => model.ProductOrderSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductPictureModels, options => options.Ignore())
                .ForMember(model => model.ProductPictureSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductSpecificationAttributeSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductsTypesSupportedByProductTemplates, options => options.Ignore())
                .ForMember(model => model.ProductTags, options => options.Ignore())
                .ForMember(model => model.ProductTypeName, options => options.Ignore())
                .ForMember(model => model.ProductWarehouseInventoryModels, options => options.Ignore())
                .ForMember(model => model.RelatedProductSearchModel, options => options.Ignore())
                .ForMember(model => model.SelectedCategoryIds, options => options.Ignore())
                .ForMember(model => model.SelectedManufacturerIds, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.MapFrom(entity => EngineContext.Current.Resolve<IUrlRecordService>().GetSeName(entity, 0, true, false)))
                .ForMember(model => model.StockQuantityHistory, options => options.Ignore())
                .ForMember(model => model.StockQuantityHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.StockQuantityStr, options => options.Ignore())
                .ForMember(model => model.TierPriceSearchModel, options => options.Ignore())
                .ForMember(model => model.UpdatedOn, options => options.Ignore());
            CreateMap<ProductModel, Product>()
                .ForMember(entity => entity.ApprovedRatingSum, options => options.Ignore())
                .ForMember(entity => entity.ApprovedTotalReviews, options => options.Ignore())
                .ForMember(entity => entity.BackorderMode, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.DiscountProductMappings, options => options.Ignore())
                .ForMember(entity => entity.DownloadActivationType, options => options.Ignore())
                .ForMember(entity => entity.GiftCardType, options => options.Ignore())
                .ForMember(entity => entity.HasDiscountsApplied, options => options.Ignore())
                .ForMember(entity => entity.HasTierPrices, options => options.Ignore())
                .ForMember(entity => entity.LowStockActivity, options => options.Ignore())
                .ForMember(entity => entity.ManageInventoryMethod, options => options.Ignore())
                .ForMember(entity => entity.NotApprovedRatingSum, options => options.Ignore())
                .ForMember(entity => entity.NotApprovedTotalReviews, options => options.Ignore())
                .ForMember(entity => entity.ParentGroupedProductId, options => options.Ignore())
                .ForMember(entity => entity.ProductAttributeCombinations, options => options.Ignore())
                .ForMember(entity => entity.ProductAttributeMappings, options => options.Ignore())
                .ForMember(entity => entity.ProductCategories, options => options.Ignore())
                .ForMember(entity => entity.ProductManufacturers, options => options.Ignore())
                .ForMember(entity => entity.ProductPictures, options => options.Ignore())
                .ForMember(entity => entity.ProductProductTagMappings, options => options.Ignore())
                .ForMember(entity => entity.ProductReviews, options => options.Ignore())
                .ForMember(entity => entity.ProductSpecificationAttributes, options => options.Ignore())
                .ForMember(entity => entity.ProductType, options => options.Ignore())
                .ForMember(entity => entity.ProductWarehouseInventory, options => options.Ignore())
                .ForMember(entity => entity.RecurringCyclePeriod, options => options.Ignore())
                .ForMember(entity => entity.RentalPricePeriod, options => options.Ignore())
                .ForMember(entity => entity.TierPrices, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ProductAttribute, ProductAttributeModel>()
                .ForMember(model => model.PredefinedProductAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductAttributeProductSearchModel, options => options.Ignore());
            CreateMap<ProductAttributeModel, ProductAttribute>();

            CreateMap<ProductEditorSettings, ProductEditorSettingsModel>();
            CreateMap<ProductEditorSettingsModel, ProductEditorSettings>();

            CreateMap<ProductTemplate, ProductTemplateModel>();
            CreateMap<ProductTemplateModel, ProductTemplate>();

            CreateMap<SpecificationAttribute, SpecificationAttributeModel>()
                .ForMember(model => model.SpecificationAttributeOptionSearchModel, options => options.Ignore())
                .ForMember(model => model.SpecificationAttributeProductSearchModel, options => options.Ignore());
            CreateMap<SpecificationAttributeModel, SpecificationAttribute>()
                .ForMember(entity => entity.SpecificationAttributeOptions, options => options.Ignore());

            CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionModel>()
                .ForMember(model => model.EnableColorSquaresRgb, options => options.Ignore())
                .ForMember(model => model.NumberOfAssociatedProducts, options => options.Ignore());
            CreateMap<SpecificationAttributeOptionModel, SpecificationAttributeOption>()
                .ForMember(entity => entity.SpecificationAttribute, options => options.Ignore());
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
                .ForMember(model => model.CityEnabled, options => options.Ignore())
                .ForMember(model => model.CityRequired, options => options.Ignore())
                .ForMember(model => model.CompanyEnabled, options => options.Ignore())
                .ForMember(model => model.CompanyRequired, options => options.Ignore())
                .ForMember(model => model.CountryEnabled, options => options.Ignore())
                .ForMember(model => model.CountryName, options => options.MapFrom(entity => entity.Country != null ? entity.Country.Name : null))
                .ForMember(model => model.CountryRequired, options => options.Ignore())
                .ForMember(model => model.CountyEnabled, options => options.Ignore())
                .ForMember(model => model.CountyRequired, options => options.Ignore())
                .ForMember(model => model.CustomAddressAttributes, options => options.Ignore())
                .ForMember(model => model.EmailEnabled, options => options.Ignore())
                .ForMember(model => model.EmailRequired, options => options.Ignore())
                .ForMember(model => model.FaxEnabled, options => options.Ignore())
                .ForMember(model => model.FaxRequired, options => options.Ignore())
                .ForMember(model => model.FirstNameEnabled, options => options.Ignore())
                .ForMember(model => model.FirstNameRequired, options => options.Ignore())
                .ForMember(model => model.FormattedCustomAddressAttributes, options => options.Ignore())
                .ForMember(model => model.LastNameEnabled, options => options.Ignore())
                .ForMember(model => model.LastNameRequired, options => options.Ignore())
                .ForMember(model => model.PhoneEnabled, options => options.Ignore())
                .ForMember(model => model.PhoneRequired, options => options.Ignore())
                .ForMember(model => model.StateProvinceEnabled, options => options.Ignore())
                .ForMember(model => model.StateProvinceName, options => options.MapFrom(entity => entity.StateProvince != null ? entity.StateProvince.Name : null))
                .ForMember(model => model.StreetAddress2Enabled, options => options.Ignore())
                .ForMember(model => model.StreetAddress2Required, options => options.Ignore())
                .ForMember(model => model.StreetAddressEnabled, options => options.Ignore())
                .ForMember(model => model.StreetAddressRequired, options => options.Ignore())
                .ForMember(model => model.ZipPostalCodeEnabled, options => options.Ignore())
                .ForMember(model => model.ZipPostalCodeRequired, options => options.Ignore());
            CreateMap<AddressModel, Address>()
                .ForMember(entity => entity.Country, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CustomAttributes, options => options.Ignore())
                .ForMember(entity => entity.StateProvince, options => options.Ignore());

            CreateMap<AddressAttribute, AddressAttributeModel>()
                .ForMember(model => model.AddressAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore());
            CreateMap<AddressAttributeModel, AddressAttribute>()
                .ForMember(entity => entity.AddressAttributeValues, options => options.Ignore())
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

            CreateMap<AddressAttributeValue, AddressAttributeValueModel>();
            CreateMap<AddressAttributeValueModel, AddressAttributeValue>()
                .ForMember(entity => entity.AddressAttribute, options => options.Ignore());

            CreateMap<AddressSettings, AddressSettingsModel>();
            CreateMap<AddressSettingsModel, AddressSettings>()
                .ForMember(settings => settings.PreselectCountryIfOnlyOne, options => options.Ignore());
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
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore())
                .ForMember(entity => entity.CustomerAttributeValues, options => options.Ignore());

            CreateMap<CustomerAttributeValue, CustomerAttributeValueModel>();
            CreateMap<CustomerAttributeValueModel, CustomerAttributeValue>()
                .ForMember(entity => entity.CustomerAttribute, options => options.Ignore());

            CreateMap<CustomerRole, CustomerRoleModel>()
                .ForMember(model => model.PurchasedWithProductName, options => options.Ignore())
                .ForMember(model => model.TaxDisplayTypeValues, options => options.Ignore());
            CreateMap<CustomerRoleModel, CustomerRole>()
                .ForMember(entity => entity.PermissionRecordCustomerRoleMappings, options => options.Ignore());

            CreateMap<CustomerSettings, CustomerSettingsModel>();
            CreateMap<CustomerSettingsModel, CustomerSettings>()
                .ForMember(settings => settings.AvatarMaximumSizeBytes, options => options.Ignore())
                .ForMember(settings => settings.DeleteGuestTaskOlderThanMinutes, options => options.Ignore())
                .ForMember(settings => settings.DownloadableProductsValidateUser, options => options.Ignore())
                .ForMember(settings => settings.HashedPasswordFormat, options => options.Ignore())
                .ForMember(settings => settings.OnlineCustomerMinutes, options => options.Ignore())
                .ForMember(settings => settings.SuffixDeletedCustomers, options => options.Ignore());

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
                .ForMember(model => model.PointsForRegistration_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.PurchasesPointsValidity_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RegistrationPointsValidity_OverrideForStore, options => options.Ignore());
            CreateMap<RewardPointsSettingsModel, RewardPointsSettings>();
        }

        /// <summary>
        /// Create directory maps 
        /// </summary>
        protected virtual void CreateDirectoryMaps()
        {
            CreateMap<Country, CountryModel>()
                .ForMember(model => model.NumberOfStates, options => options.MapFrom(entity => entity.StateProvinces != null ? entity.StateProvinces.Count : 0))
                .ForMember(model => model.StateProvinceSearchModel, options => options.Ignore());
            CreateMap<CountryModel, Country>()
                .ForMember(entity => entity.ShippingMethodCountryMappings, options => options.Ignore())
                .ForMember(entity => entity.StateProvinces, options => options.Ignore());

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
            CreateMap<StateProvinceModel, StateProvince>()
                .ForMember(entity => entity.Country, options => options.Ignore());
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
                .ForMember(entity => entity.DiscountCategoryMappings, options => options.Ignore())
                .ForMember(entity => entity.DiscountLimitation, options => options.Ignore())
                .ForMember(entity => entity.DiscountManufacturerMappings, options => options.Ignore())
                .ForMember(entity => entity.DiscountProductMappings, options => options.Ignore())
                .ForMember(entity => entity.DiscountRequirements, options => options.Ignore())
                .ForMember(entity => entity.DiscountType, options => options.Ignore());
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
                .ForMember(entity => entity.ForumGroup, options => options.Ignore())
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
                .ForMember(entity => entity.Forums, options => options.Ignore())
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
                .ForMember(settings => settings.HomePageActiveDiscussionsTopicCount, options => options.Ignore())
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
                .ForMember(model => model.LogPrivacyPolicyConsent_OverrideForStore, options => options.Ignore());
            CreateMap<GdprSettingsModel, GdprSettings>();

            CreateMap<GdprConsent, GdprConsentModel>();
            CreateMap<GdprConsentModel, GdprConsent>();
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
        }

        /// <summary>
        /// Create logging maps 
        /// </summary>
        protected virtual void CreateLoggingMaps()
        {
            CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(model => model.ActivityLogTypeName, options => options.MapFrom(entity => entity.ActivityLogType.Name))
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CustomerEmail, options => options.MapFrom(entity => entity.Customer.Email));
            CreateMap<ActivityLogModel, ActivityLog>()
                .ForMember(entity => entity.ActivityLogType, options => options.Ignore())
                .ForMember(entity => entity.ActivityLogTypeId, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Customer, options => options.Ignore())
                .ForMember(entity => entity.EntityId, options => options.Ignore())
                .ForMember(entity => entity.EntityName, options => options.Ignore());

            CreateMap<ActivityLogType, ActivityLogTypeModel>();
            CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(entity => entity.SystemKeyword, options => options.Ignore());

            CreateMap<Log, LogModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CustomerEmail, options => options.Ignore());
            CreateMap<LogModel, Log>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Customer, options => options.Ignore())
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
                .ForMember(model => model.VendorThumbPictureSize_OverrideForStore, options => options.Ignore());
            CreateMap<MediaSettingsModel, MediaSettings>()
                .ForMember(settings => settings.AutoCompleteSearchThumbPictureSize, options => options.Ignore())
                .ForMember(settings => settings.AzureCacheControlHeader, options => options.Ignore())
                .ForMember(settings => settings.ImageSquarePictureSize, options => options.Ignore());
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
                .ForMember(model => model.EmailAccountName, options => options.MapFrom(entity => entity.EmailAccount != null ? entity.EmailAccount.FriendlyName : string.Empty))
                .ForMember(model => model.PriorityName, options => options.Ignore())
                .ForMember(model => model.SendImmediately, options => options.Ignore())
                .ForMember(model => model.SentOn, options => options.Ignore());
            CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(entity => entity.AttachmentFileName, options => options.Ignore())
                .ForMember(entity => entity.AttachmentFilePath, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore())
                .ForMember(entity => entity.EmailAccount, options => options.Ignore())
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
            CreateMap<NewsItem, NewsItemModel>()
                .ForMember(model => model.ApprovedComments, options => options.Ignore())
                .ForMember(model => model.AvailableLanguages, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.EndDate, options => options.Ignore())
                .ForMember(model => model.NotApprovedComments, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.MapFrom(entity => EngineContext.Current.Resolve<IUrlRecordService>().GetSeName(entity, entity.LanguageId, true, false)))
                .ForMember(model => model.StartDate, options => options.Ignore());
            CreateMap<NewsItemModel, NewsItem>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.EndDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Language, options => options.Ignore())
                .ForMember(entity => entity.NewsComments, options => options.Ignore())
                .ForMember(entity => entity.StartDateUtc, options => options.Ignore());

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
            CreateMap<CheckoutAttribute, CheckoutAttributeModel>()
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
                .ForMember(model => model.AvailableTaxCategories, options => options.Ignore())
                .ForMember(model => model.CheckoutAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.ConditionAllowed, options => options.Ignore())
                .ForMember(model => model.ConditionModel, options => options.Ignore());
            CreateMap<CheckoutAttributeModel, CheckoutAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore())
                .ForMember(entity => entity.CheckoutAttributeValues, options => options.Ignore())
                .ForMember(entity => entity.ConditionAttributeXml, options => options.Ignore());

            CreateMap<CheckoutAttributeValue, CheckoutAttributeValueModel>()
                .ForMember(model => model.BaseWeightIn, options => options.Ignore())
                .ForMember(model => model.DisplayColorSquaresRgb, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<CheckoutAttributeValueModel, CheckoutAttributeValue>()
                .ForMember(entity => entity.CheckoutAttribute, options => options.Ignore());

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
                .ForMember(entity => entity.GiftCardUsageHistory, options => options.Ignore())
                .ForMember(entity => entity.IsRecipientNotified, options => options.Ignore())
                .ForMember(entity => entity.PurchasedWithOrderItem, options => options.Ignore())
                .ForMember(entity => entity.PurchasedWithOrderItemId, options => options.Ignore());

            CreateMap<OrderSettings, OrderSettingsModel>()
                .ForMember(model => model.AllowAdminsToBuyCallForPriceProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AnonymousCheckoutAllowed_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CheckoutDisabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumberMask_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DeleteGiftCardUsageHistory_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisableBillingAddressCheckoutStep_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisableOrderCompletedPage_OverrideForStore, options => options.Ignore())
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
                .ForMember(model => model.TermsOfServiceOnShoppingCartPage_OverrideForStore, options => options.Ignore());
            CreateMap<OrderSettingsModel, OrderSettings>()
                .ForMember(settings => settings.GeneratePdfInvoiceInCustomerLanguage, options => options.Ignore())
                .ForMember(settings => settings.MinimumOrderPlacementInterval, options => options.Ignore())
                .ForMember(settings => settings.ReturnRequestsFileMaximumSize, options => options.Ignore());

            CreateMap<ReturnRequestAction, ReturnRequestActionModel>();
            CreateMap<ReturnRequestActionModel, ReturnRequestAction>();

            CreateMap<ReturnRequestReason, ReturnRequestReasonModel>();
            CreateMap<ReturnRequestReasonModel, ReturnRequestReason>();

            CreateMap<ShoppingCartSettings, ShoppingCartSettingsModel>()
                .ForMember(model => model.AllowAnonymousUsersToEmailWishlist_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowCartItemEditing_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CartsSharedBetweenStores_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CrossSellsNumber_OverrideForStore, options => options.Ignore())
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
                .ForMember(settings => settings.GroupTierPricesForDistinctShoppingCartItems, options => options.Ignore())
                .ForMember(settings => settings.RenderAssociatedAttributeValueQuantity, options => options.Ignore())
                .ForMember(settings => settings.RoundPricesDuringCalculation, options => options.Ignore());
        }

        /// <summary>
        /// Create payments maps 
        /// </summary>
        protected virtual void CreatePaymentsMaps()
        {
            CreateMap<IPaymentMethod, PaymentMethodModel>()
                .ForMember(model => model.RecurringPaymentType, options => options.MapFrom(plugin => plugin.RecurringPaymentType.ToString()));
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
            CreateMap<Poll, PollModel>()
                .ForMember(model => model.AvailableLanguages, options => options.Ignore())
                .ForMember(model => model.EndDate, options => options.Ignore())
                .ForMember(model => model.PollAnswerSearchModel, options => options.Ignore())
                .ForMember(model => model.StartDate, options => options.Ignore());
            CreateMap<PollModel, Poll>()
                .ForMember(entity => entity.EndDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Language, options => options.Ignore())
                .ForMember(entity => entity.PollAnswers, options => options.Ignore())
                .ForMember(entity => entity.StartDateUtc, options => options.Ignore());
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
                .ForMember(model => model.ShowOnRegistrationPage_OverrideForStore, options => options.Ignore());
            CreateMap<CaptchaSettingsModel, CaptchaSettings>()
                .ForMember(settings => settings.AutomaticallyChooseLanguage, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaDefaultLanguage, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaTheme, options => options.Ignore());
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
            CreateMap<ShippingMethodModel, ShippingMethod>()
                .ForMember(entity => entity.ShippingMethodCountryMappings, options => options.Ignore());

            CreateMap<IShippingRateComputationMethod, ShippingProviderModel>();

            CreateMap<ShippingSettings, ShippingSettingsModel>()
                .ForMember(model => model.AllowPickUpInStore_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ConsiderAssociatedProductsDimensions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayPickupPointsOnMap_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayShipmentEventsToCustomers_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayShipmentEventsToStoreOwner_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EstimateShippingEnabled_OverrideForStore, options => options.Ignore())
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
                .ForMember(settings => settings.UseCubeRootMethod, options => options.Ignore());
        }

        /// <summary>
        /// Create stores maps 
        /// </summary>
        protected virtual void CreateStoresMaps()
        {
            CreateMap<Store, StoreModel>()
                .ForMember(model => model.AvailableLanguages, options => options.Ignore());
            CreateMap<StoreModel, Store>();
        }

        /// <summary>
        /// Create tasks maps 
        /// </summary>
        protected virtual void CreateTasksMaps()
        {
            CreateMap<ScheduleTask, ScheduleTaskModel>();
            CreateMap<ScheduleTaskModel, ScheduleTask>()
                .ForMember(entity => entity.Type, options => options.Ignore());
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
                .ForMember(model => model.SeName, options => options.MapFrom(entity => EngineContext.Current.Resolve<IUrlRecordService>().GetSeName(entity, 0, true, false)))
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
                .ForMember(model => model.SeName, options => options.MapFrom(entity => EngineContext.Current.Resolve<IUrlRecordService>().GetSeName(entity, 0, true, false)))
                .ForMember(model => model.VendorAttributes, options => options.Ignore())
                .ForMember(model => model.VendorNoteSearchModel, options => options.Ignore());
            CreateMap<VendorModel, Vendor>()
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.VendorNotes, options => options.Ignore());

            CreateMap<VendorAttribute, VendorAttributeModel>()
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
                .ForMember(model => model.VendorAttributeValueSearchModel, options => options.Ignore());
            CreateMap<VendorAttributeModel, VendorAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore())
                .ForMember(entity => entity.VendorAttributeValues, options => options.Ignore());

            CreateMap<VendorAttributeValue, VendorAttributeValueModel>();
            CreateMap<VendorAttributeValueModel, VendorAttributeValue>()
                .ForMember(entity => entity.VendorAttribute, options => options.Ignore());

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

        #endregion

        #region Properties

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

        #endregion
    }
}