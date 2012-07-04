using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain;
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
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Infrastructure;
using Nop.Core.IO;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.Localization;

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
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<RelatedProduct> _relatedProductRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<ForumGroup> _forumGroupRepository;
        private readonly IRepository<Forum> _forumRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IRepository<ProductTemplate> _productTemplateRepository;
        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public InstallationService(IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IRepository<Language> languageRepository,
            IRepository<Currency> currencyRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<Category> categoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<Forum> forumRepository,
            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<Discount> discountRepository,
            IRepository<BlogPost> blogPostRepository,
            IRepository<Topic> topicRepository,
            IRepository<NewsItem> newsItemRepository,
            IRepository<Poll> pollRepository,
            IRepository<ShippingMethod> shippingMethodRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<ProductTemplate> productTemplateRepository,
            IRepository<CategoryTemplate> categoryTemplateRepository,
            IRepository<ManufacturerTemplate> manufacturerTemplateRepository,
            IRepository<ScheduleTask> scheduleTaskRepository,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper)
        {
            this._measureDimensionRepository = measureDimensionRepository;
            this._measureWeightRepository = measureWeightRepository;
            this._taxCategoryRepository = taxCategoryRepository;
            this._languageRepository = languageRepository;
            this._currencyRepository = currencyRepository;
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._specificationAttributeRepository = specificationAttributeRepository;
            this._productAttributeRepository = productAttributeRepository;
            this._categoryRepository = categoryRepository;
            this._manufacturerRepository = manufacturerRepository;
            this._productRepository = productRepository;
            this._relatedProductRepository = relatedProductRepository;
            this._emailAccountRepository = emailAccountRepository;
            this._messageTemplateRepository = messageTemplateRepository;
            this._forumGroupRepository = forumGroupRepository;
            this._forumRepository = forumRepository;
            this._countryRepository = countryRepository;
            this._stateProvinceRepository = stateProvinceRepository;
            this._discountRepository = discountRepository;
            this._blogPostRepository = blogPostRepository;
            this._topicRepository = topicRepository;
            this._newsItemRepository = newsItemRepository;
            this._pollRepository = pollRepository;
            this._shippingMethodRepository = shippingMethodRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._productTagRepository = productTagRepository;
            this._productTemplateRepository = productTemplateRepository;
            this._categoryTemplateRepository = categoryTemplateRepository;
            this._manufacturerTemplateRepository = manufacturerTemplateRepository;
            this._scheduleTaskRepository = scheduleTaskRepository;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
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
            var language = _languageRepository.Table.Where(l => l.Name == "English").Single();

            //save resoureces
            foreach (var filePath in System.IO.Directory.EnumerateFiles(_webHelper.MapPath("~/App_Data/Localization/"), "*.nopres.xml", SearchOption.TopDirectoryOnly))
            {
                #region Parse resource files (with <Children> elements)
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

                var parsedXml = sb.ToString();
                #endregion

                //now we have a parsed XML file (the same structure as exported language packs)
                //let's save resources
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, parsedXml);
            }

        }

        protected virtual void InstallCurrencies()
        {
            var currencies = new List<Currency>()
            {
                new Currency
                {
                    Name = "US Dollar",
                    CurrencyCode = "USD",
                    Rate = 1,
                    DisplayLocale = "en-US",
                    CustomFormatting = "",
                    Published = true,
                    DisplayOrder = 1,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Australian Dollar",
                    CurrencyCode = "AUD",
                    Rate = 0.94M,
                    DisplayLocale = "en-AU",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 2,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "British Pound",
                    CurrencyCode = "GBP",
                    Rate = 0.61M,
                    DisplayLocale = "en-GB",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 3,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Canadian Dollar",
                    CurrencyCode = "CAD",
                    Rate = 0.98M,
                    DisplayLocale = "en-CA",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 4,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Chinese Yuan Renminbi",
                    CurrencyCode = "CNY",
                    Rate = 6.48M,
                    DisplayLocale = "zh-CN",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 5,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Euro",
                    CurrencyCode = "EUR",
                    Rate = 0.79M,
                    DisplayLocale = "",
                    //CustomFormatting = "ˆ0.00",
                    CustomFormatting = string.Format("{0}0.00", "\u20ac"),
                    Published = true,
                    DisplayOrder = 6,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Hong Kong Dollar",
                    CurrencyCode = "HKD",
                    Rate = 7.75M,
                    DisplayLocale = "zh-HK",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 7,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Japanese Yen",
                    CurrencyCode = "JPY",
                    Rate = 80.07M,
                    DisplayLocale = "ja-JP",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 8,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Russian Rouble",
                    CurrencyCode = "RUB",
                    Rate = 27.7M,
                    DisplayLocale = "ru-RU",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 9,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Swedish Krona",
                    CurrencyCode = "SEK",
                    Rate = 6.19M,
                    DisplayLocale = "sv-SE",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 10,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Romanian Leu",
                    CurrencyCode = "RON",
                    Rate = 2.85M,
                    DisplayLocale = "ro-RO",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 11,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
            };
            currencies.ForEach(c => _currencyRepository.Insert(c));
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
                Published = true,
            };
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "AA (Armed Forces Americas)",
                Abbreviation = "AA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "AE (Armed Forces Europe)",
                Abbreviation = "AE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Alabama",
                Abbreviation = "AL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Alaska",
                Abbreviation = "AK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "American Samoa",
                Abbreviation = "AS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "AP (Armed Forces Pacific)",
                Abbreviation = "AP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Arizona",
                Abbreviation = "AZ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Arkansas",
                Abbreviation = "AR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "California",
                Abbreviation = "CA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Colorado",
                Abbreviation = "CO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Connecticut",
                Abbreviation = "CT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Delaware",
                Abbreviation = "DE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "District of Columbia",
                Abbreviation = "DC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Federated States of Micronesia",
                Abbreviation = "FM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Florida",
                Abbreviation = "FL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Georgia",
                Abbreviation = "GA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Guam",
                Abbreviation = "GU",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Hawaii",
                Abbreviation = "HI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Idaho",
                Abbreviation = "ID",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Illinois",
                Abbreviation = "IL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Indiana",
                Abbreviation = "IN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Iowa",
                Abbreviation = "IA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Kansas",
                Abbreviation = "KS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Kentucky",
                Abbreviation = "KY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Louisiana",
                Abbreviation = "LA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Maine",
                Abbreviation = "ME",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Marshall Islands",
                Abbreviation = "MH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Maryland",
                Abbreviation = "MD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Massachusetts",
                Abbreviation = "MA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Michigan",
                Abbreviation = "MI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Minnesota",
                Abbreviation = "MN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Mississippi",
                Abbreviation = "MS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Missouri",
                Abbreviation = "MO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Montana",
                Abbreviation = "MT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Nebraska",
                Abbreviation = "NE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Nevada",
                Abbreviation = "NV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New Hampshire",
                Abbreviation = "NH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New Jersey",
                Abbreviation = "NJ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New Mexico",
                Abbreviation = "NM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New York",
                Abbreviation = "NY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "North Carolina",
                Abbreviation = "NC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "North Dakota",
                Abbreviation = "ND",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Northern Mariana Islands",
                Abbreviation = "MP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Ohio",
                Abbreviation = "OH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Oklahoma",
                Abbreviation = "OK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Oregon",
                Abbreviation = "OR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Palau",
                Abbreviation = "PW",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Pennsylvania",
                Abbreviation = "PA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Puerto Rico",
                Abbreviation = "PR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Rhode Island",
                Abbreviation = "RI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "South Carolina",
                Abbreviation = "SC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "South Dakota",
                Abbreviation = "SD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Tennessee",
                Abbreviation = "TN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Texas",
                Abbreviation = "TX",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Utah",
                Abbreviation = "UT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Vermont",
                Abbreviation = "VT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Virgin Islands",
                Abbreviation = "VI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Virginia",
                Abbreviation = "VA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Washington",
                Abbreviation = "WA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "West Virginia",
                Abbreviation = "WV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Wisconsin",
                Abbreviation = "WI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Wyoming",
                Abbreviation = "WY",
                Published = true,
                DisplayOrder = 1,
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
                DisplayOrder = 2,
                Published = true,
            };
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Alberta",
                Abbreviation = "AB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "British Columbia",
                Abbreviation = "BC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Manitoba",
                Abbreviation = "MB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "New Brunswick",
                Abbreviation = "NB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Newfoundland and Labrador",
                Abbreviation = "NL",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Northwest Territories",
                Abbreviation = "NT",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Nova Scotia",
                Abbreviation = "NS",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Nunavut",
                Abbreviation = "NU",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Ontario",
                Abbreviation = "ON",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Prince Edward Island",
                Abbreviation = "PE",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Quebec",
                Abbreviation = "QC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Saskatchewan",
                Abbreviation = "SK",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Yukon Territory",
                Abbreviation = "YU",
                Published = true,
                DisplayOrder = 1,
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
	                                    Name = "Croatia (local Name: Hrvatska)",
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
	                                    Name = "Russia",
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

        protected virtual void InstallCustomersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new CustomerRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Administrators,
            };
            var crForumModerators = new CustomerRole
            {
                Name = "Forum Moderators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.ForumModerators,
            };
            var crRegistered = new CustomerRole
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Registered,
            };
            var crGuests = new CustomerRole
            {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Guests,
            };
            var customerRoles = new List<CustomerRole>
                                {
                                    crAdministrators,
                                    crForumModerators,
                                    crRegistered,
                                    crGuests
                                };
            customerRoles.ForEach(cr => _customerRoleRepository.Insert(cr));

            //admin user
            var adminUser = new Customer()
            {
                CustomerGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Password = defaultUserPassword,
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc= DateTime.UtcNow,
            };
            var defaultAdminUserAddress = new Address()
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
                CreatedOnUtc = DateTime.UtcNow,
            };
            adminUser.Addresses.Add(defaultAdminUserAddress);
            adminUser.BillingAddress = defaultAdminUserAddress;
            adminUser.ShippingAddress = defaultAdminUserAddress;
            adminUser.CustomerRoles.Add(crAdministrators);
            adminUser.CustomerRoles.Add(crForumModerators);
            adminUser.CustomerRoles.Add(crRegistered);
            _customerRepository.Insert(adminUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.FirstName, "John");
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.LastName, "Smith");


            //search engine (crawler) built-in user
            var searchEngineUser = new Customer()
            {
                Email = "builtin@search_engine_record.com",
                CustomerGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.SearchEngine,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };
            searchEngineUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(searchEngineUser);
        }

        protected virtual void HashDefaultCustomerPassword(string defaultUserEmail, string defaultUserPassword)
        {
            var customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));
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

        protected virtual void InstallMessageTemplates()
        {
            var eaGeneral = _emailAccountRepository.Table.Where(ea => ea.DisplayName.Equals("General contact")).FirstOrDefault();
            //var eaSale = _emailAccountRepository.Table.Where(ea => ea.DisplayName.Equals("Sales representative")).FirstOrDefault();
            //var eaCustomer = _emailAccountRepository.Table.Where(ea => ea.DisplayName.Equals("Customer support")).FirstOrDefault();
            var messageTemplates = new List<MessageTemplate>
                               {
                                   new MessageTemplate
                                       {
                                           Name = "Blog.BlogComment",
                                           Subject = "%Store.Name%. New blog comment.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.BackInStock",
                                           Subject = "%Store.Name%. Back in stock notification",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Customer.FullName%, <br />Product \"%BackInStockSubscription.ProductName%\" is in stock.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.EmailValidationMessage",
                                           Subject = "%Store.Name%. Email validation",
                                           Body = "<a href=\"%Store.URL%\">%Store.Name%</a>  <br />  <br />  To activate your account <a href=\"%Customer.AccountActivationURL%\">click here</a>.     <br />  <br />  %Store.Name%",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.NewPM",
                                           Subject = "%Store.Name%. You have received a new private message",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />You have received a new private message.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.PasswordRecovery",
                                           Subject = "%Store.Name%. Password recovery",
                                           Body = "<a href=\"%Store.URL%\">%Store.Name%</a>  <br />  <br />  To change your password <a href=\"%Customer.PasswordRecoveryURL%\">click here</a>.     <br />  <br />  %Store.Name%",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.WelcomeMessage",
                                           Subject = "Welcome to %Store.Name%",
                                           Body = "We welcome you to <a href=\"%Store.URL%\"> %Store.Name%</a>.<br /><br />You can now take part in the various services we have to offer you. Some of these services include:<br /><br />Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.<br />Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.<br />Order History - View your history of purchases that you have made with us.<br />Products Reviews - Share your opinions on products with our other customers.<br /><br />For help with any of our online services, please email the store-owner: <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.<br /><br />Note: This email address was given to us by one of our customers. If you did not signup to be a member, please send an email to <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Forums.NewForumPost",
                                           Subject = "%Store.Name%. New Post Notification.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.<br /><br />Click <a href=\"%Forums.TopicURL%\">here</a> for more info.<br /><br />Post author: %Forums.PostAuthor%<br />Post body: %Forums.PostBody%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Forums.NewForumTopic",
                                           Subject = "%Store.Name%. New Topic Notification.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.<br /><br />Click <a href=\"%Forums.TopicURL%\">here</a> for more info.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "GiftCard.Notification",
                                           Subject = "%GiftCard.SenderName% has sent you a gift card for %Store.Name%",
                                           Body = "<p>You have received a gift card for %Store.Name%</p><p>Dear %GiftCard.RecipientName%, <br /><br />%GiftCard.SenderName% (%GiftCard.SenderEmail%) has sent you a %GiftCard.Amount% gift cart for <a href=\"%Store.URL%\"> %Store.Name%</a></p><p>You gift card code is %GiftCard.CouponCode%</p><p>%GiftCard.Message%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "NewCustomer.Notification",
                                           Subject = "New customer registration",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new customer registered with your store. Below are the customer's details:<br />Full name: %Customer.FullName%<br />Email: %Customer.Email%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "NewReturnRequest.StoreOwnerNotification",
                                           Subject = "%Store.Name%. New return request.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Customer.FullName% has just submitted a new return request. Details are below:<br />Request ID: %ReturnRequest.ID%<br />Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%<br />Reason for return: %ReturnRequest.Reason%<br />Requested action: %ReturnRequest.RequestedAction%<br />Customer comments:<br />%ReturnRequest.CustomerComment%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "News.NewsComment",
                                           Subject = "%Store.Name%. New news comment.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new news comment has been created for news \"%NewsComment.NewsTitle%\".</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "NewsLetterSubscription.ActivationMessage",
                                           Subject = "%Store.Name%. Subscription activation message.",
                                           Body = "<p><a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a></p><p>If you received this email by mistake, simply delete it.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "NewsLetterSubscription.DeactivationMessage",
                                           Subject = "%Store.Name%. Subscription deactivation message.",
                                           Body = "<p><a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from news letters.</a></p><p>If you received this email by mistake, simply delete it.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "NewVATSubmitted.StoreOwnerNotification",
                                           Subject = "New VAT number is submitted.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Customer.FullName% (%Customer.Email%) has just submitted a new VAT number. Details are below:<br />VAT number: %Customer.VatNumber%<br />VAT number status: %Customer.VatNumberStatus%<br />Received name: %VatValidationResult.Name%<br />Received address: %VatValidationResult.Address%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderCancelled.CustomerNotification",
                                           Subject = "%Store.Name%. Your order cancelled",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Your order has been cancelled. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderCompleted.CustomerNotification",
                                           Subject = "%Store.Name%. Your order completed",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Your order has been completed. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "ShipmentDelivered.CustomerNotification",
                                           Subject = "Your order from %Store.Name% has been delivered.",
                                           Body = "<p><a href=\"%Store.URL%\"> %Store.Name%</a> <br /> <br /> Hello %Order.CustomerFullName%, <br /> Good news! You order has been delivered. <br /> Order Number: %Order.OrderNumber%<br /> Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a><br /> Date Ordered: %Order.CreatedOn%<br /> <br /> <br /> <br /> Billing Address<br /> %Order.BillingFirstName% %Order.BillingLastName%<br /> %Order.BillingAddress1%<br /> %Order.BillingCity% %Order.BillingZipPostalCode%<br /> %Order.BillingStateProvince% %Order.BillingCountry%<br /> <br /> <br /> <br /> Shipping Address<br /> %Order.ShippingFirstName% %Order.ShippingLastName%<br /> %Order.ShippingAddress1%<br /> %Order.ShippingCity% %Order.ShippingZipPostalCode%<br /> %Order.ShippingStateProvince% %Order.ShippingCountry%<br /> <br /> Shipping Method: %Order.ShippingMethod% <br /> <br /> Delivered Products: <br /> <br /> %Shipment.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderPlaced.CustomerNotification",
                                           Subject = "Order receipt from %Store.Name%.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderPlaced.StoreOwnerNotification",
                                           Subject = "%Store.Name%. Purchase Receipt for Order #%Order.OrderNumber%",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Order.CustomerFullName% (%Order.CustomerEmail%) has just placed an order from your store. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "ShipmentSent.CustomerNotification",
                                           Subject = "Your order from %Store.Name% has been shipped.",
                                           Body = "<p><a href=\"%Store.URL%\"> %Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%!, <br />Good news! You order has been shipped. <br />Order Number: %Order.OrderNumber%<br />Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod% <br /> <br /> Shipped Products: <br /> <br /> %Shipment.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Product.ProductReview",
                                           Subject = "%Store.Name%. New product review.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new product review has been written for product \"%ProductReview.ProductName%\".</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "QuantityBelow.StoreOwnerNotification",
                                           Subject = "%Store.Name%. Quantity below notification. %ProductVariant.FullProductName%",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%ProductVariant.FullProductName% (ID: %ProductVariant.ID%) low quantity. <br /><br />Quantity: %ProductVariant.StockQuantity%<br /></p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "ReturnRequestStatusChanged.CustomerNotification",
                                           Subject = "%Store.Name%. Return request status was changed.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Customer.FullName%,<br />Your return request #%ReturnRequest.ID% status has been changed.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Service.EmailAFriend",
                                           Subject = "%Store.Name%. Referred Item",
                                           Body = "<p><a href=\"%Store.URL%\"> %Store.Name%</a> <br /><br />%EmailAFriend.Email% was shopping on %Store.Name% and wanted to share the following item with you. <br /><br /><b><a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">%Product.Name%</a></b> <br />%Product.ShortDescription% <br /><br />For more info click <a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">here</a> <br /><br /><br />%EmailAFriend.PersonalMessage%<br /><br />%Store.Name%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Wishlist.EmailAFriend",
                                           Subject = "%Store.Name%. Wishlist",
                                           Body = "<p><a href=\"%Store.URL%\"> %Store.Name%</a> <br /><br />%Wishlist.Email% was shopping on %Store.Name% and wanted to share a wishlist with you. <br /><br /><br />For more info click <a target=\"_blank\" href=\"%Wishlist.URLForCustomer%\">here</a> <br /><br /><br />%Wishlist.PersonalMessage%<br /><br />%Store.Name%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.NewOrderNote",
                                           Subject = "%Store.Name%. New order note has been added",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Customer.FullName%, <br />New order note has been added to your account:<br />\"%Order.NewNoteText%\".<br /><a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a></p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "RecurringPaymentCancelled.StoreOwnerNotification",
                                           Subject = "%Store.Name%. Recurring payment cancelled",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Customer.FullName% (%Customer.Email%) has just cancelled a recurring payment ID=%RecurringPayment.ID%.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                               };
            messageTemplates.ForEach(mt => _messageTemplateRepository.Insert(mt));

        }

        protected virtual void InstallTopics()
        {
            var topics = new List<Topic>
                               {
                                   new Topic
                                       {
                                           SystemName = "AboutUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "About Us",
                                           Body = "<p>Put your &quot;About Us&quot; information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "CheckoutAsGuestOrRegister",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "",
                                           Body = "<p><strong>Register and save time!</strong><br />Register with us for future convenience:</p><ul><li>Fast and easy check out</li><li>Easy access to your order history and status</li></ul>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "ConditionsOfUse",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Conditions of use",
                                           Body = "<p>Put your conditions of use information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "ContactUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "",
                                           Body = "<p>Put your contact information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "ForumWelcomeMessage",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Forums",
                                           Body = "<p>Put your welcome message here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "HomePageText",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Welcome to our store",
                                           Body = "<p>Online shopping is the process consumers go through to purchase products or services over the Internet. You can edit this in the admin site.</p><p>If you have questions, see the <a href=\"http://www.nopcommerce.com/documentation.aspx\">Documentation</a>, or post in the <a href=\"http://www.nopcommerce.com/boards/\">Forums</a> at <a href=\"http://www.nopcommerce.com\">nopCommerce.com</a></p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "LoginRegistrationInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "About login / registration",
                                           Body = "<p>Put your login / registration information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "PrivacyInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Privacy policy",
                                           Body = "<p>Put your privacy policy information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "ShippingInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Shipping & Returns",
                                           Body = "<p>Put your shipping &amp; returns information here. You can edit this in the admin site.</p>"
                                       },
                               };
            topics.ForEach(t => _topicRepository.Insert(t));

        }

        protected virtual void InstallSettings()
        {
            EngineContext.Current.Resolve<IConfigurationProvider<PdfSettings>>()
                .SaveSettings(new PdfSettings()
                {
                    Enabled = true,
                    LetterPageSizeEnabled = false,
                    RenderOrderNotes = true,
                    FontFileName = "FreeSerif.ttf",
                });

            EngineContext.Current.Resolve<IConfigurationProvider<CommonSettings>>()
                .SaveSettings(new CommonSettings()
                {
                    UseSystemEmailForContactUsForm = true,
                    UseStoredProceduresIfSupported = true,
                    SitemapEnabled = true,
                    SitemapIncludeCategories = true,
                    SitemapIncludeManufacturers = true,
                    SitemapIncludeProducts = false,
                    SitemapIncludeTopics = true,
                    DisplayJavaScriptDisabledWarning = false,
                    UseFullTextSearch = false,
                    FullTextMode = FulltextSearchMode.ExactMatch,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<SeoSettings>>()
                .SaveSettings(new SeoSettings()
                {
                    PageTitleSeparator = ". ",
                    PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
                    DefaultTitle = "Your store",
                    DefaultMetaKeywords = "",
                    DefaultMetaDescription = "",
                    ConvertNonWesternChars = false,
                    AllowUnicodeCharsInUrls = true,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<AdminAreaSettings>>()
                .SaveSettings(new AdminAreaSettings()
                {
                    GridPageSize = 15,
                    DisplayProductPictures = true,
                });
            
            EngineContext.Current.Resolve<IConfigurationProvider<CatalogSettings>>()
                .SaveSettings(new CatalogSettings()
                {
                    ShowProductSku = false,
                    ShowManufacturerPartNumber = false,
                    AllowProductSorting = true,
                    AllowProductViewModeChanging = true,
                    DefaultViewMode = "grid",
                    ShowProductsFromSubcategories = false,
                    ShowCategoryProductNumber = false,
                    ShowCategoryProductNumberIncludingSubcategories = false,
                    CategoryBreadcrumbEnabled = true,
                    ShowShareButton = true,
                    PageShareCode = "<!-- AddThis Button BEGIN --><div class=\"addthis_toolbox addthis_default_style \"><a class=\"addthis_button_preferred_1\"></a><a class=\"addthis_button_preferred_2\"></a><a class=\"addthis_button_preferred_3\"></a><a class=\"addthis_button_preferred_4\"></a><a class=\"addthis_button_compact\"></a><a class=\"addthis_counter addthis_bubble_style\"></a></div><script type=\"text/javascript\" src=\"http://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions\"></script><!-- AddThis Button END -->",
                    ProductReviewsMustBeApproved = true,
                    DefaultProductRatingValue = 5,
                    AllowAnonymousUsersToReviewProduct = false,
                    NotifyStoreOwnerAboutNewProductReviews = false,
                    EmailAFriendEnabled = true,
                    AllowAnonymousUsersToEmailAFriend = false,
                    RecentlyViewedProductsNumber = 4,
                    RecentlyViewedProductsEnabled = true,
                    RecentlyAddedProductsNumber = 4,
                    RecentlyAddedProductsEnabled = true,
                    CompareProductsEnabled = true,
                    ProductSearchAutoCompleteEnabled = true,
                    ProductSearchAutoCompleteNumberOfProducts = 10,
                    ProductSearchTermMinimumLength = 3,
                    ShowProductImagesInSearchAutoComplete = false,
                    ShowBestsellersOnHomepage = false,
                    NumberOfBestsellersOnHomepage = 3,
                    SearchPageProductsPerPage = 6,
                    ProductsAlsoPurchasedEnabled = true,
                    ProductsAlsoPurchasedNumber = 3,
                    EnableDynamicPriceUpdate = false,
                    NumberOfProductTags = 15,
                    ProductsByTagPageSize = 4,
                    IncludeShortDescriptionInCompareProducts = false,
                    IncludeFullDescriptionInCompareProducts = false,
                    UseSmallProductBoxOnHomePage =  true,
                    IncludeFeaturedProductsInNormalLists = false,
                    DisplayTierPricesWithDiscounts = true,
                    IgnoreDiscounts = false,
                    IgnoreFeaturedProducts = false,
                    DefaultCategoryPageSizeOptions = "4, 2, 8, 12",
                    DefaultManufacturerPageSizeOptions = "4, 2, 8, 12",
                    ProductsByTagAllowCustomersToSelectPageSize = true,
                    ProductsByTagPageSizeOptions = "4, 2, 8, 12",
                    MaximumBackInStockSubscriptions = 200,
                    FileUploadMaximumSizeBytes = 1024 * 200, //200KB
                    ManufacturersBlockItemsToDisplay = 5,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<LocalizationSettings>>()
                .SaveSettings(new LocalizationSettings()
                {
                    DefaultAdminLanguageId = _languageRepository.Table.Where(l => l.Name == "English").Single().Id,
                    UseImagesForLanguageSelection = false,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<CustomerSettings>>()
                .SaveSettings(new CustomerSettings()
                {
                    UsernamesEnabled = false,
                    CheckUsernameAvailabilityEnabled = false,
                    AllowUsersToChangeUsernames = false,
                    DefaultPasswordFormat = PasswordFormat.Hashed,
                    HashedPasswordFormat = "SHA1",
                    PasswordMinLength = 6,
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
                    CustomerNameFormat = CustomerNameFormat.ShowEmails,
                    GenderEnabled = true,
                    DateOfBirthEnabled = true,
                    CompanyEnabled = true,
                    StreetAddressEnabled = false,
                    StreetAddress2Enabled = false,
                    ZipPostalCodeEnabled = false,
                    CityEnabled = false,
                    CountryEnabled = false,
                    StateProvinceEnabled = false,
                    PhoneEnabled = false,
                    FaxEnabled = false,
                    NewsletterEnabled = true,
                    HideNewsletterBlock = false,
                    OnlineCustomerMinutes = 20,
                    StoreLastVisitedPage = true,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<MediaSettings>>()
                .SaveSettings(new MediaSettings()
                {
                    AvatarPictureSize = 85,
                    ProductThumbPictureSize = 125,
                    ProductDetailsPictureSize = 300,
                    ProductThumbPictureSizeOnProductDetailsPage = 70,
                    ProductVariantPictureSize = 125,
                    CategoryThumbPictureSize = 125,
                    ManufacturerThumbPictureSize = 125,
                    CartThumbPictureSize = 80,
                    MiniCartThumbPictureSize = 47,
                    AutoCompleteSearchThumbPictureSize = 20,
                    MaximumImageSize = 1280,
                    DefaultPictureZoomEnabled = false,
                    DefaultImageQuality = 100,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<StoreInformationSettings>>()
                .SaveSettings(new StoreInformationSettings()
                {
                    StoreName = "Your store name",
                    StoreUrl = "http://www.yourStore.com/",
                    StoreClosed = false,
                    StoreClosedAllowForAdmins = false,
                    DefaultStoreThemeForDesktops = "DarkOrange",
                    AllowCustomerToSelectTheme = true,
                    MobileDevicesSupported = false,
                    DefaultStoreThemeForMobileDevices = "Mobile",
                    EmulateMobileDevice = false,
                    DisplayMiniProfilerInPublicStore = false,
                    DisplayEuCookieLawWarning = false,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<RewardPointsSettings>>()
                .SaveSettings(new RewardPointsSettings()
                {
                    Enabled = false,
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
                    ActiveExchangeRateProviderSystemName = "CurrencyExchange.MoneyConverter",
                    AutoUpdateEnabled = false,
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
                    CaseInvariantReplacement = false,
                    Color1 = "#b9babe",
                    Color2 = "#ebecee",
                    Color3 = "#dde2e6",
                });

            EngineContext.Current.Resolve<IConfigurationProvider<ShoppingCartSettings>>()
                .SaveSettings(new ShoppingCartSettings()
                {
                    DisplayCartAfterAddingProduct = false,
                    DisplayWishlistAfterAddingProduct = false,
                    MaximumShoppingCartItems = 1000,
                    MaximumWishlistItems = 1000,
                    AllowOutOfStockItemsToBeAddedToWishlist = false,
                    ShowProductImagesOnShoppingCart = true,
                    ShowProductImagesOnWishList = true,
                    ShowDiscountBox = true,
                    ShowGiftCardBox = true,
                    CrossSellsNumber = 2,
                    EmailWishlistEnabled = true,
                    AllowAnonymousUsersToEmailWishlist = false,
                    MiniShoppingCartEnabled = true,
                    ShowProductImagesInMiniShoppingCart = true,
                    MiniShoppingCartProductNumber = 5,
                    RoundPricesDuringCalculation = true,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<OrderSettings>>()
                .SaveSettings(new OrderSettings()
                {
                    IsReOrderAllowed = true,
                    MinOrderSubtotalAmount = 0,
                    MinOrderTotalAmount = 0,
                    AnonymousCheckoutAllowed = true,
                    TermsOfServiceEnabled = false,
                    OnePageCheckoutEnabled = true,
                    ReturnRequestsEnabled = true,
                    ReturnRequestActions = new List<string>() { "Repair", "Replacement", "Store Credit" },
                    ReturnRequestReasons = new List<string>() { "Received Wrong Product", "Wrong Product Ordered", "There Was A Problem With The Product" },
                    NumberOfDaysReturnRequestAvailable = 365,
                    MinimumOrderPlacementInterval = 30,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<SecuritySettings>>()
                .SaveSettings(new SecuritySettings()
                {
                    ForceSslForAllPages = false,
                    EncryptionKey = "273ece6f97dd844d",
                    AdminAreaAllowedIpAddresses = null
                });

            EngineContext.Current.Resolve<IConfigurationProvider<ShippingSettings>>()
                .SaveSettings(new ShippingSettings()
                {
                    ActiveShippingRateComputationMethodSystemNames = new List<string>() { "Shipping.FixedRate" },
                    FreeShippingOverXEnabled = false,
                    FreeShippingOverXValue = decimal.Zero,
                    FreeShippingOverXIncludingTax = false,
                    EstimateShippingEnabled = true,
                    DisplayShipmentEventsToCustomers = false,
                    ReturnValidOptionsIfThereAreAny = true,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<PaymentSettings>>()
                .SaveSettings(new PaymentSettings()
                {
                    ActivePaymentMethodSystemNames = new List<string>() 
                    { 
                        "Payments.CashOnDelivery",
                        "Payments.CheckMoneyOrder",
                        "Payments.Manual",
                        "Payments.PayInStore",
                        "Payments.PurchaseOrder",
                    },
                    AllowRePostingPayments = true,
                    BypassPaymentMethodSelectionIfOnlyOne = true,
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

            EngineContext.Current.Resolve<IConfigurationProvider<BlogSettings>>()
                .SaveSettings(new BlogSettings()
                {
                    Enabled = true,
                    PostsPageSize = 10,
                    AllowNotRegisteredUsersToLeaveComments = true,
                    NotifyAboutNewBlogComments = false,
                    NumberOfTags = 15,
                    ShowHeaderRssUrl = false,
                });
            EngineContext.Current.Resolve<IConfigurationProvider<NewsSettings>>()
                .SaveSettings(new NewsSettings()
                {
                    Enabled = true,
                    AllowNotRegisteredUsersToLeaveComments = true,
                    NotifyAboutNewNewsComments = false,
                    ShowNewsOnMainPage = true,
                    MainPageNewsCount = 3,
                    NewsArchivePageSize = 10,
                    ShowHeaderRssUrl = false,
                });
            EngineContext.Current.Resolve<IConfigurationProvider<ForumSettings>>()
                .SaveSettings(new ForumSettings()
                {
                    ForumsEnabled = false,
                    RelativeDateTimeFormattingEnabled = true,
                    AllowCustomersToDeletePosts = false,
                    AllowCustomersToEditPosts = false,
                    AllowCustomersToManageSubscriptions = false,
                    AllowGuestsToCreatePosts = false,
                    AllowGuestsToCreateTopics = false,
                    TopicSubjectMaxLength = 450,
                    PostMaxLength = 4000,
                    StrippedTopicMaxLength = 45,
                    TopicsPageSize = 10,
                    PostsPageSize = 10,
                    SearchResultsPageSize = 10,
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
                    HomePageActiveDiscussionsTopicCount = 5,
                    ActiveDiscussionsPageTopicCount = 50,
                    ActiveDiscussionsFeedEnabled = false,
                    ActiveDiscussionsFeedCount = 25,
                    ForumFeedsEnabled = false,
                    ForumFeedCount = 10,
                    ForumSearchTermMinimumLength = 3,
                });

            EngineContext.Current.Resolve<IConfigurationProvider<EmailAccountSettings>>()
                .SaveSettings(new EmailAccountSettings()
                {
                    DefaultEmailAccountId = _emailAccountRepository.Table.FirstOrDefault().Id
                });
        }

        protected virtual void InstallSpecificationAttributes()
        {
            var sa1 = new SpecificationAttribute
            {
                Name = "Screensize",
                DisplayOrder = 1,
            };
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "10.0''",
                DisplayOrder = 3,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "14.1''",
                DisplayOrder = 4,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "15.4''",
                DisplayOrder = 5,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "16.0''",
                DisplayOrder = 6,
            });
            var sa2 = new SpecificationAttribute
            {
                Name = "CPU Type",
                DisplayOrder = 2,
            };
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "AMD",
                DisplayOrder = 1,
            });
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "Intel",
                DisplayOrder = 2,
            });
            var sa3 = new SpecificationAttribute
            {
                Name = "Memory",
                DisplayOrder = 3,
            };
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "1 GB",
                DisplayOrder = 1,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "3 GB",
                DisplayOrder = 2,
            });
            var sa4 = new SpecificationAttribute
            {
                Name = "Hardrive",
                DisplayOrder = 5,
            };
            sa4.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "320 GB",
                DisplayOrder = 7,
            });
            sa4.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "250 GB",
                DisplayOrder = 4,
            });
            sa4.SpecificationAttributeOptions.Add(new SpecificationAttributeOption()
            {
                Name = "160 GB",
                DisplayOrder = 3,
            });
            var specificationAttributes = new List<SpecificationAttribute>
                                {
                                    sa1,
                                    sa2,
                                    sa3,
                                    sa4
                                };
            specificationAttributes.ForEach(sa => _specificationAttributeRepository.Insert(sa));

        }

        protected virtual void InstallProductAttributes()
        {
            var productAttributes = new List<ProductAttribute>
            {
                new ProductAttribute
                {
                    Name = "Color",
                },
                new ProductAttribute
                {
                    Name = "Custom Text",
                },
                new ProductAttribute
                {
                    Name = "HDD",
                },
                new ProductAttribute
                {
                    Name = "OS",
                },
                new ProductAttribute
                {
                    Name = "Processor",
                },
                new ProductAttribute
                {
                    Name = "RAM",
                },
                new ProductAttribute
                {
                    Name = "Size",
                },
                new ProductAttribute
                {
                    Name = "Software",
                },
            };
            productAttributes.ForEach(pa => _productAttributeRepository.Insert(pa));

        }

        protected virtual void InstallCategories()
        {
            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = _webHelper.MapPath("~/content/samples/");



            var categoryTemplateInGridAndLines =
                _categoryTemplateRepository.Table.Where(pt => pt.Name == "Products in Grid or Lines").FirstOrDefault();



            //categories
            var categoryBooks = new Category
            {
                Name = "Books",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Books, Dictionary, Textbooks",
                MetaDescription = "Books category description",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_book.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Book"), true).Id,
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
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_computers.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Computers"), true).Id,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryComputers);


            var categoryDesktops = new Category
            {
                Name = "Desktops",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_desktops.jpg"), "image/pjpeg", pictureService.GetPictureSeName("Desktops"), true).Id,
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
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_notebooks.jpg"), "image/pjpeg", pictureService.GetPictureSeName("Notebooks"), true).Id,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryNotebooks);


            var categoryAccessories = new Category
            {
                Name = "Accessories",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_accessories.jpg"), "image/pjpeg", pictureService.GetPictureSeName("Accessories"), true).Id,
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
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_software.jpg"), "image/pjpeg", pictureService.GetPictureSeName("Software"), true).Id,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categorySoftware);


            var categoryGames = new Category
            {
                Name = "Games",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_games.jpg"), "image/pjpeg", pictureService.GetPictureSeName("Games"), true).Id,
                Published = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryGames);



            var categoryElectronics = new Category
            {
                Name = "Electronics",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_electronics.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Electronics"), true).Id,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryElectronics);


            var categoryCameraPhoto = new Category
            {
                Name = "Camera, photo",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_camera_photo.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Camera, photo"), true).Id,
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
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_cell_phones.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Cell phones"), true).Id,
                Published = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryCellPhones);


            var categoryApparelShoes = new Category
            {
                Name = "Apparel & Shoes",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_apparel_shoes.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Apparel & Shoes"), true).Id,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryApparelShoes);


            var categoryShirts = new Category
            {
                Name = "Shirts",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryApparelShoes.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_shirts.jpg"), "image/pjpeg", pictureService.GetPictureSeName("Shirts"), true).Id,
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
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryApparelShoes.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_jeans.jpg"), "image/pjpeg", pictureService.GetPictureSeName("Jeans"), true).Id,
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
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryApparelShoes.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_shoes.jpg"), "image/pjpeg", pictureService.GetPictureSeName("Shoes"), true).Id,
                PriceRanges = "-20;20-;",
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryShoes);


            var categoryAccessoriesShoes = new Category
            {
                Name = "Apparel accessories",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ParentCategoryId = categoryApparelShoes.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_accessories_apparel.jpg"), "image/pjpeg", pictureService.GetPictureSeName("Apparel accessories"), true).Id,
                PriceRanges = "-30;30-;",
                Published = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryAccessoriesShoes);


            var categoryDigitalDownloads = new Category
            {
                Name = "Digital downloads",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_digital_downloads.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Digital downloads"), true).Id,
                Published = true,
                DisplayOrder = 6,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryDigitalDownloads);


            var categoryJewelry = new Category
            {
                Name = "Jewelry",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_jewelry.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Jewelry"), true).Id,
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
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_gift_cards.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Gift Cards"), true).Id,
                Published = true,
                DisplayOrder = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _categoryRepository.Insert(categoryGiftCards);
        }

        protected virtual void InstallManufacturers()
        {
            var manufacturerTemplateInGridAndLines =
                _manufacturerTemplateRepository.Table.Where(pt => pt.Name == "Products in Grid or Lines").FirstOrDefault();


            var manufacturerAsus = new Manufacturer
            {
                Name = "ASUS",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _manufacturerRepository.Insert(manufacturerAsus);

            var manufacturerHp = new Manufacturer
            {
                Name = "HP",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            _manufacturerRepository.Insert(manufacturerHp);
        }

        protected virtual void InstallProducts()
        {
            var productTemplateInGrid =
                _productTemplateRepository.Table.Where(pt => pt.Name == "Variants in Grid").FirstOrDefault();
            var productTemplateSingleVariant =
                _productTemplateRepository.Table.Where(pt => pt.Name == "Single Product Variant").FirstOrDefault();
            
            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = _webHelper.MapPath("~/content/samples/");

            //downloads
            var downloadService = EngineContext.Current.Resolve<IDownloadService>();
            var sampleDownloadsPath = _webHelper.MapPath("~/content/samples/");


            //products
            var product5GiftCard = new Product()
            {
                Name = "$5 Virtual Gift Card",
                ShortDescription = "$5 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            product5GiftCard.ProductVariants.Add(new ProductVariant()
            {
                Price = 5M,
                IsGiftCard = true,
                GiftCardType = GiftCardType.Virtual,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            product5GiftCard.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Gift Cards").Single(),
                DisplayOrder = 1,
            });
            product5GiftCard.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_5giftcart.jpeg"), "image/jpeg", pictureService.GetPictureSeName(product5GiftCard.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(product5GiftCard);




            var product25GiftCard = new Product()
            {
                Name = "$25 Virtual Gift Card",
                ShortDescription = "$25 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                ShowOnHomePage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            product25GiftCard.ProductVariants.Add(new ProductVariant()
            {
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            product25GiftCard.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Gift Cards").Single(),
                DisplayOrder = 2,
            });
            product25GiftCard.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_25giftcart.jpeg"), "image/jpeg", pictureService.GetPictureSeName(product25GiftCard.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(product25GiftCard);





            var product50GiftCard = new Product()
            {
                Name = "$50 Physical Gift Card",
                ShortDescription = "$50 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            product50GiftCard.ProductVariants.Add(new ProductVariant()
            {
                Price = 50M,
                IsGiftCard = true,
                GiftCardType = GiftCardType.Physical,
                IsShipEnabled = true,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            product50GiftCard.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Gift Cards").Single(),
                DisplayOrder = 3,
            });
            product50GiftCard.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_50giftcart.jpeg"), "image/jpeg", pictureService.GetPictureSeName(product50GiftCard.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(product50GiftCard);





            var product100GiftCard = new Product()
            {
                Name = "$100 Physical Gift Card",
                ShortDescription = "$100 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            product100GiftCard.ProductVariants.Add(new ProductVariant()
            {
                Price = 100M,
                IsGiftCard = true,
                GiftCardType = GiftCardType.Physical,
                IsShipEnabled = true,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            product100GiftCard.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Gift Cards").Single(),
                DisplayOrder = 4,
            });
            product100GiftCard.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_100giftcart.jpeg"), "image/jpeg", pictureService.GetPictureSeName(product100GiftCard.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(product100GiftCard);





            var productRockabillyPolka = new Product()
            {
                Name = "50's Rockabilly Polka Dot Top JR Plus Size",
                ShortDescription = "",
                FullDescription = "<p>Fitted polkadot print cotton top with tie cap sleeves.</p>",
                ProductTemplateId = productTemplateInGrid.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productRockabillyPolka.ProductVariants.Add(new ProductVariant()
            {
                Price = 15M,
                IsShipEnabled = true,
                Weight = 1,
                Length = 2,
                Width = 3,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            var pvaRockabillyPolka1 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Size").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaRockabillyPolka1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "Small",
                DisplayOrder = 1,
            });
            pvaRockabillyPolka1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "1X",
                DisplayOrder = 2,
            });
            pvaRockabillyPolka1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "2X",
                DisplayOrder = 3,
            });
            pvaRockabillyPolka1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "3X",
                DisplayOrder = 4,
            });
            pvaRockabillyPolka1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "4X",
                DisplayOrder = 5,
            });
            pvaRockabillyPolka1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "5X",
                DisplayOrder = 6,
            });
            productRockabillyPolka.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaRockabillyPolka1);
            productRockabillyPolka.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Shirts").Single(),
                DisplayOrder = 1,
            });
            productRockabillyPolka.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_RockabillyPolka.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productRockabillyPolka.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productRockabillyPolka);





            var productAcerAspireOne = new Product()
            {
                Name = "Acer Aspire One 8.9\" Mini-Notebook Case - (Black)",
                ShortDescription = "Acer Aspire One 8.9\" Mini-Notebook and 6 Cell Battery model (AOA150-1447)",
                FullDescription = "<p>Acer Aspire One 8.9&quot; Memory Foam Pouch is the perfect fit for Acer Aspire One 8.9&quot;. This pouch is made out of premium quality shock absorbing memory form and it provides extra protection even though case is very light and slim. This pouch is water resistant and has internal supporting bands for Acer Aspire One 8.9&quot;. Made In Korea.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productAcerAspireOne.ProductVariants.Add(new ProductVariant()
            {
                Price = 21.6M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productAcerAspireOne.ProductVariants.FirstOrDefault().TierPrices.Add(new TierPrice()
            {
                Quantity = 2,
                Price = 19
            });
            productAcerAspireOne.ProductVariants.FirstOrDefault().TierPrices.Add(new TierPrice()
            {
                Quantity = 5,
                Price = 17
            });
            productAcerAspireOne.ProductVariants.FirstOrDefault().TierPrices.Add(new TierPrice()
            {
                Quantity = 10,
                Price = 15
            });
            productAcerAspireOne.ProductVariants.FirstOrDefault().HasTierPrices = true;

            productAcerAspireOne.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Accessories").Single(),
                DisplayOrder = 1,
            });
            productAcerAspireOne.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_AcerAspireOne_1.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productAcerAspireOne.Name), true),
                DisplayOrder = 1,
            });
            productAcerAspireOne.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_AcerAspireOne_2.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productAcerAspireOne.Name), true),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productAcerAspireOne);





            var productAdidasShoe = new Product()
            {
                Name = "adidas Women's Supernova CSH 7 Running Shoe",
                ShortDescription = "Now there are even more reasons to love this training favorite. An improved last, new step-in sockliner and the smooth control of 3-D ForMotion™ deliver a natural, balanced touchdown that feels better than ever.",
                FullDescription = "<p>Built to take you far and fast, Adidas Supernova Cushion 7 road-running shoes offer incredible cushioning and comfort with low weight. * Abrasion-resistant nylon mesh uppers are lightweight and highly breathable; synthetic leather overlays create structure and support * GeoFit construction at ankles provides an anatomically correct fit and extra comfort * Nylon linings and molded, antimicrobial dual-layer EVA footbeds dry quickly and fight odor * adiPRENE&reg; midsoles absorb shock in the heels and help maximize heel protection and stability * adiPRENE&reg;+ under forefeet retains natural propulsive forces for improved efficiency * Torsion&reg; system at the midfoot allows natural rotation between the rearfoot and the forefoot, helping improve surface adaptability * ForMotion&reg; freely moving, decoupled heel system allows your feet to adapt to the ground strike and adjust for forward momentum * adiWEAR&reg; rubber outsoles give ample durability in high-wear areas and offer lightweight grip and cushion Mens shoes , men's shoes , running shoes , adidas shoes , adidas running shoes , mens running shoes , snova running shoes , snova mens adidas , snova adidas running , snova shoes , sport shoes mens , sport shoes adidas , mens shoes , men's shoes , running , adidas</p>",
                ProductTemplateId = productTemplateInGrid.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productAdidasShoe.ProductVariants.Add(new ProductVariant()
            {
                Price = 40M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            var pvaAdidasShoe1 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Size").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaAdidasShoe1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "8",
                DisplayOrder = 1,
            });
            pvaAdidasShoe1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "9",
                DisplayOrder = 2,
            });
            pvaAdidasShoe1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "10",
                DisplayOrder = 3,
            });
            pvaAdidasShoe1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "11",
                DisplayOrder = 4,
            });
            productAdidasShoe.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaAdidasShoe1);
            var pvaAdidasShoe2 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Color").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaAdidasShoe2.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "White/Blue",
                DisplayOrder = 1,
            });
            pvaAdidasShoe2.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "White/Black",
                DisplayOrder = 2,
            });
            productAdidasShoe.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaAdidasShoe2);
            productAdidasShoe.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Shoes").Single(),
                DisplayOrder = 1,
            });
            productAdidasShoe.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_AdidasShoe_1.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productAdidasShoe.Name), true),
                DisplayOrder = 1,
            });
            productAdidasShoe.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_AdidasShoe_2.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productAdidasShoe.Name), true),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productAdidasShoe);





            var productAdobePhotoshop = new Product()
            {
                Name = "Adobe Photoshop Elements 7",
                ShortDescription = "Easily find and view all your photos",
                FullDescription = "<p>Adobe Photoshop Elements 7 software combines power and simplicity so you can make ordinary photos extraordinary; tell engaging stories in beautiful, personalized creations for print and web; and easily find and view all your photos. New Photoshop.com membership* works with Photoshop Elements so you can protect your photos with automatic online backup and 2 GB of storage; view your photos anywhere you are; and share your photos in fun, interactive ways with invitation-only Online Albums.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productAdobePhotoshop.ProductVariants.Add(new ProductVariant()
            {
                Price = 75M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productAdobePhotoshop.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Software").Single(),
                DisplayOrder = 1,
            });
            productAdobePhotoshop.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_AdobePhotoshop.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productAdobePhotoshop.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productAdobePhotoshop);





            var productApcUps = new Product()
            {
                Name = "APC Back-UPS RS 800VA - UPS - 800 VA - UPS battery - lead acid ( BR800BLK )",
                ShortDescription = "APC Back-UPS RS, 800VA/540W, Input 120V/Output 120V, Interface Port USB. ",
                FullDescription = "<p>The Back-UPS RS offers high performance protection for your business and office computer systems. It provides abundant battery backup power, allowing you to work through medium and extended length power outages. It also safeguards your equipment from damaging surges and spikes that travel along utility, phone and network lines. A distinguishing feature of the Back-UPS RS is automatic voltage regulation (AVR). AVR instantly adjusts both low and high voltages to safe levels, so you can work indefinitely during brownouts and overvoltage situations, saving the battery for power outages when you need it most. Award-winning shutdown software automatically powers down your computer system in the event of an extended power outage.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productApcUps.ProductVariants.Add(new ProductVariant()
            {
                Price = 75M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productApcUps.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Accessories").Single(),
                DisplayOrder = 1,
            });
            productApcUps.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_ApcUps.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productApcUps.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productApcUps);





            var productArrow = new Product()
            {
                Name = "Arrow Men's Wrinkle Free Pinpoint Solid Long Sleeve",
                ShortDescription = "",
                FullDescription = "<p>This Wrinkle Free Pinpoint Long Sleeve Dress Shirt needs minimum ironing. It is a great product at a great value!</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productArrow.ProductVariants.Add(new ProductVariant()
            {
                Price = 24M,
                IsShipEnabled = true,
                Weight = 4,
                Length = 3,
                Width = 3,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productArrow.ProductVariants.FirstOrDefault().TierPrices.Add(new TierPrice()
            {
                Quantity = 3,
                Price = 21
            });
            productArrow.ProductVariants.FirstOrDefault().TierPrices.Add(new TierPrice()
            {
                Quantity = 7,
                Price = 19
            });
            productArrow.ProductVariants.FirstOrDefault().TierPrices.Add(new TierPrice()
            {
                Quantity = 10,
                Price = 16
            });
            productArrow.ProductVariants.FirstOrDefault().HasTierPrices = true;

            productArrow.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Shirts").Single(),
                DisplayOrder = 1,
            });
            productArrow.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_arrow.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productArrow.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productArrow);





            var productAsusPc1000 = new Product()
            {
                Name = "ASUS Eee PC 1000HA 10-Inch Netbook",
                ShortDescription = "Super Hybrid Engine offers a choice of performance and power consumption modes for easy adjustments according to various needs",
                FullDescription = "<p>Much more compact than a standard-sized notebook and weighing just over 3 pounds, the Eee PC 1000HA is perfect for students toting to school or road warriors packing away to Wi-Fi hotspots. The Eee PC 1000HA also features a 160 GB hard disk drive (HDD), 1 GB of RAM, 1.3-megapixel webcam integrated into the bezel above the LCD, 54g Wi-Fi networking (802.11b/g), Secure Digital memory card slot, multiple USB ports, a VGA output for connecting to a monitor.</p><p>It comes preinstalled with the Microsoft Windows XP Home operating system, which offers more experienced users an enhanced and innovative experience that incorporates Windows Live features like Windows Live Messenger for instant messaging and Windows Live Mail for consolidated email accounts on your desktop. Complementing this is Microsoft Works, which equips the user with numerous office applications to work efficiently.</p><p>The new Eee PC 1000HA has a customized, cutting-edge Infusion casing technology in Fine Ebony. Inlaid within the chassis itself, the motifs are an integral part of the entire cover and will not fade with time. The Infusion surface also provides a new level of resilience, providing scratch resistance and a beautiful style while out and about.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productAsusPc1000.ProductVariants.Add(new ProductVariant()
            {
                Price = 2600M,
                IsShipEnabled = true,
                Weight = 3,
                Length = 3,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productAsusPc1000.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Notebooks").Single(),
                DisplayOrder = 1,
            });
            productAsusPc1000.ProductManufacturers.Add(new ProductManufacturer()
            {
                Manufacturer = _manufacturerRepository.Table.Where(c => c.Name == "ASUS").Single(),
                DisplayOrder = 2,
            });
            productAsusPc1000.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_asuspc1000.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productAsusPc1000.Name), true),
                DisplayOrder = 1,
            });
            productAsusPc1000.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Screensize").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "10.0''").Single()
            });
            productAsusPc1000.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "CPU Type").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "AMD").Single()
            });
            productAsusPc1000.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Memory").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "1 GB").Single()
            });
            productAsusPc1000.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 4,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Hardrive").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "160 GB").Single()
            });
            _productRepository.Insert(productAsusPc1000);





            var productAsusPc900 = new Product()
            {
                Name = "ASUS Eee PC 900HA 8.9-Inch Netbook Black",
                ShortDescription = "High Speed Connectivity Anywhere with Wi-Fi 802.11b/g.",
                FullDescription = "<p>Much more compact than a standard-sized notebook and weighing just 2.5 pounds, the Eee PC 900HA is perfect for students toting to school or road warriors packing away to Wi-Fi hotspots. In addition to the 160 GB hard disk drive (HDD), the Eee PC 900HA also features 1 GB of RAM, VGA-resolution webcam integrated into the bezel above the LCD, 54g Wi-Fi networking (802.11b/g), multiple USB ports, SD memory card slot, a VGA output for connecting to a monitor, and up to 10 GB of online storage (complimentary for 18 months).</p><p>It comes preinstalled with the Microsoft Windows XP Home operating system, which offers more experienced users an enhanced and innovative experience that incorporates Windows Live features like Windows Live Messenger for instant messaging and Windows Live Mail for consolidated email accounts on your desktop. Complementing this is Microsoft Works, which equips the user with numerous office applications to work efficiently.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productAsusPc900.ProductVariants.Add(new ProductVariant()
            {
                Price = 1500M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productAsusPc900.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Notebooks").Single(),
                DisplayOrder = 1,
            });
            productAsusPc900.ProductManufacturers.Add(new ProductManufacturer()
            {
                Manufacturer = _manufacturerRepository.Table.Where(c => c.Name == "ASUS").Single(),
                DisplayOrder = 1,
            });
            productAsusPc900.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_asuspc900.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productAsusPc900.Name), true),
                DisplayOrder = 1,
            });
            productAsusPc900.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "CPU Type").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "AMD").Single()
            });
            productAsusPc900.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Memory").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "1 GB").Single()
            });
            productAsusPc900.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 4,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Hardrive").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "160 GB").Single()
            });
            _productRepository.Insert(productAsusPc900);





            var productBestGrillingRecipes = new Product()
            {
                Name = "Best Grilling Recipes",
                ShortDescription = "More Than 100 Regional Favorites Tested and Perfected for the Outdoor Cook (Hardcover)",
                FullDescription = "<p>Take a winding cross-country trip and you'll discover barbecue shacks with offerings like tender-smoky Baltimore pit beef and saucy St. Louis pork steaks. To bring you the best of these hidden gems, along with all the classics, the editors of Cook's Country magazine scoured the country, then tested and perfected their favorites. HEre traditions large and small are brought into the backyard, from Hawaii's rotisserie favorite, the golden-hued Huli Huli Chicken, to fall-off-the-bone Chicago Barbecued Ribs. In Kansas City, they're all about the sauce, and for our saucy Kansas City Sticky Ribs, we found a surprise ingredient-root beer. We also tackle all the best sides. <br /><br />Not sure where or how to start? This cookbook kicks off with an easy-to-follow primer that will get newcomers all fired up. Whether you want to entertain a crowd or just want to learn to make perfect burgers, Best Grilling Recipes shows you the way.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productBestGrillingRecipes.ProductVariants.Add(new ProductVariant()
            {
                Price = 27M,
                OldPrice = 30M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Books").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productBestGrillingRecipes.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Books").Single(),
                DisplayOrder = 1,
            });
            productBestGrillingRecipes.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_BestGrillingRecipes.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productBestGrillingRecipes.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productBestGrillingRecipes);





            var productDiamondHeart = new Product()
            {
                Name = "Black & White Diamond Heart",
                ShortDescription = "Heart Pendant 1/4 Carat (ctw) in Sterling Silver",
                FullDescription = "<p>Bold black diamonds alternate with sparkling white diamonds along a crisp sterling silver heart to create a look that is simple and beautiful. This sleek and stunning 1/4 carat (ctw) diamond heart pendant which includes an 18 inch silver chain, and a free box of godiva chocolates makes the perfect Valentine's Day gift.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productDiamondHeart.ProductVariants.Add(new ProductVariant()
            {
                Price = 130M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Jewelry").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productDiamondHeart.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Jewelry").Single(),
                DisplayOrder = 1,
            });
            productDiamondHeart.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_DiamondHeart.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productDiamondHeart.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productDiamondHeart);





            var productBlackBerry = new Product()
            {
                Name = "BlackBerry Bold 9000 Phone, Black (AT&T)",
                ShortDescription = "Global Blackberry messaging smartphone with quad-band GSM",
                FullDescription = "<p>Keep yourself on track for your next meeting with turn-by-turn directions via the AT&amp;T Navigator service, which is powered by TeleNav and provides spoken or text-based turn-by-turn directions with automatic missed turn rerouting and a local business finder service in 20 countries. It also supports AT&amp;T mobile music services and access to thousands of video clips via Cellular Video. Other features include a 2-megapixel camera/camcorder, Bluetooth for handsfree communication, 1 GB of internal memory with MicroSD expansion (up to 32 GB), multi-format audio/video playback, and up to 4.5 hours of talk time.</p><p>The Blackberry Bold also comes with free access to AT&amp;T Wi-Fi Hotspots, available at more than 17,000 locations nationwide including Starbucks. The best part is that you do'nt need to sign up for anything new to use this service--Wi-Fi access for is included in all Blackberry Personal and Enterprise Rate Plans. (You must subscribe to a Blackberry Data Rate Plan to access AT&amp;T Wi-Fi Hotspots.) Additionally, the Blackberry Bold is the first RIM device that supports AT&amp;T Cellular Video (CV).</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productBlackBerry.ProductVariants.Add(new ProductVariant()
            {
                Price = 245M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productBlackBerry.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Cell phones").Single(),
                DisplayOrder = 1,
            });
            productBlackBerry.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_BlackBerry.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productBlackBerry.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productBlackBerry);




            var productBuildComputer = new Product()
            {
                Name = "Build your own computer",
                ShortDescription = "Build it",
                FullDescription = "<p>Fight back against cluttered workspaces with the stylish Sony VAIO JS All-in-One desktop PC, featuring powerful computing resources and a stunning 20.1-inch widescreen display with stunning XBRITE-HiColor LCD technology. The silver Sony VAIO VGC-JS110J/S has a built-in microphone and MOTION EYE camera with face-tracking technology that allows for easy communication with friends and family. And it has a built-in DVD burner and Sony's Movie Store software so you can create a digital entertainment library for personal viewing at your convenience. Easy to setup and even easier to use, this JS-series All-in-One includes an elegantly designed keyboard and a USB mouse.</p>",
                ProductTemplateId = productTemplateInGrid.Id,
                AllowCustomerReviews = true,
                Published = true,
                ShowOnHomePage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productBuildComputer.ProductVariants.Add(new ProductVariant()
            {
                Price = 1200M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            var pvaBuildComputer1 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Processor").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaBuildComputer1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "2.2 GHz Intel Pentium Dual-Core E2200",
                DisplayOrder = 1,
            });
            pvaBuildComputer1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "2.5 GHz Intel Pentium Dual-Core E2200",
                IsPreSelected = true,
                PriceAdjustment = 15,
                DisplayOrder = 2,
            });
            productBuildComputer.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaBuildComputer1);
            var pvaBuildComputer2 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "RAM").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaBuildComputer2.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "2 GB",
                DisplayOrder = 1,
            });
            pvaBuildComputer2.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "4GB",
                PriceAdjustment = 20,
                DisplayOrder = 2,
            });
            pvaBuildComputer2.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "8GB",
                PriceAdjustment = 60,
                DisplayOrder = 3,
            });
            productBuildComputer.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaBuildComputer2);
            var pvaBuildComputer3 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "HDD").Single(),
                AttributeControlType = AttributeControlType.RadioList,
                IsRequired = true,
            };
            pvaBuildComputer3.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "320 GB",
                DisplayOrder = 1,
            });
            pvaBuildComputer3.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "400 GB",
                PriceAdjustment = 100,
                DisplayOrder = 2,
            });
            productBuildComputer.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaBuildComputer3);
            var pvaBuildComputer4 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "OS").Single(),
                AttributeControlType = AttributeControlType.RadioList,
                IsRequired = true,
            };
            pvaBuildComputer4.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "Vista Home",
                PriceAdjustment = 50,
                IsPreSelected = true,
                DisplayOrder = 1,
            });
            pvaBuildComputer4.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "Vista Premium",
                PriceAdjustment = 60,
                DisplayOrder = 2,
            });
            productBuildComputer.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaBuildComputer4);
            var pvaBuildComputer5 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Software").Single(),
                AttributeControlType = AttributeControlType.Checkboxes,
            };
            pvaBuildComputer5.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "Microsoft Office",
                PriceAdjustment = 50,
                IsPreSelected = true,
                DisplayOrder = 1,
            });
            pvaBuildComputer5.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "Acrobat Reader",
                PriceAdjustment = 10,
                DisplayOrder = 2,
            });
            pvaBuildComputer5.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "Total Commander",
                PriceAdjustment = 5,
                DisplayOrder = 2,
            });
            productBuildComputer.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaBuildComputer5);
            productBuildComputer.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Desktops").Single(),
                DisplayOrder = 1,
            });
            productBuildComputer.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Desktops_1.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productBuildComputer.Name), true),
                DisplayOrder = 1,
            });
            productBuildComputer.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Desktops_2.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productBuildComputer.Name), true),
                DisplayOrder = 2,
            });
            productBuildComputer.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Desktops_3.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productBuildComputer.Name), true),
                DisplayOrder = 3,
            });
            _productRepository.Insert(productBuildComputer);





            var productCanonCamera = new Product()
            {
                Name = "Canon Digital Rebel XSi 12.2 MP Digital SLR Camera",
                ShortDescription = "12.2-megapixel CMOS sensor captures enough detail for poster-size, photo-quality prints",
                FullDescription = "<p>For stunning photography with point and shoot ease, look no further than Canon&rsquo;s EOS Rebel XSi. The EOS Rebel XSi brings staggering technological innovation to the masses. It features Canon&rsquo;s EOS Integrated Cleaning System, Live View Function, a powerful DIGIC III Image Processor, plus a new 12.2-megapixel CMOS sensor and is available in a kit with the new EF-S 18-55mm f/3.5-5.6 IS lens with Optical Image Stabilizer. The EOS Rebel XSi&rsquo;s refined, ergonomic design includes a new 3.0-inch LCD monitor, compatibility with SD and SDHC memory cards and new accessories that enhance every aspect of the photographic experience.</p>",
                ProductTemplateId = productTemplateInGrid.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productCanonCamera.ProductVariants.Add(new ProductVariant()
            {
                Name = "Black",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_CanonCamera_black.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Canon Digital Rebel XSi 12.2 MP Digital SLR Camera (Black)"), true).Id,
                Price = 670M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productCanonCamera.ProductVariants.Add(new ProductVariant()
            {
                Name = "Silver",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_CanonCamera_silver.jpeg"), "image/jpeg", pictureService.GetPictureSeName("Canon Digital Rebel XSi 12.2 MP Digital SLR Camera (Silver)"), true).Id,
                Price = 630M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productCanonCamera.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Camera, photo").Single(),
                DisplayOrder = 1,
            });
            productCanonCamera.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_CanonCamera_1.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productCanonCamera.Name), true),
                DisplayOrder = 1,
            });
            productCanonCamera.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_CanonCamera_2.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productCanonCamera.Name), true),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productCanonCamera);





            var productCanonCamcoder = new Product()
            {
                Name = "Canon VIXIA HF100 Camcorder",
                ShortDescription = "12x optical zoom; SuperRange Optical Image Stabilizer",
                FullDescription = "<p>From Canon's long history of optical excellence, advanced image processing, superb performance and technological innovation in photographic and broadcast television cameras comes the latest in high definition camcorders. <br /><br />Now, with the light, compact Canon VIXIA HF100, you can have stunning AVCHD (Advanced Video Codec High Definition) format recording with the ease and numerous benefits of Flash Memory. It's used in some of the world's most innovative electronic products such as laptop computers, MP3 players, PDAs and cell phones. <br /><br />Add to that the VIXIA HF100's Canon Exclusive features such as our own 3.3 Megapixel Full HD CMOS sensor and advanced DIGIC DV II Image Processor, SuperRange Optical Image Stabilization, Instant Auto Focus, our 2.7-inch Widescreen Multi-Angle Vivid LCD and the Genuine Canon 12x HD video zoom lens and you have a Flash Memory camcorder that's hard to beat.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productCanonCamcoder.ProductVariants.Add(new ProductVariant()
            {
                Price = 530M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productCanonCamcoder.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Camera, photo").Single(),
                DisplayOrder = 1,
            });
            productCanonCamcoder.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_CanonCamcoder.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productCanonCamcoder.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productCanonCamcoder);





            var productCompaq = new Product()
            {
                Name = "Compaq Presario SR1519X Pentium 4 Desktop PC with CDRW",
                ShortDescription = "Compaq Presario Desktop PC",
                FullDescription = "<p>Compaq Presario PCs give you solid performance, ease of use, and deliver just what you need so you can do more with less effort. Whether you are e-mailing family, balancing your online checkbook or creating school projects, the Presario is the right PC for you.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productCompaq.ProductVariants.Add(new ProductVariant()
            {
                Price = 500M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productCompaq.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Desktops").Single(),
                DisplayOrder = 1,
            });
            productCompaq.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Compaq.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productCompaq.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productCompaq);





            var productCookingForTwo = new Product()
            {
                Name = "Cooking for Two",
                ShortDescription = "More Than 200 Foolproof Recipes for Weeknights and Special Occasions (Hardcover)",
                FullDescription = "<p>Hardcover: 352 pages<br />Publisher: America's Test Kitchen (May 2009)<br />Language: English<br />ISBN-10: 1933615435<br />ISBN-13: 978-1933615431</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productCookingForTwo.ProductVariants.Add(new ProductVariant()
            {
                Price = 19M,
                OldPrice = 27M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Books").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productCookingForTwo.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Books").Single(),
                DisplayOrder = 1,
            });
            productCookingForTwo.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_CookingForTwo.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productCookingForTwo.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productCookingForTwo);





            var productCorel = new Product()
            {
                Name = "Corel Paint Shop Pro Photo X2",
                ShortDescription = "The ideal choice for any aspiring photographer's digital darkroom",
                FullDescription = "<p>Corel Paint Shop Pro Photo X2 is the ideal choice for any aspiring photographer's digital darkroom. Fix brightness, color, and photo flaws in a few clicks; use precision editing tools to create the picture you want; give photos a unique, exciting look using hundreds of special effects, and much more! Plus, the NEW one-of-a-kind Express Lab helps you quickly view and fix dozens of photos in the time it used to take to edit a few. Paint Shop Pro Photo X2 even includes a built-in Learning Center to help you get started, it's the easiest way to get professional-looking photos - fast!</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productCorel.ProductVariants.Add(new ProductVariant()
            {
                Price = 65M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productCorel.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Software").Single(),
                DisplayOrder = 1,
            });
            productCorel.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Corel.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productCorel.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productCorel);





            var productCustomTShirt = new Product()
            {
                Name = "Custom T-Shirt",
                ShortDescription = "T-Shirt - Add Your Content",
                FullDescription = "<p>Comfort comes in all shapes and forms, yet this tee out does it all. Rising above the rest, our classic cotton crew provides the simple practicality you need to make it through the day. Tag-free, relaxed fit wears well under dress shirts or stands alone in laid-back style. Reinforced collar and lightweight feel give way to long-lasting shape and breathability. One less thing to worry about, rely on this tee to provide comfort and ease with every wear.</p>",
                ProductTemplateId = productTemplateInGrid.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productCustomTShirt.ProductVariants.Add(new ProductVariant()
            {
                Price = 15M,
                IsShipEnabled = true,
                Weight = 4,
                Length = 3,
                Width = 3,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productCustomTShirt.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Custom Text").Single(),
                TextPrompt = "Enter your text:",
                AttributeControlType = AttributeControlType.TextBox,
                IsRequired = true,
            });

            productCustomTShirt.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Shirts").Single(),
                DisplayOrder = 1,
            });
            productCustomTShirt.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_CustomTShirt.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productCustomTShirt.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productCustomTShirt);





            var productDiamondEarrings = new Product()
            {
                Name = "Diamond Pave Earrings",
                ShortDescription = "1/2 Carat (ctw) in White Gold",
                FullDescription = "<p>Perfect for both a professional look as well as perhaps something more sensual, these 10 karat white gold huggie earrings boast 86 sparkling round diamonds set in a pave arrangement that total 1/2 carat (ctw).</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productDiamondEarrings.ProductVariants.Add(new ProductVariant()
            {
                Price = 569M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Jewelry").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productDiamondEarrings.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Jewelry").Single(),
                DisplayOrder = 1,
            });
            productDiamondEarrings.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_DiamondEarrings.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productDiamondEarrings.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productDiamondEarrings);





            var productDiamondBracelet = new Product()
            {
                Name = "Diamond Tennis Bracelet",
                ShortDescription = "1.0 Carat (ctw) in White Gold",
                FullDescription = "<p>Jazz up any outfit with this classic diamond tennis bracelet. This piece has one full carat of diamonds uniquely set in brilliant 10 karat white gold.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productDiamondBracelet.ProductVariants.Add(new ProductVariant()
            {
                Price = 360M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Jewelry").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productDiamondBracelet.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Jewelry").Single(),
                DisplayOrder = 1,
            });
            productDiamondBracelet.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_DiamondBracelet_1.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productDiamondBracelet.Name), true),
                DisplayOrder = 1,
            });
            productDiamondBracelet.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_DiamondBracelet_2.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productDiamondBracelet.Name), true),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productDiamondBracelet);





            var productEatingWell = new Product()
            {
                Name = "EatingWell in Season",
                ShortDescription = "A Farmers' Market Cookbook (Hardcover)",
                FullDescription = "<p>Trying to get big chocolate flavor into a crisp holiday cookie is no easy feat. Any decent baker can get a soft, chewy cookie to scream &ldquo;chocolate,&rdquo; but a dough that can withstand a rolling pin and cookie cutters simply can&rsquo;t be too soft. Most chocolate butter cookies skimp on the gooey chocolate and their chocolate flavor is quite modest.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productEatingWell.ProductVariants.Add(new ProductVariant()
            {
                Price = 51M,
                OldPrice = 67M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Books").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productEatingWell.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Books").Single(),
                DisplayOrder = 1,
            });
            productEatingWell.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_EatingWell.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productEatingWell.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productEatingWell);




            var productEtnies = new Product()
            {
                Name = "etnies Men's Digit Sneaker",
                ShortDescription = "This sleek shoe has all you need--from the padded tongue and collar and internal EVA midsole, to the STI Level 2 cushioning for impact absorption and stability.",
                FullDescription = "<p>Established in 1986, etnies is the first skateboarder-owned and skateboarder-operated global action sports footwear and apparel company. etnies not only pushed the envelope by creating the first pro model skate shoe, but it pioneered technological advances and changed the face of skateboard footwear forever. Today, etnies' vision is to remain the leading action sports company committed to creating functional products that provide the most style, comfort, durability and protection possible. etnies stays true to its roots by sponsoring a world-class team of skateboarding, surfing, snowboarding, moto-x, and BMX athletes and continues its dedication by giving back to each of these communities.</p>",
                ProductTemplateId = productTemplateInGrid.Id,
                AllowCustomerReviews = true,
                Published = true,
                ShowOnHomePage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productEtnies.ProductVariants.Add(new ProductVariant()
            {
                Price = 17.56M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            var pvaEtnies1 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Size").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaEtnies1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "8",
                DisplayOrder = 1,
            });
            pvaEtnies1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "9",
                DisplayOrder = 2,
            });
            pvaEtnies1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "10",
                DisplayOrder = 3,
            });
            pvaEtnies1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "11",
                DisplayOrder = 4,
            });
            productEtnies.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaEtnies1);
            var pvaEtnies2 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Color").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaEtnies2.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "White/Blue",
                DisplayOrder = 1,
            });
            pvaEtnies2.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "White/Black",
                DisplayOrder = 2,
            });
            productEtnies.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaEtnies2);
            productEtnies.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Shoes").Single(),
                DisplayOrder = 1,
            });
            productEtnies.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Etnies.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productEtnies.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productEtnies);





            var productLeatherHandbag = new Product()
            {
                Name = "Genuine Leather Handbag with Cell Phone Holder & Many Pockets",
                ShortDescription = "Classic Leather Handbag",
                FullDescription = "<p>This fine leather handbag will quickly become your favorite bag. It has a zipper organizer on the front that includes a notepad pocket, pen holder, credit card slots and zipper pocket divider. On top of this is a zipper pocket and another flap closure pocket. The main compartment is fully lined and includes a side zipper pocket. On the back is another zipper pocket. And don't forget the convenient built in cell phone holder on the side! The long strap is fully adjustable so you can wear it crossbody or over the shoulder. This is a very well-made, quality leather bag that is not too big, but not too small.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productLeatherHandbag.ProductVariants.Add(new ProductVariant()
            {
                Price = 35M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productLeatherHandbag.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Apparel accessories").Single(),
                DisplayOrder = 1,
            });
            productLeatherHandbag.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_LeatherHandbag_1.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productLeatherHandbag.Name), true),
                DisplayOrder = 1,
            });
            productLeatherHandbag.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_LeatherHandbag_2.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productLeatherHandbag.Name), true),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productLeatherHandbag);





            var productHp506 = new Product()
            {
                Name = "HP IQ506 TouchSmart Desktop PC",
                ShortDescription = "",
                FullDescription = "<p>Redesigned with a next-generation, touch-enabled 22-inch high-definition LCD screen, the HP TouchSmart IQ506 all-in-one desktop PC is designed to fit wherever life happens: in the kitchen, family room, or living room. With one touch you can check the weather, download your e-mail, or watch your favorite TV show. It's also designed to maximize energy, with a power-saving Intel Core 2 Duo processor and advanced power management technology, as well as material efficiency--right down to the packaging. It has a sleek piano black design with elegant espresso side-panel highlights, and the HP Ambient Light lets you set a mood--or see your keyboard in the dark.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productHp506.ProductVariants.Add(new ProductVariant()
            {
                Price = 1199M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productHp506.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Desktops").Single(),
                DisplayOrder = 1,
            });
            productHp506.ProductManufacturers.Add(new ProductManufacturer()
            {
                Manufacturer = _manufacturerRepository.Table.Where(c => c.Name == "HP").Single(),
                DisplayOrder = 1,
            });
            productHp506.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Hp506.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productHp506.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productHp506);





            var productHpPavilion1 = new Product()
            {
                Name = "HP Pavilion Artist Edition DV2890NR 14.1-inch Laptop",
                ShortDescription = "Unique Asian-influenced HP imprint wraps the laptop both inside and out",
                FullDescription = "<p>Optimize your mobility with a BrightView 14.1-inch display that has the same viewable area as a 15.4-inch screen--in a notebook that weighs a pound less. Encouraging more direct interaction, the backlit media control panel responds to the touch or sweep of a finger. Control settings for audio and video playback from up to 10 feet away with the included HP remote, then store it conveniently in the PC card slot. Enjoy movies or music in seconds with the external DVD or music buttons to launch HP QuickPlay (which bypasses the boot process).</p><p>It's powered by the 1.83 GHz Intel Core 2 Duo T5550 processor, which provides an optimized, multithreaded architecture for improved gaming and multitasking performance, as well as excellent battery management. It also includes Intel's 4965 AGN wireless LAN, which will connect to draft 802.11n routers and offers compatibility with 802.11a/b/g networks as well. It also features a 250 GB hard drive, 3 GB of installed RAM (4 GB maximum), LighScribe dual-layer DVD&plusmn;R burner, HDMI port for connecting to an HDTV, and Nvidia GeForce Go 8400M GS video/graphics card with up to 1407 MB of total allocated video memory (128 MB dedicated). It also includes an integrated Webcam in the LCD's bezel and an omnidirectional microphone for easy video chats.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productHpPavilion1.ProductVariants.Add(new ProductVariant()
            {
                Price = 1590M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productHpPavilion1.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Notebooks").Single(),
                DisplayOrder = 1,
            });
            productHpPavilion1.ProductManufacturers.Add(new ProductManufacturer()
            {
                Manufacturer = _manufacturerRepository.Table.Where(c => c.Name == "HP").Single(),
                DisplayOrder = 2,
            });
            productHpPavilion1.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HpPavilion1.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productHpPavilion1.Name), true),
                DisplayOrder = 1,
            });
            productHpPavilion1.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Screensize").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "14.1''").Single()
            });
            productHpPavilion1.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "CPU Type").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "Intel").Single()
            });
            productHpPavilion1.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Memory").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "3 GB").Single()
            });
            productHpPavilion1.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 4,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Hardrive").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "250 GB").Single()
            });
            _productRepository.Insert(productHpPavilion1);





            var productHpPavilion2 = new Product()
            {
                Name = "HP Pavilion Elite M9150F Desktop PC",
                ShortDescription = "Top-of-the-line multimedia desktop featuring 2.4 GHz Intel Core 2 Quad Processor Q6600 with four lightning fast execution cores",
                FullDescription = "<p>The updated chassis with sleek piano black paneling and components is far from the most significant improvements in the multimedia powerhouse HP Pavilion Elite m9150f desktop PC. It's powered by Intel's newest processor--the 2.4 GHz Intel Core 2 Quad Q6600--which delivers four complete execution cores within a single processor for unprecedented performance and responsiveness in multi-threaded and multi-tasking business/home environments. You can also go wireless and clutter-free with wireless keyboard, mouse, and remote control, and it includes the next step in Wi-Fi networking with a 54g wireless LAN (802.11b/g).</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productHpPavilion2.ProductVariants.Add(new ProductVariant()
            {
                Price = 1350M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productHpPavilion2.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Desktops").Single(),
                DisplayOrder = 1,
            });
            productHpPavilion2.ProductManufacturers.Add(new ProductManufacturer()
            {
                Manufacturer = _manufacturerRepository.Table.Where(c => c.Name == "HP").Single(),
                DisplayOrder = 3,
            });
            productHpPavilion2.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HpPavilion2_1.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productHpPavilion2.Name), true),
                DisplayOrder = 1,
            });
            productHpPavilion2.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HpPavilion2_2.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productHpPavilion2.Name), true),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productHpPavilion2);





            var productHpPavilion3 = new Product()
            {
                Name = "HP Pavilion G60-230US 16.0-Inch Laptop",
                ShortDescription = "Streamlined multimedia laptop with 16-inch screen for basic computing, entertainment and online communication",
                FullDescription = "<p>Chat face to face, or take pictures and video clips with the webcam and integrated digital microphone. Play games and enhance multimedia with the Intel GMA 4500M with up to 1309 MB of total available graphics memory. And enjoy movies or music in seconds with the external DVD or music buttons to launch HP QuickPlay (which bypasses the boot process).  It offers dual-core productivity from its 2.0 GHz Intel Pentium T4200 processor for excellent multitasking. Other features include a 320 GB hard drive, 3 GB of installed RAM (4 GB maximum capacity), dual-layer DVD&plusmn;RW drive (which also burns CDs), quad-mode Wi-Fi (802.11a/b/g/n), 5-in-1 memory card reader, and pre-installed Windows Vista Home Premium (SP1).</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productHpPavilion3.ProductVariants.Add(new ProductVariant()
            {
                Price = 1460M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productHpPavilion3.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Notebooks").Single(),
                DisplayOrder = 1,
            });
            productHpPavilion3.ProductManufacturers.Add(new ProductManufacturer()
            {
                Manufacturer = _manufacturerRepository.Table.Where(c => c.Name == "HP").Single(),
                DisplayOrder = 4,
            });
            productHpPavilion3.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HpPavilion3.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productHpPavilion3.Name), true),
                DisplayOrder = 1,
            });
            productHpPavilion3.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Screensize").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "16.0''").Single()
            });
            productHpPavilion3.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "CPU Type").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "Intel").Single()
            });
            productHpPavilion3.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Memory").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "3 GB").Single()
            });
            productHpPavilion3.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 4,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Hardrive").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "320 GB").Single()
            });
            _productRepository.Insert(productHpPavilion3);





            var productHat = new Product()
            {
                Name = "Indiana Jones® Shapeable Wool Hat",
                ShortDescription = "Wear some adventure with the same hat Indiana Jones&reg; wears in his movies.",
                FullDescription = "<p>Wear some adventure with the same hat Indiana Jones&reg; wears in his movies. Easy to shape to fit your personal style. Wool. Import. Please Note - Due to new UPS shipping rules and the size of the box, if you choose to expedite your hat order (UPS 3-day, 2-day or Overnight), an additional non-refundable $20 shipping charge per hat will be added at the time your order is processed.</p>",
                ProductTemplateId = productTemplateInGrid.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productHat.ProductVariants.Add(new ProductVariant()
            {
                Price = 30M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            var pvaHat1 = new ProductVariantAttribute()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Size").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaHat1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "Small",
                DisplayOrder = 1,
            });
            pvaHat1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "Medium",
                DisplayOrder = 2,
            });
            pvaHat1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "Large",
                DisplayOrder = 3,
            });
            pvaHat1.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue()
            {
                Name = "X-Large",
                DisplayOrder = 4,
            });
            productHat.ProductVariants.FirstOrDefault().ProductVariantAttributes.Add(pvaHat1);
            productHat.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Apparel accessories").Single(),
                DisplayOrder = 1,
            });
            productHat.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_hat.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productHat.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productHat);





            var productKensington = new Product()
            {
                Name = "Kensington 33117 International All-in-One Travel Plug Adapter",
                ShortDescription = "Includes plug adapters for use in more than 150 countries",
                FullDescription = "<p>The Kensington 33117 Travel Plug Adapter is a pocket-sized power adapter for go-anywhere convenience. This all-in-one unit provides plug adapters for use in more than 150 countries, so you never need to be at a loss for power again. The Kensington 33117 is easy to use, with slide-out power plugs that ensure you won't lose any vital pieces, in a compact, self-contained unit that eliminates any hassles. This all-in-one plug adapts power outlets for laptops, chargers, and similar devices, and features a safety release button and built-in fuse to ensure safe operation. The Kensington 33117 does not reduce or convert electrical voltage, is suitable for most consumer electronics ranging from 110-volts to Mac 275-watts, to 220-volts to Mac 550-watts. Backed by Kensington's one-year warranty, this unit weighs 0.5, and measures 1.875 x 2 x 2.25 inches (WxDxH). Please note that this adapter is not designed for use with high-watt devices such as hairdryers and irons, so users should check electronic device specifications before using.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productKensington.ProductVariants.Add(new ProductVariant()
            {
                Price = 35M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productKensington.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Accessories").Single(),
                DisplayOrder = 1,
            });
            productKensington.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Kensington.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productKensington.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productKensington);





            var productLeviJeans = new Product()
            {
                Name = "Levi's Skinny 511 Jeans",
                ShortDescription = "Levi's Faded Black Skinny 511 Jeans ",
                FullDescription = "",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productLeviJeans.ProductVariants.Add(new ProductVariant()
            {
                Price = 43.5M,
                OldPrice = 55M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productLeviJeans.ProductVariants.FirstOrDefault().TierPrices.Add(new TierPrice()
            {
                Quantity = 3,
                Price = 40
            });
            productLeviJeans.ProductVariants.FirstOrDefault().TierPrices.Add(new TierPrice()
            {
                Quantity = 6,
                Price = 38
            });
            productLeviJeans.ProductVariants.FirstOrDefault().TierPrices.Add(new TierPrice()
            {
                Quantity = 10,
                Price = 35
            });
            productLeviJeans.ProductVariants.FirstOrDefault().HasTierPrices = true;

            productLeviJeans.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Jeans").Single(),
                DisplayOrder = 1,
            });
            productLeviJeans.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_LeviJeans_1.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productLeviJeans.Name), true),
                DisplayOrder = 1,
            });
            productLeviJeans.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_LeviJeans_2.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productLeviJeans.Name), true),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productLeviJeans);





            var productBaseball = new Product()
            {
                Name = "Major League Baseball 2K9",
                ShortDescription = "Take charge of your franchise and enjoy the all-new MLB.com presentation style",
                FullDescription = "<p>Major League Baseball 2K9 captures the essence of baseball down to some of the most minute, player- specific details including batting stances, pitching windups and signature swings. 2K Sports has gone above and beyond the call of duty to deliver this in true major league fashion. Additionally, gameplay enhancements in pitching, batting, fielding and base running promise this year's installment to be user-friendly and enjoyable for rookies or veterans. New commentary and presentation provide the icing to this ultimate baseball experience. If you really want to Play Ball this is the game for you.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productBaseball.ProductVariants.Add(new ProductVariant()
            {
                Price = 14.99M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productBaseball.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Games").Single(),
                DisplayOrder = 1,
            });
            productBaseball.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Baseball.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productBaseball.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productBaseball);





            var productMedalOfHonor = new Product()
            {
                Name = "Medal of Honor - Limited Edition (Xbox 360)",
                ShortDescription = "One of the great pioneers in military simulations returns to gaming as the Medal of Honor series depicts modern warfare for the first time, with a harrowing tour of duty in current day Afghanistan.",
                FullDescription = "You'll take control of both ordinary U.S. Army Rangers and Tier 1 Elite Ops Special Forces as you fight enemy insurgents in the most dangerous theatre of war of the modern age. The intense first person combat has been created with input from U.S. military consultants and based on real-life descriptions from veteran soldiers. This allows you to use genuine military tactics and advanced technology including combat drones and targeted air strikes.",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productMedalOfHonor.ProductVariants.Add(new ProductVariant()
            {
                Price = 37M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productMedalOfHonor.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Games").Single(),
                DisplayOrder = 1,
            });
            productMedalOfHonor.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_MedalOfHonor.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productMedalOfHonor.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productMedalOfHonor);





            var productMouse = new Product()
            {
                Name = "Microsoft Bluetooth Notebook Mouse 5000 Mac/Windows",
                ShortDescription = "Enjoy reliable, transceiver-free wireless connection to your PC with Bluetooth Technology",
                FullDescription = "<p>Enjoy wireless freedom with the Microsoft&reg; Bluetooth&reg; Notebook Mouse 5000 &mdash; no transceiver to connect or lose! Keep USB ports free for other devices. And, take it with you in a convenient carrying case (included)</p>",
                ProductTemplateId = productTemplateInGrid.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productMouse.ProductVariants.Add(new ProductVariant()
            {
                Price = 37M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productMouse.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Accessories").Single(),
                DisplayOrder = 1,
            });
            productMouse.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Mouse.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productMouse.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productMouse);





            var productGolfBelt = new Product()
            {
                Name = "NIKE Golf Casual Belt",
                ShortDescription = "NIKE Golf Casual Belt is a great look for in the clubhouse after a round of golf.",
                FullDescription = "<p>NIKE Golf Casual Belt is a great look for in the clubhouse after a round of golf. The belt strap is made of full grain oil tanned leather. The buckle is made of antique brushed metal with an embossed Swoosh design on it. This belt features an English beveled edge with rivets on the tab and tip of the 38mm wide strap. Size: 32; Color: Black.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productGolfBelt.ProductVariants.Add(new ProductVariant()
            {
                Price = 45M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productGolfBelt.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Apparel accessories").Single(),
                DisplayOrder = 1,
            });
            productGolfBelt.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_GolfBelt.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productGolfBelt.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productGolfBelt);





            var productPanasonic = new Product()
            {
                Name = "Panasonic HDC-SDT750K, High Definition 3D Camcorder",
                ShortDescription = "World's first 3D Shooting Camcorder",
                FullDescription = "<p>Unlike previous 3D images that required complex, professional equipment to create, now you can shoot your own. Simply attach the 3D Conversion Lens to the SDT750 for quick and easy 3D shooting. And because the SDT750 features the Advanced 3MOS System, which has gained worldwide popularity, colors are vivid and 3D images are extremely realistic. Let the SDT750 save precious moments for you in true-to-life images.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productPanasonic.ProductVariants.Add(new ProductVariant()
            {
                Price = 1300M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productPanasonic.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Camera, photo").Single(),
                DisplayOrder = 1,
            });
            productPanasonic.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_productPanasonic.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productPanasonic.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productPanasonic);





            var productSunglasses = new Product()
            {
                Name = "Ray Ban Aviator Sunglasses RB 3025",
                ShortDescription = "Aviator sunglasses are one of the first widely popularized styles of modern day sunwear.",
                FullDescription = "<p>Since 1937, Ray-Ban can genuinely claim the title as the world's leading sunglasses and optical eyewear brand. Combining the best of fashion and sports performance, the Ray-Ban line of Sunglasses delivers a truly classic style that will have you looking great today and for years to come.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productSunglasses.ProductVariants.Add(new ProductVariant()
            {
                Price = 25M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productSunglasses.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Apparel accessories").Single(),
                DisplayOrder = 1,
            });
            productSunglasses.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Sunglasses.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productSunglasses.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productSunglasses);





            var productSamsungPhone = new Product()
            {
                Name = "Samsung Rugby A837 Phone, Black (AT&T)",
                ShortDescription = "Ruggedized 3G handset in black great for outdoor workforces",
                FullDescription = "<p>Ideal for on-site field services, the ruggedized Samsung Rugby for AT&amp;T can take just about anything you can throw at it. This highly durable handset is certified to Military Standard MIL-STD 810F standards that's perfect for users like construction foremen and landscape designers. In addition to access to AT&amp;T Navigation turn-by-turn direction service, the Rugby also features compatibility with Push to Talk communication, Enterprise Paging, and AT&amp;T's breakthrough Video Share calling services. This quad-band GSM phone runs on AT&amp;T's dual-band 3G (HSDPA/UMTS) network, for fast downloads and seamless video calls. It also offers a 1.3-megapixel camera, microSD memory expansion to 8 GB, Bluetooth for handsfree communication and stereo music streaming, access to personal email and instant messaging, and up to 5 hours of talk time.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productSamsungPhone.ProductVariants.Add(new ProductVariant()
            {
                Price = 100M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productSamsungPhone.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Cell phones").Single(),
                DisplayOrder = 1,
            });
            productSamsungPhone.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_SamsungPhone_1.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productSamsungPhone.Name), true),
                DisplayOrder = 1,
            });
            productSamsungPhone.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_SamsungPhone_2.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productSamsungPhone.Name), true),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productSamsungPhone);





            var productSonyCamcoder = new Product()
            {
                Name = "Sony DCR-SR85 1MP 60GB Hard Drive Handycam Camcorder",
                ShortDescription = "Capture video to hard disk drive; 60 GB storage",
                FullDescription = "<p>You&rsquo;ll never miss a moment because of switching tapes or discs with the DCR-SR85. Its built-in 60GB hard disk drive offers plenty of storage as you zero in on your subjects with the professional-quality Carl Zeiss Vario-Tessar lens and a powerful 25x optical/2000x digital zoom. Compose shots using the 2.7-inch wide (16:9) touch-panel LCD display, and maintain total control and clarity with the Super SteadyShot image stabilization system. Hybrid recording technology even gives you the choice to record video to either the internal hard disk drive or removable Memory Stick Pro Duo media.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productSonyCamcoder.ProductVariants.Add(new ProductVariant()
            {
                Price = 349M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productSonyCamcoder.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Camera, photo").Single(),
                DisplayOrder = 1,
            });
            productSonyCamcoder.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_SonyCamcoder.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productSonyCamcoder.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productSonyCamcoder);





            var productBestSkilletRecipes = new Product()
            {
                Name = "The Best Skillet Recipes",
                ShortDescription = "What's the Best Way to Make Lasagna With Rich, Meaty Flavor, Chunks of Tomato, and Gooey Cheese, Without Ever Turning on the Oven or Boiling a Pot of (Hardcover)",
                FullDescription = "<p>In this latest addition of the Best Recipe Classic series, <i>Cooks Illustrated</i> editor Christopher Kimball and his team of kitchen scientists celebrate the untold versatility of that ordinary workhorse, the 12-inch skillet. An indispensable tool for eggs, pan-seared meats and saut&eacute;ed vegetables, the skillet can also be used for stovetop-to-oven dishes such as All-American Mini Meatloaves; layered dishes such as tamale pie and Tuscan bean casserole; and even desserts such as hot fudge pudding cake. In the trademark style of other America's Test Kitchen publications, the cookbook contains plenty of variations on basic themes (you can make chicken and rice with peas and scallions, broccoli and cheddar, or coconut milk and pistachios); ingredient and equipment roundups; and helpful illustrations for preparing mango and stringing snowpeas. Yet the true strength of the series lies in the sheer thoughtfulness and detail of the recipes. Whether or not you properly appreciate your skillet, this book will at least teach you to wield it gracefully. <i>(Mar.)</i>   <br />Copyright &copy; Reed Business Information, a division of Reed Elsevier Inc. All rights reserved.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productBestSkilletRecipes.ProductVariants.Add(new ProductVariant()
            {
                Price = 24M,
                OldPrice = 35M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Books").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productBestSkilletRecipes.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Books").Single(),
                DisplayOrder = 1,
            });
            productBestSkilletRecipes.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_BestSkilletRecipes.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productBestSkilletRecipes.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productBestSkilletRecipes);





            var productSatellite = new Product()
            {
                Name = "Toshiba Satellite A305-S6908 15.4-Inch Laptop",
                ShortDescription = "Stylish, highly versatile laptop with 15.4-inch LCD, webcam integrated into bezel, and high-gloss finish",
                FullDescription = "<p>It's powered by the 2.0 GHz Intel Core 2 Duo T6400 processor, which boosts speed, reduces power requirements, and saves on battery life. It also offers a fast 800 MHz front-side bus speed and 2 MB L2 cache. It also includes Intel's 5100AGN wireless LAN, which will connect to draft 802.11n routers and offers compatibility with 802.11a/b/g networks as well. Other features include an enormous 250 GB hard drive,&nbsp;1 GB of installed RAM (max capacity), dual-layer DVD&plusmn;RW burner (with Labelflash disc printing), ExpressCard 54/34 slot, a combo USB/eSATA port, SPDIF digital audio output for surround sound, and a 5-in-1 memory card adapter.</p><p>This PC comes preinstalled with the 64-bit version of Microsoft Windows Vista Home Premium (SP1), which includes all of the Windows Media Center capabilities for turning your PC into an all-in-one home entertainment center. In addition to easily playing your DVD movies and managing your digital audio library, you'll be able to record and watch your favorite TV shows (even HDTV). Vista also integrates new search tools throughout the operating system, includes new parental control features, and offers new tools that can warn you of impending hardware failures</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productSatellite.ProductVariants.Add(new ProductVariant()
            {
                Price = 1360M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productSatellite.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Notebooks").Single(),
                DisplayOrder = 1,
            });
            productSatellite.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Notebooks.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productSatellite.Name), true),
                DisplayOrder = 1,
            });
            productSatellite.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Screensize").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "15.4''").Single()
            });
            productSatellite.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "CPU Type").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "Intel").Single()
            });
            productSatellite.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Memory").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "1 GB").Single()
            });
            productSatellite.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 4,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Hardrive").Single().SpecificationAttributeOptions.Where(sao => sao.Name == "250 GB").Single()
            });
            _productRepository.Insert(productSatellite);





            var productDenimShort = new Product()
            {
                Name = "V-Blue Juniors' Cuffed Denim Short with Rhinestones",
                ShortDescription = "Superior construction and reinforced seams",
                FullDescription = "",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productDenimShort.ProductVariants.Add(new ProductVariant()
            {
                Price = 10M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Apparel & Shoes").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productDenimShort.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Jeans").Single(),
                DisplayOrder = 1,
            });
            productDenimShort.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_DenimShort.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productDenimShort.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productDenimShort);





            var productEngagementRing = new Product()
            {
                Name = "Vintage Style Three Stone Diamond Engagement Ring",
                ShortDescription = "1.24 Carat (ctw) in 14K White Gold (Certified)",
                FullDescription = "<p>Dazzle her with this gleaming 14 karat white gold vintage proposal. A ravishing collection of 11 decadent diamonds come together to invigorate a superbly ornate gold shank. Total diamond weight on this antique style engagement ring equals 1 1/4 carat (ctw). Item includes diamond certificate.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productEngagementRing.ProductVariants.Add(new ProductVariant()
            {
                Price = 2100M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Jewelry").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productEngagementRing.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Jewelry").Single(),
                DisplayOrder = 1,
            });
            productEngagementRing.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_EngagementRing_1.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productEngagementRing.Name), true),
                DisplayOrder = 1,
            });
            productEngagementRing.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_EngagementRing_2.jpg"), "image/pjpeg", pictureService.GetPictureSeName(productEngagementRing.Name), true),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productEngagementRing);





            var productWoW = new Product()
            {
                Name = "World of Warcraft: Wrath of the Lich King Expansion Pack",
                ShortDescription = "This expansion pack REQUIRES the original World of Warcraft game in order to run",
                FullDescription = "<p>Fans of World of Warcraft, prepare for Blizzard Entertainment's next installment -- World of Warcraft: Wrath of King Lich. In this latest expansion, something is afoot in the cold, harsh northlands. The Lich King Arthas has set in motion events that could lead to the extinction of all life on Azeroth. The necromantic power of the plague and legions of undead armies threaten to sweep across the land. Only the mightiest heroes can oppose the Lich King and end his reign of terror.</p><p>This expansion adds a host of content to the already massive existing game world. Players will achieve soaring levels of power, explore Northrend (the vast icy continent of the Lich King), and battle high-level heroes to determine the ultimate fate of Azeroth. As you face the dangers of the frigid, harsh north, prepare to master the dark necromantic powers of the Death Night -- World of Warcraft's first Hero class. No longer servants of the Lich King, the Death Knights begin their new calling as experienced, formidable adversaries. Each is heavily armed, armored, and in possession of a deadly arsenal of forbidden magic.</p><p>If you have a World of Warcraft account with a character of at least level 55, you will be able to create a new level-55 Death Knight of any race (if on a PvP realm, the Death Knight must be the same faction as your existing character). And upon entering the new world, your Death Knight will begin to quest to level 80, gaining potent new abilities and talents along the way. This expansion allows for only one Death Knight per realm, per account.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productWoW.ProductVariants.Add(new ProductVariant()
            {
                Price = 29.5M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productWoW.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Games").Single(),
                DisplayOrder = 1,
            });
            productWoW.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_wow.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productWoW.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productWoW);





            var productSoccer = new Product()
            {
                Name = "World Wide Soccer Manager 2009",
                ShortDescription = "Worldwide Soccer Manager 2009 from Sega for the PC or Mac is an in-depth soccer management game",
                FullDescription = "<p>Worldwide Soccer Manager 2009 from Sega for the PC or Mac is an in-depth soccer management game. At the helm, you'll enter the new season with a wide array of all-new features. The most impressive update is the first-time-ever, real-time 3D match engine with motion captured animations. With over 5,000 playable teams and every management decision in the palm of your hand, you'll love watching your matches and decisions unfold from multiple camera angles as you compete in leagues around the world and major international tournaments.</p><p>Watch your match in real-time, or use the Match Time Bar to fast-forward through sluggish minutes or rewind key moments in the game. With this customization at your fingertips you can also choose the information you'd like to see during the match, such as latest scores or player performance stats for the match.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            productSoccer.ProductVariants.Add(new ProductVariant()
            {
                Price = 25.99M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Electronics & Software").Single().Id,
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
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            });
            productSoccer.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Games").Single(),
                DisplayOrder = 1,
            });
            productSoccer.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Soccer.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productSoccer.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productSoccer);





            var productPokerFace = new Product()
            {
                Name = "Poker Face",
                ShortDescription = "Poker Face by Lady GaGa",
                FullDescription = "<p>Original Release Date: October 28, 2008</p><p>Release Date: October 28, 2008</p><p>Label: Streamline/Interscoope/KonLive/Cherrytree</p><p>Copyright: (C) 2008 Interscope Records</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            var downloadPokerFace1 = new Download()
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = "application/x-zip-co",
                DownloadBinary = File.ReadAllBytes(sampleDownloadsPath + "product_PokerFace_1.zip"),
                Extension = ".zip",
                Filename = "Poker_Face_1",
                IsNew = true,
            };
            downloadService.InsertDownload(downloadPokerFace1);
            var downloadPokerFace2 = new Download()
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = "text/plain",
                DownloadBinary = File.ReadAllBytes(sampleDownloadsPath + "product_PokerFace_2.txt"),
                Extension = ".txt",
                Filename = "Poker_Face_1",
                IsNew = true,
            };
            downloadService.InsertDownload(downloadPokerFace2);
            productPokerFace.ProductVariants.Add(new ProductVariant()
            {
                Price = 2.8M,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Downloadable Products").Single().Id,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IsDownload = true,
                DownloadId = downloadPokerFace1.Id,
                DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                UnlimitedDownloads = true,
                HasUserAgreement = false,
                HasSampleDownload = true,
                SampleDownloadId = downloadPokerFace2.Id,

            });
            productPokerFace.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Digital downloads").Single(),
                DisplayOrder = 1,
            });
            productPokerFace.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_PokerFace.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productPokerFace.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productPokerFace);





            var productSingleLadies = new Product()
            {
                Name = "Single Ladies (Put A Ring On It)",
                ShortDescription = "Single Ladies (Put A Ring On It) by Beyonce",
                FullDescription = "<p>Original Release Date: November 18, 2008</p><p>Label: Music World Music/Columbia</p><p>Copyright: (P) 2008 SONY BMG MUSIC ENTERTAINMENT</p><p>Song Length: 3:13 minutes</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            var downloadSingleLadies1 = new Download()
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = "application/x-zip-co",
                DownloadBinary = File.ReadAllBytes(sampleDownloadsPath + "product_SingleLadies_1.zip"),
                Extension = ".zip",
                Filename = "Single_Ladies_1",
                IsNew = true,
            };
            downloadService.InsertDownload(downloadSingleLadies1);
            var downloadSingleLadies2 = new Download()
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = "text/plain",
                DownloadBinary = File.ReadAllBytes(sampleDownloadsPath + "product_SingleLadies_2.txt"),
                Extension = ".txt",
                Filename = "Single_Ladies_1",
                IsNew = true,
            };
            downloadService.InsertDownload(downloadSingleLadies2);
            productSingleLadies.ProductVariants.Add(new ProductVariant()
            {
                Price = 3M,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Downloadable Products").Single().Id,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IsDownload = true,
                DownloadId = downloadSingleLadies1.Id,
                DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                UnlimitedDownloads = true,
                HasUserAgreement = false,
                HasSampleDownload = true,
                SampleDownloadId = downloadSingleLadies2.Id,

            });
            productSingleLadies.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Digital downloads").Single(),
                DisplayOrder = 1,
            });
            productSingleLadies.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_SingleLadies.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productSingleLadies.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productSingleLadies);





            var productBattleOfLa = new Product()
            {
                Name = "The Battle Of Los Angeles",
                ShortDescription = "The Battle Of Los Angeles by Rage Against The Machine",
                FullDescription = "<p># Original Release Date: November 2, 1999<br /># Label: Epic<br /># Copyright: 1999 Sony Music Entertainment Inc. (c) 1999 Sony Music Entertainment Inc.</p>",
                ProductTemplateId = productTemplateSingleVariant.Id,
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            var downloadBattleOfLa = new Download()
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = "application/x-zip-co",
                DownloadBinary = File.ReadAllBytes(sampleDownloadsPath + "product_BattleOfLa_1.zip"),
                Extension = ".zip",
                Filename = "The_Battle_Of_Los_Angeles",
                IsNew = true,
            };
            downloadService.InsertDownload(downloadBattleOfLa);
            productBattleOfLa.ProductVariants.Add(new ProductVariant()
            {
                Price = 3M,
                TaxCategoryId = _taxCategoryRepository.Table.Where(tc => tc.Name == "Downloadable Products").Single().Id,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IsDownload = true,
                DownloadId = downloadBattleOfLa.Id,
                DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                UnlimitedDownloads = true,
                HasUserAgreement = false,

            });
            productBattleOfLa.ProductCategories.Add(new ProductCategory()
            {
                Category = _categoryRepository.Table.Where(c => c.Name == "Digital downloads").Single(),
                DisplayOrder = 1,
            });
            productBattleOfLa.ProductPictures.Add(new ProductPicture()
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_BattleOfLA.jpeg"), "image/jpeg", pictureService.GetPictureSeName(productBattleOfLa.Name), true),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productBattleOfLa);






            //related products
            var relatedProducts = new List<RelatedProduct>()
            {
                new RelatedProduct()
                {
                     ProductId1 = productDiamondHeart.Id,
                     ProductId2 = productDiamondBracelet.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productDiamondHeart.Id,
                     ProductId2 = productDiamondEarrings.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productDiamondHeart.Id,
                     ProductId2 = productEngagementRing.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productDiamondBracelet.Id,
                     ProductId2 = productDiamondHeart.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productDiamondBracelet.Id,
                     ProductId2 = productEngagementRing.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productDiamondBracelet.Id,
                     ProductId2 = productDiamondEarrings.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productEngagementRing.Id,
                     ProductId2 = productDiamondHeart.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productEngagementRing.Id,
                     ProductId2 = productDiamondBracelet.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productEngagementRing.Id,
                     ProductId2 = productDiamondEarrings.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productDiamondEarrings.Id,
                     ProductId2 = productDiamondHeart.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productDiamondEarrings.Id,
                     ProductId2 = productDiamondBracelet.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productDiamondEarrings.Id,
                     ProductId2 = productEngagementRing.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productSingleLadies.Id,
                     ProductId2 = productPokerFace.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productSingleLadies.Id,
                     ProductId2 = productBattleOfLa.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productPokerFace.Id,
                     ProductId2 = productSingleLadies.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productPokerFace.Id,
                     ProductId2 = productBattleOfLa.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productBestSkilletRecipes.Id,
                     ProductId2 = productCookingForTwo.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productBestSkilletRecipes.Id,
                     ProductId2 = productEatingWell.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productBestSkilletRecipes.Id,
                     ProductId2 = productBestGrillingRecipes.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productCookingForTwo.Id,
                     ProductId2 = productBestSkilletRecipes.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productCookingForTwo.Id,
                     ProductId2 = productEatingWell.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productCookingForTwo.Id,
                     ProductId2 = productBestGrillingRecipes.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productEatingWell.Id,
                     ProductId2 = productBestSkilletRecipes.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productEatingWell.Id,
                     ProductId2 = productCookingForTwo.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productEatingWell.Id,
                     ProductId2 = productBestGrillingRecipes.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productBestGrillingRecipes.Id,
                     ProductId2 = productCookingForTwo.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productBestGrillingRecipes.Id,
                     ProductId2 = productEatingWell.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productBestGrillingRecipes.Id,
                     ProductId2 = productBestSkilletRecipes.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productAsusPc900.Id,
                     ProductId2 = productSatellite.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productAsusPc900.Id,
                     ProductId2 = productAsusPc1000.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productAsusPc900.Id,
                     ProductId2 = productHpPavilion1.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productSatellite.Id,
                     ProductId2 = productAsusPc900.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productSatellite.Id,
                     ProductId2 = productAsusPc1000.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productSatellite.Id,
                     ProductId2 = productAcerAspireOne.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productAsusPc1000.Id,
                     ProductId2 = productSatellite.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productAsusPc1000.Id,
                     ProductId2 = productHpPavilion1.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productAsusPc1000.Id,
                     ProductId2 = productAcerAspireOne.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productHpPavilion3.Id,
                     ProductId2 = productAsusPc900.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productHpPavilion3.Id,
                     ProductId2 = productAsusPc1000.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productHpPavilion3.Id,
                     ProductId2 = productAcerAspireOne.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productHpPavilion1.Id,
                     ProductId2 = productAsusPc900.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productHpPavilion1.Id,
                     ProductId2 = productAsusPc1000.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productHpPavilion1.Id,
                     ProductId2 = productAcerAspireOne.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productCanonCamcoder.Id,
                     ProductId2 = productSamsungPhone.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productCanonCamcoder.Id,
                     ProductId2 = productSonyCamcoder.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productCanonCamcoder.Id,
                     ProductId2 = productCanonCamera.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productSonyCamcoder.Id,
                     ProductId2 = productSamsungPhone.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productSonyCamcoder.Id,
                     ProductId2 = productCanonCamcoder.Id,
                },
                new RelatedProduct()
                {
                     ProductId1 = productSonyCamcoder.Id,
                     ProductId2 = productCanonCamera.Id,
                },
            };
            relatedProducts.ForEach(rp => _relatedProductRepository.Insert(rp));

            //product tags
            AddProductTag(product25GiftCard, "nice");
            AddProductTag(product25GiftCard, "gift");
            AddProductTag(product5GiftCard, "nice");
            AddProductTag(product5GiftCard, "gift");
            AddProductTag(productRockabillyPolka, "cool");
            AddProductTag(productRockabillyPolka, "apparel");
            AddProductTag(productRockabillyPolka, "shirt");
            AddProductTag(productAcerAspireOne, "computer");
            AddProductTag(productAcerAspireOne, "cool");
            AddProductTag(productAdidasShoe, "cool");
            AddProductTag(productAdidasShoe, "shoes");
            AddProductTag(productAdidasShoe, "apparel");
            AddProductTag(productAdobePhotoshop, "computer");
            AddProductTag(productAdobePhotoshop, "awesome");
            AddProductTag(productApcUps, "computer");
            AddProductTag(productApcUps, "cool");
            AddProductTag(productArrow, "cool");
            AddProductTag(productArrow, "apparel");
            AddProductTag(productArrow, "shirt");
            AddProductTag(productAsusPc1000, "compact");
            AddProductTag(productAsusPc1000, "awesome");
            AddProductTag(productAsusPc1000, "computer");
            AddProductTag(productAsusPc900, "compact");
            AddProductTag(productAsusPc900, "awesome");
            AddProductTag(productAsusPc900, "computer");
            AddProductTag(productBestGrillingRecipes, "awesome");
            AddProductTag(productBestGrillingRecipes, "book");
            AddProductTag(productBestGrillingRecipes, "nice");
            AddProductTag(productDiamondHeart, "awesome");
            AddProductTag(productDiamondHeart, "jewelry");
            AddProductTag(productBlackBerry, "cell");
            AddProductTag(productBlackBerry, "compact");
            AddProductTag(productBlackBerry, "awesome");
            AddProductTag(productBuildComputer, "awesome");
            AddProductTag(productBuildComputer, "computer");
            AddProductTag(productCanonCamera, "cool");
            AddProductTag(productCanonCamera, "camera");
            AddProductTag(productCanonCamcoder, "camera");
            AddProductTag(productCanonCamcoder, "cool");
            AddProductTag(productCompaq, "cool");
            AddProductTag(productCompaq, "computer");
            AddProductTag(productCookingForTwo, "awesome");
            AddProductTag(productCookingForTwo, "book");
            AddProductTag(productCorel, "awesome");
            AddProductTag(productCorel, "computer");
            AddProductTag(productCustomTShirt, "cool");
            AddProductTag(productCustomTShirt, "shirt");
            AddProductTag(productCustomTShirt, "apparel");
            AddProductTag(productDiamondEarrings, "jewelry");
            AddProductTag(productDiamondEarrings, "awesome");
            AddProductTag(productDiamondBracelet, "awesome");
            AddProductTag(productDiamondBracelet, "jewelry");
            AddProductTag(productEatingWell, "book");
            AddProductTag(productEtnies, "cool");
            AddProductTag(productEtnies, "shoes");
            AddProductTag(productEtnies, "apparel");
            AddProductTag(productLeatherHandbag, "apparel");
            AddProductTag(productLeatherHandbag, "cool");
            AddProductTag(productLeatherHandbag, "awesome");
            AddProductTag(productHp506, "awesome");
            AddProductTag(productHp506, "computer");
            AddProductTag(productHpPavilion1, "nice");
            AddProductTag(productHpPavilion1, "computer");
            AddProductTag(productHpPavilion1, "compact");
            AddProductTag(productHpPavilion2, "nice");
            AddProductTag(productHpPavilion2, "computer");
            AddProductTag(productHpPavilion3, "computer");
            AddProductTag(productHpPavilion3, "cool");
            AddProductTag(productHpPavilion3, "compact");
            AddProductTag(productHat, "apparel");
            AddProductTag(productHat, "cool");
            AddProductTag(productKensington, "computer");
            AddProductTag(productKensington, "cool");
            AddProductTag(productLeviJeans, "cool");
            AddProductTag(productLeviJeans, "jeans");
            AddProductTag(productLeviJeans, "apparel");
            AddProductTag(productBaseball, "game");
            AddProductTag(productBaseball, "computer");
            AddProductTag(productBaseball, "cool");
            AddProductTag(productPokerFace, "awesome");
            AddProductTag(productPokerFace, "digital");
            AddProductTag(productSunglasses, "apparel");
            AddProductTag(productSunglasses, "cool");
            AddProductTag(productSamsungPhone, "awesome");
            AddProductTag(productSamsungPhone, "compact");
            AddProductTag(productSamsungPhone, "cell");
            AddProductTag(productSingleLadies, "digital");
            AddProductTag(productSingleLadies, "awesome");
            AddProductTag(productSonyCamcoder, "awesome");
            AddProductTag(productSonyCamcoder, "cool");
            AddProductTag(productSonyCamcoder, "camera");
            AddProductTag(productBattleOfLa, "digital");
            AddProductTag(productBattleOfLa, "awesome");
            AddProductTag(productBestSkilletRecipes, "book");
            AddProductTag(productSatellite, "awesome");
            AddProductTag(productSatellite, "computer");
            AddProductTag(productSatellite, "compact");
            AddProductTag(productDenimShort, "jeans");
            AddProductTag(productDenimShort, "cool");
            AddProductTag(productDenimShort, "apparel");
            AddProductTag(productEngagementRing, "jewelry");
            AddProductTag(productEngagementRing, "awesome");
            AddProductTag(productWoW, "computer");
            AddProductTag(productWoW, "cool");
            AddProductTag(productWoW, "game");
            AddProductTag(productSoccer, "game");
            AddProductTag(productSoccer, "cool");
            AddProductTag(productSoccer, "computer");
        }

        protected virtual void InstallForums()
        {
            var forumGroup = new ForumGroup()
            {
                Name = "General",
                Description = "",
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };

            _forumGroupRepository.Insert(forumGroup);

            var newProductsForum = new Forum()
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
                UpdatedOnUtc = DateTime.UtcNow,
            };
            _forumRepository.Insert(newProductsForum);

            var mobileDevicesForum = new Forum()
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
                UpdatedOnUtc = DateTime.UtcNow,
            };
            _forumRepository.Insert(mobileDevicesForum);

            var packagingShippingForum = new Forum()
            {
                ForumGroup = forumGroup,
                Name = "Packaging & Shipping",
                Description = "Discuss packaging & shipping",
                NumTopics = 0,
                NumPosts = 0,
                LastPostTime = null,
                DisplayOrder = 20,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
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
                                };
            discounts.ForEach(d => _discountRepository.Insert(d));

        }

        protected virtual void InstallBlogPosts()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();
            var blogPosts = new List<BlogPost>
                                {
                                    new BlogPost
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "Online Discount Coupons",
                                             Body = "<p>Online discount coupons enable access to great offers from some of the world&rsquo;s best sites for Internet shopping. The online coupons are designed to allow compulsive online shoppers to access massive discounts on a variety of products. The regular shopper accesses the coupons in bulk and avails of great festive offers and freebies thrown in from time to time.  The coupon code option is most commonly used when using a shopping cart. The coupon code is entered on the order page just before checking out. Every online shopping resource has a discount coupon submission option to confirm the coupon code. The dedicated web sites allow the shopper to check whether or not a discount is still applicable. If it is, the sites also enable the shopper to calculate the total cost after deducting the coupon amount like in the case of grocery coupons.  Online discount coupons are very convenient to use. They offer great deals and professionally negotiated rates if bought from special online coupon outlets. With a little research and at times, insider knowledge the online discount coupons are a real steal. They are designed to promote products by offering &lsquo;real value for money&rsquo; packages. The coupons are legitimate and help with budgeting, in the case of a compulsive shopper. They are available for special trade show promotions, nightlife, sporting events and dinner shows and just about anything that could be associated with the promotion of a product. The coupons enable the online shopper to optimize net access more effectively. Getting a &lsquo;big deal&rsquo; is not more utopian amidst rising prices. The online coupons offer internet access to the best and cheapest products displayed online. Big discounts are only a code away! By Gaynor Borade (buzzle.com)</p>",
                                             Tags = "e-commerce, money",
                                             CreatedOnUtc = DateTime.UtcNow,
                                        },
                                    new BlogPost
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "Customer Service - Client Service",
                                             Body = "<p>Managing online business requires different skills and abilities than managing a business in the &lsquo;real world.&rsquo; Customers can easily detect the size and determine the prestige of a business when they have the ability to walk in and take a look around. Not only do &lsquo;real-world&rsquo; furnishings and location tell the customer what level of professionalism to expect, but &quot;real world&quot; personal encounters allow first impressions to be determined by how the business approaches its customer service. When a customer walks into a retail business just about anywhere in the world, that customer expects prompt and personal service, especially with regards to questions that they may have about products they wish to purchase.<br /><br />Customer service or the client service is the service provided to the customer for his satisfaction during and after the purchase. It is necessary to every business organization to understand the customer needs for value added service. So customer data collection is essential. For this, a good customer service is important. The easiest way to lose a client is because of the poor customer service. The importance of customer service changes by product, industry and customer. Client service is an important part of every business organization. Each organization is different in its attitude towards customer service. Customer service requires a superior quality service through a careful design and execution of a series of activities which include people, technology and processes. Good customer service starts with the design and communication between the company and the staff.<br /><br />In some ways, the lack of a physical business location allows the online business some leeway that their &lsquo;real world&rsquo; counterparts do not enjoy. Location is not important, furnishings are not an issue, and most of the visual first impression is made through the professional design of the business website.<br /><br />However, one thing still remains true. Customers will make their first impressions on the customer service they encounter. Unfortunately, in online business there is no opportunity for front- line staff to make a good impression. Every interaction the customer has with the website will be their primary means of making their first impression towards the business and its client service. Good customer service in any online business is a direct result of good website design and planning.</p><p>By Jayashree Pakhare (buzzle.com)</p>",
                                             Tags = "e-commerce, nopCommerce, asp.net, sample tag, money",
                                             CreatedOnUtc = DateTime.UtcNow.AddSeconds(1),
                                        },
                                };
            blogPosts.ForEach(bp => _blogPostRepository.Insert(bp));

        }

        protected virtual void InstallNews()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();
            var news = new List<NewsItem>
                                {
                                    new NewsItem
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "nopCommerce new release!",
                                             Short = "nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!<br /><br />nopCommerce is a fully customizable shopping cart. It's stable and highly usable. From downloads to documentation, www.nopCommerce.com offers a comprehensive base of information, resources, and support to the nopCommerce community.",
                                             Full = "<p>nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!</p><p>For full feature list go to <a href=\"http://www.nopCommerce.com\">nopCommerce.com</a></p><p>Providing outstanding custom search engine optimization, web development services and e-commerce development solutions to our clients at a fair price in a professional manner.</p>",
                                             Published  = true,
                                             CreatedOnUtc = DateTime.UtcNow,
                                        },
                                    new NewsItem
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "New online store is open!",
                                             Short = "The new nopCommerce store is open now! We are very excited to offer our new range of products. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.",
                                             Full = "<p>Our online store is officially up and running. Stock up for the holiday season! We have a great selection of items. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.</p><p>All shipping is worldwide and will leave the same day an order is placed! Happy Shopping and spread the word!!</p>",
                                             Published  = true,
                                             CreatedOnUtc = DateTime.UtcNow.AddSeconds(1),
                                        },
                                };
            news.ForEach(n => _newsItemRepository.Insert(n));

        }

        protected virtual void InstallPolls()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();
            var poll1 = new Poll
            {
                Language = defaultLanguage,
                Name = "Do you like nopCommerce?",
                SystemKeyword = "RightColumnPoll",
                Published = true,
                DisplayOrder = 1,
            };
            poll1.PollAnswers.Add(new PollAnswer()
            {
                Name = "Excellent",
                DisplayOrder = 1,
            });
            poll1.PollAnswers.Add(new PollAnswer()
            {
                Name = "Good",
                DisplayOrder = 2,
            });
            poll1.PollAnswers.Add(new PollAnswer()
            {
                Name = "Poor",
                DisplayOrder = 3,
            });
            poll1.PollAnswers.Add(new PollAnswer()
            {
                Name = "Very bad",
                DisplayOrder = 4,
            });
            _pollRepository.Insert(poll1);
        }

        protected virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>()
                                      {
                                          //admin area activities
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
                                                  SystemKeyword = "AddNewCustomer",
                                                  Enabled = true,
                                                  Name = "Add a new customer"
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
                                                  SystemKeyword = "AddNewGiftCard",
                                                  Enabled = true,
                                                  Name = "Add a new gift card"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewManufacturer",
                                                  Enabled = true,
                                                  Name = "Add a new manufacturer"
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
                                                  SystemKeyword = "AddNewProductVariant",
                                                  Enabled = true,
                                                  Name = "Add a new product variant"
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
                                                  SystemKeyword = "AddNewWidget",
                                                  Enabled = true,
                                                  Name = "Add a new widget"
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
                                                  SystemKeyword = "DeleteCustomer",
                                                  Enabled = true,
                                                  Name = "Delete a customer"
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
                                                  SystemKeyword = "DeleteGiftCard",
                                                  Enabled = true,
                                                  Name = "Delete a gift card"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteManufacturer",
                                                  Enabled = true,
                                                  Name = "Delete a manufacturer"
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
                                                  SystemKeyword = "DeleteProductVariant",
                                                  Enabled = true,
                                                  Name = "Delete a product variant"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteReturnRequest",
                                                  Enabled = true,
                                                  Name = "Delete a return request"
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
                                                  SystemKeyword = "DeleteWidget",
                                                  Enabled = true,
                                                  Name = "Delete a widget"
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
                                                  SystemKeyword = "EditCustomer",
                                                  Enabled = true,
                                                  Name = "Edit a customer"
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
                                                  SystemKeyword = "EditGiftCard",
                                                  Enabled = true,
                                                  Name = "Edit a gift card"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditManufacturer",
                                                  Enabled = true,
                                                  Name = "Edit a manufacturer"
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
                                                  SystemKeyword = "EditProductVariant",
                                                  Enabled = true,
                                                  Name = "Edit a product variant"
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
                                                  SystemKeyword = "EditSettings",
                                                  Enabled = true,
                                                  Name = "Edit setting(s)"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditSpecAttribute",
                                                  Enabled = true,
                                                  Name = "Edit a specification attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditWidget",
                                                  Enabled = true,
                                                  Name = "Edit a widget"
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
                                      };
            activityLogTypes.ForEach(alt => _activityLogTypeRepository.Insert(alt));
        }

        protected virtual void InstallProductTemplates()
        {
            var productTemplates = new List<ProductTemplate>
                               {
                                   new ProductTemplate
                                       {
                                           Name = "Variants in Grid",
                                           ViewPath = "ProductTemplate.VariantsInGrid",
                                           DisplayOrder = 1
                                       },
                                   new ProductTemplate
                                       {
                                           Name = "Single Product Variant",
                                           ViewPath = "ProductTemplate.SingleVariant",
                                           DisplayOrder = 10
                                       },
                               };
            productTemplates.ForEach(pt => _productTemplateRepository.Insert(pt));

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
                                       },
                               };
            categoryTemplates.ForEach(ct => _categoryTemplateRepository.Insert(ct));

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
                                       },
                               };
            manufacturerTemplates.ForEach(mt => _manufacturerTemplateRepository.Insert(mt));

        }

        protected virtual void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>()
            {
                new ScheduleTask()
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "Nop.Services.Messages.QueuedMessagesSendTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask()
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "Nop.Services.Common.KeepAliveTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask()
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "Nop.Services.Customers.DeleteGuestsTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask()
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "Nop.Services.Caching.ClearCacheTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask()
                {
                    Name = "Update currency exchange rates",
                    Seconds = 900,
                    Type = "Nop.Services.Directory.UpdateExchangeRateTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false,
                },
            };

            tasks.ForEach(x => _scheduleTaskRepository.Insert(x));
        }

        private void AddProductTag(Product product, string tag)
        {
            var productTag = _productTagRepository.Table.Where(pt => pt.Name == tag).FirstOrDefault();
            if (productTag == null)
            {
                productTag = new ProductTag()
                {
                    Name = tag,
                    ProductCount = 1,
                };
                productTag.Products.Add(product);
                _productTagRepository.Insert(productTag);
            }
            else
            {
                productTag.ProductCount++;
                productTag.Products.Add(product);
                _productTagRepository.Update(productTag);
            }
        }

        #endregion

        #region Methods

        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            InstallMeasures();
            InstallTaxCategories();
            InstallLanguages();
            InstallCurrencies();
            InstallCountriesAndStates();
            InstallShippingMethods();
            InstallCustomersAndUsers(defaultUserEmail, defaultUserPassword);
            InstallEmailAccounts();
            InstallMessageTemplates();
            InstallTopics();
            InstallSettings();
            InstallLocaleResources();
            InstallActivityLogTypes();
            HashDefaultCustomerPassword(defaultUserEmail, defaultUserPassword);
            InstallProductTemplates();
            InstallCategoryTemplates();
            InstallManufacturerTemplates();
            InstallScheduleTasks();

            if (installSampleData)
            {
                InstallSpecificationAttributes();
                InstallProductAttributes();
                InstallCategories();
                InstallManufacturers();
                InstallProducts();
                InstallForums();
                InstallDiscounts();
                InstallBlogPosts();
                InstallNews();
                InstallPolls();
            }
        }

        #endregion
    }
}