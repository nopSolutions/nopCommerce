using AutoMapper;
using Nop.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Tasks;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Admin.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            //address
            Mapper.CreateMap<Address, AddressModel>();
            Mapper.CreateMap<AddressModel, Address>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore());

            //language
            ViceVersa<Language, LanguageModel>();
            //email account
            ViceVersa<EmailAccount, EmailAccountModel>();
            //queued email
            Mapper.CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(dest => dest.EmailAccountName,
                           opt => opt.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty));
            Mapper.CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(dest=> dest.CreatedOnUtc, dt=> dt.Ignore())
                .ForMember(dest => dest.SentOnUtc, dt => dt.Ignore());
            //category
            ViceVersa<Category, CategoryModel>();
            //category product
            ViceVersa<ProductCategory, CategoryProductModel>();
            //products
            ViceVersa<Product, ProductModel>();
            //product variants
            ViceVersa<ProductVariant, ProductVariantModel>();
            //logs
            ViceVersa<Log, LogModel>();
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
            Mapper.CreateMap<ITaxProvider, TaxProviderModel>();
            //tax categories
            ViceVersa<TaxCategory, TaxCategoryModel>();
            //tax settings
            ViceVersa<TaxSettings, TaxSettingsModel>();
            //shipping methods
            ViceVersa<ShippingMethod, ShippingMethodModel>();
            //shipping rate computation methods
            Mapper.CreateMap<IShippingRateComputationMethod, ShippingRateComputationMethodModel>();
            //shipping settings
            ViceVersa<ShippingSettings, ShippingSettingsModel>();
            //payment methods
            Mapper.CreateMap<IPaymentMethod, PaymentMethodModel>();
            //newsLetter subscriptions
            ViceVersa<NewsLetterSubscription, NewsLetterSubscriptionModel>();
            //forums
            ViceVersa<ForumSettings, ForumSettingsModel>();
            ViceVersa<ForumGroup, ForumGroupModel>();
            ViceVersa<Forum, ForumModel>();
            //customer roles
            ViceVersa<CustomerRole, CustomerRoleModel>();
            //customers
            Mapper.CreateMap<Customer, CustomerModel>();
            Mapper.CreateMap<CustomerModel, Customer>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.LastActivityDateUtc, dt => dt.Ignore())
                .ForMember(dest => dest.AssociatedUserId, dt => dt.Ignore());

            //product attributes
            ViceVersa<ProductAttribute, ProductAttributeModel>();
        }

        public static void ViceVersa<T1, T2>()
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