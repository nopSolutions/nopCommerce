using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
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
using Nop.Core.Domain.Payments;
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
using Nop.Data.Extensions;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Services.Installation
{
    /// <summary>
    /// Code first installation service
    /// </summary>
    public partial class CodeFirstInstallationService : IInstallationService
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly INopFileProvider _fileProvider;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Affiliate> _affiliateRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly IRepository<CheckoutAttribute> _checkoutAttributeRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerPassword> _customerPasswordRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<DeliveryDate> _deliveryDateRepository;
        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<Forum> _forumRepository;
        private readonly IRepository<ForumGroup> _forumGroupRepository;
        private readonly IRepository<GiftCard> _giftCardRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;
        private readonly IRepository<MeasureDimension> _measureDimensionRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAvailabilityRange> _productAvailabilityRangeRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IRepository<ProductTemplate> _productTemplateRepository;
        private readonly IRepository<RelatedProduct> _relatedProductRepository;
        private readonly IRepository<ReturnRequestAction> _returnRequestActionRepository;
        private readonly IRepository<ReturnRequestReason> _returnRequestReasonRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<ShipmentItem> _shipmentItemRepository;
        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<StockQuantityHistory> _stockQuantityHistoryRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<TopicTemplate> _topicTemplateRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IWebHelper _webHelper;


        #endregion

        #region Ctor

        public CodeFirstInstallationService(IGenericAttributeService genericAttributeService,
            INopFileProvider fileProvider,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<Address> addressRepository,
            IRepository<Affiliate> affiliateRepository,
            IRepository<BlogPost> blogPostRepository,
            IRepository<Category> categoryRepository,
            IRepository<CategoryTemplate> categoryTemplateRepository,
            IRepository<CheckoutAttribute> checkoutAttributeRepository,
            IRepository<Country> countryRepository,
            IRepository<Currency> currencyRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerPassword> customerPasswordRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<DeliveryDate> deliveryDateRepository,
            IRepository<Discount> discountRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<Forum> forumRepository,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<GiftCard> giftCardRepository,
            IRepository<Language> languageRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<ManufacturerTemplate> manufacturerTemplateRepository,
            IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
            IRepository<NewsItem> newsItemRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository,
            IRepository<Poll> pollRepository,
            IRepository<Product> productRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAvailabilityRange> productAvailabilityRangeRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<ProductTemplate> productTemplateRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<ReturnRequestAction> returnRequestActionRepository,
            IRepository<ReturnRequestReason> returnRequestReasonRepository,
            IRepository<ScheduleTask> scheduleTaskRepository,
            IRepository<SearchTerm> searchTermRepository,
            IRepository<Shipment> shipmentRepository,
            IRepository<ShipmentItem> shipmentItemRepository,
            IRepository<ShippingMethod> shippingMethodRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
            IRepository<Store> storeRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IRepository<Topic> topicRepository,
            IRepository<TopicTemplate> topicTemplateRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IRepository<Vendor> vendorRepository,
            IRepository<Warehouse> warehouseRepository,
            IWebHelper webHelper)
        {
            _genericAttributeService = genericAttributeService;
            _fileProvider = fileProvider;
            _activityLogRepository = activityLogRepository;
            _activityLogTypeRepository = activityLogTypeRepository;
            _addressRepository = addressRepository;
            _affiliateRepository = affiliateRepository;
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _categoryTemplateRepository = categoryTemplateRepository;
            _checkoutAttributeRepository = checkoutAttributeRepository;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
            _customerPasswordRepository = customerPasswordRepository;
            _customerRepository = customerRepository;
            _customerRoleRepository = customerRoleRepository;
            _deliveryDateRepository = deliveryDateRepository;
            _discountRepository = discountRepository;
            _emailAccountRepository = emailAccountRepository;
            _forumGroupRepository = forumGroupRepository;
            _forumRepository = forumRepository;
            _giftCardRepository = giftCardRepository;
            _languageRepository = languageRepository;
            _manufacturerRepository = manufacturerRepository;
            _manufacturerTemplateRepository = manufacturerTemplateRepository;
            _measureDimensionRepository = measureDimensionRepository;
            _measureWeightRepository = measureWeightRepository;
            _messageTemplateRepository = messageTemplateRepository;
            _newsItemRepository = newsItemRepository;
            _orderItemRepository = orderItemRepository;
            _orderNoteRepository = orderNoteRepository;
            _orderRepository = orderRepository;
            _pollRepository = pollRepository;
            _productAttributeRepository = productAttributeRepository;
            _productAvailabilityRangeRepository = productAvailabilityRangeRepository;
            _productRepository = productRepository;
            _productTagRepository = productTagRepository;
            _productTemplateRepository = productTemplateRepository;
            _relatedProductRepository = relatedProductRepository;
            _returnRequestActionRepository = returnRequestActionRepository;
            _returnRequestReasonRepository = returnRequestReasonRepository;
            _scheduleTaskRepository = scheduleTaskRepository;
            _searchTermRepository = searchTermRepository;
            _shipmentItemRepository = shipmentItemRepository;
            _shipmentRepository = shipmentRepository;
            _shippingMethodRepository = shippingMethodRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _stockQuantityHistoryRepository = stockQuantityHistoryRepository;
            _storeRepository = storeRepository;
            _taxCategoryRepository = taxCategoryRepository;
            _topicRepository = topicRepository;
            _topicTemplateRepository = topicTemplateRepository;
            _urlRecordRepository = urlRecordRepository;
            _vendorRepository = vendorRepository;
            _warehouseRepository = warehouseRepository;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual string ValidateSeName<T>(T entity, string seName) where T : BaseEntity
        {
            //duplicate of ValidateSeName method of \Nop.Services\Seo\UrlRecordService.cs (we cannot inject it here)
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            //validation
            var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            seName = seName.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (var c in seName.ToCharArray())
            {
                var c2 = c.ToString();
                if (okChars.Contains(c2))
                {
                    sb.Append(c2);
                }
            }

            seName = sb.ToString();
            seName = seName.Replace(" ", "-");
            while (seName.Contains("--"))
                seName = seName.Replace("--", "-");
            while (seName.Contains("__"))
                seName = seName.Replace("__", "_");

            //max length
            seName = CommonHelper.EnsureMaximumLength(seName, NopSeoDefaults.SearchEngineNameLength);

            //ensure this sename is not reserved yet
            var i = 2;
            var tempSeName = seName;
            while (true)
            {
                //check whether such slug already exists (and that is not the current entity)

                var query = from ur in _urlRecordRepository.Table
                            where ur.Slug == tempSeName
                            select ur;
                var urlRecord = query.FirstOrDefault();

                var entityName = entity.GetUnproxiedEntityType().Name;
                var reserved = urlRecord != null && !(urlRecord.EntityId == entity.Id && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
                if (!reserved)
                    break;

                tempSeName = $"{seName}-{i}";
                i++;
            }

            seName = tempSeName;

            return seName;
        }

        protected virtual string GetSamplesPath()
        {
            return _fileProvider.GetAbsolutePath(NopInstallationDefaults.SampleImagesPath);
        }

        protected virtual void InstallStores()
        {
            //var storeUrl = "http://www.yourStore.com/";
            var storeUrl = _webHelper.GetStoreLocation(false);
            var stores = new List<Store>
            {
                new Store
                {
                    Name = "Your store name",
                    Url = storeUrl,
                    SslEnabled = false,
                    Hosts = "yourstore.com,www.yourstore.com",
                    DisplayOrder = 1,
                    //should we set some default company info?
                    CompanyName = "Your company name",
                    CompanyAddress = "your company country, state, zip, street, etc",
                    CompanyPhoneNumber = "(123) 456-78901",
                    CompanyVat = null
                }
            };

            _storeRepository.Insert(stores);
        }

        protected virtual void InstallMeasures()
        {
            var measureDimensions = new List<MeasureDimension>
            {
                new MeasureDimension
                {
                    Name = "inch(es)",
                    SystemKeyword = "inches",
                    Ratio = 1M,
                    DisplayOrder = 1
                },
                new MeasureDimension
                {
                    Name = "feet",
                    SystemKeyword = "feet",
                    Ratio = 0.08333333M,
                    DisplayOrder = 2
                },
                new MeasureDimension
                {
                    Name = "meter(s)",
                    SystemKeyword = "meters",
                    Ratio = 0.0254M,
                    DisplayOrder = 3
                },
                new MeasureDimension
                {
                    Name = "millimetre(s)",
                    SystemKeyword = "millimetres",
                    Ratio = 25.4M,
                    DisplayOrder = 4
                }
            };

            _measureDimensionRepository.Insert(measureDimensions);

            var measureWeights = new List<MeasureWeight>
            {
                new MeasureWeight
                {
                    Name = "ounce(s)",
                    SystemKeyword = "ounce",
                    Ratio = 16M,
                    DisplayOrder = 1
                },
                new MeasureWeight
                {
                    Name = "lb(s)",
                    SystemKeyword = "lb",
                    Ratio = 1M,
                    DisplayOrder = 2
                },
                new MeasureWeight
                {
                    Name = "kg(s)",
                    SystemKeyword = "kg",
                    Ratio = 0.45359237M,
                    DisplayOrder = 3
                },
                new MeasureWeight
                {
                    Name = "gram(s)",
                    SystemKeyword = "grams",
                    Ratio = 453.59237M,
                    DisplayOrder = 4
                }
            };

            _measureWeightRepository.Insert(measureWeights);
        }

        protected virtual void InstallTaxCategories()
        {
            var taxCategories = new List<TaxCategory>
                               {
                                   new TaxCategory
                                       {
                                           Name = "Books",
                                           DisplayOrder = 1
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Electronics & Software",
                                           DisplayOrder = 5
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Downloadable Products",
                                           DisplayOrder = 10
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Jewelry",
                                           DisplayOrder = 15
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Apparel",
                                           DisplayOrder = 20
                                       }
                               };
            _taxCategoryRepository.Insert(taxCategories);
        }

        protected virtual void InstallLanguages()
        {
            var language = new Language
            {
                Name = "English",
                LanguageCulture = "en-US",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            var directoryPath = _fileProvider.MapPath(NopInstallationDefaults.LocalizationResourcesPath);
            var pattern = $"*.{NopInstallationDefaults.LocalizationResourcesFileExtension}";
            foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                using (var streamReader = new StreamReader(filePath))
                {
                    localizationService.ImportResourcesFromXml(language, streamReader);
                }
            }
        }

        protected virtual void InstallCurrencies()
        {
            var currencies = new List<Currency>
            {
                new Currency
                {
                    Name = "US Dollar",
                    CurrencyCode = "USD",
                    Rate = 1,
                    DisplayLocale = "en-US",
                    CustomFormatting = string.Empty,
                    Published = true,
                    DisplayOrder = 1,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Australian Dollar",
                    CurrencyCode = "AUD",
                    Rate = 1.34M,
                    DisplayLocale = "en-AU",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 2,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "British Pound",
                    CurrencyCode = "GBP",
                    Rate = 0.75M,
                    DisplayLocale = "en-GB",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 3,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Canadian Dollar",
                    CurrencyCode = "CAD",
                    Rate = 1.32M,
                    DisplayLocale = "en-CA",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 4,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Chinese Yuan Renminbi",
                    CurrencyCode = "CNY",
                    Rate = 6.43M,
                    DisplayLocale = "zh-CN",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 5,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Euro",
                    CurrencyCode = "EUR",
                    Rate = 0.86M,
                    DisplayLocale = string.Empty,
                    //CustomFormatting = "€0.00",
                    CustomFormatting = string.Format("{0}0.00", "\u20ac"), //euro symbol
                    Published = true,
                    DisplayOrder = 6,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Hong Kong Dollar",
                    CurrencyCode = "HKD",
                    Rate = 7.84M,
                    DisplayLocale = "zh-HK",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 7,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Japanese Yen",
                    CurrencyCode = "JPY",
                    Rate = 110.45M,
                    DisplayLocale = "ja-JP",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 8,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Russian Rouble",
                    CurrencyCode = "RUB",
                    Rate = 63.25M,
                    DisplayLocale = "ru-RU",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 9,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Swedish Krona",
                    CurrencyCode = "SEK",
                    Rate = 8.80M,
                    DisplayLocale = "sv-SE",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 10,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding1
                },
                new Currency
                {
                    Name = "Indian Rupee",
                    CurrencyCode = "INR",
                    Rate = 68.03M,
                    DisplayLocale = "en-IN",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 12,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                }
            };
            _currencyRepository.Insert(currencies);
        }

        protected virtual void InstallCountriesAndStates()
        {
            var cUsa = new Country
            {
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 840,
                SubjectToVat = false,
                DisplayOrder = 1,
                Published = true
            };
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AA (Armed Forces Americas)",
                Abbreviation = "AA",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AE (Armed Forces Europe)",
                Abbreviation = "AE",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Alabama",
                Abbreviation = "AL",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Alaska",
                Abbreviation = "AK",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "American Samoa",
                Abbreviation = "AS",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AP (Armed Forces Pacific)",
                Abbreviation = "AP",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Arizona",
                Abbreviation = "AZ",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Arkansas",
                Abbreviation = "AR",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "California",
                Abbreviation = "CA",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Colorado",
                Abbreviation = "CO",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Connecticut",
                Abbreviation = "CT",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Delaware",
                Abbreviation = "DE",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "District of Columbia",
                Abbreviation = "DC",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Federated States of Micronesia",
                Abbreviation = "FM",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Florida",
                Abbreviation = "FL",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Georgia",
                Abbreviation = "GA",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Guam",
                Abbreviation = "GU",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Hawaii",
                Abbreviation = "HI",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Idaho",
                Abbreviation = "ID",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Illinois",
                Abbreviation = "IL",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Indiana",
                Abbreviation = "IN",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Iowa",
                Abbreviation = "IA",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Kansas",
                Abbreviation = "KS",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Kentucky",
                Abbreviation = "KY",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Louisiana",
                Abbreviation = "LA",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Maine",
                Abbreviation = "ME",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Marshall Islands",
                Abbreviation = "MH",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Maryland",
                Abbreviation = "MD",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Massachusetts",
                Abbreviation = "MA",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Michigan",
                Abbreviation = "MI",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Minnesota",
                Abbreviation = "MN",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Mississippi",
                Abbreviation = "MS",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Missouri",
                Abbreviation = "MO",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Montana",
                Abbreviation = "MT",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Nebraska",
                Abbreviation = "NE",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Nevada",
                Abbreviation = "NV",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Hampshire",
                Abbreviation = "NH",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Jersey",
                Abbreviation = "NJ",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Mexico",
                Abbreviation = "NM",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New York",
                Abbreviation = "NY",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "North Carolina",
                Abbreviation = "NC",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "North Dakota",
                Abbreviation = "ND",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Northern Mariana Islands",
                Abbreviation = "MP",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Ohio",
                Abbreviation = "OH",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Oklahoma",
                Abbreviation = "OK",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Oregon",
                Abbreviation = "OR",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Palau",
                Abbreviation = "PW",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Pennsylvania",
                Abbreviation = "PA",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Puerto Rico",
                Abbreviation = "PR",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Rhode Island",
                Abbreviation = "RI",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "South Carolina",
                Abbreviation = "SC",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "South Dakota",
                Abbreviation = "SD",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Tennessee",
                Abbreviation = "TN",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Texas",
                Abbreviation = "TX",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Utah",
                Abbreviation = "UT",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Vermont",
                Abbreviation = "VT",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Virgin Islands",
                Abbreviation = "VI",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Virginia",
                Abbreviation = "VA",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Washington",
                Abbreviation = "WA",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "West Virginia",
                Abbreviation = "WV",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Wisconsin",
                Abbreviation = "WI",
                Published = true,
                DisplayOrder = 1
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Wyoming",
                Abbreviation = "WY",
                Published = true,
                DisplayOrder = 1
            });
            var cCanada = new Country
            {
                Name = "Canada",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "CA",
                ThreeLetterIsoCode = "CAN",
                NumericIsoCode = 124,
                SubjectToVat = false,
                DisplayOrder = 100,
                Published = true
            };
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Alberta",
                Abbreviation = "AB",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "British Columbia",
                Abbreviation = "BC",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Manitoba",
                Abbreviation = "MB",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "New Brunswick",
                Abbreviation = "NB",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Newfoundland and Labrador",
                Abbreviation = "NL",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Northwest Territories",
                Abbreviation = "NT",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Nova Scotia",
                Abbreviation = "NS",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Nunavut",
                Abbreviation = "NU",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Ontario",
                Abbreviation = "ON",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Prince Edward Island",
                Abbreviation = "PE",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Quebec",
                Abbreviation = "QC",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Saskatchewan",
                Abbreviation = "SK",
                Published = true,
                DisplayOrder = 1
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Yukon Territory",
                Abbreviation = "YT",
                Published = true,
                DisplayOrder = 1
            });
            var countries = new List<Country>
            {
                cUsa,
                cCanada,
                //other countries
                new Country
                {
                    Name = "Argentina",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AR",
                    ThreeLetterIsoCode = "ARG",
                    NumericIsoCode = 32,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Armenia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AM",
                    ThreeLetterIsoCode = "ARM",
                    NumericIsoCode = 51,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Aruba",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AW",
                    ThreeLetterIsoCode = "ABW",
                    NumericIsoCode = 533,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Australia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AU",
                    ThreeLetterIsoCode = "AUS",
                    NumericIsoCode = 36,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Austria",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AT",
                    ThreeLetterIsoCode = "AUT",
                    NumericIsoCode = 40,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Azerbaijan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AZ",
                    ThreeLetterIsoCode = "AZE",
                    NumericIsoCode = 31,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bahamas",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BS",
                    ThreeLetterIsoCode = "BHS",
                    NumericIsoCode = 44,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bangladesh",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BD",
                    ThreeLetterIsoCode = "BGD",
                    NumericIsoCode = 50,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Belarus",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BY",
                    ThreeLetterIsoCode = "BLR",
                    NumericIsoCode = 112,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Belgium",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BE",
                    ThreeLetterIsoCode = "BEL",
                    NumericIsoCode = 56,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Belize",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BZ",
                    ThreeLetterIsoCode = "BLZ",
                    NumericIsoCode = 84,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bermuda",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BM",
                    ThreeLetterIsoCode = "BMU",
                    NumericIsoCode = 60,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bolivia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BO",
                    ThreeLetterIsoCode = "BOL",
                    NumericIsoCode = 68,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bosnia and Herzegowina",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BA",
                    ThreeLetterIsoCode = "BIH",
                    NumericIsoCode = 70,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Brazil",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BR",
                    ThreeLetterIsoCode = "BRA",
                    NumericIsoCode = 76,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bulgaria",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BG",
                    ThreeLetterIsoCode = "BGR",
                    NumericIsoCode = 100,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cayman Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KY",
                    ThreeLetterIsoCode = "CYM",
                    NumericIsoCode = 136,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Chile",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CL",
                    ThreeLetterIsoCode = "CHL",
                    NumericIsoCode = 152,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "China",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CN",
                    ThreeLetterIsoCode = "CHN",
                    NumericIsoCode = 156,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Colombia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CO",
                    ThreeLetterIsoCode = "COL",
                    NumericIsoCode = 170,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Costa Rica",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CR",
                    ThreeLetterIsoCode = "CRI",
                    NumericIsoCode = 188,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Croatia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "HR",
                    ThreeLetterIsoCode = "HRV",
                    NumericIsoCode = 191,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cuba",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CU",
                    ThreeLetterIsoCode = "CUB",
                    NumericIsoCode = 192,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cyprus",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CY",
                    ThreeLetterIsoCode = "CYP",
                    NumericIsoCode = 196,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Czech Republic",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CZ",
                    ThreeLetterIsoCode = "CZE",
                    NumericIsoCode = 203,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Denmark",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "DK",
                    ThreeLetterIsoCode = "DNK",
                    NumericIsoCode = 208,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Dominican Republic",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "DO",
                    ThreeLetterIsoCode = "DOM",
                    NumericIsoCode = 214,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "East Timor",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TL",
                    ThreeLetterIsoCode = "TLS",
                    NumericIsoCode = 626,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ecuador",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "EC",
                    ThreeLetterIsoCode = "ECU",
                    NumericIsoCode = 218,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Egypt",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "EG",
                    ThreeLetterIsoCode = "EGY",
                    NumericIsoCode = 818,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Finland",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "FI",
                    ThreeLetterIsoCode = "FIN",
                    NumericIsoCode = 246,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "France",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "FR",
                    ThreeLetterIsoCode = "FRA",
                    NumericIsoCode = 250,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Georgia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GE",
                    ThreeLetterIsoCode = "GEO",
                    NumericIsoCode = 268,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Germany",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "DE",
                    ThreeLetterIsoCode = "DEU",
                    NumericIsoCode = 276,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Gibraltar",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GI",
                    ThreeLetterIsoCode = "GIB",
                    NumericIsoCode = 292,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Greece",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GR",
                    ThreeLetterIsoCode = "GRC",
                    NumericIsoCode = 300,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guatemala",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GT",
                    ThreeLetterIsoCode = "GTM",
                    NumericIsoCode = 320,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Hong Kong",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "HK",
                    ThreeLetterIsoCode = "HKG",
                    NumericIsoCode = 344,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Hungary",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "HU",
                    ThreeLetterIsoCode = "HUN",
                    NumericIsoCode = 348,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "India",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "IN",
                    ThreeLetterIsoCode = "IND",
                    NumericIsoCode = 356,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Indonesia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ID",
                    ThreeLetterIsoCode = "IDN",
                    NumericIsoCode = 360,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ireland",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "IE",
                    ThreeLetterIsoCode = "IRL",
                    NumericIsoCode = 372,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Israel",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "IL",
                    ThreeLetterIsoCode = "ISR",
                    NumericIsoCode = 376,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Italy",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "IT",
                    ThreeLetterIsoCode = "ITA",
                    NumericIsoCode = 380,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Jamaica",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "JM",
                    ThreeLetterIsoCode = "JAM",
                    NumericIsoCode = 388,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Japan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "JP",
                    ThreeLetterIsoCode = "JPN",
                    NumericIsoCode = 392,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Jordan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "JO",
                    ThreeLetterIsoCode = "JOR",
                    NumericIsoCode = 400,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kazakhstan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KZ",
                    ThreeLetterIsoCode = "KAZ",
                    NumericIsoCode = 398,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Korea, Democratic People's Republic of",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KP",
                    ThreeLetterIsoCode = "PRK",
                    NumericIsoCode = 408,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kuwait",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KW",
                    ThreeLetterIsoCode = "KWT",
                    NumericIsoCode = 414,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Malaysia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MY",
                    ThreeLetterIsoCode = "MYS",
                    NumericIsoCode = 458,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mexico",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MX",
                    ThreeLetterIsoCode = "MEX",
                    NumericIsoCode = 484,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Netherlands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NL",
                    ThreeLetterIsoCode = "NLD",
                    NumericIsoCode = 528,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "New Zealand",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NZ",
                    ThreeLetterIsoCode = "NZL",
                    NumericIsoCode = 554,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Norway",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NO",
                    ThreeLetterIsoCode = "NOR",
                    NumericIsoCode = 578,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Pakistan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PK",
                    ThreeLetterIsoCode = "PAK",
                    NumericIsoCode = 586,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Palestine",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PS",
                    ThreeLetterIsoCode = "PSE",
                    NumericIsoCode = 275,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Paraguay",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PY",
                    ThreeLetterIsoCode = "PRY",
                    NumericIsoCode = 600,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Peru",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PE",
                    ThreeLetterIsoCode = "PER",
                    NumericIsoCode = 604,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Philippines",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PH",
                    ThreeLetterIsoCode = "PHL",
                    NumericIsoCode = 608,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Poland",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PL",
                    ThreeLetterIsoCode = "POL",
                    NumericIsoCode = 616,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Portugal",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PT",
                    ThreeLetterIsoCode = "PRT",
                    NumericIsoCode = 620,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Puerto Rico",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PR",
                    ThreeLetterIsoCode = "PRI",
                    NumericIsoCode = 630,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Qatar",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "QA",
                    ThreeLetterIsoCode = "QAT",
                    NumericIsoCode = 634,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Romania",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "RO",
                    ThreeLetterIsoCode = "ROM",
                    NumericIsoCode = 642,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Russian Federation",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "RU",
                    ThreeLetterIsoCode = "RUS",
                    NumericIsoCode = 643,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Saudi Arabia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SA",
                    ThreeLetterIsoCode = "SAU",
                    NumericIsoCode = 682,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Singapore",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SG",
                    ThreeLetterIsoCode = "SGP",
                    NumericIsoCode = 702,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Slovakia (Slovak Republic)",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SK",
                    ThreeLetterIsoCode = "SVK",
                    NumericIsoCode = 703,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Slovenia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SI",
                    ThreeLetterIsoCode = "SVN",
                    NumericIsoCode = 705,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "South Africa",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ZA",
                    ThreeLetterIsoCode = "ZAF",
                    NumericIsoCode = 710,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Spain",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ES",
                    ThreeLetterIsoCode = "ESP",
                    NumericIsoCode = 724,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sweden",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SE",
                    ThreeLetterIsoCode = "SWE",
                    NumericIsoCode = 752,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Switzerland",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CH",
                    ThreeLetterIsoCode = "CHE",
                    NumericIsoCode = 756,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Taiwan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TW",
                    ThreeLetterIsoCode = "TWN",
                    NumericIsoCode = 158,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Thailand",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TH",
                    ThreeLetterIsoCode = "THA",
                    NumericIsoCode = 764,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Turkey",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TR",
                    ThreeLetterIsoCode = "TUR",
                    NumericIsoCode = 792,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ukraine",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "UA",
                    ThreeLetterIsoCode = "UKR",
                    NumericIsoCode = 804,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "United Arab Emirates",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AE",
                    ThreeLetterIsoCode = "ARE",
                    NumericIsoCode = 784,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "United Kingdom",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GB",
                    ThreeLetterIsoCode = "GBR",
                    NumericIsoCode = 826,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "United States minor outlying islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "UM",
                    ThreeLetterIsoCode = "UMI",
                    NumericIsoCode = 581,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Uruguay",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "UY",
                    ThreeLetterIsoCode = "URY",
                    NumericIsoCode = 858,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Uzbekistan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "UZ",
                    ThreeLetterIsoCode = "UZB",
                    NumericIsoCode = 860,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Venezuela",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "VE",
                    ThreeLetterIsoCode = "VEN",
                    NumericIsoCode = 862,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Serbia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "RS",
                    ThreeLetterIsoCode = "SRB",
                    NumericIsoCode = 688,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Afghanistan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AF",
                    ThreeLetterIsoCode = "AFG",
                    NumericIsoCode = 4,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Albania",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AL",
                    ThreeLetterIsoCode = "ALB",
                    NumericIsoCode = 8,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Algeria",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "DZ",
                    ThreeLetterIsoCode = "DZA",
                    NumericIsoCode = 12,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "American Samoa",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AS",
                    ThreeLetterIsoCode = "ASM",
                    NumericIsoCode = 16,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Andorra",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AD",
                    ThreeLetterIsoCode = "AND",
                    NumericIsoCode = 20,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Angola",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AO",
                    ThreeLetterIsoCode = "AGO",
                    NumericIsoCode = 24,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Anguilla",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AI",
                    ThreeLetterIsoCode = "AIA",
                    NumericIsoCode = 660,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Antarctica",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AQ",
                    ThreeLetterIsoCode = "ATA",
                    NumericIsoCode = 10,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Antigua and Barbuda",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AG",
                    ThreeLetterIsoCode = "ATG",
                    NumericIsoCode = 28,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bahrain",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BH",
                    ThreeLetterIsoCode = "BHR",
                    NumericIsoCode = 48,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Barbados",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BB",
                    ThreeLetterIsoCode = "BRB",
                    NumericIsoCode = 52,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Benin",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BJ",
                    ThreeLetterIsoCode = "BEN",
                    NumericIsoCode = 204,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bhutan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BT",
                    ThreeLetterIsoCode = "BTN",
                    NumericIsoCode = 64,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Botswana",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BW",
                    ThreeLetterIsoCode = "BWA",
                    NumericIsoCode = 72,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Bouvet Island",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BV",
                    ThreeLetterIsoCode = "BVT",
                    NumericIsoCode = 74,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "British Indian Ocean Territory",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "IO",
                    ThreeLetterIsoCode = "IOT",
                    NumericIsoCode = 86,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Brunei Darussalam",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BN",
                    ThreeLetterIsoCode = "BRN",
                    NumericIsoCode = 96,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Burkina Faso",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BF",
                    ThreeLetterIsoCode = "BFA",
                    NumericIsoCode = 854,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Burundi",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "BI",
                    ThreeLetterIsoCode = "BDI",
                    NumericIsoCode = 108,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cambodia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KH",
                    ThreeLetterIsoCode = "KHM",
                    NumericIsoCode = 116,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cameroon",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CM",
                    ThreeLetterIsoCode = "CMR",
                    NumericIsoCode = 120,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cape Verde",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CV",
                    ThreeLetterIsoCode = "CPV",
                    NumericIsoCode = 132,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Central African Republic",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CF",
                    ThreeLetterIsoCode = "CAF",
                    NumericIsoCode = 140,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Chad",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TD",
                    ThreeLetterIsoCode = "TCD",
                    NumericIsoCode = 148,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Christmas Island",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CX",
                    ThreeLetterIsoCode = "CXR",
                    NumericIsoCode = 162,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cocos (Keeling) Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CC",
                    ThreeLetterIsoCode = "CCK",
                    NumericIsoCode = 166,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Comoros",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KM",
                    ThreeLetterIsoCode = "COM",
                    NumericIsoCode = 174,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Congo",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CG",
                    ThreeLetterIsoCode = "COG",
                    NumericIsoCode = 178,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Congo (Democratic Republic of the)",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CD",
                    ThreeLetterIsoCode = "COD",
                    NumericIsoCode = 180,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cook Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CK",
                    ThreeLetterIsoCode = "COK",
                    NumericIsoCode = 184,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Cote D'Ivoire",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "CI",
                    ThreeLetterIsoCode = "CIV",
                    NumericIsoCode = 384,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Djibouti",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "DJ",
                    ThreeLetterIsoCode = "DJI",
                    NumericIsoCode = 262,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Dominica",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "DM",
                    ThreeLetterIsoCode = "DMA",
                    NumericIsoCode = 212,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "El Salvador",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SV",
                    ThreeLetterIsoCode = "SLV",
                    NumericIsoCode = 222,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Equatorial Guinea",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GQ",
                    ThreeLetterIsoCode = "GNQ",
                    NumericIsoCode = 226,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Eritrea",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ER",
                    ThreeLetterIsoCode = "ERI",
                    NumericIsoCode = 232,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Estonia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "EE",
                    ThreeLetterIsoCode = "EST",
                    NumericIsoCode = 233,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ethiopia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ET",
                    ThreeLetterIsoCode = "ETH",
                    NumericIsoCode = 231,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Falkland Islands (Malvinas)",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "FK",
                    ThreeLetterIsoCode = "FLK",
                    NumericIsoCode = 238,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Faroe Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "FO",
                    ThreeLetterIsoCode = "FRO",
                    NumericIsoCode = 234,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Fiji",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "FJ",
                    ThreeLetterIsoCode = "FJI",
                    NumericIsoCode = 242,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "French Guiana",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GF",
                    ThreeLetterIsoCode = "GUF",
                    NumericIsoCode = 254,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "French Polynesia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PF",
                    ThreeLetterIsoCode = "PYF",
                    NumericIsoCode = 258,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "French Southern Territories",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TF",
                    ThreeLetterIsoCode = "ATF",
                    NumericIsoCode = 260,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Gabon",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GA",
                    ThreeLetterIsoCode = "GAB",
                    NumericIsoCode = 266,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Gambia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GM",
                    ThreeLetterIsoCode = "GMB",
                    NumericIsoCode = 270,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Ghana",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GH",
                    ThreeLetterIsoCode = "GHA",
                    NumericIsoCode = 288,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Greenland",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GL",
                    ThreeLetterIsoCode = "GRL",
                    NumericIsoCode = 304,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Grenada",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GD",
                    ThreeLetterIsoCode = "GRD",
                    NumericIsoCode = 308,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guadeloupe",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GP",
                    ThreeLetterIsoCode = "GLP",
                    NumericIsoCode = 312,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guam",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GU",
                    ThreeLetterIsoCode = "GUM",
                    NumericIsoCode = 316,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guinea",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GN",
                    ThreeLetterIsoCode = "GIN",
                    NumericIsoCode = 324,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guinea-bissau",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GW",
                    ThreeLetterIsoCode = "GNB",
                    NumericIsoCode = 624,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Guyana",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GY",
                    ThreeLetterIsoCode = "GUY",
                    NumericIsoCode = 328,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Haiti",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "HT",
                    ThreeLetterIsoCode = "HTI",
                    NumericIsoCode = 332,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Heard and Mc Donald Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "HM",
                    ThreeLetterIsoCode = "HMD",
                    NumericIsoCode = 334,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Honduras",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "HN",
                    ThreeLetterIsoCode = "HND",
                    NumericIsoCode = 340,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Iceland",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "IS",
                    ThreeLetterIsoCode = "ISL",
                    NumericIsoCode = 352,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Iran (Islamic Republic of)",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "IR",
                    ThreeLetterIsoCode = "IRN",
                    NumericIsoCode = 364,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Iraq",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "IQ",
                    ThreeLetterIsoCode = "IRQ",
                    NumericIsoCode = 368,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kenya",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KE",
                    ThreeLetterIsoCode = "KEN",
                    NumericIsoCode = 404,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kiribati",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KI",
                    ThreeLetterIsoCode = "KIR",
                    NumericIsoCode = 296,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Korea",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KR",
                    ThreeLetterIsoCode = "KOR",
                    NumericIsoCode = 410,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Kyrgyzstan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KG",
                    ThreeLetterIsoCode = "KGZ",
                    NumericIsoCode = 417,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Lao People's Democratic Republic",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LA",
                    ThreeLetterIsoCode = "LAO",
                    NumericIsoCode = 418,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Latvia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LV",
                    ThreeLetterIsoCode = "LVA",
                    NumericIsoCode = 428,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Lebanon",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LB",
                    ThreeLetterIsoCode = "LBN",
                    NumericIsoCode = 422,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Lesotho",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LS",
                    ThreeLetterIsoCode = "LSO",
                    NumericIsoCode = 426,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Liberia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LR",
                    ThreeLetterIsoCode = "LBR",
                    NumericIsoCode = 430,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Libyan Arab Jamahiriya",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LY",
                    ThreeLetterIsoCode = "LBY",
                    NumericIsoCode = 434,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Liechtenstein",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LI",
                    ThreeLetterIsoCode = "LIE",
                    NumericIsoCode = 438,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Lithuania",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LT",
                    ThreeLetterIsoCode = "LTU",
                    NumericIsoCode = 440,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Luxembourg",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LU",
                    ThreeLetterIsoCode = "LUX",
                    NumericIsoCode = 442,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Macau",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MO",
                    ThreeLetterIsoCode = "MAC",
                    NumericIsoCode = 446,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Macedonia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MK",
                    ThreeLetterIsoCode = "MKD",
                    NumericIsoCode = 807,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Madagascar",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MG",
                    ThreeLetterIsoCode = "MDG",
                    NumericIsoCode = 450,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Malawi",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MW",
                    ThreeLetterIsoCode = "MWI",
                    NumericIsoCode = 454,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Maldives",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MV",
                    ThreeLetterIsoCode = "MDV",
                    NumericIsoCode = 462,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mali",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ML",
                    ThreeLetterIsoCode = "MLI",
                    NumericIsoCode = 466,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Malta",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MT",
                    ThreeLetterIsoCode = "MLT",
                    NumericIsoCode = 470,
                    SubjectToVat = true,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Marshall Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MH",
                    ThreeLetterIsoCode = "MHL",
                    NumericIsoCode = 584,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Martinique",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MQ",
                    ThreeLetterIsoCode = "MTQ",
                    NumericIsoCode = 474,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mauritania",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MR",
                    ThreeLetterIsoCode = "MRT",
                    NumericIsoCode = 478,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mauritius",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MU",
                    ThreeLetterIsoCode = "MUS",
                    NumericIsoCode = 480,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mayotte",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "YT",
                    ThreeLetterIsoCode = "MYT",
                    NumericIsoCode = 175,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Micronesia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "FM",
                    ThreeLetterIsoCode = "FSM",
                    NumericIsoCode = 583,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Moldova",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MD",
                    ThreeLetterIsoCode = "MDA",
                    NumericIsoCode = 498,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Monaco",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MC",
                    ThreeLetterIsoCode = "MCO",
                    NumericIsoCode = 492,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mongolia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MN",
                    ThreeLetterIsoCode = "MNG",
                    NumericIsoCode = 496,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Montenegro",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ME",
                    ThreeLetterIsoCode = "MNE",
                    NumericIsoCode = 499,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Montserrat",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MS",
                    ThreeLetterIsoCode = "MSR",
                    NumericIsoCode = 500,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Morocco",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MA",
                    ThreeLetterIsoCode = "MAR",
                    NumericIsoCode = 504,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Mozambique",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MZ",
                    ThreeLetterIsoCode = "MOZ",
                    NumericIsoCode = 508,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Myanmar",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MM",
                    ThreeLetterIsoCode = "MMR",
                    NumericIsoCode = 104,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Namibia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NA",
                    ThreeLetterIsoCode = "NAM",
                    NumericIsoCode = 516,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Nauru",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NR",
                    ThreeLetterIsoCode = "NRU",
                    NumericIsoCode = 520,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Nepal",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NP",
                    ThreeLetterIsoCode = "NPL",
                    NumericIsoCode = 524,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Netherlands Antilles",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "AN",
                    ThreeLetterIsoCode = "ANT",
                    NumericIsoCode = 530,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "New Caledonia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NC",
                    ThreeLetterIsoCode = "NCL",
                    NumericIsoCode = 540,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Nicaragua",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NI",
                    ThreeLetterIsoCode = "NIC",
                    NumericIsoCode = 558,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Niger",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NE",
                    ThreeLetterIsoCode = "NER",
                    NumericIsoCode = 562,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Nigeria",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NG",
                    ThreeLetterIsoCode = "NGA",
                    NumericIsoCode = 566,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Niue",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NU",
                    ThreeLetterIsoCode = "NIU",
                    NumericIsoCode = 570,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Norfolk Island",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "NF",
                    ThreeLetterIsoCode = "NFK",
                    NumericIsoCode = 574,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Northern Mariana Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "MP",
                    ThreeLetterIsoCode = "MNP",
                    NumericIsoCode = 580,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Oman",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "OM",
                    ThreeLetterIsoCode = "OMN",
                    NumericIsoCode = 512,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Palau",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PW",
                    ThreeLetterIsoCode = "PLW",
                    NumericIsoCode = 585,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Panama",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PA",
                    ThreeLetterIsoCode = "PAN",
                    NumericIsoCode = 591,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Papua New Guinea",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PG",
                    ThreeLetterIsoCode = "PNG",
                    NumericIsoCode = 598,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Pitcairn",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PN",
                    ThreeLetterIsoCode = "PCN",
                    NumericIsoCode = 612,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Reunion",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "RE",
                    ThreeLetterIsoCode = "REU",
                    NumericIsoCode = 638,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Rwanda",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "RW",
                    ThreeLetterIsoCode = "RWA",
                    NumericIsoCode = 646,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Saint Kitts and Nevis",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "KN",
                    ThreeLetterIsoCode = "KNA",
                    NumericIsoCode = 659,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Saint Lucia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LC",
                    ThreeLetterIsoCode = "LCA",
                    NumericIsoCode = 662,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Saint Vincent and the Grenadines",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "VC",
                    ThreeLetterIsoCode = "VCT",
                    NumericIsoCode = 670,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Samoa",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "WS",
                    ThreeLetterIsoCode = "WSM",
                    NumericIsoCode = 882,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "San Marino",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SM",
                    ThreeLetterIsoCode = "SMR",
                    NumericIsoCode = 674,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sao Tome and Principe",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ST",
                    ThreeLetterIsoCode = "STP",
                    NumericIsoCode = 678,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Senegal",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SN",
                    ThreeLetterIsoCode = "SEN",
                    NumericIsoCode = 686,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Seychelles",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SC",
                    ThreeLetterIsoCode = "SYC",
                    NumericIsoCode = 690,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sierra Leone",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SL",
                    ThreeLetterIsoCode = "SLE",
                    NumericIsoCode = 694,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Solomon Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SB",
                    ThreeLetterIsoCode = "SLB",
                    NumericIsoCode = 90,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Somalia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SO",
                    ThreeLetterIsoCode = "SOM",
                    NumericIsoCode = 706,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "South Georgia & South Sandwich Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "GS",
                    ThreeLetterIsoCode = "SGS",
                    NumericIsoCode = 239,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "South Sudan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SS",
                    ThreeLetterIsoCode = "SSD",
                    NumericIsoCode = 728,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sri Lanka",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "LK",
                    ThreeLetterIsoCode = "LKA",
                    NumericIsoCode = 144,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "St. Helena",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SH",
                    ThreeLetterIsoCode = "SHN",
                    NumericIsoCode = 654,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "St. Pierre and Miquelon",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "PM",
                    ThreeLetterIsoCode = "SPM",
                    NumericIsoCode = 666,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Sudan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SD",
                    ThreeLetterIsoCode = "SDN",
                    NumericIsoCode = 736,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Suriname",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SR",
                    ThreeLetterIsoCode = "SUR",
                    NumericIsoCode = 740,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Svalbard and Jan Mayen Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SJ",
                    ThreeLetterIsoCode = "SJM",
                    NumericIsoCode = 744,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Swaziland",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SZ",
                    ThreeLetterIsoCode = "SWZ",
                    NumericIsoCode = 748,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Syrian Arab Republic",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "SY",
                    ThreeLetterIsoCode = "SYR",
                    NumericIsoCode = 760,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tajikistan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TJ",
                    ThreeLetterIsoCode = "TJK",
                    NumericIsoCode = 762,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tanzania",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TZ",
                    ThreeLetterIsoCode = "TZA",
                    NumericIsoCode = 834,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Togo",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TG",
                    ThreeLetterIsoCode = "TGO",
                    NumericIsoCode = 768,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tokelau",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TK",
                    ThreeLetterIsoCode = "TKL",
                    NumericIsoCode = 772,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tonga",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TO",
                    ThreeLetterIsoCode = "TON",
                    NumericIsoCode = 776,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Trinidad and Tobago",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TT",
                    ThreeLetterIsoCode = "TTO",
                    NumericIsoCode = 780,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tunisia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TN",
                    ThreeLetterIsoCode = "TUN",
                    NumericIsoCode = 788,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Turkmenistan",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TM",
                    ThreeLetterIsoCode = "TKM",
                    NumericIsoCode = 795,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Turks and Caicos Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TC",
                    ThreeLetterIsoCode = "TCA",
                    NumericIsoCode = 796,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Tuvalu",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "TV",
                    ThreeLetterIsoCode = "TUV",
                    NumericIsoCode = 798,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Uganda",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "UG",
                    ThreeLetterIsoCode = "UGA",
                    NumericIsoCode = 800,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Vanuatu",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "VU",
                    ThreeLetterIsoCode = "VUT",
                    NumericIsoCode = 548,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Vatican City State (Holy See)",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "VA",
                    ThreeLetterIsoCode = "VAT",
                    NumericIsoCode = 336,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Viet Nam",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "VN",
                    ThreeLetterIsoCode = "VNM",
                    NumericIsoCode = 704,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Virgin Islands (British)",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "VG",
                    ThreeLetterIsoCode = "VGB",
                    NumericIsoCode = 92,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Virgin Islands (U.S.)",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "VI",
                    ThreeLetterIsoCode = "VIR",
                    NumericIsoCode = 850,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Wallis and Futuna Islands",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "WF",
                    ThreeLetterIsoCode = "WLF",
                    NumericIsoCode = 876,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Western Sahara",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "EH",
                    ThreeLetterIsoCode = "ESH",
                    NumericIsoCode = 732,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Yemen",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "YE",
                    ThreeLetterIsoCode = "YEM",
                    NumericIsoCode = 887,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Zambia",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ZM",
                    ThreeLetterIsoCode = "ZMB",
                    NumericIsoCode = 894,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                },
                new Country
                {
                    Name = "Zimbabwe",
                    AllowsBilling = true,
                    AllowsShipping = true,
                    TwoLetterIsoCode = "ZW",
                    ThreeLetterIsoCode = "ZWE",
                    NumericIsoCode = 716,
                    SubjectToVat = false,
                    DisplayOrder = 100,
                    Published = true
                }
            };
            _countryRepository.Insert(countries);
        }

        protected virtual void InstallShippingMethods()
        {
            var shippingMethods = new List<ShippingMethod>
            {
                new ShippingMethod
                {
                    Name = "Ground",
                    Description =
                        "Shipping by land transport",
                    DisplayOrder = 1
                },
                new ShippingMethod
                {
                    Name = "Next Day Air",
                    Description = "The one day air shipping",
                    DisplayOrder = 2
                },
                new ShippingMethod
                {
                    Name = "2nd Day Air",
                    Description = "The two day air shipping",
                    DisplayOrder = 3
                }
            };
            _shippingMethodRepository.Insert(shippingMethods);
        }

        protected virtual void InstallDeliveryDates()
        {
            var deliveryDates = new List<DeliveryDate>
            {
                new DeliveryDate
                {
                    Name = "1-2 days",
                    DisplayOrder = 1
                },
                new DeliveryDate
                {
                    Name = "3-5 days",
                    DisplayOrder = 5
                },
                new DeliveryDate
                {
                    Name = "1 week",
                    DisplayOrder = 10
                }
            };
            _deliveryDateRepository.Insert(deliveryDates);
        }

        protected virtual void InstallProductAvailabilityRanges()
        {
            var productAvailabilityRanges = new List<ProductAvailabilityRange>
            {
                new ProductAvailabilityRange
                {
                    Name = "2-4 days",
                    DisplayOrder = 1
                },
                new ProductAvailabilityRange
                {
                    Name = "7-10 days",
                    DisplayOrder = 2
                },
                new ProductAvailabilityRange
                {
                    Name = "2 week",
                    DisplayOrder = 3
                }
            };
            _productAvailabilityRangeRepository.Insert(productAvailabilityRanges);
        }

        protected virtual void InstallSampleCustomers()
        {
            var crRegistered = _customerRoleRepository.Table.FirstOrDefault(customerRole =>
                customerRole.SystemName.Equals(NopCustomerDefaults.RegisteredRoleName,
                    StringComparison.InvariantCultureIgnoreCase));

            if (crRegistered == null)
                throw new ArgumentNullException(nameof(crRegistered));

            //default store 
            var defaultStore = _storeRepository.Table.FirstOrDefault();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeId = defaultStore.Id;

            //second user
            var secondUserEmail = "steve_gates@nopCommerce.com";
            var secondUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = secondUserEmail,
                Username = secondUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSecondUserAddress = new Address
            {
                FirstName = "Steve",
                LastName = "Gates",
                PhoneNumber = "87654321",
                Email = secondUserEmail,
                FaxNumber = string.Empty,
                Company = "Steve Company",
                Address1 = "750 Bel Air Rd.",
                Address2 = string.Empty,
                City = "Los Angeles",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "90077",
                CreatedOnUtc = DateTime.UtcNow
            };
            //secondUser.Addresses.Add(defaultSecondUserAddress);
            secondUser.CustomerAddressMappings.Add(new CustomerAddressMapping {Address = defaultSecondUserAddress});
            secondUser.BillingAddress = defaultSecondUserAddress;
            secondUser.ShippingAddress = defaultSecondUserAddress;

            //secondUser.CustomerRoles.Add(crRegistered);
            secondUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping {CustomerRole = crRegistered});

            _customerRepository.Insert(secondUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(secondUser, NopCustomerDefaults.FirstNameAttribute, defaultSecondUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(secondUser, NopCustomerDefaults.LastNameAttribute, defaultSecondUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = secondUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //third user
            var thirdUserEmail = "arthur_holmes@nopCommerce.com";
            var thirdUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = thirdUserEmail,
                Username = thirdUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultThirdUserAddress = new Address
            {
                FirstName = "Arthur",
                LastName = "Holmes",
                PhoneNumber = "111222333",
                Email = thirdUserEmail,
                FaxNumber = string.Empty,
                Company = "Holmes Company",
                Address1 = "221B Baker Street",
                Address2 = string.Empty,
                City = "London",
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "NW1 6XE",
                CreatedOnUtc = DateTime.UtcNow
            };
            //thirdUser.Addresses.Add(defaultThirdUserAddress);
            thirdUser.CustomerAddressMappings.Add(new CustomerAddressMapping {Address = defaultThirdUserAddress});
            thirdUser.BillingAddress = defaultThirdUserAddress;
            thirdUser.ShippingAddress = defaultThirdUserAddress;

            //thirdUser.CustomerRoles.Add(crRegistered);
            thirdUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping {CustomerRole = crRegistered});

            _customerRepository.Insert(thirdUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(thirdUser, NopCustomerDefaults.FirstNameAttribute, defaultThirdUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(thirdUser, NopCustomerDefaults.LastNameAttribute, defaultThirdUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = thirdUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fourth user
            var fourthUserEmail = "james_pan@nopCommerce.com";
            var fourthUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = fourthUserEmail,
                Username = fourthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFourthUserAddress = new Address
            {
                FirstName = "James",
                LastName = "Pan",
                PhoneNumber = "369258147",
                Email = fourthUserEmail,
                FaxNumber = string.Empty,
                Company = "Pan Company",
                Address1 = "St Katharine’s West 16",
                Address2 = string.Empty,
                City = "St Andrews",
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "KY16 9AX",
                CreatedOnUtc = DateTime.UtcNow
            };
            //fourthUser.Addresses.Add(defaultFourthUserAddress);
            fourthUser.CustomerAddressMappings.Add(new CustomerAddressMapping {Address = defaultFourthUserAddress});
            fourthUser.BillingAddress = defaultFourthUserAddress;
            fourthUser.ShippingAddress = defaultFourthUserAddress;

            //fourthUser.CustomerRoles.Add(crRegistered);
            fourthUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping {CustomerRole = crRegistered});

            _customerRepository.Insert(fourthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(fourthUser, NopCustomerDefaults.FirstNameAttribute, defaultFourthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fourthUser, NopCustomerDefaults.LastNameAttribute, defaultFourthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = fourthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fifth user
            var fifthUserEmail = "brenda_lindgren@nopCommerce.com";
            var fifthUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = fifthUserEmail,
                Username = fifthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFifthUserAddress = new Address
            {
                FirstName = "Brenda",
                LastName = "Lindgren",
                PhoneNumber = "14785236",
                Email = fifthUserEmail,
                FaxNumber = string.Empty,
                Company = "Brenda Company",
                Address1 = "1249 Tongass Avenue, Suite B",
                Address2 = string.Empty,
                City = "Ketchikan",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Alaska"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "99901",
                CreatedOnUtc = DateTime.UtcNow
            };
            //fifthUser.Addresses.Add(defaultFifthUserAddress);
            fifthUser.CustomerAddressMappings.Add(new CustomerAddressMapping {Address = defaultFifthUserAddress});
            fifthUser.BillingAddress = defaultFifthUserAddress;
            fifthUser.ShippingAddress = defaultFifthUserAddress;

            //fifthUser.CustomerRoles.Add(crRegistered);
            fifthUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping {CustomerRole = crRegistered});

            _customerRepository.Insert(fifthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(fifthUser, NopCustomerDefaults.FirstNameAttribute,
                defaultFifthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fifthUser, NopCustomerDefaults.LastNameAttribute,
                defaultFifthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = fifthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //sixth user
            var sixthUserEmail = "victoria_victoria@nopCommerce.com";
            var sixthUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = sixthUserEmail,
                Username = sixthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSixthUserAddress = new Address
            {
                FirstName = "Victoria",
                LastName = "Terces",
                PhoneNumber = "45612378",
                Email = sixthUserEmail,
                FaxNumber = string.Empty,
                Company = "Terces Company",
                Address1 = "201 1st Avenue South",
                Address2 = string.Empty,
                City = "Saskatoon",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Saskatchewan"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "CAN"),
                ZipPostalCode = "S7K 1J9",
                CreatedOnUtc = DateTime.UtcNow
            };
            //sixthUser.Addresses.Add(defaultSixthUserAddress);
            sixthUser.CustomerAddressMappings.Add(new CustomerAddressMapping {Address = defaultSixthUserAddress});
            sixthUser.BillingAddress = defaultSixthUserAddress;
            sixthUser.ShippingAddress = defaultSixthUserAddress;

            //sixthUser.CustomerRoles.Add(crRegistered);
            //__sixthUser.CustomerCustomerRoleMappings.Add(new CustomerCustomerRoleMapping { CustomerRole = crRegistered });
            sixthUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping {CustomerRole = crRegistered});

            _customerRepository.Insert(sixthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(sixthUser, NopCustomerDefaults.FirstNameAttribute, defaultSixthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(sixthUser, NopCustomerDefaults.LastNameAttribute, defaultSixthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = sixthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });
        }

        protected virtual void InstallCustomersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new CustomerRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = NopCustomerDefaults.AdministratorsRoleName
            };
            var crForumModerators = new CustomerRole
            {
                Name = "Forum Moderators",
                Active = true,
                IsSystemRole = true,
                SystemName = NopCustomerDefaults.ForumModeratorsRoleName
            };
            var crRegistered = new CustomerRole
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = NopCustomerDefaults.RegisteredRoleName
            };
            var crGuests = new CustomerRole
            {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = NopCustomerDefaults.GuestsRoleName
            };
            var crVendors = new CustomerRole
            {
                Name = "Vendors",
                Active = true,
                IsSystemRole = true,
                SystemName = NopCustomerDefaults.VendorsRoleName
            };
            var customerRoles = new List<CustomerRole>
            {
                crAdministrators,
                crForumModerators,
                crRegistered,
                crGuests,
                crVendors
            };
            _customerRoleRepository.Insert(customerRoles);

            //default store 
            var defaultStore = _storeRepository.Table.FirstOrDefault();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeId = defaultStore.Id;

            //admin user
            var adminUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };

            var defaultAdminUserAddress = new Address
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "12345678",
                Email = defaultUserEmail,
                FaxNumber = string.Empty,
                Company = "Nop Solutions Ltd",
                Address1 = "21 West 52nd Street",
                Address2 = string.Empty,
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow
            };
            //adminUser.Addresses.Add(defaultAdminUserAddress);
            adminUser.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = defaultAdminUserAddress });
            adminUser.BillingAddress = defaultAdminUserAddress;
            adminUser.ShippingAddress = defaultAdminUserAddress;

            //adminUser.CustomerRoles.Add(crAdministrators);
            //adminUser.CustomerRoles.Add(crForumModerators);
            //adminUser.CustomerRoles.Add(crRegistered);
            adminUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerRole = crAdministrators });
            adminUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerRole = crForumModerators });
            adminUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerRole = crRegistered });

            _customerRepository.Insert(adminUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(adminUser, NopCustomerDefaults.FirstNameAttribute, "John");
            _genericAttributeService.SaveAttribute(adminUser, NopCustomerDefaults.LastNameAttribute, "Smith");

            //set hashed admin password
            var customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword, null, NopCustomerServiceDefaults.DefaultHashedPasswordFormat));
            
            //search engine (crawler) built-in user
            var searchEngineUser = new Customer
            {
                Email = "builtin@search_engine_record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = NopCustomerDefaults.SearchEngineCustomerName,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            //searchEngineUser.CustomerRoles.Add(crGuests);
            //__searchEngineUser.CustomerCustomerRoleMappings.Add(new CustomerCustomerRoleMapping { CustomerRole = crGuests });
            searchEngineUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerRole = crGuests });
            _customerRepository.Insert(searchEngineUser);

            //built-in user for background tasks
            var backgroundTaskUser = new Customer
            {
                Email = "builtin@background-task-record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = NopCustomerDefaults.BackgroundTaskCustomerName,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            //backgroundTaskUser.CustomerRoles.Add(crGuests);
            backgroundTaskUser.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerRole = crGuests });
            _customerRepository.Insert(backgroundTaskUser);
        }

        protected virtual void InstallOrders()
        {
            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            //first order
            var firstCustomer = _customerRepository.Table.First(c => c.Email.Equals("steve_gates@nopCommerce.com"));
            var firstOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                Customer = firstCustomer,
                CustomerLanguageId = _languageRepository.Table.First().Id,
                CustomerIp = "127.0.0.1",
                OrderSubtotalInclTax = 1855M,
                OrderSubtotalExclTax = 1855M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 1855M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                CustomerCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Processing,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Paid,
                PaidDateUtc = DateTime.UtcNow,
                BillingAddress = (Address)firstCustomer.BillingAddress.Clone(),
                ShippingAddress = (Address)firstCustomer.ShippingAddress.Clone(),
                ShippingStatus = ShippingStatus.NotYetShipped,
                ShippingMethod = "Ground",
                PickupInStore = false,
                ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };
            _orderRepository.Insert(firstOrder);
            firstOrder.CustomOrderNumber = firstOrder.Id.ToString();
            _orderRepository.Update(firstOrder);

            //item Apple iCam
            var firstOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = firstOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("Apple iCam")).Id,
                UnitPriceInclTax = 1300M,
                UnitPriceExclTax = 1300M,
                PriceInclTax = 1300M,
                PriceExclTax = 1300M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(firstOrderItem1);

            //item Leica T Mirrorless Digital Camera
            var fierstOrderItem2 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = firstOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("Leica T Mirrorless Digital Camera")).Id,
                UnitPriceInclTax = 530M,
                UnitPriceExclTax = 530M,
                PriceInclTax = 530M,
                PriceExclTax = 530M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(fierstOrderItem2);

            //item $25 Virtual Gift Card
            var firstOrderItem3 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = firstOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("$25 Virtual Gift Card")).Id,
                UnitPriceInclTax = 25M,
                UnitPriceExclTax = 25M,
                PriceInclTax = 25M,
                PriceExclTax = 25M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = "From: Steve Gates &lt;steve_gates@nopCommerce.com&gt;<br />For: Brenda Lindgren &lt;brenda_lindgren@nopCommerce.com&gt;",
                AttributesXml = "<Attributes><GiftCardInfo><RecipientName>Brenda Lindgren</RecipientName><RecipientEmail>brenda_lindgren@nopCommerce.com</RecipientEmail><SenderName>Steve Gates</SenderName><SenderEmail>steve_gates@gmail.com</SenderEmail><Message></Message></GiftCardInfo></Attributes>",
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(firstOrderItem3);

            var firstOrderGiftcard = new GiftCard
            {
                GiftCardType = GiftCardType.Virtual,
                PurchasedWithOrderItem = firstOrderItem3,
                Amount = 25M,
                IsGiftCardActivated = false,
                GiftCardCouponCode = string.Empty,
                RecipientName = "Brenda Lindgren",
                RecipientEmail = "brenda_lindgren@nopCommerce.com",
                SenderName = "Steve Gates",
                SenderEmail = "steve_gates@nopCommerce.com",
                Message = string.Empty,
                IsRecipientNotified = false,
                CreatedOnUtc = DateTime.UtcNow
            };
            _giftCardRepository.Insert(firstOrderGiftcard);

            //order notes
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                Order = firstOrder
            });
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order paid",
                Order = firstOrder
            });

            //second order
            var secondCustomer = _customerRepository.Table.First(c => c.Email.Equals("arthur_holmes@nopCommerce.com"));
            var secondOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                Customer = secondCustomer,
                CustomerLanguageId = _languageRepository.Table.First().Id,
                CustomerIp = "127.0.0.1",
                OrderSubtotalInclTax = 2460M,
                OrderSubtotalExclTax = 2460M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 2460M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                CustomerCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Pending,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Pending,
                PaidDateUtc = null,
                BillingAddress = (Address)secondCustomer.BillingAddress.Clone(),
                ShippingAddress = (Address)secondCustomer.ShippingAddress.Clone(),
                ShippingStatus = ShippingStatus.NotYetShipped,
                ShippingMethod = "Next Day Air",
                PickupInStore = false,
                ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };
            _orderRepository.Insert(secondOrder);
            secondOrder.CustomOrderNumber = secondOrder.Id.ToString();
            _orderRepository.Update(secondOrder);

            //order notes
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                Order = secondOrder
            });

            //item Vintage Style Engagement Ring
            var secondOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = secondOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("Vintage Style Engagement Ring")).Id,
                UnitPriceInclTax = 2100M,
                UnitPriceExclTax = 2100M,
                PriceInclTax = 2100M,
                PriceExclTax = 2100M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(secondOrderItem1);

            //item Flower Girl Bracelet
            var secondOrderItem2 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = secondOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("Flower Girl Bracelet")).Id,
                UnitPriceInclTax = 360M,
                UnitPriceExclTax = 360M,
                PriceInclTax = 360M,
                PriceExclTax = 360M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(secondOrderItem2);

            //third order
            var thirdCustomer = _customerRepository.Table.First(c => c.Email.Equals("james_pan@nopCommerce.com"));
            var thirdOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                Customer = thirdCustomer,
                CustomerLanguageId = _languageRepository.Table.First().Id,
                CustomerIp = "127.0.0.1",
                OrderSubtotalInclTax = 8.80M,
                OrderSubtotalExclTax = 8.80M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 8.80M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                CustomerCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Pending,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Pending,
                PaidDateUtc = null,
                BillingAddress = (Address)thirdCustomer.BillingAddress.Clone(),
                ShippingAddress = null,
                ShippingStatus = ShippingStatus.ShippingNotRequired,
                ShippingMethod = string.Empty,
                PickupInStore = false,
                ShippingRateComputationMethodSystemName = string.Empty,
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };
            _orderRepository.Insert(thirdOrder);
            thirdOrder.CustomOrderNumber = thirdOrder.Id.ToString();
            _orderRepository.Update(thirdOrder);

            //order notes
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                Order = thirdOrder
            });

            //item If You Wait
            var thirdOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = thirdOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("If You Wait (donation)")).Id,
                UnitPriceInclTax = 3M,
                UnitPriceExclTax = 3M,
                PriceInclTax = 3M,
                PriceExclTax = 3M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(thirdOrderItem1);

            //item Night Visions
            var thirdOrderItem2 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = thirdOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("Night Visions")).Id,
                UnitPriceInclTax = 2.8M,
                UnitPriceExclTax = 2.8M,
                PriceInclTax = 2.8M,
                PriceExclTax = 2.8M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(thirdOrderItem2);

            //item Science & Faith
            var thirdOrderItem3 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = thirdOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("Science & Faith")).Id,
                UnitPriceInclTax = 3M,
                UnitPriceExclTax = 3M,
                PriceInclTax = 3M,
                PriceExclTax = 3M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(thirdOrderItem3);

            //fourth order
            var fourthCustomer = _customerRepository.Table.First(c => c.Email.Equals("brenda_lindgren@nopCommerce.com"));
            var fourthOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                Customer = fourthCustomer,
                CustomerLanguageId = _languageRepository.Table.First().Id,
                CustomerIp = "127.0.0.1",
                OrderSubtotalInclTax = 102M,
                OrderSubtotalExclTax = 102M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 102M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                CustomerCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Processing,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Paid,
                PaidDateUtc = DateTime.UtcNow,
                BillingAddress = (Address)fourthCustomer.BillingAddress.Clone(),
                ShippingAddress = (Address)fourthCustomer.ShippingAddress.Clone(),
                ShippingStatus = ShippingStatus.Shipped,
                ShippingMethod = "Pickup in store",
                PickupInStore = true,
                PickupAddress = (Address)fourthCustomer.ShippingAddress.Clone(),
                ShippingRateComputationMethodSystemName = "Pickup.PickupInStore",
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };
            _orderRepository.Insert(fourthOrder);
            fourthOrder.CustomOrderNumber = fourthOrder.Id.ToString();
            _orderRepository.Update(fourthOrder);

            //order notes
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                Order = fourthOrder
            });
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order paid",
                Order = fourthOrder
            });
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order shipped",
                Order = fourthOrder
            });

            //item Pride and Prejudice
            var fourthOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = fourthOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("Pride and Prejudice")).Id,
                UnitPriceInclTax = 24M,
                UnitPriceExclTax = 24M,
                PriceInclTax = 24M,
                PriceExclTax = 24M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(fourthOrderItem1);

            //item First Prize Pies
            var fourthOrderItem2 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = fourthOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("First Prize Pies")).Id,
                UnitPriceInclTax = 51M,
                UnitPriceExclTax = 51M,
                PriceInclTax = 51M,
                PriceExclTax = 51M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(fourthOrderItem2);

            //item Fahrenheit 451 by Ray Bradbury
            var fourthOrderItem3 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = fourthOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("Fahrenheit 451 by Ray Bradbury")).Id,
                UnitPriceInclTax = 27M,
                UnitPriceExclTax = 27M,
                PriceInclTax = 27M,
                PriceExclTax = 27M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(fourthOrderItem3);

            //shipments
            //shipment 1
            var fourthOrderShipment1 = new Shipment
            {
                Order = fourthOrder,
                TrackingNumber = string.Empty,
                TotalWeight = 4M,
                ShippedDateUtc = DateTime.UtcNow,
                DeliveryDateUtc = DateTime.UtcNow,
                AdminComment = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            };
            _shipmentRepository.Insert(fourthOrderShipment1);

            var fourthOrderShipment1Item1 = new ShipmentItem
            {
                OrderItemId = fourthOrderItem1.Id,
                Quantity = 1,
                WarehouseId = 0,
                Shipment = fourthOrderShipment1
            };
            _shipmentItemRepository.Insert(fourthOrderShipment1Item1);

            var fourthOrderShipment1Item2 = new ShipmentItem
            {
                OrderItemId = fourthOrderItem2.Id,
                Quantity = 1,
                WarehouseId = 0,
                Shipment = fourthOrderShipment1
            };
            _shipmentItemRepository.Insert(fourthOrderShipment1Item2);

            //shipment 2
            var fourthOrderShipment2 = new Shipment
            {
                Order = fourthOrder,
                TrackingNumber = string.Empty,
                TotalWeight = 2M,
                ShippedDateUtc = DateTime.UtcNow,
                DeliveryDateUtc = DateTime.UtcNow,
                AdminComment = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            };
            _shipmentRepository.Insert(fourthOrderShipment2);

            var fourthOrderShipment2Item1 = new ShipmentItem
            {
                OrderItemId = fourthOrderItem3.Id,
                Quantity = 1,
                WarehouseId = 0,
                Shipment = fourthOrderShipment2
            };
            _shipmentItemRepository.Insert(fourthOrderShipment2Item1);

            //fifth order
            var fifthCustomer = _customerRepository.Table.First(c => c.Email.Equals("victoria_victoria@nopCommerce.com"));
            var fifthOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                Customer = fifthCustomer,
                CustomerLanguageId = _languageRepository.Table.First().Id,
                CustomerIp = "127.0.0.1",
                OrderSubtotalInclTax = 43.50M,
                OrderSubtotalExclTax = 43.50M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 43.50M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                CustomerCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Complete,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Paid,
                PaidDateUtc = DateTime.UtcNow,
                BillingAddress = (Address)fifthCustomer.BillingAddress.Clone(),
                ShippingAddress = (Address)fifthCustomer.ShippingAddress.Clone(),
                ShippingStatus = ShippingStatus.Delivered,
                ShippingMethod = "Ground",
                PickupInStore = false,
                ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };
            _orderRepository.Insert(fifthOrder);
            fifthOrder.CustomOrderNumber = fifthOrder.Id.ToString();
            _orderRepository.Update(fifthOrder);

            //order notes
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                Order = fifthOrder
            });
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order paid",
                Order = fifthOrder
            });
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order shipped",
                Order = fifthOrder
            });
            _orderNoteRepository.Insert(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order delivered",
                Order = fifthOrder
            });

            //item Levi's 511 Jeans
            var fifthOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                Order = fifthOrder,
                ProductId = _productRepository.Table.First(p => p.Name.Equals("Levi's 511 Jeans")).Id,
                UnitPriceInclTax = 43.50M,
                UnitPriceExclTax = 43.50M,
                PriceInclTax = 43.50M,
                PriceExclTax = 43.50M,
                OriginalProductCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };
            _orderItemRepository.Insert(fifthOrderItem1);

            //shipment 1
            var fifthOrderShipment1 = new Shipment
            {
                Order = fifthOrder,
                TrackingNumber = string.Empty,
                TotalWeight = 2M,
                ShippedDateUtc = DateTime.UtcNow,
                DeliveryDateUtc = DateTime.UtcNow,
                AdminComment = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            };
            _shipmentRepository.Insert(fifthOrderShipment1);

            var fifthOrderShipment1Item1 = new ShipmentItem
            {
                OrderItemId = fifthOrderItem1.Id,
                Quantity = 1,
                WarehouseId = 0,
                Shipment = fifthOrderShipment1
            };
            _shipmentItemRepository.Insert(fifthOrderShipment1Item1);
        }

        protected virtual void InstallActivityLog(string defaultUserEmail)
        {
            //default customer/user
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditCategory")),
                Comment = "Edited a category ('Computers')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditDiscount")),
                Comment = "Edited a discount ('Sample discount with coupon code')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditSpecAttribute")),
                Comment = "Edited a specification attribute ('CPU Type')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("AddNewProductAttribute")),
                Comment = "Added a new product attribute ('Some attribute')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("DeleteGiftCard")),
                Comment = "Deleted a gift card ('bdbbc0ef-be57')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
        }

        protected virtual void InstallSearchTerms()
        {
            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 34,
                Keyword = "computer",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 30,
                Keyword = "camera",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 27,
                Keyword = "jewelry",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 26,
                Keyword = "shoes",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 19,
                Keyword = "jeans",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm
            {
                Count = 10,
                Keyword = "gift",
                StoreId = defaultStore.Id
            });
        }

        protected virtual void InstallEmailAccounts()
        {
            var emailAccounts = new List<EmailAccount>
            {
                new EmailAccount
                {
                    Email = "test@mail.com",
                    DisplayName = "Store name",
                    Host = "smtp.mail.com",
                    Port = 25,
                    Username = "123",
                    Password = "123",
                    EnableSsl = false,
                    UseDefaultCredentials = false
                }
            };
            _emailAccountRepository.Insert(emailAccounts);
        }

        protected virtual void InstallMessageTemplates()
        {
            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");

            var messageTemplates = new List<MessageTemplate>
            {
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.BlogCommentNotification,
                    Subject = "%Store.Name%. New blog comment.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.BackInStockNotification,
                    Subject = "%Store.Name%. Back in stock notification",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}Product <a target=\"_blank\" href=\"%BackInStockSubscription.ProductUrl%\">%BackInStockSubscription.ProductName%</a> is in stock.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerEmailValidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = $"<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To activate your account <a href=\"%Customer.AccountActivationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerEmailRevalidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%!{Environment.NewLine}<br />{Environment.NewLine}To validate your new email address <a href=\"%Customer.EmailRevalidationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.PrivateMessageNotification,
                    Subject = "%Store.Name%. You have received a new private message",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You have received a new private message.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerPasswordRecoveryMessage,
                    Subject = "%Store.Name%. Password recovery",
                    Body = $"<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To change your password <a href=\"%Customer.PasswordRecoveryURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerWelcomeMessage,
                    Subject = "Welcome to %Store.Name%",
                    Body = $"We welcome you to <a href=\"%Store.URL%\"> %Store.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can now take part in the various services we have to offer you. Some of these services include:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.{Environment.NewLine}<br />{Environment.NewLine}Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{Environment.NewLine}<br />{Environment.NewLine}Order History - View your history of purchases that you have made with us.{Environment.NewLine}<br />{Environment.NewLine}Products Reviews - Share your opinions on products with our other customers.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For help with any of our online services, please email the store-owner: <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumPostMessage,
                    Subject = "%Store.Name%. New Post Notification.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Post author: %Forums.PostAuthor%{Environment.NewLine}<br />{Environment.NewLine}Post body: %Forums.PostBody%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumTopicMessage,
                    Subject = "%Store.Name%. New Topic Notification.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.GiftCardNotification,
                    Subject = "%GiftCard.SenderName% has sent you a gift card for %Store.Name%",
                    Body = $"<p>{Environment.NewLine}You have received a gift card for %Store.Name%{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}Dear %GiftCard.RecipientName%,{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%GiftCard.SenderName% (%GiftCard.SenderEmail%) has sent you a %GiftCard.Amount% gift cart for <a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}You gift card code is %GiftCard.CouponCode%{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}%GiftCard.Message%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerRegisteredNotification,
                    Subject = "%Store.Name%. New customer registration",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new customer registered with your store. Below are the customer's details:{Environment.NewLine}<br />{Environment.NewLine}Full name: %Customer.FullName%{Environment.NewLine}<br />{Environment.NewLine}Email: %Customer.Email%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification,
                    Subject = "%Store.Name%. New return request.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% has just submitted a new return request. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Request ID: %ReturnRequest.CustomNumber%{Environment.NewLine}<br />{Environment.NewLine}Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%{Environment.NewLine}<br />{Environment.NewLine}Reason for return: %ReturnRequest.Reason%{Environment.NewLine}<br />{Environment.NewLine}Requested action: %ReturnRequest.RequestedAction%{Environment.NewLine}<br />{Environment.NewLine}Customer comments:{Environment.NewLine}<br />{Environment.NewLine}%ReturnRequest.CustomerComment%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewReturnRequestCustomerNotification,
                    Subject = "%Store.Name%. New return request.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%!{Environment.NewLine}<br />{Environment.NewLine}You have just submitted a new return request. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Request ID: %ReturnRequest.CustomNumber%{Environment.NewLine}<br />{Environment.NewLine}Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%{Environment.NewLine}<br />{Environment.NewLine}Reason for return: %ReturnRequest.Reason%{Environment.NewLine}<br />{Environment.NewLine}Requested action: %ReturnRequest.RequestedAction%{Environment.NewLine}<br />{Environment.NewLine}Customer comments:{Environment.NewLine}<br />{Environment.NewLine}%ReturnRequest.CustomerComment%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsCommentNotification,
                    Subject = "%Store.Name%. New news comment.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new news comment has been created for news \"%NewsComment.NewsTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage,
                    Subject = "%Store.Name%. Subscription activation message.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage,
                    Subject = "%Store.Name%. Subscription deactivation message.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from our newsletter.</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification,
                    Subject = "%Store.Name%. New VAT number is submitted.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just submitted a new VAT number. Details are below:{Environment.NewLine}<br />{Environment.NewLine}VAT number: %Customer.VatNumber%{Environment.NewLine}<br />{Environment.NewLine}VAT number status: %Customer.VatNumberStatus%{Environment.NewLine}<br />{Environment.NewLine}Received name: %VatValidationResult.Name%{Environment.NewLine}<br />{Environment.NewLine}Received address: %VatValidationResult.Address%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderCancelledCustomerNotification,
                    Subject = "%Store.Name%. Your order cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order has been cancelled. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderCompletedCustomerNotification,
                    Subject = "%Store.Name%. Your order completed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order has been completed. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ShipmentDeliveredCustomerNotification,
                    Subject = "Your order from %Store.Name% has been delivered.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Good news! You order has been delivered.{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% Delivered Products:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Shipment.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedCustomerNotification,
                    Subject = "Order receipt from %Store.Name%.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification,
                    Subject = "%Store.Name%. Purchase Receipt for Order #%Order.OrderNumber%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.CustomerFullName% (%Order.CustomerEmail%) has just placed an order from your store. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ShipmentSentCustomerNotification,
                    Subject = "Your order from %Store.Name% has been shipped.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%!,{Environment.NewLine}<br />{Environment.NewLine}Good news! You order has been shipped.{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% Shipped Products:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Shipment.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ProductReviewStoreOwnerNotification,
                    Subject = "%Store.Name%. New product review.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new product review has been written for product \"%ProductReview.ProductName%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ProductReviewReplyCustomerNotification,
                    Subject = "%Store.Name%. Product review reply.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}You received a reply from the store administration to your review for product \"%ProductReview.ProductName%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification,
                    Subject = "%Store.Name%. Quantity below notification. %Product.Name%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Product.Name% (ID: %Product.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Quantity: %Product.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification,
                    Subject = "%Store.Name%. Quantity below notification. %Product.Name%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Product.Name% (ID: %Product.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}%AttributeCombination.Formatted%{Environment.NewLine}<br />{Environment.NewLine}Quantity: %AttributeCombination.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ReturnRequestStatusChangedCustomerNotification,
                    Subject = "%Store.Name%. Return request status was changed.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}Your return request #%ReturnRequest.CustomNumber% status has been changed.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.EmailAFriendMessage,
                    Subject = "%Store.Name%. Referred Item",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%EmailAFriend.Email% was shopping on %Store.Name% and wanted to share the following item with you.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<b><a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">%Product.Name%</a></b>{Environment.NewLine}<br />{Environment.NewLine}%Product.ShortDescription%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For more info click <a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">here</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%EmailAFriend.PersonalMessage%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.WishlistToFriendMessage,
                    Subject = "%Store.Name%. Wishlist",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Wishlist.Email% was shopping on %Store.Name% and wanted to share a wishlist with you.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For more info click <a target=\"_blank\" href=\"%Wishlist.URLForCustomer%\">here</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Wishlist.PersonalMessage%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewOrderNoteAddedCustomerNotification,
                    Subject = "%Store.Name%. New order note has been added",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}New order note has been added to your account:{Environment.NewLine}<br />{Environment.NewLine}\"%Order.NewNoteText%\".{Environment.NewLine}<br />{Environment.NewLine}<a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%RecurringPayment.CancelAfterFailedPayment%) The last payment for the recurring payment ID=%RecurringPayment.ID% failed, so it was cancelled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) %Customer.FullName% (%Customer.Email%) has just cancelled a recurring payment ID=%RecurringPayment.ID%. endif%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentCancelledCustomerNotification,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}%if (%RecurringPayment.CancelAfterFailedPayment%) It appears your credit card didn't go through for this recurring payment (<a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>){Environment.NewLine}<br />{Environment.NewLine}So your subscription has been cancelled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) The recurring payment ID=%RecurringPayment.ID% was cancelled. endif%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentFailedCustomerNotification,
                    Subject = "%Store.Name%. Last recurring payment failed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}It appears your credit card didn't go through for this recurring payment (<a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>){Environment.NewLine}<br /> %if (%RecurringPayment.RecurringPaymentType% == \"Manual\") {Environment.NewLine}You can recharge balance and manually retry payment or cancel it on the order history page. endif% %if (%RecurringPayment.RecurringPaymentType% == \"Automatic\") {Environment.NewLine}You can recharge balance and wait, we will try to make the payment again, or you can cancel it on the order history page. endif%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedVendorNotification,
                    Subject = "%Store.Name%. Order placed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just placed an order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedAffiliateNotification,
                    Subject = "%Store.Name%. Order placed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just placed an order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderRefundedCustomerNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% refunded",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Order #%Order.OrderNumber% has been has been refunded. Please allow 7-14 days for the refund to be reflected in your account.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Amount refunded: %Order.AmountRefunded%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br /{Environment.NewLine}>Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% refunded",
                    Body = $"%Store.Name%. Order #%Order.OrderNumber% refunded', N'{Environment.NewLine}<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just refunded{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Amount refunded: %Order.AmountRefunded%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidStoreOwnerNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just paid{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidCustomerNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Order #%Order.OrderNumber% has been just paid. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidVendorNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just paid.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidAffiliateNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just paid.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewVendorAccountApplyStoreOwnerNotification,
                    Subject = "%Store.Name%. New vendor account submitted.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just submitted for a vendor account. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Vendor name: %Vendor.Name%{Environment.NewLine}<br />{Environment.NewLine}Vendor email: %Vendor.Email%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can activate it in admin area.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.VendorInformationChangeNotification,
                    Subject = "%Store.Name%. Vendor information change.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Vendor %Vendor.Name% (%Vendor.Email%) has just changed information about itself.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactUsMessage,
                    Subject = "%Store.Name%. Contact us",
                    Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactVendorMessage,
                    Subject = "%Store.Name%. Contact us",
                    Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                }
            };
            _messageTemplateRepository.Insert(messageTemplates);
        }

        protected virtual void InstallTopics()
        {
            var defaultTopicTemplate =
                _topicTemplateRepository.Table.FirstOrDefault(tt => tt.Name == "Default template");
            if (defaultTopicTemplate == null)
                throw new Exception("Topic template cannot be loaded");

            var topics = new List<Topic>
            {
                new Topic
                {
                    SystemName = "AboutUs",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = true,
                    DisplayOrder = 20,
                    Published = true,
                    Title = "About us",
                    Body =
                        "<p>Put your &quot;About Us&quot; information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "CheckoutAsGuestOrRegister",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body =
                        "<p><strong>Register and save time!</strong><br />Register with us for future convenience:</p><ul><li>Fast and easy check out</li><li>Easy access to your order history and status</li></ul>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ConditionsOfUse",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = true,
                    DisplayOrder = 15,
                    Published = true,
                    Title = "Conditions of Use",
                    Body = "<p>Put your conditions of use information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ContactUs",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body = "<p>Put your contact information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ForumWelcomeMessage",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "Forums",
                    Body = "<p>Put your welcome message here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "HomepageText",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "Welcome to our store",
                    Body =
                        "<p>Online shopping is the process consumers go through to purchase products or services over the Internet. You can edit this in the admin site.</p><p>If you have questions, see the <a href=\"http://docs.nopcommerce.com/\">Documentation</a>, or post in the <a href=\"https://www.nopcommerce.com/boards/\">Forums</a> at <a href=\"https://www.nopcommerce.com\">nopCommerce.com</a></p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "LoginRegistrationInfo",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "About login / registration",
                    Body =
                        "<p>Put your login / registration information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "PrivacyInfo",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = true,
                    DisplayOrder = 10,
                    Published = true,
                    Title = "Privacy notice",
                    Body = "<p>Put your privacy policy information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "PageNotFound",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body =
                        "<p><strong>The page you requested was not found, and we have a fine guess why.</strong></p><ul><li>If you typed the URL directly, please make sure the spelling is correct.</li><li>The page no longer exists. In this case, we profusely apologize for the inconvenience and for any damage this may cause.</li></ul>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ShippingInfo",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = true,
                    DisplayOrder = 5,
                    Published = true,
                    Title = "Shipping & returns",
                    Body =
                        "<p>Put your shipping &amp; returns information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ApplyVendor",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body = "<p>Put your apply vendor instructions here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "VendorTermsOfService",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "Terms of services for vendors",
                    Body = "<p>Put your terms of service information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                }
            };
            _topicRepository.Insert(topics);

            //search engine names
            foreach (var topic in topics)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = topic.Id,
                    EntityName = nameof(Topic),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = ValidateSeName(topic, !string.IsNullOrEmpty(topic.Title) ? topic.Title : topic.SystemName)
                });
            }
        }

        protected virtual void InstallSettings()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            settingService.SaveSetting(new PdfSettings
            {
                LogoPictureId = 0,
                LetterPageSizeEnabled = false,
                RenderOrderNotes = true,
                FontFileName = "FreeSerif.ttf",
                InvoiceFooterTextColumn1 = null,
                InvoiceFooterTextColumn2 = null
            });

            settingService.SaveSetting(new SitemapSettings
            {
                SitemapEnabled = true,
                SitemapPageSize = 200,
                SitemapIncludeCategories = true,
                SitemapIncludeManufacturers = true,
                SitemapIncludeProducts = false,
                SitemapIncludeProductTags = false,
                SitemapIncludeBlogPosts = true,
                SitemapIncludeNews = false,
                SitemapIncludeTopics = true
            });

            settingService.SaveSetting(new SitemapXmlSettings
            {
                SitemapXmlEnabled = true,
                SitemapXmlIncludeBlogPosts = true,
                SitemapXmlIncludeCategories = true,
                SitemapXmlIncludeManufacturers = true,
                SitemapXmlIncludeNews = true,
                SitemapXmlIncludeProducts = true,
                SitemapXmlIncludeProductTags = true,
                SitemapXmlIncludeCustomUrls = true,
                SitemapXmlIncludeTopics = true
            });

            settingService.SaveSetting(new CommonSettings
            {
                UseSystemEmailForContactUsForm = true,
                UseStoredProcedureForLoadingCategories = true,

                DisplayJavaScriptDisabledWarning = false,
                UseFullTextSearch = false,
                FullTextMode = FulltextSearchMode.ExactMatch,
                Log404Errors = true,
                BreadcrumbDelimiter = "/",
                RenderXuaCompatible = false,
                XuaCompatibleValue = "IE=edge",
                BbcodeEditorOpenLinksInNewWindow = false,
                PopupForTermsOfServiceLinks = true,
                JqueryMigrateScriptLoggingActive = false,
                SupportPreviousNopcommerceVersions = true,
                UseResponseCompression = true,
                StaticFilesCacheControl = "public,max-age=604800",
                FaviconAndAppIconsHeadCode = "<link rel=\"apple-touch-icon\" sizes=\"180x180\" href=\"/icons/icons_0/apple-touch-icon.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"32x32\" href=\"/icons/icons_0/favicon-32x32.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"192x192\" href=\"/icons/icons_0/android-chrome-192x192.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"16x16\" href=\"/icons/icons_0/favicon-16x16.png\"><link rel=\"manifest\" href=\"/icons/icons_0/site.webmanifest\"><link rel=\"mask-icon\" href=\"/icons/icons_0/safari-pinned-tab.svg\" color=\"#5bbad5\"><link rel=\"shortcut icon\" href=\"/icons/icons_0/favicon.ico\"><meta name=\"msapplication-TileColor\" content=\"#2d89ef\"><meta name=\"msapplication-TileImage\" content=\"/icons/icons_0/mstile-144x144.png\"><meta name=\"msapplication-config\" content=\"/icons/icons_0/browserconfig.xml\"><meta name=\"theme-color\" content=\"#ffffff\">",
                EnableHtmlMinification = true,
                //we disable bundling out of the box because it requires a lot of server resources
                EnableJsBundling = false,
                EnableCssBundling = false
            });

            settingService.SaveSetting(new SeoSettings
            {
                PageTitleSeparator = ". ",
                PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
                DefaultTitle = "Your store",
                DefaultMetaKeywords = string.Empty,
                DefaultMetaDescription = string.Empty,
                GenerateProductMetaDescription = true,
                ConvertNonWesternChars = false,
                AllowUnicodeCharsInUrls = true,
                CanonicalUrlsEnabled = false,
                QueryStringInCanonicalUrlsEnabled = false,
                WwwRequirement = WwwRequirement.NoMatter,
                TwitterMetaTags = true,
                OpenGraphMetaTags = true,
                ReservedUrlRecordSlugs = new List<string>
                {
                    "admin",
                    "install",
                    "recentlyviewedproducts",
                    "newproducts",
                    "compareproducts",
                    "clearcomparelist",
                    "setproductreviewhelpfulness",
                    "login",
                    "register",
                    "logout",
                    "cart",
                    "wishlist",
                    "emailwishlist",
                    "checkout",
                    "onepagecheckout",
                    "contactus",
                    "passwordrecovery",
                    "subscribenewsletter",
                    "blog",
                    "boards",
                    "inboxupdate",
                    "sentupdate",
                    "news",
                    "sitemap",
                    "search",
                    "config",
                    "eucookielawaccept",
                    "page-not-found",
                    //system names are not allowed (anyway they will cause a runtime error),
                    "con",
                    "lpt1",
                    "lpt2",
                    "lpt3",
                    "lpt4",
                    "lpt5",
                    "lpt6",
                    "lpt7",
                    "lpt8",
                    "lpt9",
                    "com1",
                    "com2",
                    "com3",
                    "com4",
                    "com5",
                    "com6",
                    "com7",
                    "com8",
                    "com9",
                    "null",
                    "prn",
                    "aux"
                },
                CustomHeadTags = string.Empty
            });

            settingService.SaveSetting(new AdminAreaSettings
            {
                DefaultGridPageSize = 15,
                PopupGridPageSize = 7,
                GridPageSizes = "7, 15, 20, 50, 100",
                RichEditorAdditionalSettings = null,
                RichEditorAllowJavaScript = false,
                RichEditorAllowStyleTag = false,
                UseRichEditorForCustomerEmails = false,
                UseRichEditorInMessageTemplates = false,
                CheckCopyrightRemovalKey = true,
                UseIsoDateFormatInJsonResult = true
            });

            settingService.SaveSetting(new ProductEditorSettings
            {
                Weight = true,
                Dimensions = true,
                ProductAttributes = true,
                SpecificationAttributes = true
            });

            settingService.SaveSetting(new GdprSettings
            {
                GdprEnabled = false,
                LogPrivacyPolicyConsent = true,
                LogNewsletterConsent = true,
                LogUserProfileChanges = true
            });

            settingService.SaveSetting(new CatalogSettings
            {
                AllowViewUnpublishedProductPage = true,
                DisplayDiscontinuedMessageForUnpublishedProducts = true,
                PublishBackProductWhenCancellingOrders = false,
                ShowSkuOnProductDetailsPage = true,
                ShowSkuOnCatalogPages = false,
                ShowManufacturerPartNumber = false,
                ShowGtin = false,
                ShowFreeShippingNotification = true,
                AllowProductSorting = true,
                AllowProductViewModeChanging = true,
                DefaultViewMode = "grid",
                ShowProductsFromSubcategories = false,
                ShowCategoryProductNumber = false,
                ShowCategoryProductNumberIncludingSubcategories = false,
                CategoryBreadcrumbEnabled = true,
                ShowShareButton = true,
                PageShareCode = "<!-- AddThis Button BEGIN --><div class=\"addthis_toolbox addthis_default_style \"><a class=\"addthis_button_preferred_1\"></a><a class=\"addthis_button_preferred_2\"></a><a class=\"addthis_button_preferred_3\"></a><a class=\"addthis_button_preferred_4\"></a><a class=\"addthis_button_compact\"></a><a class=\"addthis_counter addthis_bubble_style\"></a></div><script src=\"http://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions\"></script><!-- AddThis Button END -->",
                ProductReviewsMustBeApproved = false,
                DefaultProductRatingValue = 5,
                AllowAnonymousUsersToReviewProduct = false,
                ProductReviewPossibleOnlyAfterPurchasing = false,
                NotifyStoreOwnerAboutNewProductReviews = false,
                NotifyCustomerAboutProductReviewReply = false,
                EmailAFriendEnabled = true,
                AllowAnonymousUsersToEmailAFriend = false,
                RecentlyViewedProductsNumber = 3,
                RecentlyViewedProductsEnabled = true,
                NewProductsNumber = 6,
                NewProductsEnabled = true,
                CompareProductsEnabled = true,
                CompareProductsNumber = 4,
                ProductSearchAutoCompleteEnabled = true,
                ProductSearchAutoCompleteNumberOfProducts = 10,
                ShowLinkToAllResultInSearchAutoComplete = false,
                ProductSearchTermMinimumLength = 3,
                ShowProductImagesInSearchAutoComplete = false,
                ShowBestsellersOnHomepage = false,
                NumberOfBestsellersOnHomepage = 4,
                SearchPageProductsPerPage = 6,
                SearchPageAllowCustomersToSelectPageSize = true,
                SearchPagePageSizeOptions = "6, 3, 9, 18",
                ProductsAlsoPurchasedEnabled = true,
                ProductsAlsoPurchasedNumber = 4,
                AjaxProcessAttributeChange = true,
                NumberOfProductTags = 15,
                ProductsByTagPageSize = 6,
                IncludeShortDescriptionInCompareProducts = false,
                IncludeFullDescriptionInCompareProducts = false,
                IncludeFeaturedProductsInNormalLists = false,
                UseLinksInRequiredProductWarnings = true,
                DisplayTierPricesWithDiscounts = true,
                IgnoreDiscounts = false,
                IgnoreFeaturedProducts = false,
                IgnoreAcl = true,
                IgnoreStoreLimitations = true,
                CacheProductPrices = false,
                ProductsByTagAllowCustomersToSelectPageSize = true,
                ProductsByTagPageSizeOptions = "6, 3, 9, 18",
                MaximumBackInStockSubscriptions = 200,
                ManufacturersBlockItemsToDisplay = 2,
                DisplayTaxShippingInfoFooter = false,
                DisplayTaxShippingInfoProductDetailsPage = false,
                DisplayTaxShippingInfoProductBoxes = false,
                DisplayTaxShippingInfoShoppingCart = false,
                DisplayTaxShippingInfoWishlist = false,
                DisplayTaxShippingInfoOrderDetailsPage = false,
                DefaultCategoryPageSizeOptions = "6, 3, 9",
                DefaultCategoryPageSize = 6,
                DefaultManufacturerPageSizeOptions = "6, 3, 9",
                DefaultManufacturerPageSize = 6,
                ShowProductReviewsTabOnAccountPage = true,
                ProductReviewsPageSizeOnAccountPage = 10,
                ProductReviewsSortByCreatedDateAscending = false,
                ExportImportProductAttributes = true,
                ExportImportProductSpecificationAttributes = true,
                ExportImportUseDropdownlistsForAssociatedEntities = true,
                ExportImportProductsCountInOneFile = 500,
                ExportImportSplitProductsFile = false,
                ExportImportRelatedEntitiesByName = true,
                CountDisplayedYearsDatePicker = 1,
                UseAjaxLoadMenu = false
            });

            settingService.SaveSetting(new LocalizationSettings
            {
                DefaultAdminLanguageId = _languageRepository.Table.Single(l => l.Name == "English").Id,
                UseImagesForLanguageSelection = false,
                SeoFriendlyUrlsForLanguagesEnabled = false,
                AutomaticallyDetectLanguage = false,
                LoadAllLocaleRecordsOnStartup = true,
                LoadAllLocalizedPropertiesOnStartup = true,
                LoadAllUrlRecordsOnStartup = false,
                IgnoreRtlPropertyForAdminArea = false
            });

            settingService.SaveSetting(new CustomerSettings
            {
                UsernamesEnabled = false,
                CheckUsernameAvailabilityEnabled = false,
                AllowUsersToChangeUsernames = false,
                DefaultPasswordFormat = PasswordFormat.Hashed,
                HashedPasswordFormat = NopCustomerServiceDefaults.DefaultHashedPasswordFormat,
                PasswordMinLength = 6,
                PasswordRequireDigit = false,
                PasswordRequireLowercase = false,
                PasswordRequireNonAlphanumeric = false,
                PasswordRequireUppercase = false,
                UnduplicatedPasswordsNumber = 4,
                PasswordRecoveryLinkDaysValid = 7,
                PasswordLifetime = 90,
                FailedPasswordAllowedAttempts = 0,
                FailedPasswordLockoutMinutes = 30,
                UserRegistrationType = UserRegistrationType.Standard,
                AllowCustomersToUploadAvatars = false,
                AvatarMaximumSizeBytes = 20000,
                DefaultAvatarEnabled = true,
                ShowCustomersLocation = false,
                ShowCustomersJoinDate = false,
                AllowViewingProfiles = false,
                NotifyNewCustomerRegistration = false,
                HideDownloadableProductsTab = false,
                HideBackInStockSubscriptionsTab = false,
                DownloadableProductsValidateUser = false,
                CustomerNameFormat = CustomerNameFormat.ShowFirstName,
                GenderEnabled = true,
                DateOfBirthEnabled = true,
                DateOfBirthRequired = false,
                DateOfBirthMinimumAge = null,
                CompanyEnabled = true,
                StreetAddressEnabled = false,
                StreetAddress2Enabled = false,
                ZipPostalCodeEnabled = false,
                CityEnabled = false,
                CountyEnabled = false,
                CountyRequired = false,
                CountryEnabled = false,
                CountryRequired = false,
                StateProvinceEnabled = false,
                StateProvinceRequired = false,
                PhoneEnabled = false,
                FaxEnabled = false,
                AcceptPrivacyPolicyEnabled = false,
                NewsletterEnabled = true,
                NewsletterTickedByDefault = true,
                HideNewsletterBlock = false,
                NewsletterBlockAllowToUnsubscribe = false,
                OnlineCustomerMinutes = 20,
                StoreLastVisitedPage = false,
                StoreIpAddresses = true,
                SuffixDeletedCustomers = false,
                EnteringEmailTwice = false,
                RequireRegistrationForDownloadableProducts = false,
                AllowCustomersToCheckGiftCardBalance = false,
                DeleteGuestTaskOlderThanMinutes = 1440
            });

            settingService.SaveSetting(new AddressSettings
            {
                CompanyEnabled = true,
                StreetAddressEnabled = true,
                StreetAddressRequired = true,
                StreetAddress2Enabled = true,
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = true,
                CityEnabled = true,
                CityRequired = true,
                CountyEnabled = false,
                CountyRequired = false,
                CountryEnabled = true,
                StateProvinceEnabled = true,
                PhoneEnabled = true,
                PhoneRequired = true,
                FaxEnabled = true
            });

            settingService.SaveSetting(new MediaSettings
            {
                AvatarPictureSize = 120,
                ProductThumbPictureSize = 415,
                ProductDetailsPictureSize = 550,
                ProductThumbPictureSizeOnProductDetailsPage = 100,
                AssociatedProductPictureSize = 220,
                CategoryThumbPictureSize = 450,
                ManufacturerThumbPictureSize = 420,
                VendorThumbPictureSize = 450,
                CartThumbPictureSize = 80,
                MiniCartThumbPictureSize = 70,
                AutoCompleteSearchThumbPictureSize = 20,
                ImageSquarePictureSize = 32,
                MaximumImageSize = 1980,
                DefaultPictureZoomEnabled = false,
                DefaultImageQuality = 80,
                MultipleThumbDirectories = false,
                ImportProductImagesUsingHash = true,
                AzureCacheControlHeader = string.Empty,
                UseAbsoluteImagePath = true
            });

            settingService.SaveSetting(new StoreInformationSettings
            {
                StoreClosed = false,
                DefaultStoreTheme = "DefaultClean",
                AllowCustomerToSelectTheme = false,
                DisplayMiniProfilerInPublicStore = false,
                DisplayMiniProfilerForAdminOnly = false,
                DisplayEuCookieLawWarning = false,
                FacebookLink = "http://www.facebook.com/nopCommerce",
                TwitterLink = "https://twitter.com/nopCommerce",
                YoutubeLink = "http://www.youtube.com/user/nopCommerce",
                HidePoweredByNopCommerce = false
            });

            settingService.SaveSetting(new ExternalAuthenticationSettings
            {
                RequireEmailValidation = false,
                AllowCustomersToRemoveAssociations = true
            });

            settingService.SaveSetting(new RewardPointsSettings
            {
                Enabled = true,
                ExchangeRate = 1,
                PointsForRegistration = 0,
                RegistrationPointsValidity = 30,
                PointsForPurchases_Amount = 10,
                PointsForPurchases_Points = 1,
                MinOrderTotalToAwardPoints = 0,
                MaximumRewardPointsToUsePerOrder = 0,
                PurchasesPointsValidity = 45,
                ActivationDelay = 0,
                ActivationDelayPeriodId = 0,
                DisplayHowMuchWillBeEarned = true,
                PointsAccumulatedForAllStores = true,
                PageSize = 10
            });

            settingService.SaveSetting(new CurrencySettings
            {
                DisplayCurrencyLabel = false,
                PrimaryStoreCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                PrimaryExchangeRateCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                ActiveExchangeRateProviderSystemName = "CurrencyExchange.ECB",
                AutoUpdateEnabled = false
            });

            settingService.SaveSetting(new MeasureSettings
            {
                BaseDimensionId = _measureDimensionRepository.Table.Single(m => m.SystemKeyword == "inches").Id,
                BaseWeightId = _measureWeightRepository.Table.Single(m => m.SystemKeyword == "lb").Id
            });

            settingService.SaveSetting(new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false,
                Color1 = "#b9babe",
                Color2 = "#ebecee",
                Color3 = "#dde2e6"
            });

            settingService.SaveSetting(new ShoppingCartSettings
            {
                DisplayCartAfterAddingProduct = false,
                DisplayWishlistAfterAddingProduct = false,
                MaximumShoppingCartItems = 1000,
                MaximumWishlistItems = 1000,
                AllowOutOfStockItemsToBeAddedToWishlist = false,
                MoveItemsFromWishlistToCart = true,
                CartsSharedBetweenStores = false,
                ShowProductImagesOnShoppingCart = true,
                ShowProductImagesOnWishList = true,
                ShowDiscountBox = true,
                ShowGiftCardBox = true,
                CrossSellsNumber = 4,
                EmailWishlistEnabled = true,
                AllowAnonymousUsersToEmailWishlist = false,
                MiniShoppingCartEnabled = true,
                ShowProductImagesInMiniShoppingCart = true,
                MiniShoppingCartProductNumber = 5,
                RoundPricesDuringCalculation = true,
                GroupTierPricesForDistinctShoppingCartItems = false,
                AllowCartItemEditing = true,
                RenderAssociatedAttributeValueQuantity = true
            });

            settingService.SaveSetting(new OrderSettings
            {
                ReturnRequestNumberMask = "{ID}",
                IsReOrderAllowed = true,
                MinOrderSubtotalAmount = 0,
                MinOrderSubtotalAmountIncludingTax = false,
                MinOrderTotalAmount = 0,
                AutoUpdateOrderTotalsOnEditingOrder = false,
                AnonymousCheckoutAllowed = true,
                TermsOfServiceOnShoppingCartPage = true,
                TermsOfServiceOnOrderConfirmPage = false,
                OnePageCheckoutEnabled = true,
                OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab = false,
                DisableBillingAddressCheckoutStep = false,
                DisableOrderCompletedPage = false,
                AttachPdfInvoiceToOrderPlacedEmail = false,
                AttachPdfInvoiceToOrderCompletedEmail = false,
                GeneratePdfInvoiceInCustomerLanguage = true,
                AttachPdfInvoiceToOrderPaidEmail = false,
                ReturnRequestsEnabled = true,
                ReturnRequestsAllowFiles = false,
                ReturnRequestsFileMaximumSize = 2048,
                NumberOfDaysReturnRequestAvailable = 365,
                MinimumOrderPlacementInterval = 30,
                ActivateGiftCardsAfterCompletingOrder = false,
                DeactivateGiftCardsAfterCancellingOrder = false,
                DeactivateGiftCardsAfterDeletingOrder = false,
                CompleteOrderWhenDelivered = true,
                CustomOrderNumberMask = "{ID}",
                ExportWithProducts = true,
                AllowAdminsToBuyCallForPriceProducts = true
            });

            settingService.SaveSetting(new SecuritySettings
            {
                ForceSslForAllPages = true,
                EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
                AdminAreaAllowedIpAddresses = null,
                EnableXsrfProtectionForAdminArea = true,
                EnableXsrfProtectionForPublicStore = true,
                HoneypotEnabled = false,
                HoneypotInputName = "hpinput",
                AllowNonAsciiCharactersInHeaders = true
            });

            settingService.SaveSetting(new ShippingSettings
            {
                ActiveShippingRateComputationMethodSystemNames = new List<string> { "Shipping.FixedByWeightByTotal" },
                ActivePickupPointProviderSystemNames = new List<string> { "Pickup.PickupInStore" },
                ShipToSameAddress = true,
                AllowPickupInStore = true,
                DisplayPickupPointsOnMap = false,
                IgnoreAdditionalShippingChargeForPickupInStore = true,
                UseWarehouseLocation = false,
                NotifyCustomerAboutShippingFromMultipleLocations = false,
                FreeShippingOverXEnabled = false,
                FreeShippingOverXValue = decimal.Zero,
                FreeShippingOverXIncludingTax = false,
                EstimateShippingEnabled = true,
                DisplayShipmentEventsToCustomers = false,
                DisplayShipmentEventsToStoreOwner = false,
                HideShippingTotal = false,
                ReturnValidOptionsIfThereAreAny = true,
                BypassShippingMethodSelectionIfOnlyOne = false,
                UseCubeRootMethod = true,
                ConsiderAssociatedProductsDimensions = true,
                ShipSeparatelyOneItemEach = true
            });

            settingService.SaveSetting(new PaymentSettings
            {
                ActivePaymentMethodSystemNames = new List<string>
                    {
                        "Payments.CheckMoneyOrder",
                        "Payments.Manual"
                    },
                AllowRePostingPayments = true,
                BypassPaymentMethodSelectionIfOnlyOne = true,
                ShowPaymentMethodDescriptions = true,
                SkipPaymentInfoStepForRedirectionPaymentMethods = false,
                CancelRecurringPaymentsAfterFailedPayment = false,
                RegenerateOrderGuidInterval = 180
            });

            settingService.SaveSetting(new TaxSettings
            {
                TaxBasedOn = TaxBasedOn.BillingAddress,
                TaxBasedOnPickupPointAddress = false,
                TaxDisplayType = TaxDisplayType.ExcludingTax,
                ActiveTaxProviderSystemName = "Tax.FixedOrByCountryStateZip",
                DefaultTaxAddressId = 0,
                DisplayTaxSuffix = false,
                DisplayTaxRates = false,
                PricesIncludeTax = false,
                AllowCustomersToSelectTaxDisplayType = false,
                ForceTaxExclusionFromOrderSubtotal = false,
                DefaultTaxCategoryId = 0,
                HideZeroTax = false,
                HideTaxInOrderSummary = false,
                ShippingIsTaxable = false,
                ShippingPriceIncludesTax = false,
                ShippingTaxClassId = 0,
                PaymentMethodAdditionalFeeIsTaxable = false,
                PaymentMethodAdditionalFeeIncludesTax = false,
                PaymentMethodAdditionalFeeTaxClassId = 0,
                EuVatEnabled = false,
                EuVatShopCountryId = 0,
                EuVatAllowVatExemption = true,
                EuVatUseWebService = false,
                EuVatAssumeValid = false,
                EuVatEmailAdminWhenNewVatSubmitted = false,
                LogErrors = false
            });

            settingService.SaveSetting(new DateTimeSettings
            {
                DefaultStoreTimeZoneId = string.Empty,
                AllowCustomersToSetTimeZone = false
            });

            settingService.SaveSetting(new BlogSettings
            {
                Enabled = true,
                PostsPageSize = 10,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewBlogComments = false,
                NumberOfTags = 15,
                ShowHeaderRssUrl = false,
                BlogCommentsMustBeApproved = false,
                ShowBlogCommentsPerStore = false
            });
            settingService.SaveSetting(new NewsSettings
            {
                Enabled = true,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewNewsComments = false,
                ShowNewsOnMainPage = true,
                MainPageNewsCount = 3,
                NewsArchivePageSize = 10,
                ShowHeaderRssUrl = false,
                NewsCommentsMustBeApproved = false,
                ShowNewsCommentsPerStore = false
            });

            settingService.SaveSetting(new ForumSettings
            {
                ForumsEnabled = false,
                RelativeDateTimeFormattingEnabled = true,
                AllowCustomersToDeletePosts = false,
                AllowCustomersToEditPosts = false,
                AllowCustomersToManageSubscriptions = false,
                AllowGuestsToCreatePosts = false,
                AllowGuestsToCreateTopics = false,
                AllowPostVoting = true,
                MaxVotesPerDay = 30,
                TopicSubjectMaxLength = 450,
                PostMaxLength = 4000,
                StrippedTopicMaxLength = 45,
                TopicsPageSize = 10,
                PostsPageSize = 10,
                SearchResultsPageSize = 10,
                ActiveDiscussionsPageSize = 50,
                LatestCustomerPostsPageSize = 10,
                ShowCustomersPostCount = true,
                ForumEditor = EditorType.BBCodeEditor,
                SignaturesEnabled = true,
                AllowPrivateMessages = false,
                ShowAlertForPM = false,
                PrivateMessagesPageSize = 10,
                ForumSubscriptionsPageSize = 10,
                NotifyAboutPrivateMessages = false,
                PMSubjectMaxLength = 450,
                PMTextMaxLength = 4000,
                HomepageActiveDiscussionsTopicCount = 5,
                ActiveDiscussionsFeedEnabled = false,
                ActiveDiscussionsFeedCount = 25,
                ForumFeedsEnabled = false,
                ForumFeedCount = 10,
                ForumSearchTermMinimumLength = 3
            });

            settingService.SaveSetting(new VendorSettings
            {
                DefaultVendorPageSizeOptions = "6, 3, 9",
                VendorsBlockItemsToDisplay = 0,
                ShowVendorOnProductDetailsPage = true,
                AllowCustomersToContactVendors = true,
                AllowCustomersToApplyForVendorAccount = true,
                TermsOfServiceEnabled = false,
                AllowVendorsToEditInfo = false,
                NotifyStoreOwnerAboutVendorInformationChange = true,
                MaximumProductNumber = 3000,
                AllowVendorsToImportProducts = true
            });

            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");
            settingService.SaveSetting(new EmailAccountSettings
            {
                DefaultEmailAccountId = eaGeneral.Id
            });

            settingService.SaveSetting(new WidgetSettings
            {
                ActiveWidgetSystemNames = new List<string> { "Widgets.NivoSlider" }
            });

            settingService.SaveSetting(new DisplayDefaultMenuItemSettings
            {
                DisplayHomepageMenuItem = true,
                DisplayNewProductsMenuItem = true,
                DisplayProductSearchMenuItem = true,
                DisplayCustomerInfoMenuItem = true,
                DisplayBlogMenuItem = true,
                DisplayForumsMenuItem = true,
                DisplayContactUsMenuItem = true
            });

            settingService.SaveSetting(new DisplayDefaultFooterItemSettings
            {
                DisplaySitemapFooterItem = true,
                DisplayContactUsFooterItem = true,
                DisplayProductSearchFooterItem = true,
                DisplayNewsFooterItem = true,
                DisplayBlogFooterItem = true,
                DisplayForumsFooterItem = true,
                DisplayRecentlyViewedProductsFooterItem = true,
                DisplayCompareProductsFooterItem = true,
                DisplayNewProductsFooterItem = true,
                DisplayCustomerInfoFooterItem = true,
                DisplayCustomerOrdersFooterItem = true,
                DisplayCustomerAddressesFooterItem = true,
                DisplayShoppingCartFooterItem = true,
                DisplayWishlistFooterItem = true,
                DisplayApplyVendorAccountFooterItem = true
            });

            settingService.SaveSetting(new CaptchaSettings
            {
                ReCaptchaDefaultLanguage = string.Empty,
                ReCaptchaPrivateKey = string.Empty,
                ReCaptchaPublicKey = string.Empty,
                ReCaptchaRequestTimeout = 20,
                ReCaptchaTheme = string.Empty,
                AutomaticallyChooseLanguage = true,
                Enabled = false,
                ShowOnApplyVendorPage = false,
                ShowOnBlogCommentPage = false,
                ShowOnContactUsPage = false,
                ShowOnEmailProductToFriendPage = false,
                ShowOnEmailWishlistToFriendPage = false,
                ShowOnForgotPasswordPage = false,
                ShowOnForum = false,
                ShowOnLoginPage = false,
                ShowOnNewsCommentPage = false,
                ShowOnProductReviewPage = false,
                ShowOnRegistrationPage = false,
            });

            settingService.SaveSetting(new MessagesSettings
            {
                UsePopupNotifications = false
            });

            settingService.SaveSetting(new ProxySettings
            {
                Enabled = false,
                Address = string.Empty,
                Port = string.Empty,
                Username = string.Empty,
                Password = string.Empty,
                BypassOnLocal = true,
                PreAuthenticate = true
            });
        }

        protected virtual void InstallCheckoutAttributes()
        {
            var ca1 = new CheckoutAttribute
            {
                Name = "Gift wrapping",
                IsRequired = true,
                ShippableProductRequired = true,
                AttributeControlType = AttributeControlType.DropdownList,
                DisplayOrder = 1
            };
            ca1.CheckoutAttributeValues.Add(new CheckoutAttributeValue
            {
                Name = "No",
                PriceAdjustment = 0,
                DisplayOrder = 1,
                IsPreSelected = true
            });
            ca1.CheckoutAttributeValues.Add(new CheckoutAttributeValue
            {
                Name = "Yes",
                PriceAdjustment = 10,
                DisplayOrder = 2
            });
            var checkoutAttributes = new List<CheckoutAttribute>
            {
                ca1
            };
            _checkoutAttributeRepository.Insert(checkoutAttributes);
        }

        protected virtual void InstallSpecificationAttributes()
        {
            var sa1 = new SpecificationAttribute
            {
                Name = "Screensize",
                DisplayOrder = 1
            };
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "13.0''",
                DisplayOrder = 2
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "13.3''",
                DisplayOrder = 3
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "14.0''",
                DisplayOrder = 4
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "15.0''",
                DisplayOrder = 4
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "15.6''",
                DisplayOrder = 5
            });
            var sa2 = new SpecificationAttribute
            {
                Name = "CPU Type",
                DisplayOrder = 2
            };
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Intel Core i5",
                DisplayOrder = 1
            });
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Intel Core i7",
                DisplayOrder = 2
            });
            var sa3 = new SpecificationAttribute
            {
                Name = "Memory",
                DisplayOrder = 3
            };
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "4 GB",
                DisplayOrder = 1
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "8 GB",
                DisplayOrder = 2
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "16 GB",
                DisplayOrder = 3
            });
            var sa4 = new SpecificationAttribute
            {
                Name = "Hard drive",
                DisplayOrder = 5
            };
            sa4.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "128 GB",
                DisplayOrder = 7
            });
            sa4.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "500 GB",
                DisplayOrder = 4
            });
            sa4.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "1 TB",
                DisplayOrder = 3
            });
            var sa5 = new SpecificationAttribute
            {
                Name = "Color",
                DisplayOrder = 1
            };
            sa5.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Grey",
                DisplayOrder = 2,
                ColorSquaresRgb = "#8a97a8"
            });
            sa5.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Red",
                DisplayOrder = 3,
                ColorSquaresRgb = "#8a374a"
            });
            sa5.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Blue",
                DisplayOrder = 4,
                ColorSquaresRgb = "#47476f"
            });
            var specificationAttributes = new List<SpecificationAttribute>
            {
                sa1,
                sa2,
                sa3,
                sa4,
                sa5
            };
            _specificationAttributeRepository.Insert(specificationAttributes);
        }

        protected virtual void InstallProductAttributes()
        {
            var productAttributes = new List<ProductAttribute>
            {
                new ProductAttribute
                {
                    Name = "Color"
                },
                new ProductAttribute
                {
                    Name = "Print"
                },
                new ProductAttribute
                {
                    Name = "Custom Text"
                },
                new ProductAttribute
                {
                    Name = "HDD"
                },
                new ProductAttribute
                {
                    Name = "OS"
                },
                new ProductAttribute
                {
                    Name = "Processor"
                },
                new ProductAttribute
                {
                    Name = "RAM"
                },
                new ProductAttribute
                {
                    Name = "Size"
                },
                new ProductAttribute
                {
                    Name = "Software"
                }
            };
            _productAttributeRepository.Insert(productAttributes);
        }

        protected virtual void InstallCategories()
        {
            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();

            var categoryTemplateInGridAndLines = _categoryTemplateRepository
                .Table.FirstOrDefault(pt => pt.Name == "Products in Grid or Lines");
            if (categoryTemplateInGridAndLines == null)
                throw new Exception("Category template cannot be loaded");

            //categories
            var allCategories = new List<Category>();
            var categoryComputers = new Category
            {
                Name = "Computers",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_computers.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Computers")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryComputers);
            _categoryRepository.Insert(categoryComputers);

            var categoryDesktops = new Category
            {
                Name = "Desktops",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_desktops.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Desktops")).Id,
                PriceRanges = "-1000;1000-1200;1200-;",
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryDesktops);
            _categoryRepository.Insert(categoryDesktops);

            var categoryNotebooks = new Category
            {
                Name = "Notebooks",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_notebooks.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Notebooks")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryNotebooks);
            _categoryRepository.Insert(categoryNotebooks);

            var categorySoftware = new Category
            {
                Name = "Software",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_software.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Software")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categorySoftware);
            _categoryRepository.Insert(categorySoftware);

            var categoryElectronics = new Category
            {
                Name = "Electronics",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_electronics.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Electronics")).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryElectronics);
            _categoryRepository.Insert(categoryElectronics);

            var categoryCameraPhoto = new Category
            {
                Name = "Camera & photo",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_camera_photo.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Camera, photo")).Id,
                PriceRanges = "-500;500-;",
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCameraPhoto);
            _categoryRepository.Insert(categoryCameraPhoto);

            var categoryCellPhones = new Category
            {
                Name = "Cell phones",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_cell_phones.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Cell phones")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCellPhones);
            _categoryRepository.Insert(categoryCellPhones);

            var categoryOthers = new Category
            {
                Name = "Others",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_accessories.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Accessories")).Id,
                IncludeInTopMenu = true,
                PriceRanges = "-100;100-;",
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryOthers);
            _categoryRepository.Insert(categoryOthers);

            var categoryApparel = new Category
            {
                Name = "Apparel",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_apparel.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Apparel")).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryApparel);
            _categoryRepository.Insert(categoryApparel);

            var categoryShoes = new Category
            {
                Name = "Shoes",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_shoes.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Shoes")).Id,
                PriceRanges = "-500;500-;",
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryShoes);
            _categoryRepository.Insert(categoryShoes);

            var categoryClothing = new Category
            {
                Name = "Clothing",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_clothing.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Clothing")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryClothing);
            _categoryRepository.Insert(categoryClothing);

            var categoryAccessories = new Category
            {
                Name = "Accessories",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_apparel_accessories.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Apparel Accessories")).Id,
                IncludeInTopMenu = true,
                PriceRanges = "-100;100-;",
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryAccessories);
            _categoryRepository.Insert(categoryAccessories);

            var categoryDigitalDownloads = new Category
            {
                Name = "Digital downloads",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_digital_downloads.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Digital downloads")).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryDigitalDownloads);
            _categoryRepository.Insert(categoryDigitalDownloads);

            var categoryBooks = new Category
            {
                Name = "Books",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Books, Dictionary, Textbooks",
                MetaDescription = "Books category description",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_book.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Book")).Id,
                PriceRanges = "-25;25-50;50-;",
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryBooks);
            _categoryRepository.Insert(categoryBooks);

            var categoryJewelry = new Category
            {
                Name = "Jewelry",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_jewelry.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Jewelry")).Id,
                PriceRanges = "0-500;500-700;700-3000;",
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 6,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryJewelry);
            _categoryRepository.Insert(categoryJewelry);

            var categoryGiftCards = new Category
            {
                Name = "Gift Cards",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "category_gift_cards.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Gift Cards")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 7,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryGiftCards);
            _categoryRepository.Insert(categoryGiftCards);

            //search engine names
            foreach (var category in allCategories)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = category.Id,
                    EntityName = nameof(Category),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = ValidateSeName(category, category.Name)
                });
            }
        }

        protected virtual void InstallManufacturers()
        {
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();

            var manufacturerTemplateInGridAndLines =
                _manufacturerTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Products in Grid or Lines");
            if (manufacturerTemplateInGridAndLines == null)
                throw new Exception("Manufacturer template cannot be loaded");

            var allManufacturers = new List<Manufacturer>();
            var manufacturerAsus = new Manufacturer
            {
                Name = "Apple",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                Published = true,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "manufacturer_apple.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Apple")).Id,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _manufacturerRepository.Insert(manufacturerAsus);
            allManufacturers.Add(manufacturerAsus);

            var manufacturerHp = new Manufacturer
            {
                Name = "HP",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                Published = true,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "manufacturer_hp.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Hp")).Id,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _manufacturerRepository.Insert(manufacturerHp);
            allManufacturers.Add(manufacturerHp);

            var manufacturerNike = new Manufacturer
            {
                Name = "Nike",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                Published = true,
                PictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "manufacturer_nike.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Nike")).Id,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _manufacturerRepository.Insert(manufacturerNike);
            allManufacturers.Add(manufacturerNike);

            //search engine names
            foreach (var manufacturer in allManufacturers)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = manufacturer.Id,
                    EntityName = nameof(Manufacturer),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = ValidateSeName(manufacturer, manufacturer.Name)
                });
            }
        }

        protected virtual void InstallComputers(ProductTemplate productTemplateSimple, List<Product> allProducts, string sampleImagesPath, IPictureService pictureService, List<RelatedProduct> relatedProducts)
        {
            var productBuildComputer = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Build your own computer",
                Sku = "COMP_CUST",
                ShortDescription = "Build it",
                FullDescription = "<p>Fight back against cluttered workspaces with the stylish IBM zBC12 All-in-One desktop PC, featuring powerful computing resources and a stunning 20.1-inch widescreen display with stunning XBRITE-HiColor LCD technology. The black IBM zBC12 has a built-in microphone and MOTION EYE camera with face-tracking technology that allows for easy communication with friends and family. And it has a built-in DVD burner and Sony's Movie Store software so you can create a digital entertainment library for personal viewing at your convenience. Easy to setup and even easier to use, this JS-series All-in-One includes an elegantly designed keyboard and a USB mouse.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "build-your-own-computer",
                AllowCustomerReviews = true,
                Price = 1200M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                ShowOnHomepage = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductAttributeMappings =
                {
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Processor"),
                        AttributeControlType = AttributeControlType.DropdownList,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "2.2 GHz Intel Pentium Dual-Core E2200",
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "2.5 GHz Intel Pentium Dual-Core E2200",
                                IsPreSelected = true,
                                PriceAdjustment = 15,
                                DisplayOrder = 2
                            }
                        }
                    },
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "RAM"),
                        AttributeControlType = AttributeControlType.DropdownList,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "2 GB",
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "4GB",
                                PriceAdjustment = 20,
                                DisplayOrder = 2
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "8GB",
                                PriceAdjustment = 60,
                                DisplayOrder = 3
                            }
                        }
                    },
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "HDD"),
                        AttributeControlType = AttributeControlType.RadioList,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "320 GB",
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "400 GB",
                                PriceAdjustment = 100,
                                DisplayOrder = 2
                            }
                        }
                    },
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "OS"),
                        AttributeControlType = AttributeControlType.RadioList,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Vista Home",
                                PriceAdjustment = 50,
                                IsPreSelected = true,
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Vista Premium",
                                PriceAdjustment = 60,
                                DisplayOrder = 2
                            }
                        }
                    },
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Software"),
                        AttributeControlType = AttributeControlType.Checkboxes,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Microsoft Office",
                                PriceAdjustment = 50,
                                IsPreSelected = true,
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Acrobat Reader",
                                PriceAdjustment = 10,
                                DisplayOrder = 2
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Total Commander",
                                PriceAdjustment = 5,
                                DisplayOrder = 2
                            }
                        }
                    }
                },
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Desktops"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productBuildComputer);
            productBuildComputer.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_Desktops_1.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productBuildComputer.Name)),
                DisplayOrder = 1
            });
            productBuildComputer.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_Desktops_2.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productBuildComputer.Name)),
                DisplayOrder = 2
            });
            _productRepository.Insert(productBuildComputer);

            AddProductTag(productBuildComputer, "awesome");
            AddProductTag(productBuildComputer, "computer");

            var productDigitalStorm = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Digital Storm VANQUISH 3 Custom Performance PC",
                Sku = "DS_VA3_PC",
                ShortDescription = "Digital Storm Vanquish 3 Desktop PC",
                FullDescription = "<p>Blow the doors off today’s most demanding games with maximum detail, speed, and power for an immersive gaming experience without breaking the bank.</p><p>Stay ahead of the competition, VANQUISH 3 is fully equipped to easily handle future upgrades, keeping your system on the cutting edge for years to come.</p><p>Each system is put through an extensive stress test, ensuring you experience zero bottlenecks and get the maximum performance from your hardware.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "compaq-presario-sr1519x-pentium-4-desktop-pc-with-cdrw",
                AllowCustomerReviews = true,
                Price = 1259M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Desktops"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productDigitalStorm);
            productDigitalStorm.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_DigitalStorm.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productDigitalStorm.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productDigitalStorm);

            AddProductTag(productDigitalStorm, "cool");
            AddProductTag(productDigitalStorm, "computer");

            var productLenovoIdeaCentre = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Lenovo IdeaCentre 600 All-in-One PC",
                Sku = "LE_IC_600",
                ShortDescription = string.Empty,
                FullDescription = "<p>The A600 features a 21.5in screen, DVD or optional Blu-Ray drive, support for the full beans 1920 x 1080 HD, Dolby Home Cinema certification and an optional hybrid analogue/digital TV tuner.</p><p>Connectivity is handled by 802.11a/b/g - 802.11n is optional - and an ethernet port. You also get four USB ports, a Firewire slot, a six-in-one card reader and a 1.3- or two-megapixel webcam.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "hp-iq506-touchsmart-desktop-pc",
                AllowCustomerReviews = true,
                Price = 500M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Desktops"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productLenovoIdeaCentre);
            productLenovoIdeaCentre.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_LenovoIdeaCentre.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productLenovoIdeaCentre.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productLenovoIdeaCentre);

            AddProductTag(productLenovoIdeaCentre, "awesome");
            AddProductTag(productLenovoIdeaCentre, "computer");

            var productAppleMacBookPro = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Apple MacBook Pro 13-inch",
                Sku = "AP_MBP_13",
                ShortDescription = "A groundbreaking Retina display. A new force-sensing trackpad. All-flash architecture. Powerful dual-core and quad-core Intel processors. Together, these features take the notebook to a new level of performance. And they will do the same for you in everything you create.",
                FullDescription = "<p>With fifth-generation Intel Core processors, the latest graphics, and faster flash storage, the incredibly advanced MacBook Pro with Retina display moves even further ahead in performance and battery life.* *Compared with the previous generation.</p><p>Retina display with 2560-by-1600 resolution</p><p>Fifth-generation dual-core Intel Core i5 processor</p><p>Intel Iris Graphics</p><p>Up to 9 hours of battery life1</p><p>Faster flash storage2</p><p>802.11ac Wi-Fi</p><p>Two Thunderbolt 2 ports for connecting high-performance devices and transferring data at lightning speed</p><p>Two USB 3 ports (compatible with USB 2 devices) and HDMI</p><p>FaceTime HD camera</p><p>Pages, Numbers, Keynote, iPhoto, iMovie, GarageBand included</p><p>OS X, the world's most advanced desktop operating system</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "asus-eee-pc-1000ha-10-inch-netbook",
                AllowCustomerReviews = true,
                Price = 1800M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 3,
                Length = 3,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 2,
                OrderMaximumQuantity = 10000,
                Published = true,
                ShowOnHomepage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1
                    }
                },
                ProductManufacturers =
                {
                    new ProductManufacturer
                    {
                        Manufacturer = _manufacturerRepository.Table.Single(c => c.Name == "Apple"),
                        DisplayOrder = 2
                    }
                },
                ProductSpecificationAttributes =
                {
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 1,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Screensize").SpecificationAttributeOptions.Single(sao => sao.Name == "13.0''")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 2,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "CPU Type").SpecificationAttributeOptions.Single(sao => sao.Name == "Intel Core i5")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 3,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Memory").SpecificationAttributeOptions.Single(sao => sao.Name == "4 GB")
                    }
                }
            };
            allProducts.Add(productAppleMacBookPro);
            productAppleMacBookPro.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_macbook_1.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAppleMacBookPro.Name)),
                DisplayOrder = 1
            });
            productAppleMacBookPro.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_macbook_2.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAppleMacBookPro.Name)),
                DisplayOrder = 2
            });
            _productRepository.Insert(productAppleMacBookPro);

            AddProductTag(productAppleMacBookPro, "compact");
            AddProductTag(productAppleMacBookPro, "awesome");
            AddProductTag(productAppleMacBookPro, "computer");

            var productAsusN551JK = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Asus N551JK-XO076H Laptop",
                Sku = "AS_551_LP",
                ShortDescription = "Laptop Asus N551JK Intel Core i7-4710HQ 2.5 GHz, RAM 16GB, HDD 1TB, Video NVidia GTX 850M 4GB, BluRay, 15.6, Full HD, Win 8.1",
                FullDescription = "<p>The ASUS N550JX combines cutting-edge audio and visual technology to deliver an unsurpassed multimedia experience. A full HD wide-view IPS panel is tailor-made for watching movies and the intuitive touchscreen makes for easy, seamless navigation. ASUS has paired the N550JX’s impressive display with SonicMaster Premium, co-developed with Bang & Olufsen ICEpower® audio experts, for true surround sound. A quad-speaker array and external subwoofer combine for distinct vocals and a low bass that you can feel.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "asus-eee-pc-900ha-89-inch-netbook-black",
                AllowCustomerReviews = true,
                Price = 1500M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1
                    }
                },
                ProductSpecificationAttributes =
                {
                     new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 1,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Screensize").SpecificationAttributeOptions.Single(sao => sao.Name == "15.6''")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 2,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "CPU Type").SpecificationAttributeOptions.Single(sao => sao.Name == "Intel Core i7")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 3,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Memory").SpecificationAttributeOptions.Single(sao => sao.Name == "16 GB")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 4,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Hard drive").SpecificationAttributeOptions.Single(sao => sao.Name == "1 TB")
                    }
                }
            };
            allProducts.Add(productAsusN551JK);
            productAsusN551JK.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_asuspc_N551JK.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAsusN551JK.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productAsusN551JK);

            AddProductTag(productAsusN551JK, "compact");
            AddProductTag(productAsusN551JK, "awesome");
            AddProductTag(productAsusN551JK, "computer");

            var productSamsungSeries = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Samsung Series 9 NP900X4C Premium Ultrabook",
                Sku = "SM_900_PU",
                ShortDescription = "Samsung Series 9 NP900X4C-A06US 15-Inch Ultrabook (1.70 GHz Intel Core i5-3317U Processor, 8GB DDR3, 128GB SSD, Windows 8) Ash Black",
                FullDescription = "<p>Designed with mobility in mind, Samsung's durable, ultra premium, lightweight Series 9 laptop (model NP900X4C-A01US) offers mobile professionals and power users a sophisticated laptop equally suited for work and entertainment. Featuring a minimalist look that is both simple and sophisticated, its polished aluminum uni-body design offers an iconic look and feel that pushes the envelope with an edge just 0.58 inches thin. This Series 9 laptop also includes a brilliant 15-inch SuperBright Plus display with HD+ technology, 128 GB Solid State Drive (SSD), 8 GB of system memory, and up to 10 hours of battery life.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "hp-pavilion-artist-edition-dv2890nr-141-inch-laptop",
                AllowCustomerReviews = true,
                Price = 1590M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                //ShowOnHomepage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1
                    }
                },
                ProductSpecificationAttributes =
                {
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 1,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Screensize").SpecificationAttributeOptions.Single(sao => sao.Name == "15.0''")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 2,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "CPU Type").SpecificationAttributeOptions.Single(sao => sao.Name == "Intel Core i5")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 3,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Memory").SpecificationAttributeOptions.Single(sao => sao.Name == "8 GB")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 4,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Hard drive").SpecificationAttributeOptions.Single(sao => sao.Name == "128 GB")
                    }
                }
            };
            allProducts.Add(productSamsungSeries);
            productSamsungSeries.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_SamsungNP900X4C.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productSamsungSeries.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productSamsungSeries);

            AddProductTag(productSamsungSeries, "nice");
            AddProductTag(productSamsungSeries, "computer");
            AddProductTag(productSamsungSeries, "compact");

            var productHpSpectre = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "HP Spectre XT Pro UltraBook",
                Sku = "HP_SPX_UB",
                ShortDescription = "HP Spectre XT Pro UltraBook / Intel Core i5-2467M / 13.3 / 4GB / 128GB / Windows 7 Professional / Laptop",
                FullDescription = "<p>Introducing HP ENVY Spectre XT, the Ultrabook designed for those who want style without sacrificing substance. It's sleek. It's thin. And with Intel. Corer i5 processor and premium materials, it's designed to go anywhere from the bistro to the boardroom, it's unlike anything you've ever seen from HP.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "hp-pavilion-elite-m9150f-desktop-pc",
                AllowCustomerReviews = true,
                Price = 1350M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1
                    }
                },
                ProductManufacturers =
                {
                    new ProductManufacturer
                    {
                        Manufacturer = _manufacturerRepository.Table.Single(c => c.Name == "HP"),
                        DisplayOrder = 3
                    }
                },
                ProductSpecificationAttributes =
                {
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 1,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Screensize").SpecificationAttributeOptions.Single(sao => sao.Name == "13.3''")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 2,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "CPU Type").SpecificationAttributeOptions.Single(sao => sao.Name == "Intel Core i5")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 3,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Memory").SpecificationAttributeOptions.Single(sao => sao.Name == "4 GB")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 4,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Hard drive").SpecificationAttributeOptions.Single(sao => sao.Name == "128 GB")
                    }
                }
            };
            allProducts.Add(productHpSpectre);
            productHpSpectre.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_HPSpectreXT_1.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHpSpectre.Name)),
                DisplayOrder = 1
            });
            productHpSpectre.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_HPSpectreXT_2.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHpSpectre.Name)),
                DisplayOrder = 2
            });
            _productRepository.Insert(productHpSpectre);

            AddProductTag(productHpSpectre, "nice");
            AddProductTag(productHpSpectre, "computer");

            var productHpEnvy = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "HP Envy 6-1180ca 15.6-Inch Sleekbook",
                Sku = "HP_ESB_15",
                ShortDescription = "HP ENVY 6-1202ea Ultrabook Beats Audio, 3rd generation Intel® CoreTM i7-3517U processor, 8GB RAM, 500GB HDD, Microsoft Windows 8, AMD Radeon HD 8750M (2 GB DDR3 dedicated)",
                FullDescription = "The UltrabookTM that's up for anything. Thin and light, the HP ENVY is the large screen UltrabookTM with Beats AudioTM. With a soft-touch base that makes it easy to grab and go, it's a laptop that's up for anything.<br /><br /><b>Features</b><br /><br />- Windows 8 or other operating systems available<br /><br /><b>Top performance. Stylish design. Take notice.</b><br /><br />- At just 19.8 mm (0.78 in) thin, the HP ENVY UltrabookTM is slim and light enough to take anywhere. It's the laptop that gets you noticed with the power to get it done.<br />- With an eye-catching metal design, it's a laptop that you want to carry with you. The soft-touch, slip-resistant base gives you the confidence to carry it with ease.<br /><br /><b>More entertaining. More gaming. More fun.</b><br /><br />- Own the UltrabookTM with Beats AudioTM, dual speakers, a subwoofer, and an awesome display. Your music, movies and photo slideshows will always look and sound their best.<br />- Tons of video memory let you experience incredible gaming and multimedia without slowing down. Create and edit videos in a flash. And enjoy more of what you love to the fullest.<br />- The HP ENVY UltrabookTM is loaded with the ports you'd expect on a world-class laptop, but on a Sleekbook instead. Like HDMI, USB, RJ-45, and a headphone jack. You get all the right connections without compromising size.<br /><br /><b>Only from HP.</b><br /><br />- Life heats up. That's why there's HP CoolSense technology, which automatically adjusts your notebook's temperature based on usage and conditions. It stays cool. You stay comfortable.<br />- With HP ProtectSmart, your notebook's data stays safe from accidental bumps and bruises. It senses motion and plans ahead, stopping your hard drive and protecting your entire digital life.<br />- Keep playing even in dimly lit rooms or on red eye flights. The optional backlit keyboard[1] is full-size so you don't compromise comfort. Backlit keyboard. Another bright idea.<br /><br />",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "hp-pavilion-g60-230us-160-inch-laptop",
                AllowCustomerReviews = true,
                Price = 1460M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1
                    }
                },
                ProductManufacturers =
                {
                    new ProductManufacturer
                    {
                        Manufacturer = _manufacturerRepository.Table.Single(c => c.Name == "HP"),
                        DisplayOrder = 4
                    }
                },
                ProductSpecificationAttributes =
                {
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 1,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Screensize").SpecificationAttributeOptions.Single(sao => sao.Name == "15.6''")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 2,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "CPU Type").SpecificationAttributeOptions.Single(sao => sao.Name == "Intel Core i7")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 3,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Memory").SpecificationAttributeOptions.Single(sao => sao.Name == "8 GB")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 4,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Hard drive").SpecificationAttributeOptions.Single(sao => sao.Name == "500 GB")
                    }
                }
            };
            allProducts.Add(productHpEnvy);
            productHpEnvy.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_HpEnvy6.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHpEnvy.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productHpEnvy);

            AddProductTag(productHpEnvy, "computer");
            AddProductTag(productHpEnvy, "cool");
            AddProductTag(productHpEnvy, "compact");

            var productLenovoThinkpad = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Lenovo Thinkpad X1 Carbon Laptop",
                Sku = "LE_TX1_CL",
                ShortDescription = "Lenovo Thinkpad X1 Carbon Touch Intel Core i7 14 Ultrabook",
                FullDescription = "<p>The X1 Carbon brings a new level of quality to the ThinkPad legacy of high standards and innovation. It starts with the durable, carbon fiber-reinforced roll cage, making for the best Ultrabook construction available, and adds a host of other new features on top of the old favorites. Because for 20 years, we haven't stopped innovating. And you shouldn't stop benefiting from that.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "toshiba-satellite-a305-s6908-154-inch-laptop",
                AllowCustomerReviews = true,
                Price = 1360M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1
                    }
                },
                ProductSpecificationAttributes =
                {
                   new ProductSpecificationAttribute
                    {
                        AllowFiltering = false,
                        ShowOnProductPage = true,
                        DisplayOrder = 1,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "Screensize").SpecificationAttributeOptions.Single(sao => sao.Name == "14.0''")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = true,
                        DisplayOrder = 2,
                        SpecificationAttributeOption = _specificationAttributeRepository.Table.Single(sa => sa.Name == "CPU Type").SpecificationAttributeOptions.Single(sao => sao.Name == "Intel Core i7")
                    }
                }
            };
            allProducts.Add(productLenovoThinkpad);
            productLenovoThinkpad.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_LenovoThinkpad.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productLenovoThinkpad.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productLenovoThinkpad);

            AddProductTag(productLenovoThinkpad, "awesome");
            AddProductTag(productLenovoThinkpad, "computer");
            AddProductTag(productLenovoThinkpad, "compact");

            var productAdobePhotoshop = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Adobe Photoshop CS4",
                Sku = "AD_CS4_PH",
                ShortDescription = "Easily find and view all your photos",
                FullDescription = "<p>Adobe Photoshop CS4 software combines power and simplicity so you can make ordinary photos extraordinary; tell engaging stories in beautiful, personalized creations for print and web; and easily find and view all your photos. New Photoshop.com membership* works with Photoshop CS4 so you can protect your photos with automatic online backup and 2 GB of storage; view your photos anywhere you are; and share your photos in fun, interactive ways with invitation-only Online Albums.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "adobe-photoshop-elements-7",
                AllowCustomerReviews = true,
                Price = 75M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Software"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productAdobePhotoshop);
            productAdobePhotoshop.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_AdobePhotoshop.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAdobePhotoshop.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productAdobePhotoshop);

            AddProductTag(productAdobePhotoshop, "computer");
            AddProductTag(productAdobePhotoshop, "awesome");

            var productWindows8Pro = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Windows 8 Pro",
                Sku = "MS_WIN_8P",
                ShortDescription = "Windows 8 is a Microsoft operating system that was released in 2012 as part of the company's Windows NT OS family. ",
                FullDescription = "<p>Windows 8 Pro is comparable to Windows 7 Professional and Ultimate and is targeted towards enthusiasts and business users; it includes all the features of Windows 8. Additional features include the ability to receive Remote Desktop connections, the ability to participate in a Windows Server domain, Encrypting File System, Hyper-V, and Virtual Hard Disk Booting, Group Policy as well as BitLocker and BitLocker To Go. Windows Media Center functionality is available only for Windows 8 Pro as a separate software package.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "corel-paint-shop-pro-photo-x2",
                AllowCustomerReviews = true,
                Price = 65M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Software"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productWindows8Pro);
            productWindows8Pro.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_Windows8.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productWindows8Pro.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productWindows8Pro);

            AddProductTag(productWindows8Pro, "awesome");
            AddProductTag(productWindows8Pro, "computer");

            var productSoundForge = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Sound Forge Pro 11 (recurring)",
                Sku = "SF_PRO_11",
                ShortDescription = "Advanced audio waveform editor.",
                FullDescription = "<p>Sound Forge™ Pro is the application of choice for a generation of creative and prolific artists, producers, and editors. Record audio quickly on a rock-solid platform, address sophisticated audio processing tasks with surgical precision, and render top-notch master files with ease. New features include one-touch recording, metering for the new critical standards, more repair and restoration tools, and exclusive round-trip interoperability with SpectraLayers Pro. Taken together, these enhancements make this edition of Sound Forge Pro the deepest and most advanced audio editing platform available.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "major-league-baseball-2k9",
                IsRecurring = true,
                RecurringCycleLength = 30,
                RecurringCyclePeriod = RecurringProductCyclePeriod.Months,
                RecurringTotalCycles = 12,
                AllowCustomerReviews = true,
                Price = 54.99M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Software"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productSoundForge);
            productSoundForge.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_SoundForge.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productSoundForge.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productSoundForge);

            AddProductTag(productSoundForge, "game");
            AddProductTag(productSoundForge, "computer");
            AddProductTag(productSoundForge, "cool");

            relatedProducts.AddRange(new[]
            {
                new RelatedProduct
                {
                    ProductId1 = productLenovoIdeaCentre.Id,
                    ProductId2 = productDigitalStorm.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productDigitalStorm.Id,
                    ProductId2 = productBuildComputer.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productDigitalStorm.Id,
                    ProductId2 = productLenovoIdeaCentre.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productDigitalStorm.Id,
                    ProductId2 = productLenovoThinkpad.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productDigitalStorm.Id,
                    ProductId2 = productAppleMacBookPro.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productLenovoIdeaCentre.Id,
                    ProductId2 = productBuildComputer.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productAsusN551JK.Id,
                    ProductId2 = productLenovoThinkpad.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productAsusN551JK.Id,
                    ProductId2 = productAppleMacBookPro.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productAsusN551JK.Id,
                    ProductId2 = productSamsungSeries.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productAsusN551JK.Id,
                    ProductId2 = productHpSpectre.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productLenovoThinkpad.Id,
                    ProductId2 = productAsusN551JK.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productLenovoThinkpad.Id,
                    ProductId2 = productAppleMacBookPro.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productLenovoThinkpad.Id,
                    ProductId2 = productSamsungSeries.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productLenovoThinkpad.Id,
                    ProductId2 = productHpEnvy.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productAppleMacBookPro.Id,
                    ProductId2 = productLenovoThinkpad.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productAppleMacBookPro.Id,
                    ProductId2 = productSamsungSeries.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productAppleMacBookPro.Id,
                    ProductId2 = productAsusN551JK.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productAppleMacBookPro.Id,
                    ProductId2 = productHpSpectre.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productHpSpectre.Id,
                    ProductId2 = productLenovoThinkpad.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productHpSpectre.Id,
                    ProductId2 = productSamsungSeries.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productHpSpectre.Id,
                    ProductId2 = productAsusN551JK.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productHpSpectre.Id,
                    ProductId2 = productHpEnvy.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productHpEnvy.Id,
                    ProductId2 = productAsusN551JK.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productHpEnvy.Id,
                    ProductId2 = productAppleMacBookPro.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productHpEnvy.Id,
                    ProductId2 = productHpSpectre.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productHpEnvy.Id,
                    ProductId2 = productSamsungSeries.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productSamsungSeries.Id,
                    ProductId2 = productAsusN551JK.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productSamsungSeries.Id,
                    ProductId2 = productAppleMacBookPro.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productSamsungSeries.Id,
                    ProductId2 = productHpEnvy.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productSamsungSeries.Id,
                    ProductId2 = productHpSpectre.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productLenovoIdeaCentre.Id,
                    ProductId2 = productLenovoThinkpad.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productLenovoIdeaCentre.Id,
                    ProductId2 = productAppleMacBookPro.Id
                }
            });
        }

        protected virtual void InstallElectronics(ProductTemplate productTemplateSimple, ProductTemplate productTemplateGrouped, List<Product> allProducts, string sampleImagesPath, IPictureService pictureService, List<RelatedProduct> relatedProducts)
        {
            //this one is a grouped product with two associated ones
            var productNikonD5500DSLR = new Product
            {
                ProductType = ProductType.GroupedProduct,
                VisibleIndividually = true,
                Name = "Nikon D5500 DSLR",
                Sku = "N5500DS_0",
                ShortDescription = "Slim, lightweight Nikon D5500 packs a vari-angle touchscreen",
                FullDescription = "Nikon has announced its latest DSLR, the D5500. A lightweight, compact DX-format camera with a 24.2MP sensor, it’s the first of its type to offer a vari-angle touchscreen. The D5500 replaces the D5300 in Nikon’s range, and while it offers much the same features the company says it’s a much slimmer and lighter prospect. There’s a deep grip for easier handling and built-in Wi-Fi that lets you transfer and share shots via your phone or tablet.",
                ProductTemplateId = productTemplateGrouped.Id,
                //SeName = "canon-digital-slr-camera",
                AllowCustomerReviews = true,
                Published = true,
                Price = 670M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Camera & photo"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productNikonD5500DSLR);
            productNikonD5500DSLR.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_NikonCamera_1.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productNikonD5500DSLR.Name)),
                DisplayOrder = 1
            });
            productNikonD5500DSLR.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_NikonCamera_2.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productNikonD5500DSLR.Name)),
                DisplayOrder = 2
            });
            _productRepository.Insert(productNikonD5500DSLR);

            AddProductTag(productNikonD5500DSLR, "cool");
            AddProductTag(productNikonD5500DSLR, "camera");

            var productNikonD5500DSLR_associated_1 = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = false, //hide this products
                ParentGroupedProductId = productNikonD5500DSLR.Id,
                Name = "Nikon D5500 DSLR - Black",
                Sku = "N5500DS_B",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "canon-digital-slr-camera-black",
                AllowCustomerReviews = true,
                Published = true,
                Price = 670M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allProducts.Add(productNikonD5500DSLR_associated_1);
            productNikonD5500DSLR_associated_1.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_NikonCamera_black.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Canon Digital SLR Camera - Black")),
                DisplayOrder = 1
            });
            _productRepository.Insert(productNikonD5500DSLR_associated_1);
            var productNikonD5500DSLR_associated_2 = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = false, //hide this products
                ParentGroupedProductId = productNikonD5500DSLR.Id,
                Name = "Nikon D5500 DSLR - Red",
                Sku = "N5500DS_R",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "canon-digital-slr-camera-silver",
                AllowCustomerReviews = true,
                Published = true,
                Price = 630M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allProducts.Add(productNikonD5500DSLR_associated_2);
            productNikonD5500DSLR_associated_2.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_NikonCamera_red.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Canon Digital SLR Camera - Silver")),
                DisplayOrder = 1
            });
            _productRepository.Insert(productNikonD5500DSLR_associated_2);

            var productLeica = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Leica T Mirrorless Digital Camera",
                Sku = "LT_MIR_DC",
                ShortDescription = "Leica T (Typ 701) Silver",
                FullDescription = "<p>The new Leica T offers a minimalist design that's crafted from a single block of aluminum.  Made in Germany and assembled by hand, this 16.3 effective mega pixel camera is easy to use.  With a massive 3.7 TFT LCD intuitive touch screen control, the user is able to configure and save their own menu system.  The Leica T has outstanding image quality and also has 16GB of built in memory.  This is Leica's first system camera to use Wi-Fi.  Add the T-App to your portable iOS device and be able to transfer and share your images (free download from the Apple App Store)</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "canon-vixia-hf100-camcorder",
                AllowCustomerReviews = true,
                Price = 530M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Camera & photo"),
                        DisplayOrder = 3
                    }
                }
            };
            allProducts.Add(productLeica);
            productLeica.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_LeicaT.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productLeica.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productLeica);

            AddProductTag(productLeica, "camera");
            AddProductTag(productLeica, "cool");

            var productAppleICam = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Apple iCam",
                Sku = "APPLE_CAM",
                ShortDescription = "Photography becomes smart",
                FullDescription = "<p>A few months ago we featured the amazing WVIL camera, by many considered the future of digital photography. This is another very good looking concept, iCam is the vision of Italian designer Antonio DeRosa, the idea is to have a device that attaches to the iPhone 5, which then allows the user to have a camera with interchangeable lenses. The device would also feature a front-touch screen and a projector. Would be great if apple picked up on this and made it reality.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "panasonic-hdc-sdt750k-high-definition-3d-camcorder",
                AllowCustomerReviews = true,
                Price = 1300M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Camera & photo"),
                        DisplayOrder = 2
                    }
                },
                ProductManufacturers =
                {
                    new ProductManufacturer
                    {
                        Manufacturer = _manufacturerRepository.Table.Single(c => c.Name == "Apple"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productAppleICam);
            productAppleICam.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_iCam.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAppleICam.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productAppleICam);

            var productHtcOne = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "HTC One M8 Android L 5.0 Lollipop",
                Sku = "M8_HTC_5L",
                ShortDescription = "HTC - One (M8) 4G LTE Cell Phone with 32GB Memory - Gunmetal (Sprint)",
                FullDescription = "<p><b>HTC One (M8) Cell Phone for Sprint:</b> With its brushed-metal design and wrap-around unibody frame, the HTC One (M8) is designed to fit beautifully in your hand. It's fun to use with amped up sound and a large Full HD touch screen, and intuitive gesture controls make it seem like your phone almost knows what you need before you do. <br /><br />Sprint Easy Pay option available in store.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "blackberry-bold-9000-phone-black-att",
                AllowCustomerReviews = true,
                Price = 245M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                ShowOnHomepage = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Cell phones"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productHtcOne);
            productHtcOne.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_HTC_One_M8.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHtcOne.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productHtcOne);

            AddProductTag(productHtcOne, "cell");
            AddProductTag(productHtcOne, "compact");
            AddProductTag(productHtcOne, "awesome");

            var productHtcOneMini = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "HTC One Mini Blue",
                Sku = "OM_HTC_BL",
                ShortDescription = "HTC One and HTC One Mini now available in bright blue hue",
                FullDescription = "<p>HTC One mini smartphone with 4.30-inch 720x1280 display powered by 1.4GHz processor alongside 1GB RAM and 4-Ultrapixel rear camera.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "samsung-rugby-a837-phone-black-att",
                AllowCustomerReviews = true,
                Price = 100M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Cell phones"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productHtcOneMini);
            productHtcOneMini.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_HTC_One_Mini_1.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHtcOneMini.Name)),
                DisplayOrder = 1
            });
            productHtcOneMini.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_HTC_One_Mini_2.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHtcOneMini.Name)),
                DisplayOrder = 2
            });
            _productRepository.Insert(productHtcOneMini);

            AddProductTag(productHtcOneMini, "awesome");
            AddProductTag(productHtcOneMini, "compact");
            AddProductTag(productHtcOneMini, "cell");

            var productNokiaLumia = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Nokia Lumia 1020",
                Sku = "N_1020_LU",
                ShortDescription = "Nokia Lumia 1020 4G Cell Phone (Unlocked)",
                FullDescription = "<p>Capture special moments for friends and family with this Nokia Lumia 1020 32GB WHITE cell phone that features an easy-to-use 41.0MP rear-facing camera and a 1.2MP front-facing camera. The AMOLED touch screen offers 768 x 1280 resolution for crisp visuals.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "sony-dcr-sr85-1mp-60gb-hard-drive-handycam-camcorder",
                AllowCustomerReviews = true,
                Price = 349M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Cell phones"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productNokiaLumia);
            productNokiaLumia.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_Lumia1020.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productNokiaLumia.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productNokiaLumia);

            AddProductTag(productNokiaLumia, "awesome");
            AddProductTag(productNokiaLumia, "cool");
            AddProductTag(productNokiaLumia, "camera");

            var productBeatsPill = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Beats Pill 2.0 Wireless Speaker",
                Sku = "BP_20_WSP",
                ShortDescription = "<b>Pill 2.0 Portable Bluetooth Speaker (1-Piece):</b> Watch your favorite movies and listen to music with striking sound quality. This lightweight, portable speaker is easy to take with you as you travel to any destination, keeping you entertained wherever you are. ",
                FullDescription = "<ul><li>Pair and play with your Bluetooth® device with 30 foot range</li><li>Built-in speakerphone</li><li>7 hour rechargeable battery</li><li>Power your other devices with USB charge out</li><li>Tap two Beats Pills™ together for twice the sound with Beats Bond™</li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "acer-aspire-one-89-mini-notebook-case-black",
                AllowCustomerReviews = true,
                Price = 79.99M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                TierPrices =
                {
                    new TierPrice
                    {
                        Quantity = 2,
                        Price = 19
                    },
                    new TierPrice
                    {
                        Quantity = 5,
                        Price = 17
                    },
                    new TierPrice
                    {
                        Quantity = 10,
                        Price = 15,
                        StartDateTimeUtc = DateTime.UtcNow.AddDays(-7),
                        EndDateTimeUtc = DateTime.UtcNow.AddDays(7)
                    }
                },
                HasTierPrices = true,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Others"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productBeatsPill);
            productBeatsPill.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_PillBeats_1.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productBeatsPill.Name)),
                DisplayOrder = 1
            });
            productBeatsPill.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_PillBeats_2.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productBeatsPill.Name)),
                DisplayOrder = 2
            });
            _productRepository.Insert(productBeatsPill);

            AddProductTag(productBeatsPill, "computer");
            AddProductTag(productBeatsPill, "cool");

            var productUniversalTabletCover = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Universal 7-8 Inch Tablet Cover",
                Sku = "TC_78I_UN",
                ShortDescription = "Universal protection for 7-inch & 8-inch tablets",
                FullDescription = "<p>Made of durable polyurethane, our Universal Cover is slim, lightweight, and strong, with protective corners that stretch to hold most 7 and 8-inch tablets securely. This tough case helps protects your tablet from bumps, scuffs, and dings.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "apc-back-ups-rs-800va-ups-800-va-ups-battery-lead-acid-br800blk",
                AllowCustomerReviews = true,
                Price = 39M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Others"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productUniversalTabletCover);
            productUniversalTabletCover.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_TabletCover.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productUniversalTabletCover.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productUniversalTabletCover);

            AddProductTag(productUniversalTabletCover, "computer");
            AddProductTag(productUniversalTabletCover, "cool");

            var productPortableSoundSpeakers = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Portable Sound Speakers",
                Sku = "PT_SPK_SN",
                ShortDescription = "Universall portable sound speakers",
                FullDescription = "<p>Your phone cut the cord, now it's time for you to set your music free and buy a Bluetooth speaker. Thankfully, there's one suited for everyone out there.</p><p>Some Bluetooth speakers excel at packing in as much functionality as the unit can handle while keeping the price down. Other speakers shuck excess functionality in favor of premium build materials instead. Whatever path you choose to go down, you'll be greeted with many options to suit your personal tastes.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "microsoft-bluetooth-notebook-mouse-5000-macwindows",
                AllowCustomerReviews = true,
                Price = 37M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Others"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productPortableSoundSpeakers);
            productPortableSoundSpeakers.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_Speakers.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productPortableSoundSpeakers.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productPortableSoundSpeakers);

            relatedProducts.AddRange(new[]
            {
                new RelatedProduct
                {
                     ProductId1 = productLeica.Id,
                     ProductId2 = productHtcOneMini.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productLeica.Id,
                     ProductId2 = productNikonD5500DSLR.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productLeica.Id,
                     ProductId2 = productAppleICam.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productLeica.Id,
                     ProductId2 = productNokiaLumia.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productHtcOne.Id,
                     ProductId2 = productHtcOneMini.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productHtcOne.Id,
                     ProductId2 = productNokiaLumia.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productHtcOne.Id,
                     ProductId2 = productBeatsPill.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productHtcOne.Id,
                     ProductId2 = productPortableSoundSpeakers.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productHtcOneMini.Id,
                     ProductId2 = productHtcOne.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productHtcOneMini.Id,
                     ProductId2 = productNokiaLumia.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productHtcOneMini.Id,
                     ProductId2 = productBeatsPill.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productHtcOneMini.Id,
                     ProductId2 = productPortableSoundSpeakers.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productNokiaLumia.Id,
                     ProductId2 = productHtcOne.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productNokiaLumia.Id,
                     ProductId2 = productHtcOneMini.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productNokiaLumia.Id,
                     ProductId2 = productBeatsPill.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productNokiaLumia.Id,
                     ProductId2 = productPortableSoundSpeakers.Id
                }
            });
        }

        protected virtual void InstallApparel(ProductTemplate productTemplateSimple, List<Product> allProducts, string sampleImagesPath, IPictureService pictureService, List<RelatedProduct> relatedProducts, ProductAvailabilityRange productAvailabilityRange)
        {
            var productNikeFloral = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Nike Floral Roshe Customized Running Shoes",
                Sku = "NK_FRC_RS",
                ShortDescription = "When you ran across these shoes, you will immediately fell in love and needed a pair of these customized beauties.",
                FullDescription = "<p>Each Rosh Run is personalized and exclusive, handmade in our workshop Custom. Run Your Rosh creations born from the hand of an artist specialized in sneakers, more than 10 years of experience.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "adidas-womens-supernova-csh-7-running-shoe",
                AllowCustomerReviews = true,
                Price = 40M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductAttributeMappings =
                {
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Size"),
                        AttributeControlType = AttributeControlType.DropdownList,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "8",
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "9",
                                DisplayOrder = 2
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "10",
                                DisplayOrder = 3
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "11",
                                DisplayOrder = 4
                            }
                        }
                    },
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Color"),
                        AttributeControlType = AttributeControlType.DropdownList,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "White/Blue",
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "White/Black",
                                DisplayOrder = 2
                            }
                        }
                    },
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Print"),
                        AttributeControlType = AttributeControlType.ImageSquares,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Natural",
                                DisplayOrder = 1,
                                ImageSquaresPictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "p_attribute_print_2.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Natural Print")).Id
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Fresh",
                                DisplayOrder = 2,
                                ImageSquaresPictureId = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "p_attribute_print_1.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Fresh Print")).Id
                            }
                        }
                    }
                },
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Shoes"),
                        DisplayOrder = 1
                    }
                },
                ProductManufacturers =
                {
                    new ProductManufacturer
                    {
                        Manufacturer = _manufacturerRepository.Table.Single(c => c.Name == "Nike"),
                        DisplayOrder = 2
                    }
                },
                ProductSpecificationAttributes =
                {
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = false,
                        DisplayOrder = 1,
                        SpecificationAttributeOption =
                            _specificationAttributeRepository.Table.Single(sa => sa.Name == "Color")
                                .SpecificationAttributeOptions.Single(sao => sao.Name == "Grey")
                    }
                }
            };
            allProducts.Add(productNikeFloral);
            productNikeFloral.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_NikeFloralShoe_1.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productNikeFloral.Name)),
                DisplayOrder = 1
            });
            productNikeFloral.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_NikeFloralShoe_2.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productNikeFloral.Name)),
                DisplayOrder = 2
            });
            _productRepository.Insert(productNikeFloral);

            AddProductTag(productNikeFloral, "cool");
            AddProductTag(productNikeFloral, "shoes");
            AddProductTag(productNikeFloral, "apparel");

            productNikeFloral.ProductAttributeMappings.First(x => x.ProductAttribute.Name == "Print").ProductAttributeValues.First(x => x.Name == "Natural").PictureId = productNikeFloral.ProductPictures.ElementAt(0).PictureId;
            productNikeFloral.ProductAttributeMappings.First(x => x.ProductAttribute.Name == "Print").ProductAttributeValues.First(x => x.Name == "Fresh").PictureId = productNikeFloral.ProductPictures.ElementAt(1).PictureId;
            _productRepository.Update(productNikeFloral);

            var productAdidas = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "adidas Consortium Campus 80s Running Shoes",
                Sku = "AD_C80_RS",
                ShortDescription = "adidas Consortium Campus 80s Primeknit Light Maroon/Running Shoes",
                FullDescription = "<p>One of three colorways of the adidas Consortium Campus 80s Primeknit set to drop alongside each other. This pair comes in light maroon and running white. Featuring a maroon-based primeknit upper with white accents. A limited release, look out for these at select adidas Consortium accounts worldwide.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "etnies-mens-digit-sneaker",
                AllowCustomerReviews = true,
                Price = 27.56M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                //ShowOnHomepage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductAttributeMappings =
                {
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Size"),
                        AttributeControlType = AttributeControlType.DropdownList,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "8",
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "9",
                                DisplayOrder = 2
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "10",
                                DisplayOrder = 3
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "11",
                                DisplayOrder = 4
                            }
                        }
                    },
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Color"),
                        AttributeControlType = AttributeControlType.ColorSquares,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Red",
                                IsPreSelected = true,
                                ColorSquaresRgb = "#663030",
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Blue",
                                ColorSquaresRgb = "#363656",
                                DisplayOrder = 2
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Silver",
                                ColorSquaresRgb = "#c5c5d5",
                                DisplayOrder = 3
                            }
                        }
                    }
                },
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Shoes"),
                        DisplayOrder = 1
                    }
                },
                ProductSpecificationAttributes =
                {
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = false,
                        DisplayOrder = 1,
                        SpecificationAttributeOption =
                            _specificationAttributeRepository.Table.Single(sa => sa.Name == "Color")
                                .SpecificationAttributeOptions.Single(sao => sao.Name == "Grey")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = false,
                        DisplayOrder = 2,
                        SpecificationAttributeOption =
                            _specificationAttributeRepository.Table.Single(sa => sa.Name == "Color")
                                .SpecificationAttributeOptions.Single(sao => sao.Name == "Red")
                    },
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = false,
                        DisplayOrder = 3,
                        SpecificationAttributeOption =
                            _specificationAttributeRepository.Table.Single(sa => sa.Name == "Color")
                                .SpecificationAttributeOptions.Single(sao => sao.Name == "Blue")
                    }
                }
            };
            allProducts.Add(productAdidas);
            productAdidas.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_adidas.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productAdidas.Name)),
                DisplayOrder = 1
            });
            productAdidas.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_adidas_2.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productAdidas.Name)),
                DisplayOrder = 2
            });
            productAdidas.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_adidas_3.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productAdidas.Name)),
                DisplayOrder = 3
            });

            _productRepository.Insert(productAdidas);

            AddProductTag(productAdidas, "cool");
            AddProductTag(productAdidas, "shoes");
            AddProductTag(productAdidas, "apparel");

            productAdidas.ProductAttributeMappings.First(x => x.ProductAttribute.Name == "Color").ProductAttributeValues.First(x => x.Name == "Red").PictureId = productAdidas.ProductPictures.ElementAt(0).PictureId;
            productAdidas.ProductAttributeMappings.First(x => x.ProductAttribute.Name == "Color").ProductAttributeValues.First(x => x.Name == "Blue").PictureId = productAdidas.ProductPictures.ElementAt(1).PictureId;
            productAdidas.ProductAttributeMappings.First(x => x.ProductAttribute.Name == "Color").ProductAttributeValues.First(x => x.Name == "Silver").PictureId = productAdidas.ProductPictures.ElementAt(2).PictureId;
            _productRepository.Update(productAdidas);

            var productNikeZoom = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Nike SB Zoom Stefan Janoski \"Medium Mint\"",
                Sku = "NK_ZSJ_MM",
                ShortDescription = "Nike SB Zoom Stefan Janoski Dark Grey Medium Mint Teal ...",
                FullDescription = "The newly Nike SB Zoom Stefan Janoski gets hit with a \"Medium Mint\" accents that sits atop a Dark Grey suede. Expected to drop in October.",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "v-blue-juniors-cuffed-denim-short-with-rhinestones",
                AllowCustomerReviews = true,
                Price = 30M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Shoes"),
                        DisplayOrder = 1
                    }
                },
                ProductManufacturers =
                {
                    new ProductManufacturer
                    {
                        Manufacturer = _manufacturerRepository.Table.Single(c => c.Name == "Nike"),
                        DisplayOrder = 2
                    }
                },
                ProductSpecificationAttributes =
                {
                    new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ShowOnProductPage = false,
                        DisplayOrder = 1,
                        SpecificationAttributeOption =
                            _specificationAttributeRepository.Table.Single(sa => sa.Name == "Color")
                                .SpecificationAttributeOptions.Single(sao => sao.Name == "Grey")
                    }
                }
            };

            allProducts.Add(productNikeZoom);
            productNikeZoom.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_NikeZoom.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productNikeZoom.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productNikeZoom);

            AddProductTag(productNikeZoom, "jeans");
            AddProductTag(productNikeZoom, "cool");
            AddProductTag(productNikeZoom, "apparel");

            var productNikeTailwind = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Nike Tailwind Loose Short-Sleeve Running Shirt",
                Sku = "NK_TLS_RS",
                ShortDescription = string.Empty,
                FullDescription = "<p>Boost your adrenaline with the Nike® Women's Tailwind Running Shirt. The lightweight, slouchy fit is great for layering, and moisture-wicking fabrics keep you feeling at your best. This tee has a notched hem for an enhanced range of motion, while flat seams with reinforcement tape lessen discomfort and irritation over longer distances. Put your keys and card in the side zip pocket and take off in your Nike® running t-shirt.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "50s-rockabilly-polka-dot-top-jr-plus-size",
                AllowCustomerReviews = true,
                Published = true,
                Price = 15M,
                IsShipEnabled = true,
                Weight = 1,
                Length = 2,
                Width = 3,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductAttributeMappings =
                {
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Size"),
                        AttributeControlType = AttributeControlType.DropdownList,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Small",
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "1X",
                                DisplayOrder = 2
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "2X",
                                DisplayOrder = 3
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "3X",
                                DisplayOrder = 4
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "4X",
                                DisplayOrder = 5
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "5X",
                                DisplayOrder = 6
                            }
                        }
                    }
                },
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Clothing"),
                        DisplayOrder = 1
                    }
                },
                ProductManufacturers =
                {
                    new ProductManufacturer
                    {
                        Manufacturer = _manufacturerRepository.Table.Single(c => c.Name == "Nike"),
                        DisplayOrder = 2
                    }
                }
            };
            allProducts.Add(productNikeTailwind);
            productNikeTailwind.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_NikeShirt.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productNikeTailwind.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productNikeTailwind);

            AddProductTag(productNikeTailwind, "cool");
            AddProductTag(productNikeTailwind, "apparel");
            AddProductTag(productNikeTailwind, "shirt");

            var productOversizedWomenTShirt = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Oversized Women T-Shirt",
                Sku = "WM_OVR_TS",
                ShortDescription = string.Empty,
                FullDescription = "<p>This oversized women t-Shirt needs minimum ironing. It is a great product at a great value!</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "arrow-mens-wrinkle-free-pinpoint-solid-long-sleeve",
                AllowCustomerReviews = true,
                Price = 24M,
                IsShipEnabled = true,
                Weight = 4,
                Length = 3,
                Width = 3,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                TierPrices =
                {
                    new TierPrice
                    {
                        Quantity = 3,
                        Price = 21
                    },
                    new TierPrice
                    {
                        Quantity = 7,
                        Price = 19
                    },
                    new TierPrice
                    {
                        Quantity = 10,
                        Price = 16
                    }
                },
                HasTierPrices = true,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Clothing"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productOversizedWomenTShirt);
            productOversizedWomenTShirt.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_WomenTShirt.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productOversizedWomenTShirt.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productOversizedWomenTShirt);

            AddProductTag(productOversizedWomenTShirt, "cool");
            AddProductTag(productOversizedWomenTShirt, "apparel");
            AddProductTag(productOversizedWomenTShirt, "shirt");

            var productCustomTShirt = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Custom T-Shirt",
                Sku = "CS_TSHIRT",
                ShortDescription = "T-Shirt - Add Your Content",
                FullDescription = "<p>Comfort comes in all shapes and forms, yet this tee out does it all. Rising above the rest, our classic cotton crew provides the simple practicality you need to make it through the day. Tag-free, relaxed fit wears well under dress shirts or stands alone in laid-back style. Reinforced collar and lightweight feel give way to long-lasting shape and breathability. One less thing to worry about, rely on this tee to provide comfort and ease with every wear.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "custom-t-shirt",
                AllowCustomerReviews = true,
                Price = 15M,
                IsShipEnabled = true,
                Weight = 4,
                Length = 3,
                Width = 3,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductAttributeMappings =
                {
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Custom Text"),
                        TextPrompt = "Enter your text:",
                        AttributeControlType = AttributeControlType.TextBox,
                        IsRequired = true
                    }
                },
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Clothing"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productCustomTShirt);
            productCustomTShirt.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_CustomTShirt.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productCustomTShirt.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productCustomTShirt);

            AddProductTag(productCustomTShirt, "cool");
            AddProductTag(productCustomTShirt, "shirt");
            AddProductTag(productCustomTShirt, "apparel");

            var productLeviJeans = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Levi's 511 Jeans",
                Sku = "LV_511_JN",
                ShortDescription = "Levi's Faded Black 511 Jeans ",
                FullDescription = "<p>Between a skinny and straight fit, our 511&trade; slim fit jeans are cut close without being too restricting. Slim throughout the thigh and leg opening for a long and lean look.</p><ul><li>Slouch1y at top; sits below the waist</li><li>Slim through the leg, close at the thigh and straight to the ankle</li><li>Stretch for added comfort</li><li>Classic five-pocket styling</li><li>99% Cotton, 1% Spandex, 11.2 oz. - Imported</li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "levis-skinny-511-jeans",
                AllowCustomerReviews = true,
                Price = 43.5M,
                OldPrice = 55M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                TierPrices =
                {
                    new TierPrice
                    {
                        Quantity = 3,
                        Price = 40
                    },
                    new TierPrice
                    {
                        Quantity = 6,
                        Price = 38
                    },
                    new TierPrice
                    {
                        Quantity = 10,
                        Price = 35
                    }
                },
                HasTierPrices = true,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Clothing"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productLeviJeans);

            productLeviJeans.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_LeviJeans_1.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productLeviJeans.Name)),
                DisplayOrder = 1
            });
            productLeviJeans.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_LeviJeans_2.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productLeviJeans.Name)),
                DisplayOrder = 2
            });
            _productRepository.Insert(productLeviJeans);

            AddProductTag(productLeviJeans, "cool");
            AddProductTag(productLeviJeans, "jeans");
            AddProductTag(productLeviJeans, "apparel");

            var productObeyHat = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Obey Propaganda Hat",
                Sku = "OB_HAT_PR",
                ShortDescription = string.Empty,
                FullDescription = "<p>Printed poplin 5 panel camp hat with debossed leather patch and web closure</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "indiana-jones-shapeable-wool-hat",
                AllowCustomerReviews = true,
                Price = 30M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductAttributeMappings =
                {
                    new ProductAttributeMapping
                    {
                        ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "Size"),
                        AttributeControlType = AttributeControlType.DropdownList,
                        IsRequired = true,
                        ProductAttributeValues =
                        {
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Small",
                                DisplayOrder = 1
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Medium",
                                DisplayOrder = 2
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "Large",
                                DisplayOrder = 3
                            },
                            new ProductAttributeValue
                            {
                                AttributeValueType = AttributeValueType.Simple,
                                Name = "X-Large",
                                DisplayOrder = 4
                            }
                        }
                    }
                },
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Accessories"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productObeyHat);
            productObeyHat.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_hat.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productObeyHat.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productObeyHat);

            AddProductTag(productObeyHat, "apparel");
            AddProductTag(productObeyHat, "cool");

            var productBelt = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Reversible Horseferry Check Belt",
                Sku = "RH_CHK_BL",
                ShortDescription = "Reversible belt in Horseferry check with smooth leather trim",
                FullDescription = "<p>Reversible belt in Horseferry check with smooth leather trim</p><p>Leather lining, polished metal buckle</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "nike-golf-casual-belt",
                AllowCustomerReviews = true,
                Price = 45M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                ProductAvailabilityRangeId = productAvailabilityRange.Id,
                StockQuantity = 0,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Accessories"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productBelt);
            productBelt.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_Belt.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productBelt.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productBelt);

            var productSunglasses = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Ray Ban Aviator Sunglasses",
                Sku = "RB_AVR_SG",
                ShortDescription = "Aviator sunglasses are one of the first widely popularized styles of modern day sunwear.",
                FullDescription = "<p>Since 1937, Ray-Ban can genuinely claim the title as the world's leading sunglasses and optical eyewear brand. Combining the best of fashion and sports performance, the Ray-Ban line of Sunglasses delivers a truly classic style that will have you looking great today and for years to come.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "ray-ban-aviator-sunglasses-rb-3025",
                AllowCustomerReviews = true,
                Price = 25M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Accessories"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productSunglasses);
            productSunglasses.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_Sunglasses.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productSunglasses.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productSunglasses);

            AddProductTag(productSunglasses, "apparel");
            AddProductTag(productSunglasses, "cool");

            relatedProducts.AddRange(new[]
            {
                 new RelatedProduct
                {
                     ProductId1 = productAdidas.Id,
                     ProductId2 = productLeviJeans.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productAdidas.Id,
                     ProductId2 = productNikeFloral.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productAdidas.Id,
                     ProductId2 = productNikeZoom.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productAdidas.Id,
                     ProductId2 = productNikeTailwind.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productLeviJeans.Id,
                     ProductId2 = productAdidas.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productLeviJeans.Id,
                     ProductId2 = productNikeFloral.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productLeviJeans.Id,
                     ProductId2 = productNikeZoom.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productLeviJeans.Id,
                     ProductId2 = productNikeTailwind.Id
                },

                new RelatedProduct
                {
                     ProductId1 = productCustomTShirt.Id,
                     ProductId2 = productLeviJeans.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productCustomTShirt.Id,
                     ProductId2 = productNikeTailwind.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productCustomTShirt.Id,
                     ProductId2 = productOversizedWomenTShirt.Id
                },
                new RelatedProduct
                {
                     ProductId1 = productCustomTShirt.Id,
                     ProductId2 = productObeyHat.Id
                }
            });
        }

        protected virtual void InstallDigitalDownloads(ProductTemplate productTemplateSimple, List<Product> allProducts, string sampleImagesPath, IPictureService pictureService, List<RelatedProduct> relatedProducts, string sampleDownloadsPath, IDownloadService downloadService)
        {
            var downloadNightVision1 = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.ApplicationXZipCo,
                DownloadBinary = _fileProvider.ReadAllBytes(sampleDownloadsPath + "product_NightVision_1.zip"),
                Extension = ".zip",
                Filename = "Night_Vision_1",
                IsNew = true
            };
            downloadService.InsertDownload(downloadNightVision1);
            var downloadNightVision2 = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.TextPlain,
                DownloadBinary = _fileProvider.ReadAllBytes(sampleDownloadsPath + "product_NightVision_2.txt"),
                Extension = ".txt",
                Filename = "Night_Vision_1",
                IsNew = true
            };
            downloadService.InsertDownload(downloadNightVision2);
            var productNightVision = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Night Visions",
                Sku = "NIGHT_VSN",
                ShortDescription = "Night Visions is the debut studio album by American rock band Imagine Dragons.",
                FullDescription = "<p>Original Release Date: September 4, 2012</p><p>Release Date: September 4, 2012</p><p>Genre - Alternative rock, indie rock, electronic rock</p><p>Label - Interscope/KIDinaKORNER</p><p>Copyright: (C) 2011 Interscope Records</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "poker-face",
                AllowCustomerReviews = true,
                Price = 2.8M,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Downloadable Products").Id,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                IsDownload = true,
                DownloadId = downloadNightVision1.Id,
                DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                UnlimitedDownloads = true,
                HasUserAgreement = false,
                HasSampleDownload = true,
                SampleDownloadId = downloadNightVision2.Id,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Digital downloads"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productNightVision);
            productNightVision.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_NightVisions.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productNightVision.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productNightVision);

            AddProductTag(productNightVision, "awesome");
            AddProductTag(productNightVision, "digital");

            var downloadIfYouWait1 = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.ApplicationXZipCo,
                DownloadBinary = _fileProvider.ReadAllBytes(sampleDownloadsPath + "product_IfYouWait_1.zip"),
                Extension = ".zip",
                Filename = "If_You_Wait_1",
                IsNew = true
            };
            downloadService.InsertDownload(downloadIfYouWait1);
            var downloadIfYouWait2 = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.TextPlain,
                DownloadBinary = _fileProvider.ReadAllBytes(sampleDownloadsPath + "product_IfYouWait_2.txt"),
                Extension = ".txt",
                Filename = "If_You_Wait_1",
                IsNew = true
            };
            downloadService.InsertDownload(downloadIfYouWait2);
            var productIfYouWait = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "If You Wait (donation)",
                Sku = "IF_YOU_WT",
                ShortDescription = "If You Wait is the debut studio album by English indie pop band London Grammar",
                FullDescription = "<p>Original Release Date: September 6, 2013</p><p>Genre - Electronica, dream pop downtempo, pop</p><p>Label - Metal & Dust/Ministry of Sound</p><p>Producer - Tim Bran, Roy Kerr London, Grammar</p><p>Length - 43:22</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "single-ladies-put-a-ring-on-it",
                CustomerEntersPrice = true,
                MinimumCustomerEnteredPrice = 0.5M,
                MaximumCustomerEnteredPrice = 100M,
                AllowCustomerReviews = true,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Downloadable Products").Id,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                IsDownload = true,
                DownloadId = downloadIfYouWait1.Id,
                DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                UnlimitedDownloads = true,
                HasUserAgreement = false,
                HasSampleDownload = true,
                SampleDownloadId = downloadIfYouWait2.Id,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Digital downloads"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productIfYouWait);

            productIfYouWait.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_IfYouWait.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productIfYouWait.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productIfYouWait);

            AddProductTag(productIfYouWait, "digital");
            AddProductTag(productIfYouWait, "awesome");

            var downloadScienceAndFaith = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.ApplicationXZipCo,
                DownloadBinary = _fileProvider.ReadAllBytes(sampleDownloadsPath + "product_ScienceAndFaith_1.zip"),
                Extension = ".zip",
                Filename = "Science_And_Faith",
                IsNew = true
            };
            downloadService.InsertDownload(downloadScienceAndFaith);
            var productScienceAndFaith = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Science & Faith",
                Sku = "SCI_FAITH",
                ShortDescription = "Science & Faith is the second studio album by Irish pop rock band The Script.",
                FullDescription = "<p># Original Release Date: September 10, 2010<br /># Label: RCA, Epic/Phonogenic(America)<br /># Copyright: 2010 RCA Records.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "the-battle-of-los-angeles",
                AllowCustomerReviews = true,
                CustomerEntersPrice = true,
                MinimumCustomerEnteredPrice = 0.5M,
                MaximumCustomerEnteredPrice = 1000M,
                Price = decimal.Zero,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Downloadable Products").Id,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                IsDownload = true,
                DownloadId = downloadScienceAndFaith.Id,
                DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                UnlimitedDownloads = true,
                HasUserAgreement = false,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Digital downloads"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productScienceAndFaith);
            productScienceAndFaith.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_ScienceAndFaith.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productScienceAndFaith.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productScienceAndFaith);

            AddProductTag(productScienceAndFaith, "digital");
            AddProductTag(productScienceAndFaith, "awesome");

            relatedProducts.AddRange(new[]
            {
                new RelatedProduct
                {
                    ProductId1 = productIfYouWait.Id,
                    ProductId2 = productNightVision.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productIfYouWait.Id,
                    ProductId2 = productScienceAndFaith.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productNightVision.Id,
                    ProductId2 = productIfYouWait.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productNightVision.Id,
                    ProductId2 = productScienceAndFaith.Id
                }
            });
        }

        protected virtual void InstallBooks(ProductTemplate productTemplateSimple, List<Product> allProducts, string sampleImagesPath, IPictureService pictureService, List<RelatedProduct> relatedProducts)
        {
            var productFahrenheit = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Fahrenheit 451 by Ray Bradbury",
                Sku = "FR_451_RB",
                ShortDescription = "Fahrenheit 451 is a dystopian novel by Ray Bradbury published in 1953. It is regarded as one of his best works.",
                FullDescription = "<p>The novel presents a future American society where books are outlawed and firemen burn any that are found. The title refers to the temperature that Bradbury understood to be the autoignition point of paper.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "best-grilling-recipes",
                AllowCustomerReviews = true,
                Price = 27M,
                OldPrice = 30M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Books").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Books"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productFahrenheit);
            productFahrenheit.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_Fahrenheit451.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productFahrenheit.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productFahrenheit);

            AddProductTag(productFahrenheit, "awesome");
            AddProductTag(productFahrenheit, "book");
            AddProductTag(productFahrenheit, "nice");

            var productFirstPrizePies = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "First Prize Pies",
                Sku = "FIRST_PRP",
                ShortDescription = "Allison Kave made pies as a hobby, until one day her boyfriend convinced her to enter a Brooklyn pie-making contest. She won. In fact, her pies were such a hit that she turned pro.",
                FullDescription = "<p>First Prize Pies, a boutique, made-to-order pie business that originated on New York's Lower East Side, has become synonymous with tempting and unusual confections. For the home baker who is passionate about seasonal ingredients and loves a creative approach to recipes, First Prize Pies serves up 52 weeks of seasonal and eclectic pastries in an interesting pie-a-week format. Clear instructions, technical tips and creative encouragement guide novice bakers as well as pie mavens. With its nostalgia-evoking photos of homemade pies fresh out of the oven, First Prize Pies will be as giftable as it is practical.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "eatingwell-in-season",
                AllowCustomerReviews = true,
                Price = 51M,
                OldPrice = 67M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Books").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Books"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productFirstPrizePies);
            productFirstPrizePies.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_FirstPrizePies.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productFirstPrizePies.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productFirstPrizePies);

            AddProductTag(productFirstPrizePies, "book");

            var productPrideAndPrejudice = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Pride and Prejudice",
                Sku = "PRIDE_PRJ",
                ShortDescription = "Pride and Prejudice is a novel of manners by Jane Austen, first published in 1813.",
                FullDescription = "<p>Set in England in the early 19th century, Pride and Prejudice tells the story of Mr and Mrs Bennet's five unmarried daughters after the rich and eligible Mr Bingley and his status-conscious friend, Mr Darcy, have moved into their neighbourhood. While Bingley takes an immediate liking to the eldest Bennet daughter, Jane, Darcy has difficulty adapting to local society and repeatedly clashes with the second-eldest Bennet daughter, Elizabeth.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "the-best-skillet-recipes",
                AllowCustomerReviews = true,
                Price = 24M,
                OldPrice = 35M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Books").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Books"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productPrideAndPrejudice);
            productPrideAndPrejudice.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_PrideAndPrejudice.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productPrideAndPrejudice.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productPrideAndPrejudice);

            AddProductTag(productPrideAndPrejudice, "book");

            relatedProducts.AddRange(new[]
            {
                new RelatedProduct
                {
                    ProductId1 = productPrideAndPrejudice.Id,
                    ProductId2 = productFirstPrizePies.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productPrideAndPrejudice.Id,
                    ProductId2 = productFahrenheit.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productFirstPrizePies.Id,
                    ProductId2 = productPrideAndPrejudice.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productFirstPrizePies.Id,
                    ProductId2 = productFahrenheit.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productFahrenheit.Id,
                    ProductId2 = productFirstPrizePies.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productFahrenheit.Id,
                    ProductId2 = productPrideAndPrejudice.Id
                }
            });
        }

        protected virtual void InstallJewelry(ProductTemplate productTemplateSimple, List<Product> allProducts, string sampleImagesPath, IPictureService pictureService, List<RelatedProduct> relatedProducts)
        {
            var productElegantGemstoneNecklace = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Elegant Gemstone Necklace (rental)",
                Sku = "EG_GEM_NL",
                ShortDescription = "Classic and elegant gemstone necklace now available in our store",
                FullDescription = "<p>For those who like jewelry, creating their ownelegant jewelry from gemstone beads provides an economical way to incorporate genuine gemstones into your jewelry wardrobe. Manufacturers create beads from all kinds of precious gemstones and semi-precious gemstones, which are available in bead shops, craft stores, and online marketplaces.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "diamond-pave-earrings",
                AllowCustomerReviews = true,
                IsRental = true,
                RentalPriceLength = 1,
                RentalPricePeriod = RentalPricePeriod.Days,
                Price = 30M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Jewelry").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Jewelry"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productElegantGemstoneNecklace);
            productElegantGemstoneNecklace.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_GemstoneNecklaces.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productElegantGemstoneNecklace.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productElegantGemstoneNecklace);

            AddProductTag(productElegantGemstoneNecklace, "jewelry");
            AddProductTag(productElegantGemstoneNecklace, "awesome");

            var productFlowerGirlBracelet = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Flower Girl Bracelet",
                Sku = "FL_GIRL_B",
                ShortDescription = "Personalised Flower Braceled",
                FullDescription = "<p>This is a great gift for your flower girl to wear on your wedding day. A delicate bracelet that is made with silver plated soldered cable chain, gives this bracelet a dainty look for young wrist. A Swarovski heart, shown in Rose, hangs off a silver plated flower. Hanging alongside the heart is a silver plated heart charm with Flower Girl engraved on both sides. This is a great style for the younger flower girl.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "diamond-tennis-bracelet",
                AllowCustomerReviews = true,
                Price = 360M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Jewelry").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Jewelry"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productFlowerGirlBracelet);
            productFlowerGirlBracelet.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_FlowerBracelet.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productFlowerGirlBracelet.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productFlowerGirlBracelet);

            AddProductTag(productFlowerGirlBracelet, "awesome");
            AddProductTag(productFlowerGirlBracelet, "jewelry");

            var productEngagementRing = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Vintage Style Engagement Ring",
                Sku = "VS_ENG_RN",
                ShortDescription = "1.24 Carat (ctw) in 14K White Gold (Certified)",
                FullDescription = "<p>Dazzle her with this gleaming 14 karat white gold vintage proposal. A ravishing collection of 11 decadent diamonds come together to invigorate a superbly ornate gold shank. Total diamond weight on this antique style engagement ring equals 1 1/4 carat (ctw). Item includes diamond certificate.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "vintage-style-three-stone-diamond-engagement-ring",
                AllowCustomerReviews = true,
                Price = 2100M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Jewelry").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Jewelry"),
                        DisplayOrder = 1
                    }
                }
            };
            allProducts.Add(productEngagementRing);
            productEngagementRing.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_EngagementRing_1.jpg")), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName(productEngagementRing.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(productEngagementRing);

            AddProductTag(productEngagementRing, "jewelry");
            AddProductTag(productEngagementRing, "awesome");

            relatedProducts.AddRange(new[]
            {
                new RelatedProduct
                {
                    ProductId1 = productFlowerGirlBracelet.Id,
                    ProductId2 = productEngagementRing.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productFlowerGirlBracelet.Id,
                    ProductId2 = productElegantGemstoneNecklace.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productEngagementRing.Id,
                    ProductId2 = productFlowerGirlBracelet.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productEngagementRing.Id,
                    ProductId2 = productElegantGemstoneNecklace.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productElegantGemstoneNecklace.Id,
                    ProductId2 = productFlowerGirlBracelet.Id
                },
                new RelatedProduct
                {
                    ProductId1 = productElegantGemstoneNecklace.Id,
                    ProductId2 = productEngagementRing.Id
                }
            });
        }

        protected virtual void InstallGiftCards(ProductTemplate productTemplateSimple, List<Product> allProducts, string sampleImagesPath, IPictureService pictureService, List<RelatedProduct> relatedProducts, DeliveryDate deliveryDate)
        {
            var product25GiftCard = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "$25 Virtual Gift Card",
                Sku = "VG_CR_025",
                ShortDescription = "$25 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "25-virtual-gift-card",
                AllowCustomerReviews = true,
                Price = 25M,
                IsGiftCard = true,
                GiftCardType = GiftCardType.Virtual,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                Published = true,
                ShowOnHomepage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Gift Cards"),
                        DisplayOrder = 2
                    }
                }
            };
            allProducts.Add(product25GiftCard);
            product25GiftCard.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_25giftcart.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(product25GiftCard.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(product25GiftCard);

            AddProductTag(product25GiftCard, "nice");
            AddProductTag(product25GiftCard, "gift");

            var product50GiftCard = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "$50 Physical Gift Card",
                Sku = "PG_CR_050",
                ShortDescription = "$50 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "50-physical-gift-card",
                AllowCustomerReviews = true,
                Price = 50M,
                IsGiftCard = true,
                GiftCardType = GiftCardType.Physical,
                IsShipEnabled = true,
                IsFreeShipping = true,
                DeliveryDateId = deliveryDate.Id,
                Weight = 1,
                Length = 1,
                Width = 1,
                Height = 1,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Gift Cards"),
                        DisplayOrder = 3
                    }
                }
            };
            allProducts.Add(product50GiftCard);
            product50GiftCard.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_50giftcart.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(product50GiftCard.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(product50GiftCard);

            var product100GiftCard = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "$100 Physical Gift Card",
                Sku = "PG_CR_100",
                ShortDescription = "$100 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "100-physical-gift-card",
                AllowCustomerReviews = true,
                Price = 100M,
                IsGiftCard = true,
                GiftCardType = GiftCardType.Physical,
                IsShipEnabled = true,
                DeliveryDateId = deliveryDate.Id,
                Weight = 1,
                Length = 1,
                Width = 1,
                Height = 1,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Gift Cards"),
                        DisplayOrder = 4
                    }
                }
            };
            allProducts.Add(product100GiftCard);
            product100GiftCard.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "product_100giftcart.jpeg")), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(product100GiftCard.Name)),
                DisplayOrder = 1
            });
            _productRepository.Insert(product100GiftCard);
        }

        protected virtual void InstallProducts(string defaultUserEmail)
        {
            var productTemplateSimple = _productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple product");
            if (productTemplateSimple == null)
                throw new Exception("Simple product template could not be loaded");
            var productTemplateGrouped = _productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Grouped product (with variants)");
            if (productTemplateGrouped == null)
                throw new Exception("Grouped product template could not be loaded");

            //delivery date
            var deliveryDate = _deliveryDateRepository.Table.FirstOrDefault();
            if (deliveryDate == null)
                throw new Exception("No default deliveryDate could be loaded");

            //product availability range
            var productAvailabilityRange = _productAvailabilityRangeRepository.Table.FirstOrDefault();
            if (productAvailabilityRange == null)
                throw new Exception("No default product availability range could be loaded");

            //default customer/user
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();

            //downloads
            var downloadService = EngineContext.Current.Resolve<IDownloadService>();
            var sampleDownloadsPath = GetSamplesPath();

            //products
            var allProducts = new List<Product>();

            //related products
            var relatedProducts = new List<RelatedProduct>();

            //desktops, notebooks, software
            InstallComputers(productTemplateSimple, allProducts, sampleImagesPath, pictureService, relatedProducts);
            //camera & photo, cell phones, others
            InstallElectronics(productTemplateSimple, productTemplateGrouped, allProducts, sampleImagesPath, pictureService, relatedProducts);
            //shoes, clothing, accessories
            InstallApparel(productTemplateSimple, allProducts, sampleImagesPath, pictureService, relatedProducts, productAvailabilityRange);
            //digital downloads
            InstallDigitalDownloads(productTemplateSimple, allProducts, sampleImagesPath, pictureService, relatedProducts, sampleDownloadsPath, downloadService);
            //books
            InstallBooks(productTemplateSimple, allProducts, sampleImagesPath, pictureService, relatedProducts);
            //jewelry
            InstallJewelry(productTemplateSimple, allProducts, sampleImagesPath, pictureService, relatedProducts);
            //gift cards
            InstallGiftCards(productTemplateSimple, allProducts, sampleImagesPath, pictureService, relatedProducts, deliveryDate);

            //search engine names
            foreach (var product in allProducts)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = product.Id,
                    EntityName = nameof(Product),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = ValidateSeName(product, product.Name)
                });
            }

            //related products
            _relatedProductRepository.Insert(relatedProducts);

            //reviews
            var random = new Random();
            foreach (var product in allProducts)
            {
                if (product.ProductType != ProductType.SimpleProduct)
                    continue;

                //only 3 of 4 products will have reviews
                if (random.Next(4) == 3)
                    continue;

                //rating from 4 to 5
                var rating = random.Next(4, 6);
                product.ProductReviews.Add(new ProductReview
                {
                    CustomerId = defaultCustomer.Id,
                    ProductId = product.Id,
                    StoreId = defaultStore.Id,
                    IsApproved = true,
                    Title = "Some sample review",
                    ReviewText = $"This sample review is for the {product.Name}. I've been waiting for this product to be available. It is priced just right.",
                    //random (4 or 5)
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    CreatedOnUtc = DateTime.UtcNow
                });
                product.ApprovedRatingSum = rating;
                product.ApprovedTotalReviews = product.ProductReviews.Count;
            }

            _productRepository.Update(allProducts);

            //stock quantity history
            foreach (var product in allProducts)
            {
                if (product.StockQuantity > 0)
                    _stockQuantityHistoryRepository.Insert(new StockQuantityHistory
                    {
                        ProductId = product.Id,
                        WarehouseId = product.WarehouseId > 0 ? (int?)product.WarehouseId : null,
                        QuantityAdjustment = product.StockQuantity,
                        StockQuantity = product.StockQuantity,
                        Message = "The stock quantity has been edited",
                        CreatedOnUtc = DateTime.UtcNow
                    });
            }
        }

        protected virtual void InstallForums()
        {
            var forumGroup = new ForumGroup
            {
                Name = "General",
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            _forumGroupRepository.Insert(forumGroup);

            var newProductsForum = new Forum
            {
                ForumGroup = forumGroup,
                Name = "New Products",
                Description = "Discuss new products and industry trends",
                NumTopics = 0,
                NumPosts = 0,
                LastPostCustomerId = 0,
                LastPostTime = null,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _forumRepository.Insert(newProductsForum);

            var mobileDevicesForum = new Forum
            {
                ForumGroup = forumGroup,
                Name = "Mobile Devices Forum",
                Description = "Discuss the mobile phone market",
                NumTopics = 0,
                NumPosts = 0,
                LastPostCustomerId = 0,
                LastPostTime = null,
                DisplayOrder = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _forumRepository.Insert(mobileDevicesForum);

            var packagingShippingForum = new Forum
            {
                ForumGroup = forumGroup,
                Name = "Packaging & Shipping",
                Description = "Discuss packaging & shipping",
                NumTopics = 0,
                NumPosts = 0,
                LastPostTime = null,
                DisplayOrder = 20,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _forumRepository.Insert(packagingShippingForum);
        }

        protected virtual void InstallDiscounts()
        {
            var discounts = new List<Discount>
            {
                new Discount
                {
                    Name = "Sample discount with coupon code",
                    DiscountType = DiscountType.AssignedToSkus,
                    DiscountLimitation = DiscountLimitationType.Unlimited,
                    UsePercentage = false,
                    DiscountAmount = 10,
                    RequiresCouponCode = true,
                    CouponCode = "123"
                },
                new Discount
                {
                    Name = "'20% order total' discount",
                    DiscountType = DiscountType.AssignedToOrderTotal,
                    DiscountLimitation = DiscountLimitationType.Unlimited,
                    UsePercentage = true,
                    DiscountPercentage = 20,
                    StartDateUtc = new DateTime(2010, 1, 1),
                    EndDateUtc = new DateTime(2020, 1, 1),
                    RequiresCouponCode = true,
                    CouponCode = "456"
                }
            };
            _discountRepository.Insert(discounts);
        }

        protected virtual void InstallBlogPosts(string defaultUserEmail)
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();

            var blogPosts = new List<BlogPost>
            {
                new BlogPost
                {
                    AllowComments = true,
                    Language = defaultLanguage,
                    Title = "How a blog can help your growing e-Commerce business",
                    BodyOverview = "<p>When you start an online business, your main aim is to sell the products, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Commerce business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p>",
                    Body = "<p>When you start an online business, your main aim is to sell the products, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Commerce business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p><h3>1) Blog is useful in educating your customers</h3><p>Blogging is one of the best way by which you can educate your customers about your products/services that you offer. This helps you as a business owner to bring more value to your brand. When you provide useful information to the customers about your products, they are more likely to buy products from you. You can use your blog for providing tutorials in regard to the use of your products.</p><p><strong>For example:</strong> If you have an online store that offers computer parts. You can write tutorials about how to build a computer or how to make your computer&rsquo;s performance better. While talking about these things, you can mention products in the tutorials and provide link to your products within the blog post from your website. Your potential customers might get different ideas of using your product and will likely to buy products from your online store.</p><h3>2) Blog helps your business in Search Engine Optimization (SEO)</h3><p>Blog posts create more internal links to your website which helps a lot in SEO. Blog is a great way to have quality content on your website related to your products/services which is indexed by all major search engines like Google, Bing and Yahoo. The more original content you write in your blog post, the better ranking you will get in search engines. SEO is an on-going process and posting blog posts regularly keeps your site active all the time which is beneficial when it comes to search engine optimization.</p><p><strong>For example:</strong> Let&rsquo;s say you sell &ldquo;Sony Television Model XYZ&rdquo; and you regularly publish blog posts about your product. Now, whenever someone searches for &ldquo;Sony Television Model XYZ&rdquo;, Google will crawl on your website knowing that you have something to do with this particular product. Hence, your website will show up on the search result page whenever this item is being searched.</p><h3>3) Blog helps in boosting your sales by convincing the potential customers to buy</h3><p>If you own an online business, there are so many ways you can share different stories with your audience in regard your products/services that you offer. Talk about how you started your business, share stories that educate your audience about what&rsquo;s new in your industry, share stories about how your product/service was beneficial to someone or share anything that you think your audience might find interesting (it does not have to be related to your product). This kind of blogging shows that you are an expert in your industry and interested in educating your audience. It sets you apart in the competitive market. This gives you an opportunity to showcase your expertise by educating the visitors and it can turn your audience into buyers.</p><p><strong>Fun Fact:</strong> Did you know that 92% of companies who decided to blog acquired customers through their blog?</p><p><a href=\"https://www.nopcommerce.com/\">nopCommerce</a> is great e-Commerce solution that also offers a variety of CMS features including blog. A store owner has full access for managing the blog posts and related comments.</p>",
                    Tags = "e-commerce, blog, moey",
                    CreatedOnUtc = DateTime.UtcNow
                },
                new BlogPost
                {
                    AllowComments = true,
                    Language = defaultLanguage,
                    Title = "Why your online store needs a wish list",
                    BodyOverview = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your customers to be able to save products in a wish list so that they review or buy them later? Would you like your customers to be able to share their wish list with friends and family for gift giving?</p><p>Offering your customers a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their customers in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p>",
                    Body = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your customers to be able to save products in a wish list so that they review or buy them later? Would you like your customers to be able to share their wish list with friends and family for gift giving?</p><p>Offering your customers a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their customers in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p><p>Does every e-Commerce store needs a wish list? The answer to this question in most cases is yes, because of the following reasons:</p><p><strong>Understanding the needs of your customers</strong> - A wish list is a great way to know what is in your customer&rsquo;s mind. Try to think the purchase history as a small portion of the customer&rsquo;s preferences. But, the wish list is like a wide open door that can give any online business a lot of valuable information about their customer and what they like or desire.</p><p><strong>Shoppers like to share their wish list with friends and family</strong> - Providing your customers a way to email their wish list to their friends and family is a pleasant way to make online shopping enjoyable for the shoppers. It is always a good idea to make the wish list sharable by a unique link so that it can be easily shared though different channels like email or on social media sites.</p><p><strong>Wish list can be a great marketing tool</strong> &ndash; Another way to look at wish list is a great marketing tool because it is extremely targeted and the recipients are always motivated to use it. For example: when your younger brother tells you that his wish list is on a certain e-Commerce store. What is the first thing you are going to do? You are most likely to visit the e-Commerce store, check out the wish list and end up buying something for your younger brother.</p><p>So, how a wish list is a marketing tool? The reason is quite simple, it introduce your online store to new customers just how it is explained in the above example.</p><p><strong>Encourage customers to return to the store site</strong> &ndash; Having a feature of wish list on the store site can increase the return traffic because it encourages customers to come back and buy later. Allowing the customers to save the wish list to their online accounts gives them a reason return to the store site and login to the account at any time to view or edit the wish list items.</p><p><strong>Wish list can be used for gifts for different occasions like weddings or birthdays. So, what kind of benefits a gift-giver gets from a wish list?</strong></p><ul><li>It gives them a surety that they didn&rsquo;t buy a wrong gift</li><li>It guarantees that the recipient will like the gift</li><li>It avoids any awkward moments when the recipient unwraps the gift and as a gift-giver you got something that the recipient do not want</li></ul><p><strong>Wish list is a great feature to have on a store site &ndash; So, what kind of benefits a business owner gets from a wish list</strong></p><ul><li>It is a great way to advertise an online store as many people do prefer to shop where their friend or family shop online</li><li>It allows the current customers to return to the store site and open doors for the new customers</li><li>It allows store admins to track what&rsquo;s in customers wish list and run promotions accordingly to target specific customer segments</li></ul><p><a href=\"https://www.nopcommerce.com/\">nopCommerce</a> offers the feature of wish list that allows customers to create a list of products that they desire or planning to buy in future.</p>",
                    Tags = "e-commerce, nopCommerce, sample tag, money",
                    CreatedOnUtc = DateTime.UtcNow.AddSeconds(1)
                }
            };
            _blogPostRepository.Insert(blogPosts);

            //search engine names
            foreach (var blogPost in blogPosts)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = blogPost.Id,
                    EntityName = nameof(BlogPost),
                    LanguageId = blogPost.LanguageId,
                    IsActive = true,
                    Slug = ValidateSeName(blogPost, blogPost.Title)
                });
            }

            //comments
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            foreach (var blogPost in blogPosts)
            {
                blogPost.BlogComments.Add(new BlogComment
                {
                    BlogPostId = blogPost.Id,
                    CustomerId = defaultCustomer.Id,
                    CommentText = "This is a sample comment for this blog post",
                    IsApproved = true,
                    StoreId = defaultStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }

            _blogPostRepository.Update(blogPosts);
        }

        protected virtual void InstallNews(string defaultUserEmail)
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();

            var news = new List<NewsItem>
            {
                new NewsItem
                {
                    AllowComments = true,
                    Language = defaultLanguage,
                    Title = "About nopCommerce",
                    Short = "It's stable and highly usable. From downloads to documentation, www.nopCommerce.com offers a comprehensive base of information, resources, and support to the nopCommerce community.",
                    Full = "<p>For full feature list go to <a href=\"https://www.nopCommerce.com\">nopCommerce.com</a></p><p>Providing outstanding custom search engine optimization, web development services and e-commerce development solutions to our clients at a fair price in a professional manner.</p>",
                    Published = true,
                    CreatedOnUtc = DateTime.UtcNow
                },
                new NewsItem
                {
                    AllowComments = true,
                    Language = defaultLanguage,
                    Title = "nopCommerce new release!",
                    Short = "nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included! nopCommerce is a fully customizable shopping cart",
                    Full = "<p>nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!</p>",
                    Published = true,
                    CreatedOnUtc = DateTime.UtcNow.AddSeconds(1)
                },
                new NewsItem
                {
                    AllowComments = true,
                    Language = defaultLanguage,
                    Title = "New online store is open!",
                    Short = "The new nopCommerce store is open now! We are very excited to offer our new range of products. We will be constantly adding to our range so please register on our site.",
                    Full = "<p>Our online store is officially up and running. Stock up for the holiday season! We have a great selection of items. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.</p><p>All shipping is worldwide and will leave the same day an order is placed! Happy Shopping and spread the word!!</p>",
                    Published = true,
                    CreatedOnUtc = DateTime.UtcNow.AddSeconds(2)
                }
            };
            _newsItemRepository.Insert(news);

            //search engine names
            foreach (var newsItem in news)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = newsItem.Id,
                    EntityName = nameof(NewsItem),
                    LanguageId = newsItem.LanguageId,
                    IsActive = true,
                    Slug = ValidateSeName(newsItem, newsItem.Title)
                });
            }

            //comments
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            foreach (var newsItem in news)
            {
                newsItem.NewsComments.Add(new NewsComment
                {
                    NewsItemId = newsItem.Id,
                    CustomerId = defaultCustomer.Id,
                    CommentTitle = "Sample comment title",
                    CommentText = "This is a sample comment...",
                    IsApproved = true,
                    StoreId = defaultStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }

            _newsItemRepository.Update(news);
        }

        protected virtual void InstallPolls()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();
            var poll1 = new Poll
            {
                Language = defaultLanguage,
                Name = "Do you like nopCommerce?",
                SystemKeyword = string.Empty,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 1
            };
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Excellent",
                DisplayOrder = 1
            });
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Good",
                DisplayOrder = 2
            });
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Poor",
                DisplayOrder = 3
            });
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Very bad",
                DisplayOrder = 4
            });
            _pollRepository.Insert(poll1);
        }

        protected virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>
            {
                //admin area activities
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttribute",
                    Enabled = true,
                    Name = "Add a new address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttributeValue",
                    Enabled = true,
                    Name = "Add a new address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAffiliate",
                    Enabled = true,
                    Name = "Add a new affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewBlogPost",
                    Enabled = true,
                    Name = "Add a new blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCampaign",
                    Enabled = true,
                    Name = "Add a new campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCategory",
                    Enabled = true,
                    Name = "Add a new category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCheckoutAttribute",
                    Enabled = true,
                    Name = "Add a new checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCountry",
                    Enabled = true,
                    Name = "Add a new country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCurrency",
                    Enabled = true,
                    Name = "Add a new currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomer",
                    Enabled = true,
                    Name = "Add a new customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerAttribute",
                    Enabled = true,
                    Name = "Add a new customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerAttributeValue",
                    Enabled = true,
                    Name = "Add a new customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerRole",
                    Enabled = true,
                    Name = "Add a new customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewDiscount",
                    Enabled = true,
                    Name = "Add a new discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewEmailAccount",
                    Enabled = true,
                    Name = "Add a new email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewGiftCard",
                    Enabled = true,
                    Name = "Add a new gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewLanguage",
                    Enabled = true,
                    Name = "Add a new language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewManufacturer",
                    Enabled = true,
                    Name = "Add a new manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureDimension",
                    Enabled = true,
                    Name = "Add a new measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureWeight",
                    Enabled = true,
                    Name = "Add a new measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewNews",
                    Enabled = true,
                    Name = "Add a new news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewProduct",
                    Enabled = true,
                    Name = "Add a new product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewProductAttribute",
                    Enabled = true,
                    Name = "Add a new product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSetting",
                    Enabled = true,
                    Name = "Add a new setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSpecAttribute",
                    Enabled = true,
                    Name = "Add a new specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStateProvince",
                    Enabled = true,
                    Name = "Add a new state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStore",
                    Enabled = true,
                    Name = "Add a new store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTopic",
                    Enabled = true,
                    Name = "Add a new topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewReviewType",
                    Enabled = true,
                    Name = "Add a new review type"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendor",
                    Enabled = true,
                    Name = "Add a new vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendorAttribute",
                    Enabled = true,
                    Name = "Add a new vendor attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendorAttributeValue",
                    Enabled = true,
                    Name = "Add a new vendor attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWarehouse",
                    Enabled = true,
                    Name = "Add a new warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWidget",
                    Enabled = true,
                    Name = "Add a new widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttribute",
                    Enabled = true,
                    Name = "Delete an address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttributeValue",
                    Enabled = true,
                    Name = "Delete an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAffiliate",
                    Enabled = true,
                    Name = "Delete an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPost",
                    Enabled = true,
                    Name = "Delete a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPostComment",
                    Enabled = true,
                    Name = "Delete a blog post comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCampaign",
                    Enabled = true,
                    Name = "Delete a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCategory",
                    Enabled = true,
                    Name = "Delete category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCheckoutAttribute",
                    Enabled = true,
                    Name = "Delete a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCountry",
                    Enabled = true,
                    Name = "Delete a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCurrency",
                    Enabled = true,
                    Name = "Delete a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomer",
                    Enabled = true,
                    Name = "Delete a customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerAttribute",
                    Enabled = true,
                    Name = "Delete a customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerAttributeValue",
                    Enabled = true,
                    Name = "Delete a customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerRole",
                    Enabled = true,
                    Name = "Delete a customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteDiscount",
                    Enabled = true,
                    Name = "Delete a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteEmailAccount",
                    Enabled = true,
                    Name = "Delete an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteGiftCard",
                    Enabled = true,
                    Name = "Delete a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteLanguage",
                    Enabled = true,
                    Name = "Delete a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteManufacturer",
                    Enabled = true,
                    Name = "Delete a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureDimension",
                    Enabled = true,
                    Name = "Delete a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureWeight",
                    Enabled = true,
                    Name = "Delete a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMessageTemplate",
                    Enabled = true,
                    Name = "Delete a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteNews",
                    Enabled = true,
                    Name = "Delete a news"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "DeleteNewsComment",
                    Enabled = true,
                    Name = "Delete a news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteOrder",
                    Enabled = true,
                    Name = "Delete an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeletePlugin",
                    Enabled = true,
                    Name = "Delete a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProduct",
                    Enabled = true,
                    Name = "Delete a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProductAttribute",
                    Enabled = true,
                    Name = "Delete a product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProductReview",
                    Enabled = true,
                    Name = "Delete a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteReturnRequest",
                    Enabled = true,
                    Name = "Delete a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteReviewType",
                    Enabled = true,
                    Name = "Delete a review type"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSetting",
                    Enabled = true,
                    Name = "Delete a setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSpecAttribute",
                    Enabled = true,
                    Name = "Delete a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStateProvince",
                    Enabled = true,
                    Name = "Delete a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStore",
                    Enabled = true,
                    Name = "Delete a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSystemLog",
                    Enabled = true,
                    Name = "Delete system log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTopic",
                    Enabled = true,
                    Name = "Delete a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendor",
                    Enabled = true,
                    Name = "Delete a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendorAttribute",
                    Enabled = true,
                    Name = "Delete a vendor attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendorAttributeValue",
                    Enabled = true,
                    Name = "Delete a vendor attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWarehouse",
                    Enabled = true,
                    Name = "Delete a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWidget",
                    Enabled = true,
                    Name = "Delete a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttribute",
                    Enabled = true,
                    Name = "Edit an address attribute"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttributeValue",
                    Enabled = true,
                    Name = "Edit an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAffiliate",
                    Enabled = true,
                    Name = "Edit an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditBlogPost",
                    Enabled = true,
                    Name = "Edit a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCampaign",
                    Enabled = true,
                    Name = "Edit a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCategory",
                    Enabled = true,
                    Name = "Edit category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCheckoutAttribute",
                    Enabled = true,
                    Name = "Edit a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCountry",
                    Enabled = true,
                    Name = "Edit a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCurrency",
                    Enabled = true,
                    Name = "Edit a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomer",
                    Enabled = true,
                    Name = "Edit a customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerAttribute",
                    Enabled = true,
                    Name = "Edit a customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerAttributeValue",
                    Enabled = true,
                    Name = "Edit a customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerRole",
                    Enabled = true,
                    Name = "Edit a customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditDiscount",
                    Enabled = true,
                    Name = "Edit a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditEmailAccount",
                    Enabled = true,
                    Name = "Edit an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditGiftCard",
                    Enabled = true,
                    Name = "Edit a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditLanguage",
                    Enabled = true,
                    Name = "Edit a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditManufacturer",
                    Enabled = true,
                    Name = "Edit a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureDimension",
                    Enabled = true,
                    Name = "Edit a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureWeight",
                    Enabled = true,
                    Name = "Edit a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMessageTemplate",
                    Enabled = true,
                    Name = "Edit a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditNews",
                    Enabled = true,
                    Name = "Edit a news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditOrder",
                    Enabled = true,
                    Name = "Edit an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPlugin",
                    Enabled = true,
                    Name = "Edit a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProduct",
                    Enabled = true,
                    Name = "Edit a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProductAttribute",
                    Enabled = true,
                    Name = "Edit a product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProductReview",
                    Enabled = true,
                    Name = "Edit a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPromotionProviders",
                    Enabled = true,
                    Name = "Edit promotion providers"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditReturnRequest",
                    Enabled = true,
                    Name = "Edit a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditReviewType",
                    Enabled = true,
                    Name = "Edit a review type"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSettings",
                    Enabled = true,
                    Name = "Edit setting(s)"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStateProvince",
                    Enabled = true,
                    Name = "Edit a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStore",
                    Enabled = true,
                    Name = "Edit a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTask",
                    Enabled = true,
                    Name = "Edit a task"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSpecAttribute",
                    Enabled = true,
                    Name = "Edit a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendor",
                    Enabled = true,
                    Name = "Edit a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendorAttribute",
                    Enabled = true,
                    Name = "Edit a vendor attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendorAttributeValue",
                    Enabled = true,
                    Name = "Edit a vendor attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWarehouse",
                    Enabled = true,
                    Name = "Edit a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTopic",
                    Enabled = true,
                    Name = "Edit a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWidget",
                    Enabled = true,
                    Name = "Edit a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Started",
                    Enabled = true,
                    Name = "Customer impersonation session. Started"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Finished",
                    Enabled = true,
                    Name = "Customer impersonation session. Finished"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportCategories",
                    Enabled = true,
                    Name = "Categories were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportManufacturers",
                    Enabled = true,
                    Name = "Manufacturers were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportProducts",
                    Enabled = true,
                    Name = "Products were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportStates",
                    Enabled = true,
                    Name = "States were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "InstallNewPlugin",
                    Enabled = true,
                    Name = "Install a new plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UninstallPlugin",
                    Enabled = true,
                    Name = "Uninstall a plugin"
                },
                //public store activities
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewCategory",
                    Enabled = false,
                    Name = "Public store. View a category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewManufacturer",
                    Enabled = false,
                    Name = "Public store. View a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewProduct",
                    Enabled = false,
                    Name = "Public store. View a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.PlaceOrder",
                    Enabled = false,
                    Name = "Public store. Place an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.SendPM",
                    Enabled = false,
                    Name = "Public store. Send PM"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ContactUs",
                    Enabled = false,
                    Name = "Public store. Use contact us form"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToCompareList",
                    Enabled = false,
                    Name = "Public store. Add to compare list"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToShoppingCart",
                    Enabled = false,
                    Name = "Public store. Add to shopping cart"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToWishlist",
                    Enabled = false,
                    Name = "Public store. Add to wishlist"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Login",
                    Enabled = false,
                    Name = "Public store. Login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Logout",
                    Enabled = false,
                    Name = "Public store. Logout"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddProductReview",
                    Enabled = false,
                    Name = "Public store. Add product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddNewsComment",
                    Enabled = false,
                    Name = "Public store. Add news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddBlogComment",
                    Enabled = false,
                    Name = "Public store. Add blog comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumTopic",
                    Enabled = false,
                    Name = "Public store. Add forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumTopic",
                    Enabled = false,
                    Name = "Public store. Edit forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumTopic",
                    Enabled = false,
                    Name = "Public store. Delete forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumPost",
                    Enabled = false,
                    Name = "Public store. Add forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumPost",
                    Enabled = false,
                    Name = "Public store. Edit forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumPost",
                    Enabled = false,
                    Name = "Public store. Delete forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UploadNewPlugin",
                    Enabled = true,
                    Name = "Upload a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UploadNewTheme",
                    Enabled = true,
                    Name = "Upload a theme"
                }
            };
            _activityLogTypeRepository.Insert(activityLogTypes);
        }

        protected virtual void InstallProductTemplates()
        {
            var productTemplates = new List<ProductTemplate>
            {
                new ProductTemplate
                {
                    Name = "Simple product",
                    ViewPath = "ProductTemplate.Simple",
                    DisplayOrder = 10,
                    IgnoredProductTypes = ((int)ProductType.GroupedProduct).ToString()
                },
                new ProductTemplate
                {
                    Name = "Grouped product (with variants)",
                    ViewPath = "ProductTemplate.Grouped",
                    DisplayOrder = 100,
                    IgnoredProductTypes = ((int)ProductType.SimpleProduct).ToString()
                }
            };
            _productTemplateRepository.Insert(productTemplates);
        }

        protected virtual void InstallCategoryTemplates()
        {
            var categoryTemplates = new List<CategoryTemplate>
            {
                new CategoryTemplate
                {
                    Name = "Products in Grid or Lines",
                    ViewPath = "CategoryTemplate.ProductsInGridOrLines",
                    DisplayOrder = 1
                }
            };
            _categoryTemplateRepository.Insert(categoryTemplates);
        }

        protected virtual void InstallManufacturerTemplates()
        {
            var manufacturerTemplates = new List<ManufacturerTemplate>
            {
                new ManufacturerTemplate
                {
                    Name = "Products in Grid or Lines",
                    ViewPath = "ManufacturerTemplate.ProductsInGridOrLines",
                    DisplayOrder = 1
                }
            };
            _manufacturerTemplateRepository.Insert(manufacturerTemplates);
        }

        protected virtual void InstallTopicTemplates()
        {
            var topicTemplates = new List<TopicTemplate>
            {
                new TopicTemplate
                {
                    Name = "Default template",
                    ViewPath = "TopicDetails",
                    DisplayOrder = 1
                }
            };
            _topicTemplateRepository.Insert(topicTemplates);
        }

        protected virtual void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "Nop.Services.Messages.QueuedMessagesSendTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "Nop.Services.Common.KeepAliveTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "Nop.Services.Customers.DeleteGuestsTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "Nop.Services.Caching.ClearCacheTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Nop.Services.Logging.ClearLogTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Update currency exchange rates",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Nop.Services.Directory.UpdateExchangeRateTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false
                }
            };

            _scheduleTaskRepository.Insert(tasks);
        }

        protected virtual void InstallReturnRequestReasons()
        {
            var returnRequestReasons = new List<ReturnRequestReason>
            {
                new ReturnRequestReason
                {
                    Name = "Received Wrong Product",
                    DisplayOrder = 1
                },
                new ReturnRequestReason
                {
                    Name = "Wrong Product Ordered",
                    DisplayOrder = 2
                },
                new ReturnRequestReason
                {
                    Name = "There Was A Problem With The Product",
                    DisplayOrder = 3
                }
            };
            _returnRequestReasonRepository.Insert(returnRequestReasons);
        }

        protected virtual void InstallReturnRequestActions()
        {
            var returnRequestActions = new List<ReturnRequestAction>
            {
                new ReturnRequestAction
                {
                    Name = "Repair",
                    DisplayOrder = 1
                },
                new ReturnRequestAction
                {
                    Name = "Replacement",
                    DisplayOrder = 2
                },
                new ReturnRequestAction
                {
                    Name = "Store Credit",
                    DisplayOrder = 3
                }
            };
            _returnRequestActionRepository.Insert(returnRequestActions);
        }

        protected virtual void InstallWarehouses()
        {
            var warehouse1address = new Address
            {
                Address1 = "21 West 52nd Street",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow
            };
            _addressRepository.Insert(warehouse1address);
            var warehouse2address = new Address
            {
                Address1 = "300 South Spring Stree",
                City = "Los Angeles",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "90013",
                CreatedOnUtc = DateTime.UtcNow
            };
            _addressRepository.Insert(warehouse2address);
            var warehouses = new List<Warehouse>
            {
                new Warehouse
                {
                    Name = "Warehouse 1 (New York)",
                    AddressId = warehouse1address.Id
                },
                new Warehouse
                {
                    Name = "Warehouse 2 (Los Angeles)",
                    AddressId = warehouse2address.Id
                }
            };

            _warehouseRepository.Insert(warehouses);
        }

        protected virtual void InstallVendors()
        {
            var vendors = new List<Vendor>
            {
                new Vendor
                {
                    Name = "Vendor 1",
                    Email = "vendor1email@gmail.com",
                    Description = "Some description...",
                    AdminComment = string.Empty,
                    PictureId = 0,
                    Active = true,
                    DisplayOrder = 1,
                    PageSize = 6,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = "6, 3, 9, 18"
                },
                new Vendor
                {
                    Name = "Vendor 2",
                    Email = "vendor2email@gmail.com",
                    Description = "Some description...",
                    AdminComment = string.Empty,
                    PictureId = 0,
                    Active = true,
                    DisplayOrder = 2,
                    PageSize = 6,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = "6, 3, 9, 18"
                }
            };

            _vendorRepository.Insert(vendors);

            //search engine names
            foreach (var vendor in vendors)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = vendor.Id,
                    EntityName = nameof(Vendor),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = ValidateSeName(vendor, vendor.Name)
                });
            }
        }

        protected virtual void InstallAffiliates()
        {
            var affiliateAddress = new Address
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "affiliate_email@gmail.com",
                Company = "Company name here...",
                City = "New York",
                Address1 = "21 West 52nd Street",
                ZipPostalCode = "10021",
                PhoneNumber = "123456789",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                CreatedOnUtc = DateTime.UtcNow
            };
            _addressRepository.Insert(affiliateAddress);
            var affilate = new Affiliate
            {
                Active = true,
                Address = affiliateAddress
            };
            _affiliateRepository.Insert(affilate);
        }

        private void AddProductTag(Product product, string tag)
        {
            var productTag = _productTagRepository.Table.FirstOrDefault(pt => pt.Name == tag);
            var exist = productTag != null;
            if (productTag == null)
            {
                productTag = new ProductTag
                {
                    Name = tag
                };
            }
            product.ProductProductTagMappings.Add(new ProductProductTagMapping { ProductTag = productTag });
            _productRepository.Update(product);
            if (!exist)
            {
                //search engine name
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = productTag.Id,
                    EntityName = nameof(ProductTag),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = ValidateSeName(productTag, productTag.Name)
                });
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Install required data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <param name="defaultUserPassword">Default user password</param>
        public virtual void InstallRequiredData(string defaultUserEmail, string defaultUserPassword)
        {
            InstallStores();
            InstallMeasures();
            InstallTaxCategories();
            InstallLanguages();
            InstallCurrencies();
            InstallCountriesAndStates();
            InstallShippingMethods();
            InstallDeliveryDates();
            InstallProductAvailabilityRanges();
            InstallEmailAccounts();
            InstallMessageTemplates();
            InstallTopicTemplates();
            InstallSettings();
            InstallCustomersAndUsers(defaultUserEmail, defaultUserPassword);
            InstallTopics();
            InstallLocaleResources();
            InstallActivityLogTypes();
            InstallProductTemplates();
            InstallCategoryTemplates();
            InstallManufacturerTemplates();
            InstallScheduleTasks();
            InstallReturnRequestReasons();
            InstallReturnRequestActions();
        }

        /// <summary>
        /// Install sample data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        public virtual void InstallSampleData(string defaultUserEmail)
        {
            InstallSampleCustomers();
            InstallCheckoutAttributes();
            InstallSpecificationAttributes();
            InstallProductAttributes();
            InstallCategories();
            InstallManufacturers();
            InstallProducts(defaultUserEmail);
            InstallForums();
            InstallDiscounts();
            InstallBlogPosts(defaultUserEmail);
            InstallNews(defaultUserEmail);
            InstallPolls();
            InstallWarehouses();
            InstallVendors();
            InstallAffiliates();
            InstallOrders();
            InstallActivityLog(defaultUserEmail);
            InstallSearchTerms();

            var settingService = EngineContext.Current.Resolve<ISettingService>();

            settingService.SaveSetting(new DisplayDefaultMenuItemSettings
            {
                DisplayHomepageMenuItem = false,
                DisplayNewProductsMenuItem = false,
                DisplayProductSearchMenuItem = false,
                DisplayCustomerInfoMenuItem = false,
                DisplayBlogMenuItem = false,
                DisplayForumsMenuItem = false,
                DisplayContactUsMenuItem = false
            });
        }

        #endregion
    }
}