
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Core.IO;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Media;


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
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
        private readonly IRepository<ForumGroup> _forumGroupRepository;
        private readonly IRepository<Forum> _forumRepository;
        private readonly IRepository<ForumTopic> _forumTopicRepository;
        private readonly IRepository<ForumPost> _forumPostRepository;
        private readonly IRepository<PrivateMessage> _forumPrivateMessageRepository;
        private readonly IRepository<ForumSubscription> _forumSubscriptionRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<ShippingMethod> _shippingMethodRepository;

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
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<Category> categoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<QueuedEmail> queuedEmailRepository,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<Forum> forumRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<PrivateMessage> forumPrivateMessageRepository,
            IRepository<ForumSubscription> forumSubscriptionRepository,
            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<Discount> discountRepository,
            IRepository<ShippingMethod> shippingMethodRepository)
        {
            this._measureDimensionRepository = measureDimensionRepository;
            this._measureWeightRepository = measureWeightRepository;
            this._taxCategoryRepository = taxCategoryRepository;
            this._languageRepository = languageRepository;
            this._currencyRepository = currencyRepository;
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._userRepository = userRepository;

            this._specificationAttributeRepository = specificationAttributeRepository;
            this._categoryRepository = categoryRepository;
            this._manufacturerRepository = manufacturerRepository;
            this._productRepository = productRepository;

            this._emailAccountRepository = emailAccountRepository;
            this._queuedEmailRepository = queuedEmailRepository;

            this._forumGroupRepository = forumGroupRepository;
            this._forumRepository = forumRepository;
            this._forumTopicRepository = forumTopicRepository;
            this._forumPostRepository = forumPostRepository;
            this._forumPrivateMessageRepository = forumPrivateMessageRepository;
            this._forumSubscriptionRepository = forumSubscriptionRepository;

            this._countryRepository = countryRepository;
            this._stateProvinceRepository = stateProvinceRepository;

            this._discountRepository = discountRepository;

            this._shippingMethodRepository = shippingMethodRepository;
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

        private void AddLocaleResources(Language language)
        {
            //insert default sting resources (temporary solution). Requires some performance optimization
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
                            foreach (var res1 in language.LocaleStringResources)
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
                            language.LocaleStringResources.Add(lsr);
                        }
                    }
            }
        }

        protected virtual void InstallMeasures()
        {
            var measureDimensions = new List<MeasureDimension>()
            {
                new MeasureDimension()
                {
                    Name = "inch(es)",
                    SystemKeyword = "inches",
                    Ratio = 1M,
                    DisplayOrder = 1,
                },
                new MeasureDimension()
                {
                    Name = "feet",
                    SystemKeyword = "feet",
                    Ratio = 0.08333333M,
                    DisplayOrder = 2,
                },
                new MeasureDimension()
                {
                    Name = "meter(s)",
                    SystemKeyword = "meters",
                    Ratio = 0.0254M,
                    DisplayOrder = 3,
                },
                new MeasureDimension()
                {
                    Name = "millimetre(s)",
                    SystemKeyword = "millimetres",
                    Ratio = 25.4M,
                    DisplayOrder = 4,
                }
            };

            measureDimensions.ForEach(x => _measureDimensionRepository.Insert(x));

            var measureWeights = new List<MeasureWeight>()
            {
                new MeasureWeight()
                {
                    Name = "ounce(s)",
                    SystemKeyword = "ounce",
                    Ratio = 16M,
                    DisplayOrder = 1,
                },
                new MeasureWeight()
                {
                    Name = "lb(s)",
                    SystemKeyword = "lb",
                    Ratio = 1M,
                    DisplayOrder = 2,
                },
                new MeasureWeight()
                {
                    Name = "kg(s)",
                    SystemKeyword = "kg",
                    Ratio = 0.45359237M,
                    DisplayOrder = 3,
                },
                new MeasureWeight()
                {
                    Name = "gram(s)",
                    SystemKeyword = "grams",
                    Ratio = 453.59237M,
                    DisplayOrder = 4,
                }
            };

            measureWeights.ForEach(x => _measureWeightRepository.Insert(x));
        }

        protected virtual void InstallTaxCategories()
        {
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

        }

        protected virtual void InstallLanguagesAndResources()
        {
            var languageEng = new Language
            {
                Name = "English",
                LanguageCulture = "en-US",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            AddLocaleResources(languageEng);
            _languageRepository.Insert(languageEng);

        }

        protected virtual void InstallCurrencies()
        {
            var currencyUsd = new Currency
            {
                Name = "US Dollar",
                CurrencyCode = "USD",
                Rate = 1,
                DisplayLocale = "en-US",
                CustomFormatting = "",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
            _currencyRepository.Insert(currencyUsd);
        }

        protected virtual void InstallCountriesAndStates()
        {
            var countries = new List<Country>
                                {
                                    new Country
                                        {
                                            Name = "United States",
                                            AllowsBilling = true,
                                            AllowsShipping = true,
                                            AllowsRegistration = true,
                                            TwoLetterIsoCode = "US",
                                            ThreeLetterIsoCode = "USA",
                                            NumericIsoCode = 840,
                                            SubjectToVat = false,
                                            DisplayOrder = 1,
                                            Published = true,
                                            StateProvinces = new List<StateProvince>()
                                            {
                                                new StateProvince()
                                                {
                                                    Name = "Alabama",
                                                    Abbreviation = "AL",
                                                    DisplayOrder = 1,
                                                },
                                                new StateProvince()
                                                {
                                                    Name = "Alaska",
                                                    Abbreviation = "AK",
                                                    DisplayOrder = 1,
                                                },
                                                //UNDONE insert other states
                                                new StateProvince()
                                                {
                                                    Name = "New York",
                                                    Abbreviation = "NY",
                                                    DisplayOrder = 1,
                                                },
                                            }
                                        },
                                    new Country
                                        {
                                            Name = "Canada",
                                            AllowsBilling = true,
                                            AllowsShipping = true,
                                            AllowsRegistration = true,
                                            TwoLetterIsoCode = "CA",
                                            ThreeLetterIsoCode = "CAN",
                                            NumericIsoCode = 124,
                                            SubjectToVat = false,
                                            DisplayOrder = 2,
                                            Published = true,
                                            StateProvinces = new List<StateProvince>()
                                            {
                                                new StateProvince()
                                                {
                                                    Name = "Alberta",
                                                    Abbreviation = "AB",
                                                    DisplayOrder = 1,
                                                },
                                                new StateProvince()
                                                {
                                                    Name = "British Columbia",
                                                    Abbreviation = "BC",
                                                    DisplayOrder = 1,
                                                },
                                                //UNDONE insert other states
                                            }
                                        },
                                    new Country
                                        {
                                            Name = "Russia",
                                            AllowsBilling = true,
                                            AllowsShipping = true,
                                            AllowsRegistration = true,
                                            TwoLetterIsoCode = "RU",
                                            ThreeLetterIsoCode = "RUS",
                                            NumericIsoCode = 643,
                                            SubjectToVat = false,
                                            DisplayOrder = 100,
                                            Published = true,
                                        },
                                };
            countries.ForEach(c => _countryRepository.Insert(c));

        }

        protected virtual void InstallShippingMethods()
        {
            var shippingMethods = new List<ShippingMethod>
                                {
                                    new ShippingMethod
                                        {
                                            Name = "In-Store Pickup",
                                            Description ="Pick up your items at the store",
                                            DisplayOrder = 0
                                        },
                                    new ShippingMethod
                                        {
                                            Name = "By Ground",
                                            Description ="Compared to other shipping methods, like by flight or over seas, ground shipping is carried out closer to the earth",
                                            DisplayOrder = 1
                                        },
                                    new ShippingMethod
                                        {
                                            Name = "By Air",
                                            Description ="The one day air shipping",
                                            DisplayOrder = 3
                                        },
                                };
            shippingMethods.ForEach(sm => _shippingMethodRepository.Insert(sm));

        }

        protected virtual void InstallCustomersAndUsers()
        {
            var adminUser = new User()
            {
                UserGuid = Guid.NewGuid(),
                Email = "admin@yourStore.com",
                Username = "admin@yourStore.com",
                Password = "admin",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                SecurityQuestion = "",
                SecurityAnswer = "",
                IsApproved = true,
                IsLockedOut = false,
                CreatedOnUtc = DateTime.UtcNow,
            };
            _userRepository.Insert(adminUser);

            var customers = new List<Customer>
                                {
                                    new Customer
                                        {
                                            CustomerGuid = Guid.NewGuid(),
                                            AssociatedUserId = adminUser.Id,
                                            Active = true,
                                            CreatedOnUtc = DateTime.UtcNow,
                                        }
                                };
            customers.FirstOrDefault().AddAddress(new Address()
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "12345678",
                Email = "admin@yourStore.com",
                FaxNumber = "",
                Company = "Nop Solutions",
                Address1 = "21 West 52nd Street",
                Address2 = "",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.Where(sp => sp.Name == "New York").FirstOrDefault(),
                Country = _countryRepository.Table.Where(c => c.ThreeLetterIsoCode == "USA").FirstOrDefault(),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow
            });
            customers.FirstOrDefault().AddAddress(new Address()
            {
                FirstName = "test 1",
                LastName = "test 2",
                PhoneNumber = "test 3",
                Email = "admin@yourStore.com",
                FaxNumber = "",
                Company = "test 4",
                Address1 = "test 5",
                Address2 = "",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.Where(sp => sp.Name == "New York").FirstOrDefault(),
                Country = _countryRepository.Table.Where(c => c.ThreeLetterIsoCode == "USA").FirstOrDefault(),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow
            });
            customers.ForEach(c => _customerRepository.Insert(c));

            var testGuests = new List<Customer>();
            for (int i = 0; i < 15; i++)
                testGuests.Add(new Customer
                {
                    CustomerGuid = Guid.NewGuid(),
                    Active = true,
                    CreatedOnUtc = DateTime.UtcNow,
                });
            testGuests.ForEach(c => _customerRepository.Insert(c));

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
                                            Customers = testGuests
                                        }
                                };
            customerRoles.ForEach(cr => _customerRoleRepository.Insert(cr));

            //test users
            for (int i = 1; i <= 30; i++)
            {
                var testUser = new User()
                {
                    UserGuid = Guid.NewGuid(),
                    Email = string.Format("admin{0}@yourStore.com", i),
                    Username = string.Format("admin{0}@yourStore.com", i),
                    Password = "admin",
                    PasswordFormat = PasswordFormat.Clear,
                    IsApproved = true,
                    CreatedOnUtc = DateTime.UtcNow,
                };

                _userRepository.Insert(testUser);
            }
        }

        protected virtual void InstallEmailAccounts()
        {
            var emailAccounts = new List<EmailAccount>
                               {
                                   new EmailAccount
                                       {
                                           Email = "test@mail.com",
                                           DisplayName = "General contact",
                                           Host = "smtp.mail.com",
                                           Port = 25,
                                           Username = "123",
                                           Password = "123",
                                           EnableSsl = false,
                                           UseDefaultCredentials = false
                                       },
                                   new EmailAccount
                                       {
                                           Email = "test@mail.com",
                                           DisplayName = "Sales representative",
                                           Host = "smtp.mail.com",
                                           Port = 25,
                                           Username = "123",
                                           Password = "123",
                                           EnableSsl = false,
                                           UseDefaultCredentials = false
                                       },
                                   new EmailAccount
                                       {
                                           Email = "test@mail.com",
                                           DisplayName = "Customer support",
                                           Host = "smtp.mail.com",
                                           Port = 25,
                                           Username = "123",
                                           Password = "123",
                                           EnableSsl = false,
                                           UseDefaultCredentials = false
                                       }, 
                               };
            emailAccounts.ForEach(ea => _emailAccountRepository.Insert(ea));

        }

        protected virtual void InstallQueuedEmails()
        {

            var queuedEmail = new List<QueuedEmail>()
            {
                new QueuedEmail()
                {
                    EmailAccountId = 1,
                    Priority = 1,
                    From = "admin@test.com",
                    FromName = "Adminstrator",
                    To = "cust@test.com",
                    ToName = "Customer",
                    CC = "admincc@test.com",
                    Bcc = "adminbcc@test.com",
                    Body = "Body",
                    Subject = "Subject",
                    CreatedOnUtc = DateTime.UtcNow,
                    SentTries = 0,
                    SentOnUtc = null
                },
                new QueuedEmail()
                {
                    EmailAccountId = 2,
                    Priority = 2,
                    From = "admin@test.com",
                    FromName = "Adminstrator",
                    To = "cust@test.com",
                    ToName = "Customer",
                    CC = "admincc@test.com",
                    Bcc = "adminbcc@test.com",
                    Body = "Body",
                    Subject = "Subject",
                    CreatedOnUtc = DateTime.UtcNow,
                    SentTries = 2,
                    SentOnUtc = DateTime.UtcNow
                }
            };
            for (var i = 1; i <= 50; i++)
            {
                queuedEmail.Add(
                    new QueuedEmail()
                    {
                        EmailAccountId = 1,
                        Priority = i,
                        From = "admin@test.com",
                        FromName = "Adminstrator",
                        To = "cust@test.com",
                        ToName = "Customer",
                        CC = "admincc@test.com",
                        Bcc = "adminbcc@test.com",
                        Body = "Body" + i,
                        Subject = "Subject" + i,
                        CreatedOnUtc = DateTime.UtcNow,
                        SentTries = i,
                        SentOnUtc = null
                    }
                );
            }

            queuedEmail.ForEach(qe => _queuedEmailRepository.Insert(qe));

        }

        protected virtual void InstallSettings()
        {
            EngineContext.Current.Resolve<IConfigurationProvider<CatalogSettings>>()
                .SaveSettings(new CatalogSettings()
                {
                    HidePricesForNonRegistered = false,
                    ShowProductSku = false,
                    ShowManufacturerPartNumber = false,
                    AllowProductSorting = true,
                    AllowProductViewModeChanging = true,
                    ShowCategoryProductNumber = false,
                    ShowCategoryProductNumberIncludingSubcategories = false,
                    CategoryBreadcrumbEnabled = true,
                    PageShareCode = "<!-- AddThis Button BEGIN --> <a class=\"addthis_button\" href=\"http://www.addthis.com/bookmark.php?v=250&amp;username=nopsolutions\"><img src=\"http://s7.addthis.com/static/btn/v2/lg-share-en.gif\" width=\"125\" height=\"16\" alt=\"Bookmark and Share\" style=\"border:0\"/></a><script type=\"text/javascript\" src=\"http://s7.addthis.com/js/250/addthis_widget.js#username=nopsolutions\"></script> <!-- AddThis Button END -->",
                    ProductReviewsMustBeApproved = true,
                    AllowAnonymousUsersToReviewProduct = false,
                    NotifyStoreOwnerAboutNewProductReviews = false,
                    AllowAnonymousUsersToEmailAFriend = false,
                    WishlistEnabled = true
                });

            EngineContext.Current.Resolve<IConfigurationProvider<LocalizationSettings>>()
                .SaveSettings(new LocalizationSettings()
                {
                    DefaultAdminLanguageId = _languageRepository.Table.Where(l => l.Name == "English").Single().Id
                });

            EngineContext.Current.Resolve<IConfigurationProvider<CustomerSettings>>()
                .SaveSettings(new CustomerSettings()
                {
                    AnonymousCheckoutAllowed = false,
                    AllowCustomersToUploadAvatars = false,
                    AvatarMaximumSizeBytes = 20000,
                    DefaultAvatarEnabled = true,
                    ShowCustomersLocation = false,
                    ShowCustomersJoinDate = false,
                    AllowViewingProfiles = false,
                    NotifyNewCustomerRegistration = false,
                    CustomerNameFormat = CustomerNameFormat.ShowEmails
                });

            EngineContext.Current.Resolve<IConfigurationProvider<MediaSettings>>()
                .SaveSettings(new MediaSettings()
                {
                    AvatarPictureSize = 85,
                    ProductThumbPictureSize = 125,
                    ProductDetailsPictureSize = 300,
                    ProductVariantPictureSize = 125,
                    CategoryThumbPictureSize = 125,
                    ManufacturerThumbPictureSize = 125,
                    CartThumbPictureSize = 80,
                    MaximumImageSize = 1280,
                    DefaultPictureZoomEnabled = false,
                });
            EngineContext.Current.Resolve<IConfigurationProvider<FormFieldSettings>>()
                .SaveSettings(new FormFieldSettings()
                {
                    GenderEnabled = true,
                    DateOfBirthEnabled = true,
                    NewsletterEnabled = true,
                    CompanyEnabled = true
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
                    PointsForPurchases_Points = 1,
                    PointsForPurchases_Awarded = OrderStatus.Complete,
                    PointsForPurchases_Canceled = OrderStatus.Cancelled,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<CurrencySettings>>()
                .SaveSettings(new CurrencySettings()
                {
                    PrimaryStoreCurrencyId = _currencyRepository.Table.Where(c => c.CurrencyCode == "USD").Single().Id,
                    PrimaryExchangeRateCurrencyId = _currencyRepository.Table.Where(c => c.CurrencyCode == "USD").Single().Id,
                    ActiveExchangeRateProviderSystemName = "CurrencyExchange.McExchange",
                    AutoUpdateEnabled = true,
                    LastUpdateTime = 0
                });

            EngineContext.Current.Resolve<IConfigurationProvider<MeasureSettings>>()
                .SaveSettings(new MeasureSettings()
                {
                    BaseDimensionId = _measureDimensionRepository.Table.Where(m => m.SystemKeyword == "inches").Single().Id,
                    BaseWeightId = _measureWeightRepository.Table.Where(m => m.SystemKeyword == "lb").Single().Id,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<MessageTemplatesSettings>>()
                .SaveSettings(new MessageTemplatesSettings()
                {
                    CaseInvariantReplacement = false
                });

            EngineContext.Current.Resolve<IConfigurationProvider<SMSSettings>>()
                .SaveSettings(new SMSSettings()
                {
                    ActiveSMSProviderSystemNames = new List<string>()
                });

            EngineContext.Current.Resolve<IConfigurationProvider<ShoppingCartSettings>>()
                .SaveSettings(new ShoppingCartSettings()
                {
                    MaximumShoppingCartItems = 1000,
                    MaximumWishlistItems = 1000,
                    ShowProductImagesOnShoppingCart = true,
                    ShowProductImagesOnWishList = true,
                    ShowDiscountBox = true,
                    ShowGiftCardBox = true,
                    CrossSellsNumber = 2,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<OrderSettings>>()
                .SaveSettings(new OrderSettings()
                {
                    IsReOrderAllowed = true,
                    MinOrderSubtotalAmount = 0,
                    MinOrderTotalAmount = 0,
                    AnonymousCheckoutAllowed = false,
                    ReturnRequestsEnabled = true,
                    ReturnRequestActions = new List<string>() { "Received Wrong Product", "Wrong Product Ordered", "There Was A Problem With The Product" },
                    ReturnRequestReasons = new List<string>() { "Repair", "Replacement", "Store Credit" },
                    TermsOfServiceEnabled = false,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<UserSettings>>()
                .SaveSettings(new UserSettings()
                {
                    UsernamesEnabled = false,
                    AllowUsersToChangeUsernames = false,
                    HashedPasswordFormat = "SHA1",
                    UserRegistrationType = UserRegistrationType.Standard
                });

            EngineContext.Current.Resolve<IConfigurationProvider<SecuritySettings>>()
                .SaveSettings(new SecuritySettings()
                {
                    EncryptionKey = "273ece6f97dd844d"
                });

            EngineContext.Current.Resolve<IConfigurationProvider<ShippingSettings>>()
                .SaveSettings(new ShippingSettings()
                {
                    ActiveShippingRateComputationMethodSystemNames = new List<string>() { "Shipping.FixedRate" },
                    EstimateShippingEnabled = true,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<PaymentSettings>>()
                .SaveSettings(new PaymentSettings()
                {
                    ActivePaymentMethodSystemNames = new List<string>() { "Payments.Manual" },
                });

            EngineContext.Current.Resolve<IConfigurationProvider<TaxSettings>>()
                .SaveSettings(new TaxSettings()
                {
                    TaxBasedOn = TaxBasedOn.BillingAddress,
                    TaxDisplayType = TaxDisplayType.ExcludingTax,
                    ActiveTaxProviderSystemName = "Tax.FixedRate",
                    DefaultTaxAddressId = 0,
                    DisplayTaxSuffix = false,
                    DisplayTaxRates = false,
                    PricesIncludeTax = false,
                    AllowCustomersToSelectTaxDisplayType = false,
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
                    EuVatEmailAdminWhenNewVatSubmitted = false
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

            EngineContext.Current.Resolve<IConfigurationProvider<ForumSettings>>()
                .SaveSettings(new ForumSettings()
                {
                    ForumsEnabled = true,
                    RelativeDateTimeFormattingEnabled = true,
                    AllowCustomersToEditPosts = true,
                    AllowCustomersToManageSubscriptions = true,
                    AllowGuestsToCreatePosts = false,
                    AllowGuestsToCreateTopics = false,
                    AllowCustomersToDeletePosts = true,
                    TopicSubjectMaxLength = 450,
                    PostMaxLength = 4000,
                    TopicsPageSize = 10,
                    PostsPageSize = 10,
                    TopicPostsPageLinkDisplayCount = 5,
                    ForumTopicsPageLinkDisplayCount = 5,
                    SearchPageLinkDisplayCount = 10,
                    SearchResultsPageSize = 15,
                    LatestCustomerPostsPageSize = 5,
                    ShowCustomersPostCount = true,
                    ForumEditor = EditorType.BBCodeEditor,
                    SignaturesEnabled = true,
                    AllowPrivateMessages = true,
                    NotifyAboutPrivateMessages = false,
                    PMSubjectMaxLength = 450,
                    PMTextMaxLength = 4000,
                    ActiveDiscussionsCount = 25,
                    ActiveDiscussionsFeedCount = 25,
                    ForumFeedCount = 10,
                    ForumSearchTermMinimumLength = 3,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<EmailAccountSettings>>()
                .SaveSettings(new EmailAccountSettings()
                {
                    DefaultEmailAccountId = 1
                });
        }

        protected virtual void InstallSpecificationAttributes()
        {
            var specificationAttributes = new List<SpecificationAttribute>
                                {
                                    new SpecificationAttribute
                                        {
                                            Name = "Screensize",
                                            DisplayOrder = 1,
                                            SpecificationAttributeOptions = new List<SpecificationAttributeOption>()
                                            {
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "10.0''",
                                                    DisplayOrder = 3,
                                                },
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "14.1''",
                                                    DisplayOrder = 4,
                                                },
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "15.4''",
                                                    DisplayOrder = 5,
                                                },
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "16.0''",
                                                    DisplayOrder = 6,
                                                },
                                            }
                                        },
                                    new SpecificationAttribute
                                        {
                                            Name = "CPU Type",
                                            DisplayOrder = 2,
                                            SpecificationAttributeOptions = new List<SpecificationAttributeOption>()
                                            {
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "AMD",
                                                    DisplayOrder = 1,
                                                },
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "Intel",
                                                    DisplayOrder = 2,
                                                },
                                            }
                                        },
                                    new SpecificationAttribute
                                        {
                                            Name = "Memory",
                                            DisplayOrder = 3,
                                            SpecificationAttributeOptions = new List<SpecificationAttributeOption>()
                                            {
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "1 GB",
                                                    DisplayOrder = 1,
                                                },
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "3 GB",
                                                    DisplayOrder = 2,
                                                },
                                            }
                                        },
                                    new SpecificationAttribute
                                        {
                                            Name = "Hardrive",
                                            DisplayOrder = 5,
                                            SpecificationAttributeOptions = new List<SpecificationAttributeOption>()
                                            {
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "160 GB",
                                                    DisplayOrder = 3,
                                                },
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "250 GB",
                                                    DisplayOrder = 4,
                                                },
                                                new SpecificationAttributeOption()
                                                {
                                                    Name = "320 GB",
                                                    DisplayOrder = 7,
                                                },
                                            }
                                        },
                                };
            specificationAttributes.ForEach(sa => _specificationAttributeRepository.Insert(sa));

        }

        protected virtual void InstallCategories()
        {
            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = string.Format("{0}content\\images\\samples\\", HttpContext.Current.Request.PhysicalApplicationPath);

            var categoryPictureBook = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_book.jpeg"), "image/jpeg", true);
            var categoryPictureComputers = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_computers.jpeg"), "image/jpeg", true);
            var categoryPictureAccessories = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_accessories.jpg"), "image/pjpeg", true);
            var categoryPictureSoftware = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_software.jpg"), "image/pjpeg", true);
            var categoryPictureGames = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_games.jpg"), "image/pjpeg", true);
            var categoryPictureElectronics = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_electronics.jpeg"), "image/jpeg", true);
            var categoryPictureCameraPhoto = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_camera_photo.jpeg"), "image/jpeg", true);
            var categoryPictureCellPhones = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_cell_phones.jpeg"), "image/jpeg", true);
            var categoryPictureApparelShoes = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_apparel_shoes.jpeg"), "image/jpeg", true);
            var categoryPictureShirts = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_shirts.jpg"), "image/pjpeg", true);
            var categoryPictureJeans = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_jeans.jpg"), "image/pjpeg", true);
            var categoryPictureShoes = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_shoes.jpg"), "image/pjpeg", true);
            var categoryPictureAccessoriesShoes = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_accessories_shoes.jpg"), "image/pjpeg", true);
            var categoryPictureDigitalDownloads = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_digital_downloads.jpeg"), "image/jpeg", true);
            var categoryPictureJewelry = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_jewelry.jpeg"), "image/jpeg", true);
            var categoryPictureDesktops = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_desktops.jpg"), "image/pjpeg", true);
            var categoryPictureNotebooks = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_notebooks.jpg"), "image/pjpeg", true);
            var categoryPictureGiftCards = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_gift_cards.jpeg"), "image/jpeg", true);

            

            //categories
            var categoryBooks = new Category
            {
                Name = "Books",
                MetaKeywords = "Books, Dictionary, Textbooks",
                MetaDescription = "Books category description",
                PageSize = 4,
                PictureId = categoryPictureBook.Id,
                PriceRanges = "-25;25-50;50-;",
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryBooks);

            var categoryComputers = new Category
            {
                Name = "Computers",
                PageSize = 4,
                PictureId = categoryPictureComputers.Id,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryComputers);

            var categoryDesktops = new Category
            {
                Name = "Desktops",
                PageSize = 4,
                ParentCategoryId = categoryComputers.Id,
                PictureId = categoryPictureDesktops.Id,
                PriceRanges = "-1000;1000-1200;1200-;",
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryDesktops);

            var categoryNotebooks = new Category
            {
                Name = "Notebooks",
                PageSize = 4,
                ParentCategoryId = categoryComputers.Id,
                PictureId = categoryPictureNotebooks.Id,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryNotebooks);

            var categoryAccessories = new Category
            {
                Name = "Accessories",
                PageSize = 4,
                ParentCategoryId = categoryComputers.Id,
                PictureId = categoryPictureAccessories.Id,
                PriceRanges = "-100;100-;",
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryAccessories);

            var categorySoftware = new Category
            {
                Name = "Software",
                PageSize = 4,
                ParentCategoryId = categoryComputers.Id,
                PictureId = categoryPictureSoftware.Id,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categorySoftware);

            var categoryGames = new Category
            {
                Name = "Games",
                PageSize = 4,
                ParentCategoryId = categoryComputers.Id,
                PictureId = categoryPictureGames.Id,
                Published = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryGames);

            var categoryElectronics = new Category
            {
                Name = "Electronics",
                PageSize = 4,
                PictureId = categoryPictureElectronics.Id,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryElectronics);

            var categoryCameraPhoto = new Category
            {
                Name = "Camera, photo",
                PageSize = 4,
                ParentCategoryId = categoryElectronics.Id,
                PictureId = categoryPictureCameraPhoto.Id,
                PriceRanges = "-500;500-;",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryCameraPhoto);

            var categoryCellPhones = new Category
            {
                Name = "Cell phones",
                PageSize = 4,
                ParentCategoryId = categoryElectronics.Id,
                PictureId = categoryPictureCellPhones.Id,
                Published = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryCellPhones);

            var categoryApparelShoes = new Category
            {
                Name = "Apparel & Shoes",
                PageSize = 4,
                PictureId = categoryPictureApparelShoes.Id,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryApparelShoes);

            var categoryShirts = new Category
            {
                Name = "Shirts",
                PageSize = 4,
                ParentCategoryId = categoryApparelShoes.Id,
                PictureId = categoryPictureShirts.Id,
                PriceRanges = "-20;20-;",
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryShirts);

            var categoryJeans = new Category
            {
                Name = "Jeans",
                PageSize = 4,
                ParentCategoryId = categoryApparelShoes.Id,
                PictureId = categoryPictureJeans.Id,
                PriceRanges = "-20;20-;",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryJeans);

            var categoryShoes = new Category
            {
                Name = "Shoes",
                PageSize = 4,
                ParentCategoryId = categoryApparelShoes.Id,
                PictureId = categoryPictureShoes.Id,
                PriceRanges = "-20;20-;",
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryShoes);

            var categoryAccessoriesShoes = new Category
            {
                Name = "Accessories",
                PageSize = 4,
                ParentCategoryId = categoryApparelShoes.Id,
                PictureId = categoryPictureAccessoriesShoes.Id,
                PriceRanges = "-30;30-;",
                Published = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryAccessoriesShoes);
            _categoryRepository.Insert(categoryShoes);

            var categoryDigitalDownloads = new Category
            {
                Name = "Digital downloads",
                PageSize = 4,
                PictureId = categoryPictureDigitalDownloads.Id,
                Published = true,
                DisplayOrder = 6,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryDigitalDownloads);

            var categoryJewelry = new Category
            {
                Name = "Jewelry",
                PageSize = 4,
                PictureId = categoryPictureJewelry.Id,
                PriceRanges = "0-500;500-700;700-3000;",
                Published = true,
                DisplayOrder = 7,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryJewelry);

            var categoryGiftCards = new Category
            {
                Name = "Gift Cards",
                PageSize = 4,
                PictureId = categoryPictureGiftCards.Id,
                Published = true,
                DisplayOrder = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryGiftCards);
        }

        protected virtual void InstallManufacturers()
        {
            var manufacturerAsus = new Manufacturer
            {
                Name = "ASUS",
                PageSize = 4,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _manufacturerRepository.Insert(manufacturerAsus);

            var manufacturerHp = new Manufacturer
            {
                Name = "HP",
                PageSize = 4,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _manufacturerRepository.Insert(manufacturerHp);
        }

        protected virtual void InstallProducts()
        {

            var allCategories = _categoryRepository.Table.ToList();

            for (var i = 1; i <= 20; i++)
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
                    ApprovedRatingSum = 0,
                    NotApprovedRatingSum = 0,
                    ApprovedTotalReviews = 0,
                    NotApprovedTotalReviews = 0,
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
                    StockQuantity = 10000,
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
                    UpdatedOnUtc = DateTime.UtcNow,
                    TaxCategoryId = 0
                };
                product.ProductVariants.Add(productVariant);

                foreach (var category in allCategories)
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

        }

        protected virtual void InstallForums()
        {

            int forumGroupCount = 2;
            int forumCount = 5;
            int topicCount = 1;
            int postCount = 1;

            var customer = _customerRepository.Table.FirstOrDefault();

            for (int a = 1; a <= forumGroupCount; a++)
            {
                var forumGroup = new ForumGroup()
                {
                    Name = "Forum Group " + a.ToString(),
                    Description = "ForumGroup " + a.ToString() + " Description",
                    DisplayOrder = a,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Forums = new List<Forum>()
                };

                _forumGroupRepository.Insert(forumGroup);

                for (int b = 1; b <= forumCount; b++)
                {
                    var forum = new Forum()
                    {
                        Id = forumGroup.Id,
                        Name = String.Format("FG{0} Forum {1}", a.ToString(), b.ToString()),
                        Description = String.Format("FG{0}, Forum {1} Description", a.ToString(), b.ToString()),
                        NumTopics = 0,
                        NumPosts = 0,
                        LastPostCustomerId = customer.Id,
                        LastPostTime = DateTime.UtcNow,
                        DisplayOrder = b,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                        ForumGroup = forumGroup,
                        ForumTopics = new List<ForumTopic>()
                    };

                    _forumRepository.Insert(forum);
                    forumGroup.Forums.Add(forum);

                    for (int c = 1; c <= topicCount; c++)
                    {
                        var forumTopic = new ForumTopic()
                        {
                            Id = forum.Id,
                            Forum = forum,
                            CustomerId = customer.Id,
                            TopicTypeId = (int)ForumTopicType.Normal,
                            Subject = String.Format("FG{0}, F{1}, Topic {2} Subject", a.ToString(), b.ToString(), c.ToString()),
                            NumPosts = 0,
                            Views = 0,
                            LastPostId = 0,
                            LastPostTime = DateTime.UtcNow,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow,
                            ForumPosts = new List<ForumPost>()
                        };

                        _forumTopicRepository.Insert(forumTopic);
                        forum.ForumTopics.Add(forumTopic);
                        forum.LastTopicId = forumTopic.Id;

                        for (int d = 1; d <= postCount; d++)
                        {
                            var forumPost = new ForumPost()
                            {
                                Id = forumTopic.Id,
                                ForumTopic = forumTopic,
                                CustomerId = customer.Id,
                                Text = String.Format("Post {0} Text. {1}", d.ToString(), forumTopic.Subject),
                                IPAddress = "127.0.0.1",
                                CreatedOnUtc = DateTime.UtcNow,
                                UpdatedOnUtc = DateTime.UtcNow
                            };
                            _forumPostRepository.Insert(forumPost);
                            forumTopic.ForumPosts.Add(forumPost);
                            forum.LastPostId = forumPost.Id;
                            forum.LastPostTime = DateTime.UtcNow;
                            forumTopic.LastPostId = forumPost.Id;
                            forumTopic.LastPostTime = DateTime.UtcNow;
                        }

                        forumTopic.NumPosts = postCount;
                        forum.NumPosts += postCount;
                    }
                    forum.NumTopics = topicCount;
                }
            }
        }

        protected virtual void InstallDiscounts()
        {
            var disounts = new List<Discount>
                                {
                                    new Discount
                                        {
                                            Name = "Sample discount with coupon code",
                                            DiscountType = DiscountType.AssignedToSkus,
                                            DiscountLimitation = DiscountLimitationType.Unlimited,
                                            UsePercentage = false,
                                            DiscountAmount = 10,
                                            RequiresCouponCode = true,
                                            CouponCode = "123",
                                        },
                                    new Discount
                                        {
                                            Name = "'20% order total' discount",
                                            DiscountType = DiscountType.AssignedToOrderTotal,
                                            DiscountLimitation = DiscountLimitationType.Unlimited,
                                            UsePercentage = true,
                                            DiscountPercentage = 20,
                                            StartDateUtc = new DateTime(2010,1,1),
                                            EndDateUtc = new DateTime(2020,1,1),
                                            RequiresCouponCode = true,
                                            CouponCode = "456",
                                        },
                                    new Discount
                                        {
                                            Name = "test discount 1. TODO remove",
                                            DiscountType = DiscountType.AssignedToCategories,
                                            DiscountLimitation = DiscountLimitationType.Unlimited,
                                            UsePercentage = false,
                                            DiscountAmount = 10,
                                            StartDateUtc = new DateTime(2010,1,1),
                                            EndDateUtc = new DateTime(2020,1,1),
                                        },
                                    new Discount
                                        {
                                            Name = "test discount 2. TODO remove",
                                            DiscountType = DiscountType.AssignedToCategories,
                                            DiscountLimitation = DiscountLimitationType.Unlimited,
                                            UsePercentage = false,
                                            DiscountAmount = 10,
                                            StartDateUtc = new DateTime(2010,1,1),
                                            EndDateUtc = new DateTime(2020,1,1),
                                        },
                                    new Discount
                                        {
                                            Name = "test discount 3. TODO remove",
                                            DiscountType = DiscountType.AssignedToCategories,
                                            DiscountLimitation = DiscountLimitationType.Unlimited,
                                            UsePercentage = false,
                                            DiscountAmount = 10,
                                            StartDateUtc = new DateTime(2010,1,1),
                                            EndDateUtc = new DateTime(2020,1,1),
                                        },
                                    new Discount
                                        {
                                            Name = "test discount 4. TODO remove",
                                            DiscountType = DiscountType.AssignedToSkus,
                                            DiscountLimitation = DiscountLimitationType.Unlimited,
                                            UsePercentage = false,
                                            DiscountAmount = 10,
                                            StartDateUtc = new DateTime(2010,1,1),
                                            EndDateUtc = new DateTime(2020,1,1),
                                        },
                                    new Discount
                                        {
                                            Name = "test discount 5. TODO remove",
                                            DiscountType = DiscountType.AssignedToSkus,
                                            DiscountLimitation = DiscountLimitationType.Unlimited,
                                            UsePercentage = false,
                                            DiscountAmount = 10,
                                            StartDateUtc = new DateTime(2010,1,1),
                                            EndDateUtc = new DateTime(2020,1,1),
                                        },
                                };
            disounts.ForEach(d => _discountRepository.Insert(d));

        }

        #endregion

        #region Methods

        public void InstallData(bool installSampleData = true)
        {
            InstallMeasures();
            InstallTaxCategories();
            InstallLanguagesAndResources();
            InstallCurrencies();
            InstallCountriesAndStates();
            InstallShippingMethods();
            InstallCustomersAndUsers();
            InstallEmailAccounts();
            InstallQueuedEmails(); //TODo remove. Just for testing
            InstallSettings();

            if (installSampleData)
            {
                InstallSpecificationAttributes();
                InstallCategories();
                InstallManufacturers();
                InstallProducts();
                InstallForums();
                InstallDiscounts();
            }
        }

        #endregion
    }
}