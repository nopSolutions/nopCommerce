using Nop.Core.Configuration;
using Nop.Data.Configuration;
using Nop.Web.Framework.WebOptimizer;
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
using Nop.Core.Domain.Menus;
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
using Nop.Web.Areas.Admin.Models.Menus;
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
using Riok.Mapperly.Abstractions;

namespace Nop.Web.Areas.Admin.Infrastructure.Mapper;

/// <summary>
/// Admin area mapper
/// </summary>
public interface IAdminMapper
{
    // Configuration mappings
     CacheConfigModel Map(CacheConfig source);
     CacheConfig Map(CacheConfigModel source);

     HostingConfigModel Map(HostingConfig source);
     HostingConfig Map(HostingConfigModel source);

     DistributedCacheConfigModel Map(DistributedCacheConfig source);
     DistributedCacheConfig Map(DistributedCacheConfigModel source);

     InstallationConfigModel Map(InstallationConfig source);
     InstallationConfig Map(InstallationConfigModel source);

     PluginConfigModel Map(PluginConfig source);
     PluginConfig Map(PluginConfigModel source);

     CommonConfigModel Map(CommonConfig source);
     CommonConfig Map(CommonConfigModel source);


    // Affiliates mappings
     AffiliateModel Map(Affiliate source);
     Affiliate Map(AffiliateModel source);

     AffiliatedOrderModel Map(Order source);
     AffiliatedCustomerModel Map(Customer source);

    // Authentication mappings
     ExternalAuthenticationMethodModel Map(IExternalAuthenticationMethod source);
     MultiFactorAuthenticationMethodModel Map(IMultiFactorAuthenticationMethod source);

    // Blogs mappings
     BlogCommentModel Map(BlogComment source);
     BlogComment Map(BlogCommentModel source);

     BlogPostModel Map(BlogPost source);
     BlogPost Map(BlogPostModel source);

     BlogSettingsModel Map(BlogSettings source);
     BlogSettings Map(BlogSettingsModel source);

    // Catalog mappings
     CatalogSettingsModel Map(CatalogSettings source);
     CatalogSettings Map(CatalogSettingsModel source);

     CategoryProductModel Map(ProductCategory source);
     ProductCategory Map(CategoryProductModel source);

     CategoryModel Map(Category source);
     Category Map(CategoryModel source);

     CategoryTemplateModel Map(CategoryTemplate source);
     CategoryTemplate Map(CategoryTemplateModel source);

     ManufacturerProductModel Map(ProductManufacturer source);
     ProductManufacturer Map(ManufacturerProductModel source);

     ManufacturerModel Map(Manufacturer source);
     Manufacturer Map(ManufacturerModel source);

     ManufacturerTemplateModel Map(ManufacturerTemplate source);
     ManufacturerTemplate Map(ManufacturerTemplateModel source);

     ReviewTypeModel Map(ReviewType source);
     ReviewType Map(ReviewTypeModel source);

     ProductReviewModel Map(ProductReview source);
     ProductReviewReviewTypeMappingModel Map(ProductReviewReviewTypeMapping source);

    // Products mappings
     ProductModel Map(Product source);
     Product Map(ProductModel source);

     DiscountProductModel MapToDiscountProductModel(Product source);
     AssociatedProductModel MapToAssociatedProductModel(Product source);

     ProductAttributeCombinationModel Map(ProductAttributeCombination source);
     ProductAttributeCombination Map(ProductAttributeCombinationModel source);

     ProductAttributeModel Map(ProductAttribute source);
     ProductAttribute Map(ProductAttributeModel source);

     ProductAttributeProductModel MapToProductAttributeProductModel(Product source);

     PredefinedProductAttributeValueModel Map(PredefinedProductAttributeValue source);
     PredefinedProductAttributeValue Map(PredefinedProductAttributeValueModel source);

     ProductAttributeMappingModel Map(ProductAttributeMapping source);
     ProductAttributeMapping Map(ProductAttributeMappingModel source);

     ProductAttributeValueModel Map(ProductAttributeValue source);
     ProductAttributeValue Map(ProductAttributeValueModel source);

     ProductEditorSettingsModel Map(ProductEditorSettings source);
     ProductEditorSettings Map(ProductEditorSettingsModel source);

