using System;
using AutoMapper;
using Nop.Admin.Models.Blogs;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Cms;
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
using Nop.Admin.Models.Settings;
using Nop.Admin.Models.Shipping;
using Nop.Admin.Models.Stores;
using Nop.Admin.Models.Tax;
using Nop.Admin.Models.Templates;
using Nop.Admin.Models.Topics;
using Nop.Admin.Models.Vendors;
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
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Admin.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration for admin area models
    /// </summary>
    public class AdminMapperConfiguration : IMapperConfiguration
    {
        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns>Mapper configuration action</returns>
        public Action<IMapperConfigurationExpression> GetConfiguration()
        {
            //TODO remove 'CreatedOnUtc' ignore mappings because now presentation layer models have 'CreatedOn' property and core entities have 'CreatedOnUtc' property (distinct names)

            Action<IMapperConfigurationExpression> action = cfg =>
            {
                //address
                cfg.CreateMap<Address, AddressModel>()
                    .ForMember(dest => dest.AddressHtml, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomAddressAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.FormattedCustomAddressAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCountries, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStates, mo => mo.Ignore())
                    .ForMember(dest => dest.FirstNameEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.FirstNameRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.LastNameEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.LastNameRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.CompanyEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CompanyRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.CountryEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CountryRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.StateProvinceEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CityEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CityRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddressEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddressRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddress2Enabled, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddress2Required, mo => mo.Ignore())
                    .ForMember(dest => dest.ZipPostalCodeEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.ZipPostalCodeRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.PhoneEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.PhoneRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.FaxEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.FaxRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.CountryName,
                        mo => mo.MapFrom(src => src.Country != null ? src.Country.Name : null))
                    .ForMember(dest => dest.StateProvinceName,
                        mo => mo.MapFrom(src => src.StateProvince != null ? src.StateProvince.Name : null))
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<AddressModel, Address>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.Country, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.StateProvince, mo => mo.Ignore());

                //countries
                cfg.CreateMap<CountryModel, Country>()
                    .ForMember(dest => dest.StateProvinces, mo => mo.Ignore())
                    .ForMember(dest => dest.RestrictedShippingMethods, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                cfg.CreateMap<Country, CountryModel>()
                    .ForMember(dest => dest.NumberOfStates,
                        mo => mo.MapFrom(src => src.StateProvinces != null ? src.StateProvinces.Count : 0))
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //state/provinces
                cfg.CreateMap<StateProvince, StateProvinceModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<StateProvinceModel, StateProvince>()
                    .ForMember(dest => dest.Country, mo => mo.Ignore());

                //language
                cfg.CreateMap<Language, LanguageModel>()
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCurrencies, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.Search, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<LanguageModel, Language>()
                    .ForMember(dest => dest.LocaleStringResources, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //email account
                cfg.CreateMap<EmailAccount, EmailAccountModel>()
                    .ForMember(dest => dest.Password, mo => mo.Ignore())
                    .ForMember(dest => dest.IsDefaultEmailAccount, mo => mo.Ignore())
                    .ForMember(dest => dest.SendTestEmailTo, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<EmailAccountModel, EmailAccount>()
                    .ForMember(dest => dest.Password, mo => mo.Ignore());
                //message template
                cfg.CreateMap<MessageTemplate, MessageTemplateModel>()
                    .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                    .ForMember(dest => dest.HasAttachedDownload, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.ListOfStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SendImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<MessageTemplateModel, MessageTemplate>()
                    .ForMember(dest => dest.DelayPeriod, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //queued email
                cfg.CreateMap<QueuedEmail, QueuedEmailModel>()
                    .ForMember(dest => dest.EmailAccountName,
                        mo => mo.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty))
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.PriorityName, mo => mo.Ignore())
                    .ForMember(dest => dest.DontSendBeforeDate, mo => mo.Ignore())
                    .ForMember(dest => dest.SendImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.SentOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<QueuedEmailModel, QueuedEmail>()
                    .ForMember(dest => dest.Priority, dt => dt.Ignore())
                    .ForMember(dest => dest.PriorityId, dt => dt.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                    .ForMember(dest => dest.DontSendBeforeDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.SentOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAccount, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachmentFilePath, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachmentFileName, mo => mo.Ignore());
                //campaign
                cfg.CreateMap<Campaign, CampaignModel>()
                    .ForMember(dest => dest.DontSendBeforeDate, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                    .ForMember(dest => dest.TestEmail, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CampaignModel, Campaign>()
                    .ForMember(dest => dest.DontSendBeforeDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
                //topcis
                cfg.CreateMap<Topic, TopicModel>()
                    .ForMember(dest => dest.AvailableTopicTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.Url, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<TopicModel, Topic>()
                    .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());

                //category
                cfg.CreateMap<Category, CategoryModel>()
                    .ForMember(dest => dest.AvailableCategoryTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.Breadcrumb, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableDiscounts, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedDiscountIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CategoryModel, Category>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                    .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                    .ForMember(dest => dest.AppliedDiscounts, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //manufacturer
                cfg.CreateMap<Manufacturer, ManufacturerModel>()
                    .ForMember(dest => dest.AvailableManufacturerTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableDiscounts, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedDiscountIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ManufacturerModel, Manufacturer>()
                    .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore())
                    .ForMember(dest => dest.AppliedDiscounts, mo => mo.Ignore());
                ;

                //vendors
                cfg.CreateMap<Vendor, VendorModel>()
                    .ForMember(dest => dest.AssociatedCustomers, mo => mo.Ignore())
                    .ForMember(dest => dest.Address, mo => mo.Ignore())
                    .ForMember(dest => dest.AddVendorNoteMessage, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<VendorModel, Vendor>()
                    .ForMember(dest => dest.VendorNotes, mo => mo.Ignore())
                    .ForMember(dest => dest.Deleted, mo => mo.Ignore());

                //products
                cfg.CreateMap<Product, ProductModel>()
                    .ForMember(dest => dest.ProductsTypesSupportedByProductTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.AssociatedToProductId, mo => mo.Ignore())
                    .ForMember(dest => dest.AssociatedToProductName, mo => mo.Ignore())
                    .ForMember(dest => dest.StockQuantityStr, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductTags, mo => mo.Ignore())
                    .ForMember(dest => dest.PictureThumbnailUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableVendors, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableProductTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableManufacturers, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableProductAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.AddPictureModel, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductPictureModels, mo => mo.Ignore())
                    .ForMember(dest => dest.AddSpecificationAttributeModel, mo => mo.Ignore())
                    .ForMember(dest => dest.CopyProductModel, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductWarehouseInventoryModels, mo => mo.Ignore())
                    .ForMember(dest => dest.IsLoggedInAsVendor, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableTaxCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                    .ForMember(dest => dest.BaseDimensionIn, mo => mo.Ignore())
                    .ForMember(dest => dest.BaseWeightIn, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableDiscounts, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCategoryIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedManufacturerIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedDiscountIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableDeliveryDates, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableProductAvailabilityRanges, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableWarehouses, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableBasepriceUnits, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableBasepriceBaseUnits, mo => mo.Ignore())
                    .ForMember(dest => dest.LastStockQuantity, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductEditorSettingsModel, mo => mo.Ignore())
                    .ForMember(dest => dest.StockQuantityHistory, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ProductModel, Product>()
                    .ForMember(dest => dest.ProductTags, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.ParentGroupedProductId, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductType, mo => mo.Ignore())
                    .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                    .ForMember(dest => dest.ApprovedRatingSum, mo => mo.Ignore())
                    .ForMember(dest => dest.NotApprovedRatingSum, mo => mo.Ignore())
                    .ForMember(dest => dest.ApprovedTotalReviews, mo => mo.Ignore())
                    .ForMember(dest => dest.NotApprovedTotalReviews, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductManufacturers, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductPictures, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductReviews, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductSpecificationAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductWarehouseInventory, mo => mo.Ignore())
                    .ForMember(dest => dest.HasTierPrices, mo => mo.Ignore())
                    .ForMember(dest => dest.HasDiscountsApplied, mo => mo.Ignore())
                    .ForMember(dest => dest.BackorderMode, mo => mo.Ignore())
                    .ForMember(dest => dest.DownloadActivationType, mo => mo.Ignore())
                    .ForMember(dest => dest.GiftCardType, mo => mo.Ignore())
                    .ForMember(dest => dest.LowStockActivity, mo => mo.Ignore())
                    .ForMember(dest => dest.ManageInventoryMethod, mo => mo.Ignore())
                    .ForMember(dest => dest.RecurringCyclePeriod, mo => mo.Ignore())
                    .ForMember(dest => dest.RentalPricePeriod, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductAttributeMappings, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductAttributeCombinations, mo => mo.Ignore())
                    .ForMember(dest => dest.TierPrices, mo => mo.Ignore())
                    .ForMember(dest => dest.AppliedDiscounts, mo => mo.Ignore())
                    .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //logs
                cfg.CreateMap<Log, LogModel>()
                    .ForMember(dest => dest.CustomerEmail, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<LogModel, Log>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LogLevelId, mo => mo.Ignore())
                    .ForMember(dest => dest.Customer, mo => mo.Ignore());
                //ActivityLogType
                cfg.CreateMap<ActivityLogTypeModel, ActivityLogType>()
                    .ForMember(dest => dest.SystemKeyword, mo => mo.Ignore());
                cfg.CreateMap<ActivityLogType, ActivityLogTypeModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ActivityLog, ActivityLogModel>()
                    .ForMember(dest => dest.ActivityLogTypeName, mo => mo.MapFrom(src => src.ActivityLogType.Name))
                    .ForMember(dest => dest.CustomerEmail, mo => mo.MapFrom(src => src.Customer.Email))
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //currencies
                cfg.CreateMap<Currency, CurrencyModel>()
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.IsPrimaryExchangeRateCurrency, mo => mo.Ignore())
                    .ForMember(dest => dest.IsPrimaryStoreCurrency, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CurrencyModel, Currency>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore())
                    .ForMember(dest => dest.RoundingType, mo => mo.Ignore());
                //measure weights
                cfg.CreateMap<MeasureWeight, MeasureWeightModel>()
                    .ForMember(dest => dest.IsPrimaryWeight, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<MeasureWeightModel, MeasureWeight>();
                //measure dimensions
                cfg.CreateMap<MeasureDimension, MeasureDimensionModel>()
                    .ForMember(dest => dest.IsPrimaryDimension, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<MeasureDimensionModel, MeasureDimension>();
                //tax providers
                cfg.CreateMap<ITaxProvider, TaxProviderModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.IsPrimaryTaxProvider, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //tax categories
                cfg.CreateMap<TaxCategory, TaxCategoryModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<TaxCategoryModel, TaxCategory>();
                //shipping methods
                cfg.CreateMap<ShippingMethod, ShippingMethodModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ShippingMethodModel, ShippingMethod>()
                    .ForMember(dest => dest.RestrictedCountries, mo => mo.Ignore());
                //delivery dates
                cfg.CreateMap<DeliveryDate, DeliveryDateModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<DeliveryDateModel, DeliveryDate>();
                //product availability ranges
                cfg.CreateMap<ProductAvailabilityRange, ProductAvailabilityRangeModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ProductAvailabilityRangeModel, ProductAvailabilityRange>();
                //shipping rate computation methods
                cfg.CreateMap<IShippingRateComputationMethod, ShippingRateComputationMethodModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.LogoUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //pickup point providers
                cfg.CreateMap<IPickupPointProvider, PickupPointProviderModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.LogoUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //payment methods
                cfg.CreateMap<IPaymentMethod, PaymentMethodModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
                    .ForMember(dest => dest.RecurringPaymentType,
                        mo => mo.MapFrom(src => src.RecurringPaymentType.ToString()))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.LogoUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //external authentication methods
                cfg.CreateMap<IExternalAuthenticationMethod, AuthenticationMethodModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //widgets
                cfg.CreateMap<IWidgetPlugin, WidgetModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //plugins
                cfg.CreateMap<PluginDescriptor, PluginModel>()
                    .ForMember(dest => dest.ConfigurationUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.CanChangeEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.IsEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.LogoUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //newsLetter subscriptions
                cfg.CreateMap<NewsLetterSubscription, NewsLetterSubscriptionModel>()
                    .ForMember(dest => dest.StoreName, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<NewsLetterSubscriptionModel, NewsLetterSubscription>()
                    .ForMember(dest => dest.StoreId, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.NewsLetterSubscriptionGuid, mo => mo.Ignore());
                //forums
                cfg.CreateMap<ForumGroup, ForumGroupModel>()
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ForumGroupModel, ForumGroup>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.Forums, mo => mo.Ignore());
                cfg.CreateMap<Forum, ForumModel>()
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumGroups, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ForumModel, Forum>()
                    .ForMember(dest => dest.NumTopics, mo => mo.Ignore())
                    .ForMember(dest => dest.NumPosts, mo => mo.Ignore())
                    .ForMember(dest => dest.LastTopicId, mo => mo.Ignore())
                    .ForMember(dest => dest.LastPostId, mo => mo.Ignore())
                    .ForMember(dest => dest.LastPostCustomerId, mo => mo.Ignore())
                    .ForMember(dest => dest.LastPostTime, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumGroup, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
                //blogs
                cfg.CreateMap<BlogPost, BlogPostModel>()
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(src.LanguageId, true, false)))
                    .ForMember(dest => dest.ApprovedComments, mo => mo.Ignore())
                    .ForMember(dest => dest.NotApprovedComments, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDate, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDate, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore());
                cfg.CreateMap<BlogPostModel, BlogPost>()
                    .ForMember(dest => dest.BlogComments, mo => mo.Ignore())
                    .ForMember(dest => dest.Language, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //news
                cfg.CreateMap<NewsItem, NewsItemModel>()
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(src.LanguageId, true, false)))
                    .ForMember(dest => dest.ApprovedComments, mo => mo.Ignore())
                    .ForMember(dest => dest.NotApprovedComments, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDate, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDate, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore());
                cfg.CreateMap<NewsItemModel, NewsItem>()
                    .ForMember(dest => dest.NewsComments, mo => mo.Ignore())
                    .ForMember(dest => dest.Language, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //news
                cfg.CreateMap<Poll, PollModel>()
                    .ForMember(dest => dest.StartDate, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDate, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore());
                cfg.CreateMap<PollModel, Poll>()
                    .ForMember(dest => dest.PollAnswers, mo => mo.Ignore())
                    .ForMember(dest => dest.Language, mo => mo.Ignore())
                    .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore());
                //customer roles
                cfg.CreateMap<CustomerRole, CustomerRoleModel>()
                    .ForMember(dest => dest.PurchasedWithProductName, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CustomerRoleModel, CustomerRole>()
                    .ForMember(dest => dest.PermissionRecords, mo => mo.Ignore());

                //product attributes
                cfg.CreateMap<ProductAttribute, ProductAttributeModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ProductAttributeModel, ProductAttribute>();
                //specification attributes
                cfg.CreateMap<SpecificationAttribute, SpecificationAttributeModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<SpecificationAttributeModel, SpecificationAttribute>()
                    .ForMember(dest => dest.SpecificationAttributeOptions, mo => mo.Ignore());
                cfg.CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionModel>()
                    .ForMember(dest => dest.NumberOfAssociatedProducts, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.EnableColorSquaresRgb, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<SpecificationAttributeOptionModel, SpecificationAttributeOption>()
                    .ForMember(dest => dest.SpecificationAttribute, mo => mo.Ignore());
                //checkout attributes
                cfg.CreateMap<CheckoutAttribute, CheckoutAttributeModel>()
                    .ForMember(dest => dest.AvailableTaxCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.ConditionAllowed, mo => mo.Ignore())
                    .ForMember(dest => dest.ConditionModel, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CheckoutAttributeModel, CheckoutAttribute>()
                    .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
                    .ForMember(dest => dest.ConditionAttributeXml, mo => mo.Ignore())
                    .ForMember(dest => dest.CheckoutAttributeValues, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //customer attributes
                cfg.CreateMap<CustomerAttribute, CustomerAttributeModel>()
                    .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CustomerAttributeModel, CustomerAttribute>()
                    .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomerAttributeValues, mo => mo.Ignore());
                //address attributes
                cfg.CreateMap<AddressAttribute, AddressAttributeModel>()
                    .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<AddressAttributeModel, AddressAttribute>()
                    .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
                    .ForMember(dest => dest.AddressAttributeValues, mo => mo.Ignore());
                //discounts
                cfg.CreateMap<Discount, DiscountModel>()
                    .ForMember(dest => dest.DiscountTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.TimesUsed, mo => mo.Ignore())
                    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                    .ForMember(dest => dest.AddDiscountRequirement, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableDiscountRequirementRules, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableRequirementGroups, mo => mo.Ignore())
                    .ForMember(dest => dest.GroupName, mo => mo.Ignore())
                    .ForMember(dest => dest.RequirementGroupId, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<DiscountModel, Discount>()
                    .ForMember(dest => dest.DiscountType, mo => mo.Ignore())
                    .ForMember(dest => dest.DiscountLimitation, mo => mo.Ignore())
                    .ForMember(dest => dest.DiscountRequirements, mo => mo.Ignore())
                    .ForMember(dest => dest.AppliedToCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.AppliedToManufacturers, mo => mo.Ignore())
                    .ForMember(dest => dest.AppliedToProducts, mo => mo.Ignore());
                //gift cards
                cfg.CreateMap<GiftCard, GiftCardModel>()
                    .ForMember(dest => dest.PurchasedWithOrderId, mo => mo.Ignore())
                    .ForMember(dest => dest.AmountStr, mo => mo.Ignore())
                    .ForMember(dest => dest.RemainingAmountStr, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.PurchasedWithOrderNumber, mo => mo.Ignore());
                cfg.CreateMap<GiftCardModel, GiftCard>()
                    .ForMember(dest => dest.PurchasedWithOrderItemId, mo => mo.Ignore())
                    .ForMember(dest => dest.GiftCardType, mo => mo.Ignore())
                    .ForMember(dest => dest.GiftCardUsageHistory, mo => mo.Ignore())
                    .ForMember(dest => dest.PurchasedWithOrderItem, mo => mo.Ignore())
                    .ForMember(dest => dest.IsRecipientNotified, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
                //stores
                cfg.CreateMap<Store, StoreModel>()
                    .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<StoreModel, Store>();

                //Settings
                cfg.CreateMap<CaptchaSettings, GeneralCommonSettingsModel.CaptchaSettingsModel>()
                    .ForMember(dest => dest.AvailableReCaptchaVersions, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<GeneralCommonSettingsModel.CaptchaSettingsModel, CaptchaSettings>()
                    .ForMember(dest => dest.ReCaptchaTheme, mo => mo.Ignore())
                    .ForMember(dest => dest.ReCaptchaLanguage, mo => mo.Ignore());
                cfg.CreateMap<TaxSettings, TaxSettingsModel>()
                    .ForMember(dest => dest.DefaultTaxAddress, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxDisplayTypeValues, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxBasedOnValues, mo => mo.Ignore())
                    .ForMember(dest => dest.PaymentMethodAdditionalFeeTaxCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxCategories, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatShopCountries, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.PricesIncludeTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToSelectTaxDisplayType_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxDisplayType_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxSuffix_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxRates_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.HideZeroTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.HideTaxInOrderSummary_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ForceTaxExclusionFromOrderSubtotal_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultTaxCategoryId_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxBasedOn_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TaxBasedOnPickupPointAddress_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultTaxAddress_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShippingIsTaxable_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShippingPriceIncludesTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShippingTaxClassId_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatShopCountryId_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatAllowVatExemption_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatUseWebService_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatAssumeValid_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<TaxSettingsModel, TaxSettings>()
                    .ForMember(dest => dest.ActiveTaxProviderSystemName, mo => mo.Ignore())
                    .ForMember(dest => dest.LogErrors, mo => mo.Ignore());
                cfg.CreateMap<NewsSettings, NewsSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.Enabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyAboutNewNewsComments_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowNewsOnMainPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MainPageNewsCount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NewsArchivePageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowHeaderRssUrl_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NewsCommentsMustBeApproved_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<NewsSettingsModel, NewsSettings>();
                cfg.CreateMap<ForumSettings, ForumSettingsModel>()
                    .ForMember(dest => dest.ForumEditorValues, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.RelativeDateTimeFormattingEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowCustomersPostCount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowGuestsToCreatePosts_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowGuestsToCreateTopics_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToEditPosts_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToDeletePosts_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowPostVoting_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MaxVotesPerDay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToManageSubscriptions_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TopicsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PostsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumEditor_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SignaturesEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowPrivateMessages_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowAlertForPM_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyAboutPrivateMessages_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveDiscussionsFeedEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveDiscussionsFeedCount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumFeedsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumFeedCount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SearchResultsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveDiscussionsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ForumSettingsModel, ForumSettings>()
                    .ForMember(dest => dest.TopicSubjectMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.StrippedTopicMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.PostMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.LatestCustomerPostsPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.PrivateMessagesPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumSubscriptionsPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.PMSubjectMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.PMTextMaxLength, mo => mo.Ignore())
                    .ForMember(dest => dest.HomePageActiveDiscussionsTopicCount, mo => mo.Ignore())
                    .ForMember(dest => dest.ForumSearchTermMinimumLength, mo => mo.Ignore());
                cfg.CreateMap<BlogSettings, BlogSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.Enabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PostsPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyAboutNewBlogComments_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NumberOfTags_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowHeaderRssUrl_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.BlogCommentsMustBeApproved_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<BlogSettingsModel, BlogSettings>();
                cfg.CreateMap<VendorSettings, VendorSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.VendorsBlockItemsToDisplay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowVendorOnProductDetailsPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToContactVendors_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCustomersToApplyForVendorAccount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowSearchByVendor_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowVendorsToEditInfo_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumProductNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowVendorsToImportProducts_OverrideForStore, mo => mo.Ignore());
                cfg.CreateMap<VendorSettingsModel, VendorSettings>()
                    .ForMember(dest => dest.DefaultVendorPageSizeOptions, mo => mo.Ignore());
                cfg.CreateMap<ShippingSettings, ShippingSettingsModel>()
                    .ForMember(dest => dest.ShippingOriginAddress, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowPickUpInStore_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShipToSameAddress_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayPickupPointsOnMap_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.GoogleMapsApiKey_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.UseWarehouseLocation_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.FreeShippingOverXEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.FreeShippingOverXValue_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.FreeShippingOverXIncludingTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EstimateShippingEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayShipmentEventsToCustomers_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayShipmentEventsToStoreOwner_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.HideShippingTotal_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ConsiderAssociatedProductsDimensions_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShippingOriginAddress_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ShippingSettingsModel, ShippingSettings>()
                    .ForMember(dest => dest.ActiveShippingRateComputationMethodSystemNames, mo => mo.Ignore())
                    .ForMember(dest => dest.ActivePickupPointProviderSystemNames, mo => mo.Ignore())
                    .ForMember(dest => dest.ReturnValidOptionsIfThereAreAny, mo => mo.Ignore())
                    .ForMember(dest => dest.UseCubeRootMethod, mo => mo.Ignore());
                cfg.CreateMap<CatalogSettings, CatalogSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowViewUnpublishedProductPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.ShowSkuOnProductDetailsPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowSkuOnCatalogPages_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowManufacturerPartNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowGtin_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowFreeShippingNotification_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowProductSorting_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowProductViewModeChanging_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowProductsFromSubcategories_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowCategoryProductNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.CategoryBreadcrumbEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowShareButton_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PageShareCode_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductReviewsMustBeApproved_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowAnonymousUsersToReviewProduct_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowProductReviewsPerStore_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAFriendEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowAnonymousUsersToEmailAFriend_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.RecentlyViewedProductsNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.RecentlyViewedProductsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NewProductsNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NewProductsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CompareProductsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowBestsellersOnHomepage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NumberOfBestsellersOnHomepage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SearchPageProductsPerPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SearchPageAllowCustomersToSelectPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.SearchPagePageSizeOptions_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductSearchAutoCompleteEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.ShowProductImagesInSearchAutoComplete_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductSearchTermMinimumLength_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductsAlsoPurchasedEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductsAlsoPurchasedNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NumberOfProductTags_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductsByTagPageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.ProductsByTagPageSizeOptions_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.IncludeShortDescriptionInCompareProducts_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.IncludeFullDescriptionInCompareProducts_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ManufacturersBlockItemsToDisplay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoFooter_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoProductBoxes_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoShoppingCart_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoWishlist_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowProductReviewsOnAccountPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductReviewsPageSizeOnAccountPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ExportImportProductAttributes_OverrideForStore, mo => mo.Ignore());
                cfg.CreateMap<CatalogSettingsModel, CatalogSettings>()
                    .ForMember(dest => dest.PublishBackProductWhenCancellingOrders, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultViewMode, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultProductRatingValue, mo => mo.Ignore())
                    .ForMember(dest => dest.IncludeFeaturedProductsInNormalLists, mo => mo.Ignore())
                    .ForMember(dest => dest.AjaxProcessAttributeChange, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumBackInStockSubscriptions, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayTierPricesWithDiscounts, mo => mo.Ignore())
                    .ForMember(dest => dest.CompareProductsNumber, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultCategoryPageSizeOptions, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultCategoryPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultManufacturerPageSizeOptions, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultManufacturerPageSize, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductSortingEnumDisabled, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductSortingEnumDisplayOrder, mo => mo.Ignore())
                    .ForMember(dest => dest.ExportImportUseDropdownlistsForAssociatedEntities, mo => mo.Ignore());
                cfg.CreateMap<RewardPointsSettings, RewardPointsSettingsModel>()
                    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.Enabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ExchangeRate_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MinimumRewardPointsToUse_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PointsForRegistration_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PointsForPurchases_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActivationDelay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActivatePointsImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayHowMuchWillBeEarned_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<RewardPointsSettingsModel, RewardPointsSettings>();
                cfg.CreateMap<OrderSettings, OrderSettingsModel>()
                    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                    .ForMember(dest => dest.OrderIdent, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.IsReOrderAllowed_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MinOrderSubtotalAmount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MinOrderSubtotalAmountIncludingTax_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MinOrderTotalAmount_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AnonymousCheckoutAllowed_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TermsOfServiceOnShoppingCartPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.TermsOfServiceOnOrderConfirmPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.OnePageCheckoutEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ReturnRequestsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ReturnRequestsAllowFiles_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.NumberOfDaysReturnRequestAvailable_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisableBillingAddressCheckoutStep_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisableOrderCompletedPage_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.ReturnRequestNumberMask_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomOrderNumberMask_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ExportWithProducts_OverrideForStore, mo => mo.Ignore());
                cfg.CreateMap<OrderSettingsModel, OrderSettings>()
                    .ForMember(dest => dest.GeneratePdfInvoiceInCustomerLanguage, mo => mo.Ignore())
                    .ForMember(dest => dest.ReturnRequestsFileMaximumSize, mo => mo.Ignore())
                    .ForMember(dest => dest.MinimumOrderPlacementInterval, mo => mo.Ignore());
                cfg.CreateMap<ShoppingCartSettings, ShoppingCartSettingsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayCartAfterAddingProduct_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayWishlistAfterAddingProduct_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumShoppingCartItems_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumWishlistItems_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MoveItemsFromWishlistToCart_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CartsSharedBetweenStores_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowProductImagesOnShoppingCart_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowProductImagesOnWishList_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowDiscountBox_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowGiftCardBox_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CrossSellsNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailWishlistEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowAnonymousUsersToEmailWishlist_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MiniShoppingCartEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ShowProductImagesInMiniShoppingCart_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MiniShoppingCartProductNumber_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowCartItemEditing_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ShoppingCartSettingsModel, ShoppingCartSettings>()
                    .ForMember(dest => dest.RoundPricesDuringCalculation, mo => mo.Ignore())
                    .ForMember(dest => dest.GroupTierPricesForDistinctShoppingCartItems, mo => mo.Ignore())
                    .ForMember(dest => dest.RenderAssociatedAttributeValueQuantity, mo => mo.Ignore());
                cfg.CreateMap<MediaSettings, MediaSettingsModel>()
                    .ForMember(dest => dest.PicturesStoredIntoDatabase, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.AvatarPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductDetailsPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.AssociatedProductPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CategoryThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ManufacturerThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.VendorThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CartThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MiniCartThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumImageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MultipleThumbDirectories_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultImageQuality_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.ImportProductImagesUsingHash_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultPictureZoomEnabled_OverrideForStore, mo => mo.Ignore());
                cfg.CreateMap<MediaSettingsModel, MediaSettings>()
                    .ForMember(dest => dest.ImageSquarePictureSize, mo => mo.Ignore())
                    .ForMember(dest => dest.AutoCompleteSearchThumbPictureSize, mo => mo.Ignore());
                cfg.CreateMap<CustomerSettings, CustomerUserSettingsModel.CustomerSettingsModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CustomerUserSettingsModel.CustomerSettingsModel, CustomerSettings>()
                    .ForMember(dest => dest.HashedPasswordFormat, mo => mo.Ignore())
                    .ForMember(dest => dest.AvatarMaximumSizeBytes, mo => mo.Ignore())
                    .ForMember(dest => dest.DownloadableProductsValidateUser, mo => mo.Ignore())
                    .ForMember(dest => dest.OnlineCustomerMinutes, mo => mo.Ignore())
                    .ForMember(dest => dest.SuffixDeletedCustomers, mo => mo.Ignore())
                    .ForMember(dest => dest.DeleteGuestTaskOlderThanMinutes, mo => mo.Ignore());
                cfg.CreateMap<AddressSettings, CustomerUserSettingsModel.AddressSettingsModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CustomerUserSettingsModel.AddressSettingsModel, AddressSettings>();
                cfg.CreateMap<ProductEditorSettings, ProductEditorSettingsModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ProductEditorSettingsModel, ProductEditorSettings>();

                //return request reasons
                cfg.CreateMap<ReturnRequestReason, ReturnRequestReasonModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ReturnRequestReasonModel, ReturnRequestReason>();
                //return request actions
                cfg.CreateMap<ReturnRequestAction, ReturnRequestActionModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ReturnRequestActionModel, ReturnRequestAction>();

                //category template
                cfg.CreateMap<CategoryTemplate, CategoryTemplateModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CategoryTemplateModel, CategoryTemplate>();
                //manufacturer template
                cfg.CreateMap<ManufacturerTemplate, ManufacturerTemplateModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ManufacturerTemplateModel, ManufacturerTemplate>();
                //product template
                cfg.CreateMap<ProductTemplate, ProductTemplateModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ProductTemplateModel, ProductTemplate>();
                //topic template
                cfg.CreateMap<TopicTemplate, TopicTemplateModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<TopicTemplateModel, TopicTemplate>();

            };
            return action;
        }

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }
}