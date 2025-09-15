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
[Mapper]
public partial class AdminMapper : IAdminMapper
{
    // Configuration mappings
     public partial CacheConfigModel Map(CacheConfig source);
     public partial CacheConfig Map(CacheConfigModel source);

     public partial HostingConfigModel Map(HostingConfig source);
     public partial HostingConfig Map(HostingConfigModel source);

     public partial DistributedCacheConfigModel Map(DistributedCacheConfig source);
     public partial DistributedCacheConfig Map(DistributedCacheConfigModel source);

     public partial InstallationConfigModel Map(InstallationConfig source);
     public partial InstallationConfig Map(InstallationConfigModel source);

     public partial PluginConfigModel Map(PluginConfig source);
     public partial PluginConfig Map(PluginConfigModel source);

     public partial CommonConfigModel Map(CommonConfig source);
     public partial CommonConfig Map(CommonConfigModel source);

    // Affiliates mappings
     public partial AffiliateModel Map(Affiliate source);
     public partial Affiliate Map(AffiliateModel source);

     public partial AffiliatedOrderModel Map(Order source);
     public partial AffiliatedCustomerModel Map(Customer source);

    // Authentication mappings
     public partial ExternalAuthenticationMethodModel Map(IExternalAuthenticationMethod source);
     public partial MultiFactorAuthenticationMethodModel Map(IMultiFactorAuthenticationMethod source);

    // Blogs mappings
     public partial BlogCommentModel Map(BlogComment source);
     public partial BlogComment Map(BlogCommentModel source);

     public partial BlogPostModel Map(BlogPost source);
     public partial BlogPost Map(BlogPostModel source);

     public partial BlogSettingsModel Map(BlogSettings source);
     public partial BlogSettings Map(BlogSettingsModel source);

    // Catalog mappings
     public partial CatalogSettingsModel Map(CatalogSettings source);
     public partial CatalogSettings Map(CatalogSettingsModel source);

     public partial CategoryProductModel Map(ProductCategory source);
     public partial ProductCategory Map(CategoryProductModel source);

     public partial CategoryModel Map(Category source);
     public partial Category Map(CategoryModel source);

     public partial CategoryTemplateModel Map(CategoryTemplate source);
     public partial CategoryTemplate Map(CategoryTemplateModel source);

     public partial ManufacturerProductModel Map(ProductManufacturer source);
     public partial ProductManufacturer Map(ManufacturerProductModel source);

     public partial ManufacturerModel Map(Manufacturer source);
     public partial Manufacturer Map(ManufacturerModel source);

     public partial ManufacturerTemplateModel Map(ManufacturerTemplate source);
     public partial ManufacturerTemplate Map(ManufacturerTemplateModel source);

     public partial ReviewTypeModel Map(ReviewType source);
     public partial ReviewType Map(ReviewTypeModel source);

     public partial ProductReviewModel Map(ProductReview source);
     public partial ProductReviewReviewTypeMappingModel Map(ProductReviewReviewTypeMapping source);

    // Products mappings
     public partial ProductModel Map(Product source);
     public partial Product Map(ProductModel source);

     public partial DiscountProductModel MapToDiscountProductModel(Product source);
     public partial AssociatedProductModel MapToAssociatedProductModel(Product source);

     public partial ProductAttributeCombinationModel Map(ProductAttributeCombination source);
     public partial ProductAttributeCombination Map(ProductAttributeCombinationModel source);

     public partial ProductAttributeModel Map(ProductAttribute source);
     public partial ProductAttribute Map(ProductAttributeModel source);

     public partial ProductAttributeProductModel MapToProductAttributeProductModel(Product source);

     public partial PredefinedProductAttributeValueModel Map(PredefinedProductAttributeValue source);
     public partial PredefinedProductAttributeValue Map(PredefinedProductAttributeValueModel source);

     public partial ProductAttributeMappingModel Map(ProductAttributeMapping source);
     public partial ProductAttributeMapping Map(ProductAttributeMappingModel source);

     public partial ProductAttributeValueModel Map(ProductAttributeValue source);
     public partial ProductAttributeValue Map(ProductAttributeValueModel source);