     ProductPictureModel Map(ProductPicture source);
     ProductVideoModel Map(ProductVideo source);

     SpecificationAttributeProductModel MapToSpecificationAttributeProductModel(Product source);

     ProductSpecificationAttributeModel Map(ProductSpecificationAttribute source);
     AddSpecificationAttributeModel MapToAddSpecificationAttributeModel(ProductSpecificationAttribute source);
     ProductSpecificationAttribute Map(AddSpecificationAttributeModel source);

     ProductTagModel Map(ProductTag source);

     ProductTemplateModel Map(ProductTemplate source);
     ProductTemplate Map(ProductTemplateModel source);

     RelatedProductModel Map(RelatedProduct source);

     SpecificationAttributeModel Map(SpecificationAttribute source);
     SpecificationAttribute Map(SpecificationAttributeModel source);

     SpecificationAttributeOptionModel Map(SpecificationAttributeOption source);
     SpecificationAttributeOption Map(SpecificationAttributeOptionModel source);

     SpecificationAttributeGroupModel Map(SpecificationAttributeGroup source);
     SpecificationAttributeGroup Map(SpecificationAttributeGroupModel source);

     StockQuantityHistoryModel Map(StockQuantityHistory source);
     TierPriceModel Map(TierPrice source);
     TierPrice Map(TierPriceModel source);

    // CMS mappings
     WidgetModel Map(IWidgetPlugin source);

    // Common mappings
     AddressModel Map(Address source);
     Address Map(AddressModel source);

     AddressAttributeModel Map(AddressAttribute source);
     AddressAttribute Map(AddressAttributeModel source);

     AddressAttributeValueModel Map(AddressAttributeValue source);
     AddressAttributeValue Map(AddressAttributeValueModel source);

     AddressSettingsModel Map(AddressSettings source);
     AddressSettings Map(AddressSettingsModel source);

     SettingModel Map(Setting source);

    // Customers mappings
     CustomerAttributeModel Map(CustomerAttribute source);
     CustomerAttribute Map(CustomerAttributeModel source);

     CustomerAttributeValueModel Map(CustomerAttributeValue source);
     CustomerAttributeValue Map(CustomerAttributeValueModel source);

     CustomerRoleModel Map(CustomerRole source);
     CustomerRole Map(CustomerRoleModel source);

     CustomerSettingsModel Map(CustomerSettings source);
     CustomerSettings Map(CustomerSettingsModel source);

     MultiFactorAuthenticationSettingsModel Map(MultiFactorAuthenticationSettings source);
     MultiFactorAuthenticationSettings Map(MultiFactorAuthenticationSettingsModel source);

     RewardPointsSettingsModel Map(RewardPointsSettings source);
     RewardPointsSettings Map(RewardPointsSettingsModel source);

     CustomerRewardPointsModel Map(RewardPointsHistory source);
     CustomerActivityLogModel Map(ActivityLog source);

     CustomerModel MapToCustomerModel(Customer source);
     Customer MapToCustomer(CustomerModel source);

     OnlineCustomerModel MapToOnlineCustomerModel(Customer source);
     CustomerBackInStockSubscriptionModel Map(BackInStockSubscription source);

    // Directory mappings
     CountryModel Map(Country source);
     Country Map(CountryModel source);

     CurrencyModel Map(Currency source);
     Currency Map(CurrencyModel source);

     MeasureDimensionModel Map(MeasureDimension source);
     MeasureDimension Map(MeasureDimensionModel source);

     MeasureWeightModel Map(MeasureWeight source);
     MeasureWeight Map(MeasureWeightModel source);

     StateProvinceModel Map(StateProvince source);
     StateProvince Map(StateProvinceModel source);

    // Discounts mappings
     DiscountModel Map(Discount source);
     Discount Map(DiscountModel source);

     DiscountUsageHistoryModel Map(DiscountUsageHistory source);

     DiscountCategoryModel MapToDiscountCategoryModel(Category source);
     DiscountManufacturerModel MapToDiscountManufacturerModel(Manufacturer source);

    // Forums mappings
     ForumModel Map(Forum source);
     Forum Map(ForumModel source);

     ForumGroupModel Map(ForumGroup source);
     ForumGroup Map(ForumGroupModel source);

