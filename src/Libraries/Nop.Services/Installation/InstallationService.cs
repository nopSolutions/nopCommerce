
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Core.IO;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Helpers;

namespace Nop.Services.Installation
{
    public partial class InstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<MeasureDimension> _measureDimensionRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariant> _productVariantRepository;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public InstallationService(IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IRepository<Language> languageRepository,
            IRepository<Currency> currencyRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<User> userRepository,
            IRepository<Category> categoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IRepository<ProductVariant> productVariantRepository,
            ISettingService settingService)
        {
            this._measureDimensionRepository = measureDimensionRepository;
            this._measureWeightRepository = measureWeightRepository;
            this._taxCategoryRepository = taxCategoryRepository;
            this._languageRepository = languageRepository;
            this._currencyRepository = currencyRepository;
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._userRepository = userRepository;

            this._categoryRepository = categoryRepository;
            this._manufacturerRepository = manufacturerRepository;
            this._productRepository = productRepository;
            this._productVariantRepository = productVariantRepository;

            this._settingService = settingService;
        }

        #endregion

        #region Classes

        private class LocaleStringResourceParent : LocaleStringResource
        {
            public LocaleStringResourceParent(XmlNode localStringResource, string nameSpace = "")
            {
                Namespace = nameSpace;
                var resNameAttribute = localStringResource.Attributes["Name"];
                var resValueNode = localStringResource.SelectSingleNode("Value");

                if (resNameAttribute == null)
                {
                    throw new NopException("All language resources must have an attribute Name=\"Value\".");
                }
                var resName = resNameAttribute.Value.Trim();
                if (string.IsNullOrEmpty(resName))
                {
                    throw new NopException("All languages resource attributes 'Name' must have a value.'");
                }
                ResourceName = resName;
               
                if (resValueNode == null || string.IsNullOrEmpty(resValueNode.InnerText.Trim()))
                {
                    IsPersistable = false;
                }
                else
                {
                    IsPersistable = true;
                    ResourceValue = resValueNode.InnerText.Trim();
                }

                foreach (XmlNode childResource in localStringResource.SelectNodes("Children/LocaleResource"))
                {
                    ChildLocaleStringResources.Add(new LocaleStringResourceParent(childResource, NameWithNamespace));
                }
            }
            public string Namespace { get; set; }
            public IList<LocaleStringResourceParent> ChildLocaleStringResources = new List<LocaleStringResourceParent>();

            public bool IsPersistable { get; set; }

            public string NameWithNamespace
            {
                get
                {
                    var newNamespace = Namespace;
                    if (!string.IsNullOrEmpty(newNamespace))
                    {
                        newNamespace += ".";
                    }
                    return newNamespace + ResourceName;
                }
            }
        }

        private class ComparisonComparer<T> : IComparer<T>, IComparer
        {
            private readonly Comparison<T> _comparison;

            public ComparisonComparer(Comparison<T> comparison)
            {
                _comparison = comparison;
            }

            public int Compare(T x, T y)
            {
                return _comparison(x, y);
            }

            public int Compare(object o1, object o2)
            {
                return _comparison((T)o1, (T)o2);
            }
        }

        #endregion

        #region Utilities