     public partial ProductEditorSettingsModel Map(ProductEditorSettings source);
     public partial ProductEditorSettings Map(ProductEditorSettingsModel source);

     public partial ProductPictureModel Map(ProductPicture source);
     public partial ProductVideoModel Map(ProductVideo source);

     public partial SpecificationAttributeProductModel MapToSpecificationAttributeProductModel(Product source);

     public partial ProductSpecificationAttributeModel Map(ProductSpecificationAttribute source);
     public partial AddSpecificationAttributeModel MapToAddSpecificationAttributeModel(ProductSpecificationAttribute source);
     public partial ProductSpecificationAttribute Map(AddSpecificationAttributeModel source);

     public partial ProductTagModel Map(ProductTag source);

     public partial ProductTemplateModel Map(ProductTemplate source);
     public partial ProductTemplate Map(ProductTemplateModel source);

     public partial RelatedProductModel Map(RelatedProduct source);

     public partial SpecificationAttributeModel Map(SpecificationAttribute source);
     public partial SpecificationAttribute Map(SpecificationAttributeModel source);

     public partial SpecificationAttributeOptionModel Map(SpecificationAttributeOption source);
     public partial SpecificationAttributeOption Map(SpecificationAttributeOptionModel source);

     public partial SpecificationAttributeGroupModel Map(SpecificationAttributeGroup source);
     public partial SpecificationAttributeGroup Map(SpecificationAttributeGroupModel source);

     public partial StockQuantityHistoryModel Map(StockQuantityHistory source);
     public partial TierPriceModel Map(TierPrice source);
     public partial TierPrice Map(TierPriceModel source);

    // CMS mappings
     public partial WidgetModel Map(IWidgetPlugin source);

    // Common mappings
     public partial AddressModel Map(Address source);
     public partial Address Map(AddressModel source);

     public partial AddressAttributeModel Map(AddressAttribute source);
     public partial AddressAttribute Map(AddressAttributeModel source);

     public partial AddressAttributeValueModel Map(AddressAttributeValue source);
     public partial AddressAttributeValue Map(AddressAttributeValueModel source);

     public partial AddressSettingsModel Map(AddressSettings source);
     public partial AddressSettings Map(AddressSettingsModel source);

     public partial SettingModel Map(Setting source);

    // Customers mappings
     public partial CustomerAttributeModel Map(CustomerAttribute source);
     public partial CustomerAttribute Map(CustomerAttributeModel source);

     public partial CustomerAttributeValueModel Map(CustomerAttributeValue source);
     public partial CustomerAttributeValue Map(CustomerAttributeValueModel source);

     public partial CustomerRoleModel Map(CustomerRole source);
     public partial CustomerRole Map(CustomerRoleModel source);

     public partial CustomerSettingsModel Map(CustomerSettings source);
     public partial CustomerSettings Map(CustomerSettingsModel source);

     public partial MultiFactorAuthenticationSettingsModel Map(MultiFactorAuthenticationSettings source);
     public partial MultiFactorAuthenticationSettings Map(MultiFactorAuthenticationSettingsModel source);

     public partial RewardPointsSettingsModel Map(RewardPointsSettings source);
     public partial RewardPointsSettings Map(RewardPointsSettingsModel source);

     public partial CustomerRewardPointsModel Map(RewardPointsHistory source);
     public partial CustomerActivityLogModel Map(ActivityLog source);

     public partial CustomerModel MapToCustomerModel(Customer source);
     public partial Customer MapToCustomer(CustomerModel source);

     public partial OnlineCustomerModel MapToOnlineCustomerModel(Customer source);
     public partial CustomerBackInStockSubscriptionModel Map(BackInStockSubscription source);

    // Directory mappings
     public partial CountryModel Map(Country source);
     public partial Country Map(CountryModel source);

     public partial CurrencyModel Map(Currency source);
     public partial Currency Map(CurrencyModel source);

     public partial MeasureDimensionModel Map(MeasureDimension source);
     public partial MeasureDimension Map(MeasureDimensionModel source);

     public partial MeasureWeightModel Map(MeasureWeight source);
     public partial MeasureWeight Map(MeasureWeightModel source);