     ForumSettingsModel Map(ForumSettings source);
     ForumSettings Map(ForumSettingsModel source);

    // GDPR mappings
     GdprSettingsModel Map(GdprSettings source);
     GdprSettings Map(GdprSettingsModel source);

     GdprConsentModel Map(GdprConsent source);
     GdprConsent Map(GdprConsentModel source);

     GdprLogModel Map(GdprLog source);

    // Localization mappings
     LanguageModel Map(Language source);
     Language Map(LanguageModel source);

     LocaleStringResource Map(LocaleResourceModel source);

    // Logging mappings
     ActivityLogModel MapToActivityLogModel(ActivityLog source);
     ActivityLog MapToActivityLog(ActivityLogModel source);

     ActivityLogTypeModel Map(ActivityLogType source);
     ActivityLogType Map(ActivityLogTypeModel source);

     LogModel Map(Log source);
     Log Map(LogModel source);

    // Media mappings
     MediaSettingsModel Map(MediaSettings source);
     MediaSettings Map(MediaSettingsModel source);

    // Messages mappings
     CampaignModel Map(Campaign source);
     Campaign Map(CampaignModel source);

     EmailAccountModel Map(EmailAccount source);
     EmailAccount Map(EmailAccountModel source);

     MessageTemplateModel Map(MessageTemplate source);
     MessageTemplate Map(MessageTemplateModel source);

     NewsLetterSubscriptionModel Map(NewsLetterSubscription source);
     NewsLetterSubscription Map(NewsLetterSubscriptionModel source);

     NewsLetterSubscriptionTypeModel Map(NewsLetterSubscriptionType source);
     NewsLetterSubscriptionType Map(NewsLetterSubscriptionTypeModel source);

     QueuedEmailModel Map(QueuedEmail source);
     QueuedEmail Map(QueuedEmailModel source);

    // News mappings
     NewsCommentModel Map(NewsComment source);
     NewsComment Map(NewsCommentModel source);

     NewsItemModel Map(NewsItem source);
     NewsItem Map(NewsItemModel source);

     NewsSettingsModel Map(NewsSettings source);
     NewsSettings Map(NewsSettingsModel source);

    // Orders mappings
     CustomerOrderModel MapToCustomerOrderModel(Order source);
     OrderNoteModel Map(OrderNote source);

     CheckoutAttributeModel Map(CheckoutAttribute source);
     CheckoutAttribute Map(CheckoutAttributeModel source);

     CheckoutAttributeValueModel Map(CheckoutAttributeValue source);
     CheckoutAttributeValue Map(CheckoutAttributeValueModel source);

     GiftCardModel Map(GiftCard source);
     GiftCard Map(GiftCardModel source);

     GiftCardUsageHistoryModel Map(GiftCardUsageHistory source);
     OrderSettingsModel Map(OrderSettings source);
     OrderSettings Map(OrderSettingsModel source);

     ReturnRequestActionModel Map(ReturnRequestAction source);
     ReturnRequestAction Map(ReturnRequestActionModel source);

     ReturnRequestReasonModel Map(ReturnRequestReason source);
     ReturnRequestReason Map(ReturnRequestReasonModel source);

     ReturnRequestModel Map(ReturnRequest source);
     ReturnRequest Map(ReturnRequestModel source);

     ShoppingCartSettingsModel Map(ShoppingCartSettings source);
     ShoppingCartSettings Map(ShoppingCartSettingsModel source);

     ShoppingCartItemModel Map(ShoppingCartItem source);

    // Payments mappings
     PaymentMethodModel Map(IPaymentMethod source);
     RecurringPaymentModel Map(RecurringPayment source);
     RecurringPayment Map(RecurringPaymentModel source);

     RecurringPaymentHistoryModel Map(RecurringPaymentHistory source);

    // Plugins mappings
     PluginModel Map(PluginDescriptor source);

    // Polls mappings
     PollAnswerModel Map(PollAnswer source);
     PollAnswer Map(PollAnswerModel source);

     PollModel Map(Poll source);
     Poll Map(PollModel source);

    // Security mappings
     CaptchaSettingsModel Map(CaptchaSettings source);
     CaptchaSettings Map(CaptchaSettingsModel source);