        private void RecursivelyWriteResource(LocaleStringResourceParent resource, XmlWriter writer)
        {
            //The value isn't actually used, but the name is used to create a namespace.
            if (resource.IsPersistable)
            {
                writer.WriteStartElement("LocaleResource", "");

                writer.WriteStartAttribute("Name", "");
                writer.WriteString(resource.NameWithNamespace);
                writer.WriteEndAttribute();

                writer.WriteStartElement("Value", "");
                writer.WriteString(resource.ResourceValue);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            foreach (var child in resource.ChildLocaleStringResources)
            {
                RecursivelyWriteResource(child, writer);
            }

        }

        private void RecursivelySortChildrenResource(LocaleStringResourceParent resource)
        {
            ArrayList.Adapter((IList)resource.ChildLocaleStringResources).Sort(new InstallationService.ComparisonComparer<LocaleStringResourceParent>((x1, x2) => x1.ResourceName.CompareTo(x2.ResourceName)));
            
            foreach (var child in resource.ChildLocaleStringResources)
            {
                RecursivelySortChildrenResource(child);
            }

        }

        private void AddLocaleResources(Language language1)
        {
            //insert default sting resources (temporary solution). Requires some performance optimization
            //TODO find better way to insert default locale string resources
            //TODO use IStorageProvider instead of HostingEnvironment.MapPath
            foreach (var filePath in System.IO.Directory.EnumerateFiles(HostingEnvironment.MapPath("~/App_Data/"), "*.nopres.xml"))
            {
                //read and parse original file with resources (with <Children> elements)

                var originalXmlDocument = new XmlDocument();
                originalXmlDocument.Load(filePath);

                var resources = new List<LocaleStringResourceParent>();

                foreach (XmlNode resNode in originalXmlDocument.SelectNodes(@"//Language/LocaleResource"))
                    resources.Add(new LocaleStringResourceParent(resNode));

                resources.Sort((x1, x2) => x1.ResourceName.CompareTo(x2.ResourceName));

                foreach (var resource in resources)
                    RecursivelySortChildrenResource(resource);

                var sb = new StringBuilder();
                var writer = XmlWriter.Create(sb);
                writer.WriteStartDocument();
                writer.WriteStartElement("Language", "");

                writer.WriteStartAttribute("Name", "");
                writer.WriteString(originalXmlDocument.SelectSingleNode(@"//Language").Attributes["Name"].InnerText.Trim());
                writer.WriteEndAttribute();

                foreach (var resource in resources)
                    RecursivelyWriteResource(resource, writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();



                //read and parse resources (without <Children> elements)
                var resXml = new XmlDocument();
                var sr = new StringReader(sb.ToString());
                resXml.Load(sr);
                var resNodeList = resXml.SelectNodes(@"//Language/LocaleResource");
                foreach (XmlNode resNode in resNodeList)
                    if (resNode.Attributes != null && resNode.Attributes["Name"] != null)
                    {
                        string resName = resNode.Attributes["Name"].InnerText.Trim();
                        string resValue = resNode.SelectSingleNode("Value").InnerText;
                        if (!String.IsNullOrEmpty(resName))
                        {
                            //ensure it's not duplicate
                            bool duplicate = false;
                            foreach (var res1 in language1.LocaleStringResources)
                                if (resName.Equals(res1.ResourceName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    duplicate = true;
                                    break;
                                }
                            if (duplicate)
                                continue;

                            //insert resource
                            var lsr = new LocaleStringResource
                            {
                                ResourceName = resName,
                                ResourceValue = resValue
                            };
                            language1.LocaleStringResources.Add(lsr);
                        }
                    }
            }
        }

        #endregion

        #region Methods

        public void InstallData(bool installSampleData = true)
        {
            #region Measures

            var measureDimensionInches = new MeasureDimension()
            {
                Name = "inch(es)",
                SystemKeyword = "inches",
                Ratio = 1M,
                DisplayOrder = 1,
            };
            _measureDimensionRepository.Insert(measureDimensionInches);
            var measureDimensionFeet = new MeasureDimension()
            {
                Name = "feet",
                SystemKeyword = "feet",
                Ratio = 0.08333333M,
                DisplayOrder = 2,
            };
            _measureDimensionRepository.Insert(measureDimensionFeet);
            var measureDimensionMeters = new MeasureDimension()
            {
                Name = "meter(s)",
                SystemKeyword = "meters",
                Ratio = 0.0254M,
                DisplayOrder = 3,
            };
            _measureDimensionRepository.Insert(measureDimensionMeters);
            var measureDimensionMillimetres = new MeasureDimension()
            {
                Name = "millimetre(s)",
                SystemKeyword = "millimetres",
                Ratio = 25.4M,
                DisplayOrder = 4,
            };
            _measureDimensionRepository.Insert(measureDimensionMillimetres);

            var measureWeightOunce = new MeasureWeight()
            {
                Name = "ounce(s)",
                SystemKeyword = "ounce",
                Ratio = 16M,
                DisplayOrder = 1,
            };
            _measureWeightRepository.Insert(measureWeightOunce);
            var measureWeightLb = new MeasureWeight()
            {
                Name = "lb(s)",
                SystemKeyword = "lb",
                Ratio = 1M,
                DisplayOrder = 2,
            };
            _measureWeightRepository.Insert(measureWeightLb);
            var measureWeightKg = new MeasureWeight()
            {
                Name = "kg(s)",
                SystemKeyword = "kg",
                Ratio = 0.45359237M,
                DisplayOrder = 3,
            };
            _measureWeightRepository.Insert(measureWeightKg);
            var measureWeightGram = new MeasureWeight()
            {
                Name = "gram(s)",
                SystemKeyword = "grams",
                Ratio = 453.59237M,
                DisplayOrder = 4,
            };
            _measureWeightRepository.Insert(measureWeightGram);

            #endregion

            #region Tax classes

            var taxCategories = new List<TaxCategory>
                               {
                                   new TaxCategory
                                       {
                                           Name = "Books",
                                           DisplayOrder = 1,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Electronics & Software",
                                           DisplayOrder = 5,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Downloadable Products",
                                           DisplayOrder = 10,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Jewelry",
                                           DisplayOrder = 15,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Apparel & Shoes",
                                           DisplayOrder = 20,
                                       },
                               };
            taxCategories.ForEach(tc => _taxCategoryRepository.Insert(tc));

            #endregion

            #region Language & locale resources

            var language1 = new Language
            {
                Name = "English",
                LanguageCulture = "en-US",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            AddLocaleResources(language1);
            _languageRepository.Insert(language1);
            
            #endregion

            #region Currency

            var currencyUSD = new Currency
            {
                Name = "US Dollar",
                CurrencyCode = "USD",
                Rate = 1,
                DisplayLocale = "en-US",
                CustomFormatting = "CustomFormatting 1",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
            _currencyRepository.Insert(currencyUSD);

            #endregion

            #region Customers & Users

            var user = new User()
            {
                UserGuid = Guid.NewGuid(),
                Email = "admin@yourStore.com",
                Username = "admin@yourStore.com",
                Password = "admin",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                FirstName = "John",
                LastName = "Smith",
                SecurityQuestion = "",
                SecurityAnswer = "",
                IsApproved = true,
                IsLockedOut = false,
                CreatedOnUtc = DateTime.UtcNow,
            };
            _userRepository.Insert(user);

            var customers = new List<Customer>
                                {
                                    new Customer
                                        {
                                            CustomerGuid = Guid.NewGuid(),
                                            AssociatedUserId = user.Id,
                                            AdminComment = string.Empty,
                                            Active = true,
                                            Deleted = false,
                                            CreatedOnUtc = DateTime.UtcNow,
                                        }
                                };
            customers.ForEach(c => _customerRepository.Insert(c));

            var customerRoles = new List<CustomerRole>
                                {
                                    new CustomerRole
                                        {
                                            Name = "Administrators",
                                            Active = true,
                                            IsSystemRole = true,
                                            SystemName = SystemCustomerRoleNames.Administrators,
                                            Customers = new List<Customer>()
                                            {
                                                customers.FirstOrDefault()
                                            }
                                        },
                                    new CustomerRole
                                        {
                                            Name = "Registered",
                                            Active = true,
                                            IsSystemRole = true,
                                            SystemName = SystemCustomerRoleNames.Registered,
                                            Customers = new List<Customer>()
                                            {
                                                customers.FirstOrDefault()
                                            }
                                        },
                                    new CustomerRole
                                        {
                                            Name = "Guests",
                                            Active = true,
                                            IsSystemRole = true,
                                            SystemName = SystemCustomerRoleNames.Guests,
                                        }
                                };
            customerRoles.ForEach(cr => _customerRoleRepository.Insert(cr));

            var users = new List<User>
                                {
                                    new User
                                        {
                                            Username = "admin@yourStore.com",
                                            Password = "admin",
                                            PasswordFormat = PasswordFormat.Clear,
                                            PasswordSalt = "",
                                            FirstName = "John",
                                            LastName = "Smith",
                                            Email = "admin@yourStore.com",
                                            SecurityQuestion = "",
                                            SecurityAnswer = "",
                                            IsApproved = true,
                                            IsLockedOut = false,
                                            CreatedOnUtc = DateTime.UtcNow
                                        }
                                };
            users.ForEach(u => _userRepository.Insert(u));

            #endregion

            #region Settings

            EngineContext.Current.Resolve<IConfigurationProvider<CustomerSettings>>()
                .SaveSettings(new CustomerSettings()
                {
                    AnonymousCheckoutAllowed = false,
                    AllowUsersToUploadAvatars = false,
                    AllowAnonymousUsersToReviewProduct = false,
                    AllowAnonymousUsersToSetProductRatings = false,
                    AllowAnonymousUsersToEmailAFriend = false
                });

            EngineContext.Current.Resolve<IConfigurationProvider<StoreInformationSettings>>()
                .SaveSettings(new StoreInformationSettings()
                {
                    StoreName = "Your store name",
                    StoreUrl = "http://www.yourStore.com",
                });

            EngineContext.Current.Resolve<IConfigurationProvider<RewardPointsSettings>>()
                .SaveSettings(new RewardPointsSettings()
                {
                    Enabled = true,
                    ExchangeRate = 1,
                    PointsForRegistration = 0,
                    PointsForPurchases_Amount = 10,
                    PointsForPurchases_Points = 1
                });

            EngineContext.Current.Resolve<IConfigurationProvider<CurrencySettings>>()
                .SaveSettings(new CurrencySettings()
                {
                    PrimaryStoreCurrencyId = currencyUSD.Id,
                    PrimaryExchangeRateCurrencyId = currencyUSD.Id,
                    ActiveExchangeRateProviderSystemName = "CurrencyExchange.ECB"
                });

            EngineContext.Current.Resolve<IConfigurationProvider<MeasureSettings>>()
                .SaveSettings(new MeasureSettings()
                {
                    BaseDimensionId = measureDimensionInches.Id,
                    BaseWeightId = measureWeightLb.Id,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<MessageTemplatesSettings>>()
                .SaveSettings(new MessageTemplatesSettings()
                {
                    CaseInvariantReplacement = false
                });

            EngineContext.Current.Resolve<IConfigurationProvider<SMSSettings>>()
                .SaveSettings(new SMSSettings()
                {
                    //TODO IList<> property is not saved because GenericListTypeConverter is not loaded yet
                    ActiveSMSProviderSystemNames = new List<string>() 
                });

            EngineContext.Current.Resolve<IConfigurationProvider<ShoppingCartSettings>>()
                .SaveSettings(new ShoppingCartSettings()
                {
                    MaximumShoppingCartItems = 10000,
                    MaximumWishlistItems = 10000
                });

            EngineContext.Current.Resolve<IConfigurationProvider<UserSettings>>()
                .SaveSettings(new UserSettings()
                {
                    UsernamesEnabled = false,
                    AllowUsersToChangeUsernames = false,
                    HashedPasswordFormat= "SHA1"
                });

            EngineContext.Current.Resolve<IConfigurationProvider<ShippingSettings>>()
                .SaveSettings(new ShippingSettings()
                {
                    //TODO IList<> property is not saved because GenericListTypeConverter is not loaded yet
                    ActiveShippingRateComputationMethodSystemNames = new List<string>() { "Shipping.FixedRate" },
                });

            EngineContext.Current.Resolve<IConfigurationProvider<TaxSettings>>()
                .SaveSettings(new TaxSettings()
                {
                    TaxBasedOn = TaxBasedOn.BillingAddress,
                    TaxDisplayType= TaxDisplayType.ExcludingTax,
                    ActiveTaxProviderSystemName = "Tax.FixedRate",
                    DefaultTaxAddressId = 0,
                    DisplayTaxSuffix = false,
                    DisplayTaxRates= false,
                    PricesIncludeTax = false,
                    AllowCustomersToSelectTaxDisplayType= false,
                    HideZeroTax = false,
                    HideTaxInOrderSummary= false,
                    ShippingIsTaxable = false,
                    ShippingPriceIncludesTax =false,
                    ShippingTaxClassId= 0,
                    PaymentMethodAdditionalFeeIsTaxable= false,
                    PaymentMethodAdditionalFeeIncludesTax= false,
                    PaymentMethodAdditionalFeeTaxClassId= 0,
                    EuVatEnabled = false,
                    EuVatShopCountryId = 0,
                    EuVatAllowVatExemption= true,
                    EuVatUseWebService = false,
                    EuVatEmailAdminWhenNewVatSubmitted= false
                });

            EngineContext.Current.Resolve<IConfigurationProvider<FileSystemSettings>>()
                .SaveSettings(new FileSystemSettings()
                {
                });

            EngineContext.Current.Resolve<IConfigurationProvider<DateTimeSettings>>()
                .SaveSettings(new DateTimeSettings()
                {
                    DefaultStoreTimeZoneId = "",
                    AllowCustomersToSetTimeZone = false
                });
            #endregion

            if (installSampleData)
            {
                #region Categories

                for (int i = 1; i <= 30; i++)
                {
                    var cat = new Category()
                    {
                        Name = "sample category" + i,
                        Description = "Some description 1",
                        MetaKeywords = string.Empty,
                        MetaDescription = string.Empty,
                        MetaTitle = string.Empty,
                        SeName = string.Empty,
                        ParentCategoryId = 0,
                        PageSize = 3,
                        PriceRanges = string.Empty,
                        Published = true,
                        DisplayOrder = i,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };
                    _categoryRepository.Insert(cat);
                }
                Category category1 = _categoryRepository.Table.FirstOrDefault();
                
                #endregion

                #region Manufacturers

                var manufacturer1 = new Manufacturer()
                {
                    Name = "Manufacturer 1",
                    Description = "Some description 1",
                    MetaKeywords = string.Empty,
                    MetaDescription = string.Empty,
                    MetaTitle = string.Empty,
                    SeName = string.Empty,
                    PageSize = 3,
                    PriceRanges = string.Empty,
                    Published = true,
                    DisplayOrder = 7,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };
                _manufacturerRepository.Insert(manufacturer1);

                #endregion

                #region Products

                var allCategories = _categoryRepository.Table.ToList();

                for (var i = 1; i <= 50; i++)
                {
                    var product = new Product()
                    {
                        Name = "Product " + i,
                        ShortDescription = "ShortDescription " + i,
                        FullDescription = "FullDescription " + i,
                        AdminComment = string.Empty,
                        ShowOnHomePage = false,
                        MetaKeywords = string.Empty,
                        MetaDescription = string.Empty,
                        MetaTitle = string.Empty,
                        SeName = string.Empty,
                        AllowCustomerReviews = true,
                        AllowCustomerRatings = true,
                        RatingSum = 0,
                        TotalRatingVotes = 0,
                        Published = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };
                    var productVariant = new ProductVariant()
                    {
                        Name = string.Empty,
                        Sku = string.Empty,
                        Description = string.Empty,
                        AdminComment = string.Empty,
                        ManufacturerPartNumber = string.Empty,
                        UserAgreementText = string.Empty,
                        IsShipEnabled = true,
                        ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                        LowStockActivity = LowStockActivity.Unpublish,
                        BackorderMode = BackorderMode.NoBackorders,
                        OrderMinimumQuantity = 1,
                        OrderMaximumQuantity = 10000,
                        Price = 130.12345M,
                        Weight = 1,
                        Length = 1,
                        Width = 1,
                        Height = 1,
                        Published = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };
                    product.ProductVariants.Add(productVariant);

                    foreach (var category in CommonHelper.SelectNRandom(allCategories, 15))
                    {
                        var pcm = new ProductCategory()
                        {
                            Category = category,
                            Product = product,
                            DisplayOrder = i
                        };
                        product.ProductCategories.Add(pcm);
                    }
                    
                    _productRepository.Insert(product);
                }
                
                #endregion
            }
        }

        #endregion
    }
}