     public partial StateProvinceModel Map(StateProvince source);
     public partial StateProvince Map(StateProvinceModel source);

    // Discounts mappings
     public partial DiscountModel Map(Discount source);
     public partial Discount Map(DiscountModel source);

     public partial DiscountUsageHistoryModel Map(DiscountUsageHistory source);

     public partial DiscountCategoryModel MapToDiscountCategoryModel(Category source);
     public partial DiscountManufacturerModel MapToDiscountManufacturerModel(Manufacturer source);

    // Forums mappings
     public partial ForumModel Map(Forum source);
     public partial Forum Map(ForumModel source);

     public partial ForumGroupModel Map(ForumGroup source);
     public partial ForumGroup Map(ForumGroupModel source);

     public partial ForumSettingsModel Map(ForumSettings source);
     public partial ForumSettings Map(ForumSettingsModel source);

    // GDPR mappings
     public partial GdprSettingsModel Map(GdprSettings source);
     public partial GdprSettings Map(GdprSettingsModel source);

     public partial GdprConsentModel Map(GdprConsent source);
     public partial GdprConsent Map(GdprConsentModel source);

     public partial GdprLogModel Map(GdprLog source);

    // Localization mappings
     public partial LanguageModel Map(Language source);
     public partial Language Map(LanguageModel source);

     public partial LocaleStringResource Map(LocaleResourceModel source);

    // Logging mappings
     public partial ActivityLogModel MapToActivityLogModel(ActivityLog source);
     public partial ActivityLog MapToActivityLog(ActivityLogModel source);

     public partial ActivityLogTypeModel Map(ActivityLogType source);
     public partial ActivityLogType Map(ActivityLogTypeModel source);

     public partial LogModel Map(Log source);
     public partial Log Map(LogModel source);

    // Media mappings
     public partial MediaSettingsModel Map(MediaSettings source);
     public partial MediaSettings Map(MediaSettingsModel source);

    // Messages mappings
     public partial CampaignModel Map(Campaign source);
     public partial Campaign Map(CampaignModel source);

     public partial EmailAccountModel Map(EmailAccount source);
     public partial EmailAccount Map(EmailAccountModel source);

     public partial MessageTemplateModel Map(MessageTemplate source);
     public partial MessageTemplate Map(MessageTemplateModel source);

     public partial NewsLetterSubscriptionModel Map(NewsLetterSubscription source);
     public partial NewsLetterSubscription Map(NewsLetterSubscriptionModel source);

     public partial NewsLetterSubscriptionTypeModel Map(NewsLetterSubscriptionType source);
     public partial NewsLetterSubscriptionType Map(NewsLetterSubscriptionTypeModel source);

     public partial QueuedEmailModel Map(QueuedEmail source);
     public partial QueuedEmail Map(QueuedEmailModel source);

    // News mappings
     public partial NewsCommentModel Map(NewsComment source);
     public partial NewsComment Map(NewsCommentModel source);

     public partial NewsItemModel Map(NewsItem source);
     public partial NewsItem Map(NewsItemModel source);

     public partial NewsSettingsModel Map(NewsSettings source);
     public partial NewsSettings Map(NewsSettingsModel source);

    // Orders mappings
     public partial CustomerOrderModel MapToCustomerOrderModel(Order source);
     public partial OrderNoteModel Map(OrderNote source);

     public partial CheckoutAttributeModel Map(CheckoutAttribute source);
     public partial CheckoutAttribute Map(CheckoutAttributeModel source);

     public partial CheckoutAttributeValueModel Map(CheckoutAttributeValue source);
     public partial CheckoutAttributeValue Map(CheckoutAttributeValueModel source);

     public partial GiftCardModel Map(GiftCard source);
     public partial GiftCard Map(GiftCardModel source);

     public partial GiftCardUsageHistoryModel Map(GiftCardUsageHistory source);
     public partial OrderSettingsModel Map(OrderSettings source);
     public partial OrderSettings Map(OrderSettingsModel source);

     public partial ReturnRequestActionModel Map(ReturnRequestAction source);
     public partial ReturnRequestAction Map(ReturnRequestActionModel source);

