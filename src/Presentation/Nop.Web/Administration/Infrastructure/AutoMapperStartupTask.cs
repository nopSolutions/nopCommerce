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
using Nop.Core.Data;
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
using Nop.Core.Tasks;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.PromotionFeed;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Admin.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;
            //TODO remove 'CreatedOnUtc' ignore mappings because now presentation layer models have 'CreatedOn' property and core entities have 'CreatedOnUtc' property (distinct names)


            //address
            Mapper.CreateMap<Address, AddressModel>();
            Mapper.CreateMap<AddressModel, Address>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore());

            //countries
            Mapper.CreateMap<CountryModel, Country>();
            Mapper.CreateMap<Country, CountryModel>()
                .ForMember(dest => dest.NumberOfStates, opt => opt.MapFrom(src => src.StateProvinces != null ? src.StateProvinces.Count : 0));
            //state/provinces
            Mapper.CreateMap<StateProvince, StateProvinceModel>()
                .ForMember(dest => dest.DisplayOrder1, dt => dt.MapFrom(src => src.DisplayOrder));
            Mapper.CreateMap<StateProvinceModel, StateProvince>()
                .ForMember(dest => dest.DisplayOrder, dt => dt.MapFrom(src => src.DisplayOrder1));

            //language
            ViceVersa<Language, LanguageModel>();
            //email account
            ViceVersa<EmailAccount, EmailAccountModel>();
            //message template
            ViceVersa<MessageTemplate, MessageTemplateModel>();
            //queued email
            Mapper.CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(dest => dest.EmailAccountName,
                           opt => opt.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty));
            Mapper.CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(dest=> dest.CreatedOnUtc, dt=> dt.Ignore())
                .ForMember(dest => dest.SentOnUtc, dt => dt.Ignore());
            //campaign
            Mapper.CreateMap<Campaign, CampaignModel>();
            Mapper.CreateMap<CampaignModel, Campaign>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore());
            //topcis
            ViceVersa<Topic, TopicModel>();

            //category
            Mapper.CreateMap<Category, CategoryModel>();
            Mapper.CreateMap<CategoryModel, Category>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.Deleted, dt => dt.Ignore());
            //manufacturer
            Mapper.CreateMap<Manufacturer, ManufacturerModel>();
            Mapper.CreateMap<ManufacturerModel, Manufacturer>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.Deleted, dt => dt.Ignore());
            //products
            Mapper.CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.ProductTags, dt => dt.Ignore());
            Mapper.CreateMap<ProductModel, Product>()
                .ForMember(dest => dest.ProductTags, dt => dt.Ignore())
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.Deleted, dt => dt.Ignore());
            //product variants
            Mapper.CreateMap<ProductVariant, ProductVariantModel>();
            Mapper.CreateMap<ProductVariantModel, ProductVariant>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.Deleted, dt => dt.Ignore());
            //logs
            Mapper.CreateMap<Log, LogModel>();
            Mapper.CreateMap<LogModel, Log>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore());
            //ActivityLogType
            ViceVersa<ActivityLogTypeModel, ActivityLogType>();
            Mapper.CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(dest => dest.ActivityLogTypeName,
                           opt => opt.MapFrom(src => src.ActivityLogType.Name))
                .ForMember(dest => dest.CustomerName,
                           opt => opt.MapFrom(src => src.Customer.Email));
            //currencies
            ViceVersa<Currency, CurrencyModel>();
            //locale resource
            Mapper.CreateMap<LocaleStringResource, LanguageResourceModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ResourceName))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.ResourceValue))
                .ForMember(dest => dest.LanguageName,
                           opt => opt.MapFrom(src => src.Language != null ? src.Language.Name : string.Empty));
            Mapper.CreateMap<LanguageResourceModel, LocaleStringResource>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ResourceValue, opt => opt.MapFrom(src => src.Value));
            //measure weights
            ViceVersa<MeasureWeight, MeasureWeightModel>();
            //measure dimensions
            ViceVersa<MeasureDimension, MeasureDimensionModel>();
            //tax providers
            Mapper.CreateMap<ITaxProvider, TaxProviderModel>()
                .ForMember(dest => dest.FriendlyName, opt => opt.MapFrom(src => src.PluginDescriptor.FriendlyName))
                .ForMember(dest => dest.SystemName, opt => opt.MapFrom(src => src.PluginDescriptor.SystemName));
            //tax categories
            ViceVersa<TaxCategory, TaxCategoryModel>();
            //shipping methods
            ViceVersa<ShippingMethod, ShippingMethodModel>();
            //shipping rate computation methods
            Mapper.CreateMap<IShippingRateComputationMethod, ShippingRateComputationMethodModel>()
                .ForMember(dest => dest.FriendlyName, opt => opt.MapFrom(src => src.PluginDescriptor.FriendlyName))
                .ForMember(dest => dest.SystemName, opt => opt.MapFrom(src => src.PluginDescriptor.SystemName))
                .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.PluginDescriptor.DisplayOrder));
            //payment methods
            Mapper.CreateMap<IPaymentMethod, PaymentMethodModel>()
                .ForMember(dest => dest.FriendlyName, opt => opt.MapFrom(src => src.PluginDescriptor.FriendlyName))
                .ForMember(dest => dest.SystemName, opt => opt.MapFrom(src => src.PluginDescriptor.SystemName))
                .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.PluginDescriptor.DisplayOrder));
            //external authentication methods
            Mapper.CreateMap<IExternalAuthenticationMethod, AuthenticationMethodModel>()
                .ForMember(dest => dest.FriendlyName, opt => opt.MapFrom(src => src.PluginDescriptor.FriendlyName))
                .ForMember(dest => dest.SystemName, opt => opt.MapFrom(src => src.PluginDescriptor.SystemName))
                .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.PluginDescriptor.DisplayOrder));
            //SMS providers
            Mapper.CreateMap<ISmsProvider, SmsProviderModel>()
                .ForMember(dest => dest.FriendlyName, opt => opt.MapFrom(src => src.PluginDescriptor.FriendlyName))
                .ForMember(dest => dest.SystemName, opt => opt.MapFrom(src => src.PluginDescriptor.SystemName));
            //Promotion feeds
            Mapper.CreateMap<IPromotionFeed, PromotionFeedModel>()
                .ForMember(dest => dest.FriendlyName, opt => opt.MapFrom(src => src.PluginDescriptor.FriendlyName))
                .ForMember(dest => dest.SystemName, opt => opt.MapFrom(src => src.PluginDescriptor.SystemName));
            //plugins
            Mapper.CreateMap<PluginDescriptor, PluginModel>();
            //newsLetter subscriptions
            Mapper.CreateMap<NewsLetterSubscription, NewsLetterSubscriptionModel>();
            Mapper.CreateMap<NewsLetterSubscriptionModel, NewsLetterSubscription>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore());
            //forums
            Mapper.CreateMap<ForumGroup, ForumGroupModel>();
            Mapper.CreateMap<ForumGroupModel, ForumGroup>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, dt => dt.Ignore());
            Mapper.CreateMap<Forum, ForumModel>();
            Mapper.CreateMap<ForumModel, Forum>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, dt => dt.Ignore());
            //blogs
            Mapper.CreateMap<BlogPost, BlogPostModel>();
            Mapper.CreateMap<BlogPostModel, BlogPost>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore());
            //news
            Mapper.CreateMap<NewsItem, NewsItemModel>();
            Mapper.CreateMap<NewsItemModel, NewsItem>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore());
            //news
            Mapper.CreateMap<Poll, PollModel>();
            Mapper.CreateMap<PollModel, Poll>()
                .ForMember(dest => dest.StartDateUtc, dt => dt.Ignore())
                .ForMember(dest => dest.EndDateUtc, dt => dt.Ignore());
            //customer roles
            ViceVersa<CustomerRole, CustomerRoleModel>();

            //product attributes
            ViceVersa<ProductAttribute, ProductAttributeModel>();
            //specification attributes
            ViceVersa<SpecificationAttribute, SpecificationAttributeModel>();
            ViceVersa<SpecificationAttributeOption, SpecificationAttributeOptionModel>();
            //checkout attributes
            ViceVersa<CheckoutAttribute, CheckoutAttributeModel>();
            ViceVersa<CheckoutAttributeValue, CheckoutAttributeValueModel>();
            //discounts
            ViceVersa<Discount, DiscountModel>();
            //gift cards
            Mapper.CreateMap<GiftCard, GiftCardModel>();
            Mapper.CreateMap<GiftCardModel, GiftCard>()
                .ForMember(dest => dest.IsRecipientNotified, dt => dt.Ignore())
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore());

            //Settings
            ViceVersa<TaxSettings, TaxSettingsModel>();
            ViceVersa<NewsSettings, NewsSettingsModel>();
            ViceVersa<ForumSettings, ForumSettingsModel>();
            ViceVersa<BlogSettings, BlogSettingsModel>();
            ViceVersa<ShippingSettings, ShippingSettingsModel>();
            ViceVersa<CatalogSettings, CatalogSettingsModel>();
            ViceVersa<RewardPointsSettings, RewardPointsSettingsModel>();
            ViceVersa<OrderSettings, OrderSettingsModel>();
            ViceVersa<ShoppingCartSettings, ShoppingCartSettingsModel>();
            ViceVersa<MediaSettings, MediaSettingsModel>();
            ViceVersa<CustomerSettings, CustomerUserSettingsModel.CustomerSettingsModel>();
        }

        protected virtual void ViceVersa<T1, T2>()
        {
            Mapper.CreateMap<T1, T2>();
            Mapper.CreateMap<T2, T1>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}