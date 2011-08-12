using AutoMapper;
using Nop.Admin.Models.Blogs;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Common;
using Nop.Admin.Models.Customers;
using Nop.Admin.Models.Directory;
using Nop.Admin.Models.Discounts;
using Nop.Admin.Models.ExternalAuthentication;
using Nop.Admin.Models.Forums;
using Nop.Admin.Models.Localization;
using Nop.Admin.Models.Logging;
using Nop.Admin.Models.Messages;
using Nop.Admin.Models.News;
using Nop.Admin.Models.Orders;
using Nop.Admin.Models.Payments;
using Nop.Admin.Models.Plugins;
using Nop.Admin.Models.Polls;
using Nop.Admin.Models.PromotionFeeds;
using Nop.Admin.Models.Settings;
using Nop.Admin.Models.Shipping;
using Nop.Admin.Models.Tax;
using Nop.Admin.Models.Topics;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.PromotionFeed;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Admin
{
    public static class MappingExtensions
    {
        #region Category

        public static CategoryModel ToModel(this Category entity)
        {
            return Mapper.Map<Category, CategoryModel>(entity);
        }

        public static Category ToEntity(this CategoryModel model)
        {
            return Mapper.Map<CategoryModel, Category>(model);
        }

        public static Category ToEntity(this CategoryModel model, Category destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Manufacturer

        public static ManufacturerModel ToModel(this Manufacturer entity)
        {
            return Mapper.Map<Manufacturer, ManufacturerModel>(entity);
        }

        public static Manufacturer ToEntity(this ManufacturerModel model)
        {
            return Mapper.Map<ManufacturerModel, Manufacturer>(model);
        }

        public static Manufacturer ToEntity(this ManufacturerModel model, Manufacturer destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Products

        public static ProductModel ToModel(this Product entity)
        {
            return Mapper.Map<Product, ProductModel>(entity);
        }

        public static Product ToEntity(this ProductModel model)
        {
            return Mapper.Map<ProductModel, Product>(model);
        }

        public static Product ToEntity(this ProductModel model, Product destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Product variants

        public static ProductVariantModel ToModel(this ProductVariant entity)
        {
            return Mapper.Map<ProductVariant, ProductVariantModel>(entity);
        }

        public static ProductVariant ToEntity(this ProductVariantModel model)
        {
            return Mapper.Map<ProductVariantModel, ProductVariant>(model);
        }

        public static ProductVariant ToEntity(this ProductVariantModel model, ProductVariant destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Product attributes

        public static ProductAttributeModel ToModel(this ProductAttribute entity)
        {
            return Mapper.Map<ProductAttribute, ProductAttributeModel>(entity);
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model)
        {
            return Mapper.Map<ProductAttributeModel, ProductAttribute>(model);
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model, ProductAttribute destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Specification attributes

        //attributes
        public static SpecificationAttributeModel ToModel(this SpecificationAttribute entity)
        {
            return Mapper.Map<SpecificationAttribute, SpecificationAttributeModel>(entity);
        }

        public static SpecificationAttribute ToEntity(this SpecificationAttributeModel model)
        {
            return Mapper.Map<SpecificationAttributeModel, SpecificationAttribute>(model);
        }

        public static SpecificationAttribute ToEntity(this SpecificationAttributeModel model, SpecificationAttribute destination)
        {
            return Mapper.Map(model, destination);
        }

        //attribute options
        public static SpecificationAttributeOptionModel ToModel(this SpecificationAttributeOption entity)
        {
            return Mapper.Map<SpecificationAttributeOption, SpecificationAttributeOptionModel>(entity);
        }

        public static SpecificationAttributeOption ToEntity(this SpecificationAttributeOptionModel model)
        {
            return Mapper.Map<SpecificationAttributeOptionModel, SpecificationAttributeOption>(model);
        }

        public static SpecificationAttributeOption ToEntity(this SpecificationAttributeOptionModel model, SpecificationAttributeOption destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Checkout attributes

        //attributes
        public static CheckoutAttributeModel ToModel(this CheckoutAttribute entity)
        {
            return Mapper.Map<CheckoutAttribute, CheckoutAttributeModel>(entity);
        }

        public static CheckoutAttribute ToEntity(this CheckoutAttributeModel model)
        {
            return Mapper.Map<CheckoutAttributeModel, CheckoutAttribute>(model);
        }

        public static CheckoutAttribute ToEntity(this CheckoutAttributeModel model, CheckoutAttribute destination)
        {
            return Mapper.Map(model, destination);
        }

        //checkout attribute values
        public static CheckoutAttributeValueModel ToModel(this CheckoutAttributeValue entity)
        {
            return Mapper.Map<CheckoutAttributeValue, CheckoutAttributeValueModel>(entity);
        }

        public static CheckoutAttributeValue ToEntity(this CheckoutAttributeValueModel model)
        {
            return Mapper.Map<CheckoutAttributeValueModel, CheckoutAttributeValue>(model);
        }

        public static CheckoutAttributeValue ToEntity(this CheckoutAttributeValueModel model, CheckoutAttributeValue destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Languages

        public static LanguageModel ToModel(this Language entity)
        {
            return Mapper.Map<Language, LanguageModel>(entity);
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return Mapper.Map<LanguageModel, Language>(model);
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return Mapper.Map(model, destination);
        }

        #region Resources

        public static LanguageResourceModel ToModel(this LocaleStringResource entity)
        {
            return Mapper.Map<LocaleStringResource, LanguageResourceModel>(entity);
        }

        public static LocaleStringResource ToEntity(this LanguageResourceModel model)
        {
            return Mapper.Map<LanguageResourceModel, LocaleStringResource>(model);
        }

        public static LocaleStringResource ToEntity(this LanguageResourceModel model, LocaleStringResource destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #endregion

        #region Email account

        public static EmailAccountModel ToModel(this EmailAccount entity)
        {
            return Mapper.Map<EmailAccount, EmailAccountModel>(entity);
        }

        public static EmailAccount ToEntity(this EmailAccountModel model)
        {
            return Mapper.Map<EmailAccountModel, EmailAccount>(model);
        }

        public static EmailAccount ToEntity(this EmailAccountModel model, EmailAccount destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Message templates

        public static MessageTemplateModel ToModel(this MessageTemplate entity)
        {
            return Mapper.Map<MessageTemplate, MessageTemplateModel>(entity);
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model)
        {
            return Mapper.Map<MessageTemplateModel, MessageTemplate>(model);
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model, MessageTemplate destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return Mapper.Map<QueuedEmail, QueuedEmailModel>(entity);
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            return Mapper.Map<QueuedEmailModel, QueuedEmail>(model);
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Campaigns

        public static CampaignModel ToModel(this Campaign entity)
        {
            return Mapper.Map<Campaign, CampaignModel>(entity);
        }

        public static Campaign ToEntity(this CampaignModel model)
        {
            return Mapper.Map<CampaignModel, Campaign>(model);
        }

        public static Campaign ToEntity(this CampaignModel model, Campaign destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Topics

        public static TopicModel ToModel(this Topic entity)
        {
            return Mapper.Map<Topic, TopicModel>(entity);
        }

        public static Topic ToEntity(this TopicModel model)
        {
            return Mapper.Map<TopicModel, Topic>(model);
        }

        public static Topic ToEntity(this TopicModel model, Topic destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Log

        public static LogModel ToModel(this Log entity)
        {
            return Mapper.Map<Log, LogModel>(entity);
        }

        public static Log ToEntity(this LogModel model)
        {
            return Mapper.Map<LogModel, Log>(model);
        }

        public static Log ToEntity(this LogModel model, Log destination)
        {
            return Mapper.Map(model, destination);
        }

        public static ActivityLogTypeModel ToModel(this ActivityLogType entity)
        {
            return Mapper.Map<ActivityLogType, ActivityLogTypeModel>(entity);
        }

        public static ActivityLogModel ToModel(this ActivityLog entity)
        {
            return Mapper.Map<ActivityLog, ActivityLogModel>(entity);
        }

        #endregion
        
        #region Currencies

        public static CurrencyModel ToModel(this Currency entity)
        {
            return Mapper.Map<Currency, CurrencyModel>(entity);
        }

        public static Currency ToEntity(this CurrencyModel model)
        {
            return Mapper.Map<CurrencyModel, Currency>(model);
        }

        public static Currency ToEntity(this CurrencyModel model, Currency destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Measure weights

        public static MeasureWeightModel ToModel(this MeasureWeight entity)
        {
            return Mapper.Map<MeasureWeight, MeasureWeightModel>(entity);
        }

        public static MeasureWeight ToEntity(this MeasureWeightModel model)
        {
            return Mapper.Map<MeasureWeightModel, MeasureWeight>(model);
        }

        public static MeasureWeight ToEntity(this MeasureWeightModel model, MeasureWeight destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Measure dimension

        public static MeasureDimensionModel ToModel(this MeasureDimension entity)
        {
            return Mapper.Map<MeasureDimension, MeasureDimensionModel>(entity);
        }

        public static MeasureDimension ToEntity(this MeasureDimensionModel model)
        {
            return Mapper.Map<MeasureDimensionModel, MeasureDimension>(model);
        }

        public static MeasureDimension ToEntity(this MeasureDimensionModel model, MeasureDimension destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Tax providers

        public static TaxProviderModel ToModel(this ITaxProvider entity)
        {
            return Mapper.Map<ITaxProvider, TaxProviderModel>(entity);
        }

        #endregion

        #region Tax categories

        public static TaxCategoryModel ToModel(this TaxCategory entity)
        {
            return Mapper.Map<TaxCategory, TaxCategoryModel>(entity);
        }

        public static TaxCategory ToEntity(this TaxCategoryModel model)
        {
            return Mapper.Map<TaxCategoryModel, TaxCategory>(model);
        }

        public static TaxCategory ToEntity(this TaxCategoryModel model, TaxCategory destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion
        
        #region Shipping rate computation method

        public static ShippingRateComputationMethodModel ToModel(this IShippingRateComputationMethod entity)
        {
            return Mapper.Map<IShippingRateComputationMethod, ShippingRateComputationMethodModel>(entity);
        }

        #endregion

        #region Shipping methods

        public static ShippingMethodModel ToModel(this ShippingMethod entity)
        {
            return Mapper.Map<ShippingMethod, ShippingMethodModel>(entity);
        }

        public static ShippingMethod ToEntity(this ShippingMethodModel model)
        {
            return Mapper.Map<ShippingMethodModel, ShippingMethod>(model);
        }

        public static ShippingMethod ToEntity(this ShippingMethodModel model, ShippingMethod destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion
        
        #region Payment methods

        public static PaymentMethodModel ToModel(this IPaymentMethod entity)
        {
            return Mapper.Map<IPaymentMethod, PaymentMethodModel>(entity);
        }

        #endregion

        #region External authentication methods

        public static AuthenticationMethodModel ToModel(this IExternalAuthenticationMethod entity)
        {
            return Mapper.Map<IExternalAuthenticationMethod, AuthenticationMethodModel>(entity);
        }

        #endregion

        #region SMS providers

        public static SmsProviderModel ToModel(this ISmsProvider entity)
        {
            return Mapper.Map<ISmsProvider, SmsProviderModel>(entity);
        }

        #endregion

        #region Promotion feeds

        public static PromotionFeedModel ToModel(this IPromotionFeed entity)
        {
            return Mapper.Map<IPromotionFeed, PromotionFeedModel>(entity);
        }

        #endregion

        #region Address

        public static AddressModel ToModel(this Address entity)
        {
            return Mapper.Map<Address, AddressModel>(entity);
        }

        public static Address ToEntity(this AddressModel model)
        {
            return Mapper.Map<AddressModel, Address>(model);
        }

        public static Address ToEntity(this AddressModel model, Address destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region NewsLetter subscriptions

        public static NewsLetterSubscriptionModel ToModel(this NewsLetterSubscription entity)
        {
            return Mapper.Map<NewsLetterSubscription, NewsLetterSubscriptionModel>(entity);
        }

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionModel model)
        {
            return Mapper.Map<NewsLetterSubscriptionModel, NewsLetterSubscription>(model);
        }

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionModel model, NewsLetterSubscription destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Discounts

        public static DiscountModel ToModel(this Discount entity)
        {
            return Mapper.Map<Discount, DiscountModel>(entity);
        }

        public static Discount ToEntity(this DiscountModel model)
        {
            return Mapper.Map<DiscountModel, Discount>(model);
        }

        public static Discount ToEntity(this DiscountModel model, Discount destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Forums

        //forum groups
        public static ForumGroupModel ToModel(this ForumGroup entity)
        {
            return Mapper.Map<ForumGroup, ForumGroupModel>(entity);
        }

        public static ForumGroup ToEntity(this ForumGroupModel model)
        {
            return Mapper.Map<ForumGroupModel, ForumGroup>(model);
        }

        public static ForumGroup ToEntity(this ForumGroupModel model, ForumGroup destination)
        {
            return Mapper.Map(model, destination);
        }
        //forums
        public static ForumModel ToModel(this Forum entity)
        {
            return Mapper.Map<Forum, ForumModel>(entity);
        }

        public static Forum ToEntity(this ForumModel model)
        {
            return Mapper.Map<ForumModel, Forum>(model);
        }

        public static Forum ToEntity(this ForumModel model, Forum destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Blog

        //blog posts
        public static BlogPostModel ToModel(this BlogPost entity)
        {
            return Mapper.Map<BlogPost, BlogPostModel>(entity);
        }

        public static BlogPost ToEntity(this BlogPostModel model)
        {
            return Mapper.Map<BlogPostModel, BlogPost>(model);
        }

        public static BlogPost ToEntity(this BlogPostModel model, BlogPost destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region News

        //news items
        public static NewsItemModel ToModel(this NewsItem entity)
        {
            return Mapper.Map<NewsItem, NewsItemModel>(entity);
        }

        public static NewsItem ToEntity(this NewsItemModel model)
        {
            return Mapper.Map<NewsItemModel, NewsItem>(model);
        }

        public static NewsItem ToEntity(this NewsItemModel model, NewsItem destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Polls

        //news items
        public static PollModel ToModel(this Poll entity)
        {
            return Mapper.Map<Poll, PollModel>(entity);
        }

        public static Poll ToEntity(this PollModel model)
        {
            return Mapper.Map<PollModel, Poll>(model);
        }

        public static Poll ToEntity(this PollModel model, Poll destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Customers/users/customer roles
        //customer roles
        public static CustomerRoleModel ToModel(this CustomerRole entity)
        {
            return Mapper.Map<CustomerRole, CustomerRoleModel>(entity);
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model)
        {
            return Mapper.Map<CustomerRoleModel, CustomerRole>(model);
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model, CustomerRole destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Gift Cards

        public static GiftCardModel ToModel(this GiftCard entity)
        {
            return Mapper.Map<GiftCard, GiftCardModel>(entity);
        }

        public static GiftCard ToEntity(this GiftCardModel model)
        {
            return Mapper.Map<GiftCardModel, GiftCard>(model);
        }

        public static GiftCard ToEntity(this GiftCardModel model, GiftCard destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Countries / states

        public static CountryModel ToModel(this Country entity)
        {
            return Mapper.Map<Country, CountryModel>(entity);
        }

        public static Country ToEntity(this CountryModel model)
        {
            return Mapper.Map<CountryModel, Country>(model);
        }

        public static Country ToEntity(this CountryModel model, Country destination)
        {
            return Mapper.Map(model, destination);
        }

        public static StateProvinceModel ToModel(this StateProvince entity)
        {
            return Mapper.Map<StateProvince, StateProvinceModel>(entity);
        }

        public static StateProvince ToEntity(this StateProvinceModel model)
        {
            return Mapper.Map<StateProvinceModel, StateProvince>(model);
        }

        public static StateProvince ToEntity(this StateProvinceModel model, StateProvince destination)
        {
            return Mapper.Map(model, destination);
        }


        #endregion

        #region Settings

        public static TaxSettingsModel ToModel(this TaxSettings entity)
        {
            return Mapper.Map<TaxSettings, TaxSettingsModel>(entity);
        }
        public static TaxSettings ToEntity(this TaxSettingsModel model)
        {
            return Mapper.Map<TaxSettingsModel, TaxSettings>(model);
        }
        public static TaxSettings ToEntity(this TaxSettingsModel model, TaxSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static ShippingSettingsModel ToModel(this ShippingSettings entity)
        {
            return Mapper.Map<ShippingSettings, ShippingSettingsModel>(entity);
        }
        public static ShippingSettings ToEntity(this ShippingSettingsModel model)
        {
            return Mapper.Map<ShippingSettingsModel, ShippingSettings>(model);
        }
        public static ShippingSettings ToEntity(this ShippingSettingsModel model, ShippingSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static ForumSettingsModel ToModel(this ForumSettings entity)
        {
            return Mapper.Map<ForumSettings, ForumSettingsModel>(entity);
        }
        public static ForumSettings ToEntity(this ForumSettingsModel model)
        {
            return Mapper.Map<ForumSettingsModel, ForumSettings>(model);
        }
        public static ForumSettings ToEntity(this ForumSettingsModel model, ForumSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static BlogSettingsModel ToModel(this BlogSettings entity)
        {
            return Mapper.Map<BlogSettings, BlogSettingsModel>(entity);
        }
        public static BlogSettings ToEntity(this BlogSettingsModel model)
        {
            return Mapper.Map<BlogSettingsModel, BlogSettings>(model);
        }
        public static BlogSettings ToEntity(this BlogSettingsModel model, BlogSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static NewsSettingsModel ToModel(this NewsSettings entity)
        {
            return Mapper.Map<NewsSettings, NewsSettingsModel>(entity);
        }
        public static NewsSettings ToEntity(this NewsSettingsModel model)
        {
            return Mapper.Map<NewsSettingsModel, NewsSettings>(model);
        }
        public static NewsSettings ToEntity(this NewsSettingsModel model, NewsSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static CatalogSettingsModel ToModel(this CatalogSettings entity)
        {
            return Mapper.Map<CatalogSettings, CatalogSettingsModel>(entity);
        }
        public static CatalogSettings ToEntity(this CatalogSettingsModel model)
        {
            return Mapper.Map<CatalogSettingsModel, CatalogSettings>(model);
        }
        public static CatalogSettings ToEntity(this CatalogSettingsModel model, CatalogSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static RewardPointsSettingsModel ToModel(this RewardPointsSettings entity)
        {
            return Mapper.Map<RewardPointsSettings, RewardPointsSettingsModel>(entity);
        }
        public static RewardPointsSettings ToEntity(this RewardPointsSettingsModel model)
        {
            return Mapper.Map<RewardPointsSettingsModel, RewardPointsSettings>(model);
        }
        public static RewardPointsSettings ToEntity(this RewardPointsSettingsModel model, RewardPointsSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static OrderSettingsModel ToModel(this OrderSettings entity)
        {
            return Mapper.Map<OrderSettings, OrderSettingsModel>(entity);
        }
        public static OrderSettings ToEntity(this OrderSettingsModel model)
        {
            return Mapper.Map<OrderSettingsModel, OrderSettings>(model);
        }
        public static OrderSettings ToEntity(this OrderSettingsModel model, OrderSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static ShoppingCartSettingsModel ToModel(this ShoppingCartSettings entity)
        {
            return Mapper.Map<ShoppingCartSettings, ShoppingCartSettingsModel>(entity);
        }
        public static ShoppingCartSettings ToEntity(this ShoppingCartSettingsModel model)
        {
            return Mapper.Map<ShoppingCartSettingsModel, ShoppingCartSettings>(model);
        }
        public static ShoppingCartSettings ToEntity(this ShoppingCartSettingsModel model, ShoppingCartSettings destination)
        {
            return Mapper.Map(model, destination);
        }


        public static MediaSettingsModel ToModel(this MediaSettings entity)
        {
            return Mapper.Map<MediaSettings, MediaSettingsModel>(entity);
        }
        public static MediaSettings ToEntity(this MediaSettingsModel model)
        {
            return Mapper.Map<MediaSettingsModel, MediaSettings>(model);
        }
        public static MediaSettings ToEntity(this MediaSettingsModel model, MediaSettings destination)
        {
            return Mapper.Map(model, destination);
        }

        //customer/user settings
        public static CustomerUserSettingsModel.CustomerSettingsModel ToModel(this CustomerSettings entity)
        {
            return Mapper.Map<CustomerSettings, CustomerUserSettingsModel.CustomerSettingsModel>(entity);
        }
        public static CustomerSettings ToEntity(this CustomerUserSettingsModel.CustomerSettingsModel model)
        {
            return Mapper.Map<CustomerUserSettingsModel.CustomerSettingsModel, CustomerSettings>(model);
        }
        public static CustomerSettings ToEntity(this CustomerUserSettingsModel.CustomerSettingsModel model, CustomerSettings destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Plugins

        public static PluginModel ToModel(this PluginDescriptor entity)
        {
            return Mapper.Map<PluginDescriptor, PluginModel>(entity);
        }

        #endregion
    }
}