     public partial ReturnRequestReasonModel Map(ReturnRequestReason source);
     public partial ReturnRequestReason Map(ReturnRequestReasonModel source);

     public partial ReturnRequestModel Map(ReturnRequest source);
     public partial ReturnRequest Map(ReturnRequestModel source);

     public partial ShoppingCartSettingsModel Map(ShoppingCartSettings source);
     public partial ShoppingCartSettings Map(ShoppingCartSettingsModel source);

     public partial ShoppingCartItemModel Map(ShoppingCartItem source);

    // Payments mappings
     public partial PaymentMethodModel Map(IPaymentMethod source);
     public partial RecurringPaymentModel Map(RecurringPayment source);
     public partial RecurringPayment Map(RecurringPaymentModel source);

     public partial RecurringPaymentHistoryModel Map(RecurringPaymentHistory source);

    // Plugins mappings
     public partial PluginModel Map(PluginDescriptor source);

    // Polls mappings
     public partial PollAnswerModel Map(PollAnswer source);
     public partial PollAnswer Map(PollAnswerModel source);

     public partial PollModel Map(Poll source);
     public partial Poll Map(PollModel source);

    // Security mappings
     public partial CaptchaSettingsModel Map(CaptchaSettings source);
     public partial CaptchaSettings Map(CaptchaSettingsModel source);

    // SEO mappings
     public partial UrlRecordModel Map(UrlRecord source);
     public partial UrlRecord Map(UrlRecordModel source);

    // Shipping mappings
     public partial DeliveryDateModel Map(DeliveryDate source);
     public partial DeliveryDate Map(DeliveryDateModel source);

     public partial PickupPointProviderModel Map(IPickupPointProvider source);
     public partial ProductAvailabilityRangeModel Map(ProductAvailabilityRange source);
     public partial ProductAvailabilityRange Map(ProductAvailabilityRangeModel source);

     public partial ShippingMethodModel Map(ShippingMethod source);
     public partial ShippingMethod Map(ShippingMethodModel source);

     public partial ShippingProviderModel Map(IShippingRateComputationMethod source);
     public partial ShipmentModel Map(Shipment source);

     public partial ShippingSettingsModel Map(ShippingSettings source);
     public partial ShippingSettings Map(ShippingSettingsModel source);

    // Stores mappings
     public partial StoreModel Map(Store source);
     public partial Store Map(StoreModel source);

    // Tasks mappings
     public partial ScheduleTaskModel Map(ScheduleTask source);
     public partial ScheduleTask Map(ScheduleTaskModel source);

    // Tax mappings
     public partial TaxCategoryModel Map(TaxCategory source);
     public partial TaxCategory Map(TaxCategoryModel source);

     public partial TaxProviderModel Map(ITaxProvider source);
     public partial TaxSettingsModel Map(TaxSettings source);
     public partial TaxSettings Map(TaxSettingsModel source);

    // Topics mappings
     public partial TopicModel Map(Topic source);
     public partial Topic Map(TopicModel source);

     public partial TopicTemplateModel Map(TopicTemplate source);
     public partial TopicTemplate Map(TopicTemplateModel source);

    // Vendors mappings
     public partial VendorModel Map(Vendor source);
     public partial Vendor Map(VendorModel source);

     public partial VendorNoteModel Map(VendorNote source);
     public partial VendorAttributeModel Map(VendorAttribute source);
     public partial VendorAttribute Map(VendorAttributeModel source);

     public partial VendorAttributeValueModel Map(VendorAttributeValue source);
     public partial VendorAttributeValue Map(VendorAttributeValueModel source);

     public partial VendorSettingsModel Map(VendorSettings source);
     public partial VendorSettings Map(VendorSettingsModel source);

    // Warehouse mappings
     public partial WarehouseModel Map(Warehouse source);
     public partial Warehouse Map(WarehouseModel source);

    // Menu mappings
     public partial MenuModel Map(Menu source);
     public partial Menu Map(MenuModel source);

     public partial MenuItemModel Map(MenuItem source);
     public partial MenuItem Map(MenuItemModel source);

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
     public partial void MapToBaseNopModel(BaseNopModel source, BaseNopModel target);
}