    // SEO mappings
     UrlRecordModel Map(UrlRecord source);
     UrlRecord Map(UrlRecordModel source);

    // Shipping mappings
     DeliveryDateModel Map(DeliveryDate source);
     DeliveryDate Map(DeliveryDateModel source);

     PickupPointProviderModel Map(IPickupPointProvider source);
     ProductAvailabilityRangeModel Map(ProductAvailabilityRange source);
     ProductAvailabilityRange Map(ProductAvailabilityRangeModel source);

     ShippingMethodModel Map(ShippingMethod source);
     ShippingMethod Map(ShippingMethodModel source);

     ShippingProviderModel Map(IShippingRateComputationMethod source);
     ShipmentModel Map(Shipment source);

     ShippingSettingsModel Map(ShippingSettings source);
     ShippingSettings Map(ShippingSettingsModel source);

    // Stores mappings
     StoreModel Map(Store source);
     Store Map(StoreModel source);

    // Tasks mappings
     ScheduleTaskModel Map(ScheduleTask source);
     ScheduleTask Map(ScheduleTaskModel source);

    // Tax mappings
     TaxCategoryModel Map(TaxCategory source);
     TaxCategory Map(TaxCategoryModel source);

     TaxProviderModel Map(ITaxProvider source);
     TaxSettingsModel Map(TaxSettings source);
     TaxSettings Map(TaxSettingsModel source);

    // Topics mappings
     TopicModel Map(Topic source);
     Topic Map(TopicModel source);

     TopicTemplateModel Map(TopicTemplate source);
     TopicTemplate Map(TopicTemplateModel source);

    // Vendors mappings
     VendorModel Map(Vendor source);
     Vendor Map(VendorModel source);

     VendorNoteModel Map(VendorNote source);
     VendorAttributeModel Map(VendorAttribute source);
     VendorAttribute Map(VendorAttributeModel source);

     VendorAttributeValueModel Map(VendorAttributeValue source);
     VendorAttributeValue Map(VendorAttributeValueModel source);

     VendorSettingsModel Map(VendorSettings source);
     VendorSettings Map(VendorSettingsModel source);

    // Warehouse mappings
     WarehouseModel Map(Warehouse source);
     Warehouse Map(WarehouseModel source);

    // Menu mappings
     MenuModel Map(Menu source);
     Menu Map(MenuModel source);

     MenuItemModel Map(MenuItem source);
     MenuItem Map(MenuItemModel source);

    // Global ignores
    [MapperIgnoreTarget(nameof(BaseNopModel.CustomProperties))]
    [MapperIgnoreTarget(nameof(ISettingsModel.ActiveStoreScopeConfiguration))]
    [MapperIgnoreTarget(nameof(IConfig.Name))]
    [MapperIgnoreTarget(nameof(IStoreMappingSupported.LimitedToStores))]
    [MapperIgnoreTarget(nameof(IStoreMappingSupportedModel.AvailableStores))]
    [MapperIgnoreTarget(nameof(IStoreMappingSupportedModel.SelectedStoreIds))]
    [MapperIgnoreTarget(nameof(IAclSupported.SubjectToAcl))]
    [MapperIgnoreTarget(nameof(IAclSupportedModel.AvailableCustomerRoles))]
    [MapperIgnoreTarget(nameof(IAclSupportedModel.SelectedCustomerRoleIds))]
    [MapperIgnoreTarget(nameof(IDiscountSupportedModel.AvailableDiscounts))]
    [MapperIgnoreTarget(nameof(IDiscountSupportedModel.SelectedDiscountIds))]
    [MapperIgnoreTarget(nameof(ITranslationSupportedModel.PreTranslationAvailable))]
    [MapperIgnoreTarget(nameof(IPluginModel.ConfigurationUrl))]
    [MapperIgnoreTarget(nameof(IPluginModel.IsActive))]
    [MapperIgnoreTarget(nameof(IPluginModel.LogoUrl))]
    [MapperIgnoreTarget(nameof(IPluginModel.DisplayOrder))]
    [MapperIgnoreTarget(nameof(IPluginModel.FriendlyName))]
    [MapperIgnoreTarget(nameof(IPluginModel.SystemName))]
     void MapToBaseNopModel(BaseNopModel source, BaseNopModel target);
}