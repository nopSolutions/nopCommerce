using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Menus;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Translation;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Core.Security;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;

namespace Nop.Services.Installation;

public partial class InstallationService
{
    #region Utilities

    /// <summary>
    /// Installs a default stores
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    protected virtual async Task InstallStoresAsync()
    {
        var storeUrl = _webHelper.GetStoreLocation();
        var stores = new List<Store>
        {
            new() {
                Name = "Your store name",
                DefaultTitle = "Your store",
                DefaultMetaKeywords = string.Empty,
                DefaultMetaDescription = string.Empty,
                HomepageTitle = "Home page title",
                HomepageDescription = "Home page description",
                Url = storeUrl,
                SslEnabled = _webHelper.IsCurrentConnectionSecured(),
                Hosts = "yourstore.com,www.yourstore.com",
                DisplayOrder = 1,
                //should we set some default company info?
                CompanyName = "Your company name",
                CompanyAddress = "your company country, state, zip, street, etc",
                CompanyPhoneNumber = "(123) 456-78901",
                CompanyVat = null
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(stores);
    }

    /// <summary>
    /// Installs a default measures
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    protected virtual async Task InstallMeasuresAsync()
    {
        var isMetric = _installationSettings.RegionInfo?.IsMetric ?? false;

        var measureDimensions = new List<MeasureDimension>
        {
            new() {
                Name = "inch(es)",
                SystemKeyword = "inches",
                Ratio = isMetric ? 39.3701M : 1M,
                DisplayOrder = isMetric ? 1 : 0
            },
            new() {
                Name = "feet",
                SystemKeyword = "feet",
                Ratio = isMetric ? 3.28084M : 0.08333333M,
                DisplayOrder = isMetric ? 1 : 0
            },
            new() {
                Name = "meter(s)",
                SystemKeyword = "meters",
                Ratio = isMetric ? 1M : 0.0254M,
                DisplayOrder = isMetric ? 0 : 1
            },
            new() {
                Name = "millimetre(s)",
                SystemKeyword = "millimetres",
                Ratio = isMetric ? 1000M : 25.4M,
                DisplayOrder = isMetric ? 0 : 1
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(measureDimensions);

        var measureWeights = new List<MeasureWeight>
        {
            new() {
                Name = "ounce(s)",
                SystemKeyword = "ounce",
                Ratio = isMetric ? 35.274M : 16M,
                DisplayOrder = isMetric ? 1 : 0
            },
            new() {
                Name = "lb(s)",
                SystemKeyword = "lb",
                Ratio = isMetric ? 2.20462M : 1M,
                DisplayOrder = isMetric ? 1 : 0
            },
            new() {
                Name = "kg(s)",
                SystemKeyword = "kg",
                Ratio = isMetric ? 1M : 0.45359237M,
                DisplayOrder = isMetric ? 0 : 1
            },
            new() {
                Name = "gram(s)",
                SystemKeyword = "grams",
                Ratio = isMetric ? 1000M : 453.59237M,
                DisplayOrder = isMetric ? 0 : 1
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(measureWeights);
    }

    /// <summary>
    /// Installs a default tax categories
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    protected virtual async Task InstallTaxCategoriesAsync()
    {
        var taxCategories = new List<TaxCategory>
        {
            new() {Name = "Books", DisplayOrder = 1},
            new() {Name = "Electronics & Software", DisplayOrder = 5},
            new() {Name = "Downloadable Products", DisplayOrder = 10},
            new() {Name = "Jewelry", DisplayOrder = 15},
            new() {Name = "Apparel", DisplayOrder = 20}
        };

        await _dataProvider.BulkInsertEntitiesAsync(taxCategories);
    }

    /// <summary>
    /// Import language resources from XML file
    /// </summary>
    /// <param name="language">Language</param>
    /// <param name="xmlStreamReader">Stream reader of XML file</param>
    /// <param name="updateExistingResources">A value indicating whether to update existing resources</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportResourcesFromXmlAsync(Language language, StreamReader xmlStreamReader, bool updateExistingResources = true)
    {
        HashSet<(string name, string value)> loadLocaleResourcesFromStream()
        {
            var result = new HashSet<(string name, string value)>();

            using var xmlReader = XmlReader.Create(xmlStreamReader);
            while (xmlReader.ReadToFollowing("Language"))
            {
                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                using var languageReader = xmlReader.ReadSubtree();
                while (languageReader.ReadToFollowing("LocaleResource"))
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.GetAttribute("Name") is { } name)
                    {
                        using var lrReader = languageReader.ReadSubtree();
                        if (lrReader.ReadToFollowing("Value") && lrReader.NodeType == XmlNodeType.Element)
                            result.Add((name.ToLowerInvariant(), lrReader.ReadString()));
                    }

                break;
            }

            return result;
        }

        if (xmlStreamReader.EndOfStream)
            return;

        var lsNamesList = new Dictionary<string, LocaleStringResource>();

        foreach (var localeStringResource in Table<LocaleStringResource>().Where(lsr => lsr.LanguageId == language.Id)
                     .OrderBy(lsr => lsr.Id))
            lsNamesList[localeStringResource.ResourceName.ToLowerInvariant()] = localeStringResource;

        var lrsToUpdateList = new List<LocaleStringResource>();
        var lrsToInsertList = new Dictionary<string, LocaleStringResource>();

        foreach (var (name, value) in loadLocaleResourcesFromStream())
            if (lsNamesList.TryGetValue(name, out var localString))
            {
                if (!updateExistingResources)
                    continue;

                localString.ResourceValue = value;
                lrsToUpdateList.Add(localString);
            }
            else
                lrsToInsertList[name] = new LocaleStringResource
                {
                    LanguageId = language.Id,
                    ResourceName = name,
                    ResourceValue = value
                };

        await _dataProvider.UpdateEntitiesAsync(lrsToUpdateList);
        await _dataProvider.BulkInsertEntitiesAsync(lrsToInsertList.Values);
    }

    /// <summary>
    /// Import states from TXT file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of imported states
    /// </returns>
    protected virtual async Task<int> ImportStatesFromTxtAsync(Stream stream)
    {
        var count = 0;
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;
            var tmp = line.Split(',');

            if (tmp.Length != 5)
                throw new NopException("Wrong file format");

            //parse
            var countryTwoLetterIsoCode = tmp[0].Trim();
            var name = tmp[1].Trim();
            var abbreviation = tmp[2].Trim();
            var published = bool.Parse(tmp[3].Trim());
            var displayOrder = int.Parse(tmp[4].Trim());

            var country = await Table<Country>().Where(c => c.TwoLetterIsoCode == countryTwoLetterIsoCode).FirstOrDefaultAsync();
            //country cannot be loaded. skip
            if (country == null)
                continue;

            //import
            var states = await Table<StateProvince>()
                .OrderBy(sp => sp.DisplayOrder)
                .ThenBy(sp => sp.Name)
                .Where(sp => sp.CountryId == country.Id)
                .ToListAsync();
            var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (state != null)
            {
                state.Abbreviation = abbreviation;
                state.Published = published;
                state.DisplayOrder = displayOrder;

                await _dataProvider.UpdateEntityAsync(state);
            }
            else
            {
                state = new StateProvince
                {
                    CountryId = country.Id,
                    Name = name,
                    Abbreviation = abbreviation,
                    Published = published,
                    DisplayOrder = displayOrder
                };

                await _dataProvider.InsertEntityAsync(state);
            }

            count++;
        }

        return count;
    }

    /// <summary>
    /// Installs a default languages
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallLanguagesAsync()
    {
        var defaultCulture = new CultureInfo(NopCommonDefaults.DefaultLanguageCulture);
        var re = new Regex(" \\(.*\\)", RegexOptions.Compiled);
        var languageName = re.Replace(defaultCulture.NativeName, string.Empty);
        languageName = languageName[0].ToString().ToUpper() + languageName[1..];

        var defaultLanguage = new Language
        {
            Name = languageName,
            LanguageCulture = defaultCulture.Name,
            UniqueSeoCode = defaultCulture.TwoLetterISOLanguageName,
            FlagImageFileName = $"{defaultCulture.Name.ToLowerInvariant()[^2..]}.png",
            Rtl = defaultCulture.TextInfo.IsRightToLeft,
            Published = true,
            DisplayOrder = 1
        };
        await _dataProvider.InsertEntityAsync(defaultLanguage);

        //Install locale resources for default culture
        var directoryPath = _fileProvider.MapPath(NopInstallationDefaults.LocalizationResourcesPath);
        var pattern = $"*.{NopInstallationDefaults.LocalizationResourcesFileExtension}";
        foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
        {
            using var streamReader = new StreamReader(filePath);
            await ImportResourcesFromXmlAsync(defaultLanguage, streamReader);
        }

        var cultureInfo = _installationSettings.CultureInfo;
        var regionInfo = _installationSettings.RegionInfo;

        if (cultureInfo == null || regionInfo == null || cultureInfo.Name == NopCommonDefaults.DefaultLanguageCulture)
            return;

        languageName = re.Replace(cultureInfo.NativeName, string.Empty);
        languageName = languageName[0].ToString().ToUpper() + languageName[1..];

        var language = new Language
        {
            Name = languageName,
            LanguageCulture = cultureInfo.Name,
            UniqueSeoCode = cultureInfo.TwoLetterISOLanguageName,
            FlagImageFileName = $"{regionInfo.TwoLetterISORegionName.ToLowerInvariant()}.png",
            Rtl = cultureInfo.TextInfo.IsRightToLeft,
            Published = true,
            DisplayOrder = 2
        };
        await _dataProvider.InsertEntityAsync(language);

        if (string.IsNullOrEmpty(_installationSettings.LanguagePackDownloadLink))
            return;

        //download and import language pack
        try
        {
            var httpClient = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
            await using var stream = await httpClient.GetStreamAsync(_installationSettings.LanguagePackDownloadLink);
            using var streamReader = new StreamReader(stream);
            await ImportResourcesFromXmlAsync(language, streamReader);

            //set this language as default
            language.DisplayOrder = 0;
            await _dataProvider.UpdateEntityAsync(language);

            //save progress for showing in admin panel (only for first start)
            await _dataProvider.InsertEntityAsync(new GenericAttribute
            {
                EntityId = language.Id,
                Key = NopCommonDefaults.LanguagePackProgressAttribute,
                KeyGroup = nameof(Language),
                Value = _installationSettings.LanguagePackProgress.ToString(),
                StoreId = 0,
                CreatedOrUpdatedDateUTC = DateTime.UtcNow
            });
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Installs a default currencies
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallCurrenciesAsync()
    {
        //set some currencies with a rate against the USD
        var defaultCurrencies = new List<string> { "USD", "AUD", "GBP", "CAD", "CNY", "EUR", "HKD", "JPY", "RUB", "SEK", "INR" };
        var currencies = new List<Currency>
            {
                new() {
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
                new() {
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
                new() {
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
                new() {
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
                new() {
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
                new() {
                    Name = "Euro",
                    CurrencyCode = "EUR",
                    Rate = 0.86M,
                    DisplayLocale = string.Empty,
                    CustomFormatting = $"{"\u20ac"}0.00", //euro symbol
                    Published = false,
                    DisplayOrder = 6,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new() {
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
                new() {
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
                new() {
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
                new() {
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
                new() {
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

        //set additional currency
        var cultureInfo = _installationSettings.CultureInfo;
        var regionInfo = _installationSettings.RegionInfo;

        if (cultureInfo != null && regionInfo != null)
        {
            if (!defaultCurrencies.Contains(regionInfo.ISOCurrencySymbol))
            {
                currencies.Add(new Currency
                {
                    Name = regionInfo.CurrencyEnglishName,
                    CurrencyCode = regionInfo.ISOCurrencySymbol,
                    Rate = 1,
                    DisplayLocale = cultureInfo.Name,
                    CustomFormatting = string.Empty,
                    Published = true,
                    DisplayOrder = 0,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                });
            }

            foreach (var currency in currencies.Where(currency => currency.CurrencyCode == regionInfo.ISOCurrencySymbol))
            {
                currency.Published = true;
                currency.DisplayOrder = 0;
            }
        }

        await _dataProvider.BulkInsertEntitiesAsync(currencies);
    }

    /// <summary>
    /// Installs a default countries and states
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    protected virtual async Task InstallCountriesAndStatesAsync()
    {
        var countries = ISO3166.GetCollection().Select(country => new Country
        {
            Name = country.Name,
            AllowsBilling = true,
            AllowsShipping = true,
            TwoLetterIsoCode = country.Alpha2,
            ThreeLetterIsoCode = country.Alpha3,
            NumericIsoCode = country.NumericCode,
            SubjectToVat = country.SubjectToVat,
            DisplayOrder = country.NumericCode == 840 ? 1 : 100,
            Published = true
        }).ToList();

        await _dataProvider.BulkInsertEntitiesAsync(countries.ToArray());

        //import states for all countries
        var directoryPath = _fileProvider.MapPath(NopInstallationDefaults.LocalizationResourcesPath);
        var pattern = "*.txt";

        foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
        {
            await using var stream = new FileStream(filePath, FileMode.Open);
            await ImportStatesFromTxtAsync(stream);
        }
    }

    /// <summary>
    /// Installs a default shipping methods
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallShippingMethodsAsync()
    {
        var shippingMethods = new List<ShippingMethod>
            {
                new() {
                    Name = "Ground",
                    Description =
                        "Shipping by land transport",
                    DisplayOrder = 1
                },
                new() {
                    Name = "Next Day Air",
                    Description = "The one day air shipping",
                    DisplayOrder = 2
                },
                new() {
                    Name = "2nd Day Air",
                    Description = "The two day air shipping",
                    DisplayOrder = 3
                }
            };

        await _dataProvider.BulkInsertEntitiesAsync(shippingMethods);
    }

    /// <summary>
    /// Installs a default delivery dates
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallDeliveryDatesAsync()
    {
        var deliveryDates = new List<DeliveryDate>
        {
            new() {
                Name = "1-2 days",
                DisplayOrder = 1
            },
            new() {
                Name = "3-5 days",
                DisplayOrder = 5
            },
            new() {
                Name = "1 week",
                DisplayOrder = 10
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(deliveryDates);
    }

    /// <summary>
    /// Installs a default product availability ranges
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallProductAvailabilityRangesAsync()
    {
        var productAvailabilityRanges = new List<ProductAvailabilityRange>
        {
            new() {
                Name = "2-4 days",
                DisplayOrder = 1
            },
            new() {
                Name = "7-10 days",
                DisplayOrder = 2
            },
            new() {
                Name = "2 weeks",
                DisplayOrder = 3
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(productAvailabilityRanges);
    }

    /// <summary>
    /// Installs a default email accounts
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallEmailAccountsAsync()
    {
        var emailAccounts = new List<EmailAccount>
            {
                new() {
                    Email = "test@mail.com",
                    DisplayName = "Store name",
                    Host = "smtp.mail.com",
                    Port = 25,
                    Username = "123",
                    Password = "123",
                    EnableSsl = false
                }
            };

        await _dataProvider.BulkInsertEntitiesAsync(emailAccounts);
    }

    /// <summary>
    /// Installs a default message templates
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallMessageTemplatesAsync()
    {
        var eaGeneral = await Table<EmailAccount>().FirstOrDefaultAsync() ?? throw new Exception("Default email account cannot be loaded");

        var messageTemplates = new List<MessageTemplate>
            {
                new() {
                    Name = MessageTemplateSystemNames.BLOG_COMMENT_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. New blog comment.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.BACK_IN_STOCK_NOTIFICATION,
                    Subject = "%Store.Name%. Back in stock notification",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}Product <a target=\"_blank\" href=\"%BackInStockSubscription.ProductUrl%\">%BackInStockSubscription.ProductName%</a> is in stock.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.CUSTOMER_EMAIL_VALIDATION_MESSAGE,
                    Subject = "%Store.Name%. Email validation",
                    Body = $"<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To activate your account <a href=\"%Customer.AccountActivationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.CUSTOMER_EMAIL_REVALIDATION_MESSAGE,
                    Subject = "%Store.Name%. Email validation",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%!{Environment.NewLine}<br />{Environment.NewLine}To validate your new email address <a href=\"%Customer.EmailRevalidationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.PRIVATE_MESSAGE_NOTIFICATION,
                    Subject = "%Store.Name%. You have received a new private message",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You have received a new private message.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.CUSTOMER_PASSWORD_RECOVERY_MESSAGE,
                    Subject = "%Store.Name%. Password recovery",
                    Body = $"<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To change your password <a href=\"%Customer.PasswordRecoveryURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.CUSTOMER_WELCOME_MESSAGE,
                    Subject = "Welcome to %Store.Name%",
                    Body = $"We welcome you to <a href=\"%Store.URL%\"> %Store.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can now take part in the various services we have to offer you. Some of these services include:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.{Environment.NewLine}<br />{Environment.NewLine}Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{Environment.NewLine}<br />{Environment.NewLine}Order History - View your history of purchases that you have made with us.{Environment.NewLine}<br />{Environment.NewLine}Products Reviews - Share your opinions on products with our other customers.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For help with any of our online services, please email the store-owner: <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEW_FORUM_POST_MESSAGE,
                    Subject = "%Store.Name%. New Post Notification.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Post author: %Forums.PostAuthor%{Environment.NewLine}<br />{Environment.NewLine}Post body: %Forums.PostBody%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEW_FORUM_TOPIC_MESSAGE,
                    Subject = "%Store.Name%. New Topic Notification.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.GIFT_CARD_NOTIFICATION,
                    Subject = "%GiftCard.SenderName% has sent you a gift card for %Store.Name%",
                    Body = $"<p>{Environment.NewLine}You have received a gift card for %Store.Name%{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}Dear %GiftCard.RecipientName%,{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%GiftCard.SenderName% (%GiftCard.SenderEmail%) has sent you a %GiftCard.Amount% gift card for <a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}Your gift card code is %GiftCard.CouponCode%{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}%GiftCard.Message%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.CUSTOMER_REGISTERED_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. New customer registration",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new customer registered with your store. Below are the customer's details:{Environment.NewLine}<br />{Environment.NewLine}Full name: %Customer.FullName%{Environment.NewLine}<br />{Environment.NewLine}Email: %Customer.Email%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEW_RETURN_REQUEST_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. New return request.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% has just submitted a new return request. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Request ID: %ReturnRequest.CustomNumber%{Environment.NewLine}<br />{Environment.NewLine}Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%{Environment.NewLine}<br />{Environment.NewLine}Reason for return: %ReturnRequest.Reason%{Environment.NewLine}<br />{Environment.NewLine}Requested action: %ReturnRequest.RequestedAction%{Environment.NewLine}<br />{Environment.NewLine}Customer comments:{Environment.NewLine}<br />{Environment.NewLine}%ReturnRequest.CustomerComment%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new()
                {
                    Name = MessageTemplateSystemNames.DELETE_CUSTOMER_REQUEST_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. New request to delete customer (GDPR)",
                    Body = $"%Customer.Email% has requested account deletion. You can consider this in the admin area.",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEW_RETURN_REQUEST_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. New return request.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%!{Environment.NewLine}<br />{Environment.NewLine}You have just submitted a new return request. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Request ID: %ReturnRequest.CustomNumber%{Environment.NewLine}<br />{Environment.NewLine}Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%{Environment.NewLine}<br />{Environment.NewLine}Reason for return: %ReturnRequest.Reason%{Environment.NewLine}<br />{Environment.NewLine}Requested action: %ReturnRequest.RequestedAction%{Environment.NewLine}<br />{Environment.NewLine}Customer comments:{Environment.NewLine}<br />{Environment.NewLine}%ReturnRequest.CustomerComment%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEWS_COMMENT_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. New news comment.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new news comment has been created for news \"%NewsComment.NewsTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEWSLETTER_SUBSCRIPTION_ACTIVATION_MESSAGE,
                    Subject = "%Store.Name%. Subscription activation message.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEWSLETTER_SUBSCRIPTION_DEACTIVATION_MESSAGE,
                    Subject = "%Store.Name%. Subscription deactivation message.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from our newsletter.</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEW_VAT_SUBMITTED_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. New VAT number is submitted.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just submitted a new VAT number. Details are below:{Environment.NewLine}<br />{Environment.NewLine}VAT number: %Customer.VatNumber%{Environment.NewLine}<br />{Environment.NewLine}VAT number status: %Customer.VatNumberStatus%{Environment.NewLine}<br />{Environment.NewLine}Received name: %VatValidationResult.Name%{Environment.NewLine}<br />{Environment.NewLine}Received address: %VatValidationResult.Address%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_CANCELLED_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. Your order cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order has been cancelled. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_CANCELLED_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been cancelled by customer.{Environment.NewLine}<br />{Environment.NewLine}Customer: %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_CANCELLED_VENDOR_NOTIFICATION,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% cancelled",
                     Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been cancelled.{Environment.NewLine}<br />{Environment.NewLine}Customer: %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_PROCESSING_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. Your order is processing",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order is processing. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_COMPLETED_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. Your order completed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order has been completed. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_COMPLETED_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% completed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.CustomerFullName% has just completed an order. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = false, //this template is disabled by default
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.SHIPMENT_DELIVERED_CUSTOMER_NOTIFICATION,
                    Subject = "Your order from %Store.Name% has been %if (!%Order.IsCompletelyDelivered%) partially endif%delivered.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Good news! Your order has been %if (!%Order.IsCompletelyDelivered%) partially endif%delivered.{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% Delivered Products:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Shipment.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_PLACED_CUSTOMER_NOTIFICATION,
                    Subject = "Order receipt from %Store.Name%.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_PLACED_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. Purchase Receipt for Order #%Order.OrderNumber%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.CustomerFullName% (%Order.CustomerEmail%) has just placed an order from your store. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.SHIPMENT_SENT_CUSTOMER_NOTIFICATION,
                    Subject = "Your order from %Store.Name% has been %if (!%Order.IsCompletelyShipped%) partially endif%shipped.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%!,{Environment.NewLine}<br />{Environment.NewLine}Good news! Your order has been %if (!%Order.IsCompletelyShipped%) partially endif%shipped.{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% Shipped Products:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Shipment.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.SHIPMENT_READY_FOR_PICKUP_CUSTOMER_NOTIFICATION,
                    Subject = "Your order from %Store.Name% has been %if (!%Order.IsCompletelyReadyForPickup%) partially endif%ready for pickup.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%!,{Environment.NewLine}<br />{Environment.NewLine}Good news! Your order has been %if (!%Order.IsCompletelyReadyForPickup%) partially endif%ready for pickup.{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% Products ready for pickup:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Shipment.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.PRODUCT_REVIEW_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. New product review.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new product review has been written for product \"%ProductReview.ProductName%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.PRODUCT_REVIEW_REPLY_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. Product review reply.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}You received a reply from the store administration to your review for product \"%ProductReview.ProductName%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.QUANTITY_BELOW_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. Quantity below notification. %Product.Name%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Product.Name% (ID: %Product.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Quantity: %Product.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.QUANTITY_BELOW_ATTRIBUTE_COMBINATION_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. Quantity below notification. %Product.Name%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Product.Name% (ID: %Product.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}%AttributeCombination.Formatted%{Environment.NewLine}<br />{Environment.NewLine}Quantity: %AttributeCombination.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.QUANTITY_BELOW_VENDOR_NOTIFICATION,
                    Subject = "%Store.Name%. Quantity below notification. %Product.Name%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Product.Name% (ID: %Product.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Quantity: %Product.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.QUANTITY_BELOW_ATTRIBUTE_COMBINATION_VENDOR_NOTIFICATION,
                    Subject = "%Store.Name%. Quantity below notification. %Product.Name%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Product.Name% (ID: %Product.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}%AttributeCombination.Formatted%{Environment.NewLine}<br />{Environment.NewLine}Quantity: %AttributeCombination.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.RETURN_REQUEST_STATUS_CHANGED_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. Return request status was changed.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}Your return request #%ReturnRequest.CustomNumber% status has been changed.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.EMAIL_A_FRIEND_MESSAGE,
                    Subject = "%Store.Name%. Referred Item",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%EmailAFriend.Email% was shopping on %Store.Name% and wanted to share the following item with you.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<b><a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">%Product.Name%</a></b>{Environment.NewLine}<br />{Environment.NewLine}%Product.ShortDescription%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For more info click <a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">here</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%EmailAFriend.PersonalMessage%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.WISHLIST_TO_FRIEND_MESSAGE,
                    Subject = "%Store.Name%. Wishlist",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Wishlist.Email% was shopping on %Store.Name% and wanted to share a wishlist with you.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For more info click <a target=\"_blank\" href=\"%Wishlist.URLForCustomer%\">here</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Wishlist.PersonalMessage%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEW_ORDER_NOTE_ADDED_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. New order note has been added",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}New order note has been added to your account:{Environment.NewLine}<br />{Environment.NewLine}\"%Order.NewNoteText%\".{Environment.NewLine}<br />{Environment.NewLine}<a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.RECURRING_PAYMENT_CANCELLED_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%RecurringPayment.CancelAfterFailedPayment%) The last payment for the recurring payment ID=%RecurringPayment.ID% failed, so it was cancelled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) %Customer.FullName% (%Customer.Email%) has just cancelled a recurring payment ID=%RecurringPayment.ID%. endif%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.RECURRING_PAYMENT_CANCELLED_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}%if (%RecurringPayment.CancelAfterFailedPayment%) It appears your credit card didn't go through for this recurring payment (<a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>){Environment.NewLine}<br />{Environment.NewLine}So your subscription has been cancelled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) The recurring payment ID=%RecurringPayment.ID% was cancelled. endif%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.RECURRING_PAYMENT_FAILED_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. Last recurring payment failed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}It appears your credit card didn't go through for this recurring payment (<a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>){Environment.NewLine}<br /> %if (%RecurringPayment.RecurringPaymentType% == \"Manual\") {Environment.NewLine}You can recharge balance and manually retry payment or cancel it on the order history page. endif% %if (%RecurringPayment.RecurringPaymentType% == \"Automatic\") {Environment.NewLine}You can recharge balance and wait, we will try to make the payment again, or you can cancel it on the order history page. endif%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_PLACED_VENDOR_NOTIFICATION,
                    Subject = "%Store.Name%. Order placed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just placed an order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_PLACED_AFFILIATE_NOTIFICATION,
                    Subject = "%Store.Name%. Order placed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just placed an order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_REFUNDED_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% refunded",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Order #%Order.OrderNumber% has been has been refunded. Please allow 7-14 days for the refund to be reflected in your account.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Amount refunded: %Order.AmountRefunded%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br /{Environment.NewLine}>Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_REFUNDED_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% refunded",
                    Body = $"%Store.Name%. Order #%Order.OrderNumber% refunded', N'{Environment.NewLine}<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just refunded{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Amount refunded: %Order.AmountRefunded%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_PAID_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just paid{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_PAID_CUSTOMER_NOTIFICATION,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Order #%Order.OrderNumber% has been just paid. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_PAID_VENDOR_NOTIFICATION,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just paid.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.ORDER_PAID_AFFILIATE_NOTIFICATION,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just paid.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.NEW_VENDOR_ACCOUNT_APPLY_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. New vendor account submitted.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just submitted for a vendor account. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Vendor name: %Vendor.Name%{Environment.NewLine}<br />{Environment.NewLine}Vendor email: %Vendor.Email%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can activate it in admin area.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.VENDOR_INFORMATION_CHANGE_STORE_OWNER_NOTIFICATION,
                    Subject = "%Store.Name%. Vendor information change.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Vendor %Vendor.Name% (%Vendor.Email%) has just changed information about itself.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.CONTACT_US_MESSAGE,
                    Subject = "%Store.Name%. Contact us",
                    Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.CONTACT_VENDOR_MESSAGE,
                    Subject = "%Store.Name%. Contact us",
                    Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new() {
                    Name = MessageTemplateSystemNames.CUSTOMER_FAILED_LOGIN_ATTEMPT_NOTIFICATION,
                    Subject = "%Store.Name%. Failed Login Attempt",
                    Body = $"<p>{Environment.NewLine}You have received this notification because we registered a login attempt with invalid authentication on <a href=\"%Store.URL%\">%Store.Name%</a>.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
            };

        await _dataProvider.BulkInsertEntitiesAsync(messageTemplates);
    }

    /// <summary>
    /// Installs a default type of newsletter subscription
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallNewsLetterSubscriptionTypeAsync()
    {
        var newsLetterSubscriptionType = new List<NewsLetterSubscriptionType>
        {
            new() {
                Name = NopMessageDefaults.DefaultSubscriptionType,
                TickedByDefault = true,
                DisplayOrder = 0
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(newsLetterSubscriptionType);
    }

    /// <summary>
    /// Installs a default topic templates
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallTopicTemplatesAsync()
    {
        var topicTemplates = new List<TopicTemplate>
        {
            new() {
                Name = "Default template",
                ViewPath = "TopicDetails",
                DisplayOrder = 1
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(topicTemplates);
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <param name="allSettings">All settings in the dictionary view</param>
    /// <param name="type">Type</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SetSettingAsync(Dictionary<string, IList<Setting>> allSettings, Type type, string key, object value, int storeId = 0)
    {
        ArgumentNullException.ThrowIfNull(key);
        key = key.Trim().ToLowerInvariant();
        var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

        var settingForCaching = allSettings.TryGetValue(key, out var settings) ?
            settings.FirstOrDefault(x => x.StoreId == storeId) : null;
        if (settingForCaching != null)
        {
            //update
            var setting = await Table<Setting>().Where(s => s.Id == settingForCaching.Id).FirstOrDefaultAsync();
            setting.Value = valueStr;

            await _dataProvider.UpdateEntityAsync(setting);
        }
        else
        {
            //insert
            var setting = new Setting
            {
                Name = key,
                Value = valueStr,
                StoreId = storeId
            };

            await _dataProvider.InsertEntityAsync(setting);
        }
    }

    /// <summary>
    /// Save settings object
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="allSettings">All settings in the dictionary view</param>
    /// <param name="settings">Setting instance</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SaveSettingAsync<T>(Dictionary<string, IList<Setting>> allSettings, T settings, int storeId = 0) where T : ISettings, new()
    {
        foreach (var prop in typeof(T).GetProperties())
        {
            // get properties we can read and write to
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                continue;

            var key = typeof(T).Name + "." + prop.Name;
            var value = prop.GetValue(settings, null);
            if (value != null)
                await SetSettingAsync(allSettings, prop.PropertyType, key, value, storeId);
            else
                await SetSettingAsync(allSettings, typeof(string), key, string.Empty, storeId);
        }
    }

    /// <summary>
    /// Installs a default settings
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallSettingsAsync()
    {
        var isMetric = _installationSettings.RegionInfo?.IsMetric ?? false;
        var country = _installationSettings.RegionInfo?.TwoLetterISORegionName ?? string.Empty;
        var isGermany = country == "DE";
        var isEurope = ISO3166.FromCountryCode(country)?.SubjectToVat ?? false;

        var settings = await Table<Setting>()
            .OrderBy(s => s.Name)
            .ThenBy(s => s.StoreId)
            .ToListAsync();

        var dictionary = new Dictionary<string, IList<Setting>>();
        foreach (var s in settings)
        {
            var resourceName = s.Name.ToLowerInvariant();
            var settingForCaching = new Setting
            {
                Id = s.Id,
                Name = s.Name,
                Value = s.Value,
                StoreId = s.StoreId
            };
            if (!dictionary.TryGetValue(resourceName, out var value))
                //first setting
                dictionary.Add(resourceName, new List<Setting>
                {
                    settingForCaching
                });
            else
                //already added
                //most probably it's the setting with the same name but for some certain store (storeId > 0)
                value.Add(settingForCaching);
        }

        await SaveSettingAsync(dictionary, new PdfSettings
        {
            LogoPictureId = 0,
            LetterPageSizeEnabled = false,
            RenderOrderNotes = true,
            LtrFontName = NopCommonDefaults.PdfLtrFontName,
            RtlFontName = NopCommonDefaults.PdfRtlFontName,
            InvoiceFooterTextColumn1 = null,
            InvoiceFooterTextColumn2 = null,
            BaseFontSize = 10,
            ImageTargetSize = 200
        });

        await SaveSettingAsync(dictionary, new SitemapSettings
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

        await SaveSettingAsync(dictionary, new SitemapXmlSettings
        {
            SitemapXmlEnabled = true,
            SitemapXmlIncludeBlogPosts = true,
            SitemapXmlIncludeCategories = true,
            SitemapXmlIncludeManufacturers = true,
            SitemapXmlIncludeNews = true,
            SitemapXmlIncludeProducts = true,
            SitemapXmlIncludeProductTags = true,
            SitemapXmlIncludeCustomUrls = true,
            SitemapXmlIncludeTopics = true,
            RebuildSitemapXmlAfterHours = 2 * 24,
            SitemapBuildOperationDelay = 60
        });

        await SaveSettingAsync(dictionary, new CommonSettings
        {
            UseSystemEmailForContactUsForm = true,
            SubjectFieldOnContactUsForm = false,
            DisplayJavaScriptDisabledWarning = false,
            Log404Errors = true,
            BreadcrumbDelimiter = "/",
            BbcodeEditorOpenLinksInNewWindow = false,
            PopupForTermsOfServiceLinks = true,
            JqueryMigrateScriptLoggingActive = false,
            UseResponseCompression = true,
            FaviconAndAppIconsHeadCode =
                "<link rel=\"apple-touch-icon\" sizes=\"180x180\" href=\"/icons/icons_0/apple-touch-icon.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"32x32\" href=\"/icons/icons_0/favicon-32x32.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"192x192\" href=\"/icons/icons_0/android-chrome-192x192.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"16x16\" href=\"/icons/icons_0/favicon-16x16.png\"><link rel=\"manifest\" href=\"/icons/icons_0/site.webmanifest\"><link rel=\"mask-icon\" href=\"/icons/icons_0/safari-pinned-tab.svg\" color=\"#5bbad5\"><link rel=\"shortcut icon\" href=\"/icons/icons_0/favicon.ico\"><meta name=\"msapplication-TileColor\" content=\"#2d89ef\"><meta name=\"msapplication-TileImage\" content=\"/icons/icons_0/mstile-144x144.png\"><meta name=\"msapplication-config\" content=\"/icons/icons_0/browserconfig.xml\"><meta name=\"theme-color\" content=\"#ffffff\">",
            EnableHtmlMinification = false,
            RestartTimeout = NopCommonDefaults.RestartTimeout,
            HeaderCustomHtml = string.Empty,
            FooterCustomHtml = string.Empty
        });

        await SaveSettingAsync(dictionary, new SeoSettings
        {
            PageTitleSeparator = ". ",
            PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
            GenerateProductMetaDescription = true,
            ConvertNonWesternChars = false,
            AllowUnicodeCharsInUrls = true,
            CanonicalUrlsEnabled = false,
            QueryStringInCanonicalUrlsEnabled = false,
            WwwRequirement = WwwRequirement.NoMatter,
            TwitterMetaTags = true,
            OpenGraphMetaTags = true,
            MicrodataEnabled = true,
            ReservedUrlRecordSlugs = NopSeoDefaults.ReservedUrlRecordSlugs,
            CustomHeadTags = string.Empty
        });

        await SaveSettingAsync(dictionary, new AdminAreaSettings
        {
            DefaultGridPageSize = 15,
            ProductsBulkEditGridPageSize = 100,
            PopupGridPageSize = 7,
            GridPageSizes = "7, 15, 20, 50, 100",
            RichEditorAdditionalSettings = null,
            RichEditorAllowJavaScript = false,
            UseRichEditorForCustomerEmails = false,
            UseRichEditorInMessageTemplates = false,
            HideAdvertisementsOnAdminArea = false,
            CheckLicense = true,
            LastNewsTitleAdminArea = string.Empty,
            LicenseTerms = string.Empty,
            UseIsoDateFormatInJsonResult = true,
            ShowDocumentationReferenceLinks = true,
            UseStickyHeaderLayout = false,
            MinimumDropdownItemsForSearch = 50
        });

        await SaveSettingAsync(dictionary, new ProductEditorSettings
        {
            Weight = true,
            Dimensions = true,
            ProductAttributes = true,
            SpecificationAttributes = true,
            FilterLevelValuesProducts = true,
            PAngV = isGermany
        });

        await SaveSettingAsync(dictionary, new GdprSettings
        {
            DeleteInactiveCustomersAfterMonths = 36,
            GdprEnabled = false,
            LogPrivacyPolicyConsent = true,
            LogNewsletterConsent = true,
            LogUserProfileChanges = true
        });

        await SaveSettingAsync(dictionary, new CatalogSettings
        {
            AllowViewUnpublishedProductPage = true,
            DisplayDiscontinuedMessageForUnpublishedProducts = true,
            PublishBackProductWhenCancellingOrders = false,
            ShowSkuOnProductDetailsPage = true,
            ShowSkuOnCatalogPages = false,
            ShowManufacturerPartNumber = false,
            ShowGtin = false,
            ShowFreeShippingNotification = true,
            ShowShortDescriptionOnCatalogPages = false,
            AllowProductSorting = true,
            AllowProductViewModeChanging = true,
            DefaultViewMode = "grid",
            ShowProductsFromSubcategories = false,
            ShowCategoryProductNumber = false,
            ShowCategoryProductNumberIncludingSubcategories = false,
            CategoryBreadcrumbEnabled = true,
            ShowShareButton = true,
            PageShareCode =
                "<!-- ShareThis Button BEGIN --><div class=\"sharethis-inline-share-buttons\"></div><script type=\"text/javascript\" src=\"https://platform-api.sharethis.com/js/sharethis.js#property=64428a0865e28d00193ae8a9&product=inline-share-buttons&source=nopcommerce\" async=\"async\"></script><!-- ShareThis Button END -->",
            ProductReviewsMustBeApproved = false,
            OneReviewPerProductFromCustomer = false,
            DefaultProductRatingValue = 5,
            AllowAnonymousUsersToReviewProduct = false,
            ProductReviewPossibleOnlyAfterPurchasing = false,
            NotifyStoreOwnerAboutNewProductReviews = false,
            NotifyCustomerAboutProductReviewReply = false,
            EmailAFriendEnabled = true,
            AllowAnonymousUsersToEmailAFriend = false,
            RecentlyViewedProductsNumber = 3,
            RecentlyViewedProductsEnabled = true,
            NewProductsEnabled = true,
            NewProductsPageSize = 6,
            NewProductsAllowCustomersToSelectPageSize = true,
            NewProductsPageSizeOptions = "6, 3, 9",
            CompareProductsEnabled = true,
            CompareProductsNumber = 4,
            ProductSearchAutoCompleteEnabled = true,
            ProductSearchEnabled = true,
            ProductSearchAutoCompleteNumberOfProducts = 10,
            ShowLinkToAllResultInSearchAutoComplete = false,
            ProductSearchTermMinimumLength = 3,
            ShowProductImagesInSearchAutoComplete = false,
            ShowBestsellersOnHomepage = false,
            NumberOfBestsellersOnHomepage = 4,
            ShowSearchBoxCategories = false,
            SearchPageProductsPerPage = 6,
            SearchPageAllowCustomersToSelectPageSize = true,
            SearchPagePageSizeOptions = "6, 3, 9, 18",
            SearchPagePriceRangeFiltering = true,
            SearchPageManuallyPriceRange = true,
            SearchPagePriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
            SearchPagePriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
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
            ProductsByTagPriceRangeFiltering = true,
            ProductsByTagManuallyPriceRange = true,
            ProductsByTagPriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
            ProductsByTagPriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
            MaximumBackInStockSubscriptions = 200,
            ManufacturersBlockItemsToDisplay = 2,
            DisplayTaxShippingInfoFooter = isGermany,
            DisplayTaxShippingInfoProductDetailsPage = isGermany,
            DisplayTaxShippingInfoProductBoxes = isGermany,
            DisplayTaxShippingInfoShoppingCart = isGermany,
            DisplayTaxShippingInfoWishlist = isGermany,
            DisplayTaxShippingInfoOrderDetailsPage = isGermany,
            DefaultCategoryPageSizeOptions = "6, 3, 9",
            DefaultCategoryPageSize = 6,
            DefaultManufacturerPageSizeOptions = "6, 3, 9",
            DefaultManufacturerPageSize = 6,
            ShowProductReviewsTabOnAccountPage = true,
            ProductReviewsPageSizeOnAccountPage = 10,
            ProductReviewsSortByCreatedDateAscending = false,
            ExportImportProductAttributes = true,
            ExportImportProductSpecificationAttributes = true,
            ExportImportTierPrices = true,
            ExportImportUseDropdownlistsForAssociatedEntities = true,
            ExportImportProductsCountInOneFile = 500,
            ExportImportSplitProductsFile = false,
            ExportImportRelatedEntitiesByName = true,
            ExportImportCategoryUseLimitedToStores = false,
            CountDisplayedYearsDatePicker = 1,
            UseAjaxCatalogProductsLoading = true,
            EnableManufacturerFiltering = true,
            EnablePriceRangeFiltering = true,
            EnableSpecificationAttributeFiltering = true,
            DisplayFromPrices = false,
            AttributeValueOutOfStockDisplayType = AttributeValueOutOfStockDisplayType.AlwaysDisplay,
            AllowCustomersToSearchWithCategoryName = false,
            AllowCustomersToSearchWithManufacturerName = false,
            DisplayAllPicturesOnCatalogPages = false,
            ProductUrlStructureTypeId = (int)ProductUrlStructureType.Product,
            ActiveSearchProviderSystemName = string.Empty,
            UseStandardSearchWhenSearchProviderThrowsException = true
        });

        await SaveSettingAsync(dictionary, new ArtificialIntelligenceSettings
        {
            Enabled = false,
            ChatGptApiKey = string.Empty,
            DeepSeekApiKey = string.Empty,
            GeminiApiKey = string.Empty,
            ProviderType = ArtificialIntelligenceProviderType.Gemini,
            RequestTimeout = ArtificialIntelligenceDefaults.RequestTimeout,
            AllowProductDescriptionGeneration = true,
            ProductDescriptionQuery = ArtificialIntelligenceDefaults.ProductDescriptionQuery,
            AllowMetaTitleGeneration = true,
            MetaTitleQuery = ArtificialIntelligenceDefaults.MetaTitleQuery,
            AllowMetaKeywordsGeneration = true,
            MetaKeywordsQuery = ArtificialIntelligenceDefaults.MetaKeywordsQuery,
            AllowMetaDescriptionGeneration = true,
            MetaDescriptionQuery = ArtificialIntelligenceDefaults.MetaDescriptionQuery
        });

        await SaveSettingAsync(dictionary, new LocalizationSettings
        {
            DefaultAdminLanguageId = (await Table<Language>().SingleAsync(l => l.LanguageCulture == NopCommonDefaults.DefaultLanguageCulture)).Id,
            UseImagesForLanguageSelection = false,
            SeoFriendlyUrlsForLanguagesEnabled = false,
            AutomaticallyDetectLanguage = false,
            LoadAllLocaleRecordsOnStartup = true,
            LoadAllLocalizedPropertiesOnStartup = true,
            LoadAllUrlRecordsOnStartup = false,
            IgnoreRtlPropertyForAdminArea = false
        });

        await SaveSettingAsync(dictionary, new TranslationSettings
        {
            TranslateFromLanguageId = (await Table<Language>().FirstAsync()).Id,
            AllowPreTranslate = false,
            GoogleApiKey = string.Empty,
            NotTranslateLanguages = new List<int>(),
            DeepLAuthKey = string.Empty,
            TranslationServiceId = (int)TranslationServiceType.GoogleTranslate
        });

        await SaveSettingAsync(dictionary, new CustomerSettings
        {
            UsernamesEnabled = false,
            CheckUsernameAvailabilityEnabled = false,
            AllowUsersToChangeUsernames = false,
            DefaultPasswordFormat = PasswordFormat.Hashed,
            HashedPasswordFormat = NopCustomerServicesDefaults.DefaultHashedPasswordFormat,
            PasswordMinLength = 6,
            PasswordMaxLength = 64,
            PasswordRequireDigit = false,
            PasswordRequireLowercase = false,
            PasswordRequireNonAlphanumeric = false,
            PasswordRequireUppercase = false,
            UnduplicatedPasswordsNumber = 4,
            PasswordRecoveryLinkDaysValid = 7,
            PasswordLifetime = 90,
            FailedPasswordAllowedAttempts = 0,
            FailedPasswordLockoutMinutes = 30,
            RequiredReLoginAfterPasswordChange = false,
            UserRegistrationType = UserRegistrationType.Standard,
            AllowCustomersToUploadAvatars = false,
            AvatarMaximumSizeBytes = 20000,
            DefaultAvatarEnabled = true,
            ShowCustomersLocation = false,
            ShowCustomersJoinDate = false,
            AllowViewingProfiles = false,
            NotifyFailedLoginAttempt = false,
            NotifyNewCustomerRegistration = false,
            HideDownloadableProductsTab = false,
            HideBackInStockSubscriptionsTab = false,
            DownloadableProductsValidateUser = false,
            CustomerNameFormat = CustomerNameFormat.ShowFirstName,
            FirstNameEnabled = true,
            FirstNameRequired = true,
            LastNameEnabled = true,
            LastNameRequired = true,
            GenderEnabled = true,
            NeutralGenderEnabled = false,
            DateOfBirthEnabled = false,
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
            HideNewsletterBlock = false,
            NewsletterBlockAllowToUnsubscribe = false,
            OnlineCustomerMinutes = 20,
            StoreLastVisitedPage = false,
            StoreIpAddresses = true,
            LastActivityMinutes = 15,
            SuffixDeletedCustomers = false,
            EnteringEmailTwice = false,
            RequireRegistrationForDownloadableProducts = false,
            AllowCustomersToCheckGiftCardBalance = false,
            DeleteGuestTaskOlderThanMinutes = 1440,
            PhoneNumberValidationEnabled = false,
            PhoneNumberValidationUseRegex = false,
            PhoneNumberValidationRule = "^[0-9]{1,14}?$",
            DefaultCountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == _installationSettings.RegionInfo.ThreeLetterISORegionName)
        });

        await SaveSettingAsync(dictionary, new MultiFactorAuthenticationSettings
        {
            ForceMultifactorAuthentication = false
        });

        await SaveSettingAsync(dictionary, new AddressSettings
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
            FaxEnabled = true,
            DefaultCountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == _installationSettings.RegionInfo.ThreeLetterISORegionName),
            PrePopulateCountryByCustomer = true
        });

        await SaveSettingAsync(dictionary, new MediaSettings
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
            OrderThumbPictureSize = 80,
            MiniCartThumbPictureSize = 70,
            AutoCompleteSearchThumbPictureSize = 20,
            ImageSquarePictureSize = 32,
            MaximumImageSize = 1980,
            DefaultPictureZoomEnabled = false,
            AllowSvgUploads = false,
            DefaultImageQuality = 80,
            MultipleThumbDirectories = false,
            ImportProductImagesUsingHash = true,
            UseAbsoluteImagePath = true,
            AutoOrientImage = false,
            VideoIframeAllow = "fullscreen",
            VideoIframeWidth = 300,
            VideoIframeHeight = 150,
            PicturePath = NopMediaDefaults.DefaultImagesPath
        });

        await SaveSettingAsync(dictionary, new StoreInformationSettings
        {
            StoreClosed = false,
            DefaultStoreTheme = "DefaultClean",
            AllowCustomerToSelectTheme = false,
            DisplayEuCookieLawWarning = isEurope,
            FacebookLink = "https://www.facebook.com/nopCommerce",
            TwitterLink = "https://twitter.com/nopCommerce",
            YoutubeLink = "https://www.youtube.com/user/nopCommerce",
            InstagramLink = "https://www.instagram.com/nopcommerce_official",
            HidePoweredByNopCommerce = false
        });

        await SaveSettingAsync(dictionary, new ExternalAuthenticationSettings
        {
            RequireEmailValidation = false,
            LogErrors = false,
            AllowCustomersToRemoveAssociations = true
        });

        await SaveSettingAsync(dictionary, new RewardPointsSettings
        {
            Enabled = true,
            ExchangeRate = 1,
            PointsForRegistration = 0,
            RegistrationPointsValidity = 30,
            PointsForPurchases_Amount = 10,
            PointsForPurchases_Points = 1,
            MinOrderTotalToAwardPoints = 0,
            MaximumRewardPointsToUsePerOrder = 0,
            MaximumRedeemedRate = 0,
            PurchasesPointsValidity = 45,
            ActivationDelay = 0,
            ActivationDelayPeriodId = 0,
            DisplayHowMuchWillBeEarned = true,
            PointsAccumulatedForAllStores = true,
            PageSize = 10
        });

        var primaryCurrency = "USD";
        await SaveSettingAsync(dictionary, new CurrencySettings
        {
            DisplayCurrencyLabel = false,
            PrimaryStoreCurrencyId = (await Table<Currency>().SingleAsync(c => c.CurrencyCode == primaryCurrency)).Id,
            PrimaryExchangeRateCurrencyId = (await Table<Currency>().SingleAsync(c => c.CurrencyCode == primaryCurrency)).Id,
            ActiveExchangeRateProviderSystemName = "CurrencyExchange.ECB",
            AutoUpdateEnabled = false,
            DisplayCurrencySymbolInCurrencySelector = false
        });

        var baseDimension = isMetric ? "meters" : "inches";
        var baseWeight = isMetric ? "kg" : "lb";

        await SaveSettingAsync(dictionary, new MeasureSettings
        {
            BaseDimensionId = (await Table<MeasureDimension>().SingleAsync(m => m.SystemKeyword == baseDimension)).Id,
            BaseWeightId = (await Table<MeasureWeight>().SingleAsync(m => m.SystemKeyword == baseWeight)).Id
        });

        await SaveSettingAsync(dictionary, new MessageTemplatesSettings
        {
            CaseInvariantReplacement = false,
            Color1 = "#b9babe",
            Color2 = "#ebecee",
            Color3 = "#dde2e6"
        });

        await SaveSettingAsync(dictionary, new ShoppingCartSettings
        {
            DisplayCartAfterAddingProduct = false,
            DisplayWishlistAfterAddingProduct = false,
            MaximumShoppingCartItems = 1000,
            MaximumWishlistItems = 1000,
            AllowMultipleWishlist = true,
            MaximumNumberOfCustomWishlist = 10,
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

        await SaveSettingAsync(dictionary, new OrderSettings
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
            DisplayPickupInStoreOnShippingMethodPage = false,
            AttachPdfInvoiceToOrderPlacedEmail = false,
            AttachPdfInvoiceToOrderProcessingEmail = false,
            AttachPdfInvoiceToOrderCompletedEmail = false,
            GeneratePdfInvoiceInCustomerLanguage = true,
            AttachPdfInvoiceToOrderPaidEmail = false,
            ReturnRequestsEnabled = true,
            ReturnRequestsAllowFiles = false,
            ReturnRequestsFileMaximumSize = 2048,
            NumberOfDaysReturnRequestAvailable = 365,
            MinimumOrderPlacementInterval = 1,
            ActivateGiftCardsAfterCompletingOrder = false,
            DeactivateGiftCardsAfterCancellingOrder = false,
            DeactivateGiftCardsAfterDeletingOrder = false,
            CompleteOrderWhenDelivered = true,
            CustomOrderNumberMask = "{ID}",
            ExportWithProducts = true,
            AllowAdminsToBuyCallForPriceProducts = true,
            AllowCustomersCancelOrders = true,
            ShowProductThumbnailInOrderDetailsPage = true,
            DisplayCustomerCurrencyOnOrders = false,
            DisplayOrderSummary = true,
            PlaceOrderWithLock = false,
            CustomerOrdersPageSize = 10
        });

        await SaveSettingAsync(dictionary, new SecuritySettings
        {
            EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
            AdminAreaAllowedIpAddresses = null,
            HoneypotEnabled = false,
            HoneypotInputName = "hpinput",
            AllowNonAsciiCharactersInHeaders = true,
            UseAesEncryptionAlgorithm = true,
            AllowStoreOwnerExportImportCustomersWithHashedPassword = true
        });

        await SaveSettingAsync(dictionary, new ShippingSettings
        {
            ActiveShippingRateComputationMethodSystemNames = ["Shipping.FixedByWeightByTotal"],
            ActivePickupPointProviderSystemNames = ["Pickup.PickupInStore"],
            ShipToSameAddress = true,
            AllowPickupInStore = true,
            DisplayPickupPointsOnMap = false,
            IgnoreAdditionalShippingChargeForPickupInStore = true,
            UseWarehouseLocation = false,
            NotifyCustomerAboutShippingFromMultipleLocations = false,
            FreeShippingOverXEnabled = false,
            FreeShippingOverXValue = decimal.Zero,
            FreeShippingOverXIncludingTax = false,
            EstimateShippingProductPageEnabled = true,
            EstimateShippingCartPageEnabled = true,
            EstimateShippingCityNameEnabled = false,
            DisplayShipmentEventsToCustomers = false,
            DisplayShipmentEventsToStoreOwner = false,
            HideShippingTotal = false,
            ReturnValidOptionsIfThereAreAny = true,
            BypassShippingMethodSelectionIfOnlyOne = false,
            UseCubeRootMethod = true,
            ConsiderAssociatedProductsDimensions = true,
            ShipSeparatelyOneItemEach = false,
            RequestDelay = 300,
            ShippingSorting = ShippingSortingEnum.Position,
        });

        await SaveSettingAsync(dictionary, new PaymentSettings
        {
            ActivePaymentMethodSystemNames = ["Payments.CheckMoneyOrder", "Payments.Manual"],
            AllowRePostingPayments = true,
            BypassPaymentMethodSelectionIfOnlyOne = true,
            ShowPaymentMethodDescriptions = true,
            SkipPaymentInfoStepForRedirectionPaymentMethods = false,
            CancelRecurringPaymentsAfterFailedPayment = false,
            RegenerateOrderGuidInterval = 180
        });

        await SaveSettingAsync(dictionary, new TaxSettings
        {
            TaxBasedOn = TaxBasedOn.BillingAddress,
            TaxBasedOnPickupPointAddress = false,
            TaxDisplayType = TaxDisplayType.ExcludingTax,
            ActiveTaxProviderSystemName = "Tax.FixedOrByCountryStateZip",
            DefaultTaxAddressId = 0,
            DisplayTaxSuffix = false,
            DisplayTaxRates = false,
            PricesIncludeTax = false,
            AutomaticallyDetectCountry = true,
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
            EuVatEnabled = isEurope,
            EuVatEnabledForGuests = false,
            EuVatRequired = false,
            EuVatShopCountryId = isEurope ? (await GetFirstEntityIdAsync<Country>(x => x.TwoLetterIsoCode == country) ?? 0) : 0,
            EuVatAllowVatExemption = true,
            EuVatUseWebService = false,
            EuVatAssumeValid = false,
            EuVatEmailAdminWhenNewVatSubmitted = false,
            HmrcApiUrl = "https://api.service.hmrc.gov.uk",
            HmrcClientId = string.Empty,
            HmrcClientSecret = string.Empty,
            LogErrors = false
        });

        await SaveSettingAsync(dictionary, new DateTimeSettings
        {
            DefaultStoreTimeZoneId = string.Empty,
            AllowCustomersToSetTimeZone = false
        });

        await SaveSettingAsync(dictionary, new BlogSettings
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
        await SaveSettingAsync(dictionary, new NewsSettings
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

        await SaveSettingAsync(dictionary, new ForumSettings
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
            ForumEditor = EditorType.MarkdownEditor,
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
            ForumSearchTermMinimumLength = 3,
            TopicMetaDescriptionLength = 160
        });

        await SaveSettingAsync(dictionary, new VendorSettings
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
            AllowVendorsToImportProducts = true,
            MaximumProductPicturesNumber = 5
        });

        var eaGeneral = await Table<EmailAccount>().FirstOrDefaultAsync() ?? throw new Exception("Default email account cannot be loaded");
        await SaveSettingAsync(dictionary, new EmailAccountSettings { DefaultEmailAccountId = eaGeneral.Id });

        await SaveSettingAsync(dictionary, new CaptchaSettings
        {
            ReCaptchaApiUrl = "https://www.google.com/recaptcha/",
            ReCaptchaDefaultLanguage = string.Empty,
            ReCaptchaPrivateKey = string.Empty,
            ReCaptchaPublicKey = string.Empty,
            ReCaptchaRequestTimeout = 20,
            ReCaptchaTheme = string.Empty,
            AutomaticallyChooseLanguage = true,
            Enabled = false,
            CaptchaType = CaptchaType.CheckBoxReCaptchaV2,
            ReCaptchaV3ScoreThreshold = 0.5M,
            ShowOnApplyVendorPage = false,
            ShowOnBlogCommentPage = false,
            ShowOnContactUsPage = false,
            ShowOnEmailProductToFriendPage = false,
            ShowOnEmailWishlistToFriendPage = false,
            ShowOnForgotPasswordPage = false,
            ShowOnForum = false,
            ShowOnLoginPage = false,
            ShowOnNewsCommentPage = false,
            ShowOnNewsletterPage = false,
            ShowOnProductReviewPage = false,
            ShowOnRegistrationPage = false,
            ShowOnCheckoutPageForGuests = false,
            ShowOnCheckGiftCardBalance = true
        });

        await SaveSettingAsync(dictionary, new MessagesSettings { UsePopupNotifications = false });

        await SaveSettingAsync(dictionary, new ProxySettings
        {
            Enabled = false,
            Address = string.Empty,
            Port = string.Empty,
            Username = string.Empty,
            Password = string.Empty,
            BypassOnLocal = true,
            PreAuthenticate = true
        });

        await SaveSettingAsync(dictionary, new CookieSettings
        {
            CompareProductsCookieExpires = 24 * 10,
            RecentlyViewedProductsCookieExpires = 24 * 10,
            CustomerCookieExpires = 24 * 365
        });

        await SaveSettingAsync(dictionary, new RobotsTxtSettings
        {
            DisallowPaths =
            [
                "/admin",
                "/bin/",
                "/files/",
                "/files/exportimport/",
                "/install",
                "/*?*returnUrl=",
                "/*?*returnurl=",
                "/*?*ReturnUrl=",
                //AJAX urls
                "/cart/estimateshipping",
                "/cart/selectshippingoption",
                "/customer/addressdelete",
                "/customer/removeexternalassociation",
                "/customer/checkusernameavailability",
                "/catalog/searchtermautocomplete",
                "/addproducttocart/catalog/*",
                "/addproducttocart/details/*",
                "/compareproducts/add/*",
                "/backinstocksubscribe/*",
                "/subscribenewsletter",
                "/t-popup/*",
                "/setproductreviewhelpfulness",
                "/poll/vote",
                "/country/getstatesbycountryid/",
                "/eucookielawaccept",
                "/topic/authenticate",
                "/category/products/",
                "/product/combinations",
                "/uploadfileproductattribute/*",
                "/shoppingcart/productdetails_attributechange/*",
                "/uploadfilereturnrequest",
                "/boards/topicwatch/*",
                "/boards/forumwatch/*",
                "/install/restartapplication",
                "/boards/postvote",
                "/product/estimateshipping/*",
                "/shoppingcart/checkoutattributechange/*"
            ],
            LocalizableDisallowPaths =
            [
                "/addproducttocart/catalog/",
                "/addproducttocart/details/",
                "/backinstocksubscriptions/manage",
                "/boards/forumsubscriptions",
                "/boards/forumwatch",
                "/boards/postedit",
                "/boards/postdelete",
                "/boards/postcreate",
                "/boards/topicedit",
                "/boards/topicdelete",
                "/boards/topiccreate",
                "/boards/topicmove",
                "/boards/topicwatch",
                "/cart$",
                "/changecurrency",
                "/changelanguage",
                "/changetaxtype",
                "/checkout",
                "/checkout/billingaddress",
                "/checkout/completed",
                "/checkout/confirm",
                "/checkout/shippingaddress",
                "/checkout/shippingmethod",
                "/checkout/paymentinfo",
                "/checkout/paymentmethod",
                "/clearcomparelist",
                "/compareproducts",
                "/compareproducts/add/*",
                "/customer/avatar",
                "/customer/activation",
                "/customer/addresses",
                "/customer/changepassword",
                "/customer/checkusernameavailability",
                "/customer/downloadableproducts",
                "/customer/info",
                "/customer/productreviews",
                "/deletepm",
                "/emailwishlist",
                "/eucookielawaccept",
                "/inboxupdate",
                "/newsletter/subscriptionactivation",
                "/onepagecheckout",
                "/order/history",
                "/orderdetails",
                "/passwordrecovery/confirm",
                "/poll/vote",
                "/privatemessages",
                "/recentlyviewedproducts",
                "/returnrequest",
                "/returnrequest/history",
                "/rewardpoints/history",
                "/search?",
                "/sendpm",
                "/sentupdate",
                "/shoppingcart/*",
                "/storeclosed",
                "/subscribenewsletter",
                "/topic/authenticate",
                "/viewpm",
                "/uploadfilecheckoutattribute",
                "/uploadfileproductattribute",
                "/uploadfilereturnrequest",
                "/wishlist"
            ]
        });

        await SaveSettingAsync(dictionary, new FilterLevelSettings
        {
            DisplayOnHomePage = true,
            DisplayOnProductDetailsPage = true
        });

        await SaveSettingAsync(dictionary, new MenuSettings
        {
            NumberOfSubItemsPerGridElement = 3,
            NumberOfItemsPerGridRow = 4,
            MaximumNumberEntities = 8,
            GridThumbPictureSize = 220,
            MaximumMainMenuLevels = 2
        });
    }

    /// <summary>
    /// Installs a default customers
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallCustomersAndUsersAsync()
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

        await _dataProvider.BulkInsertEntitiesAsync(customerRoles);

        //default store 
        var defaultStore = await Table<Store>().FirstOrDefaultAsync() ?? throw new Exception("No default store could be loaded");

        var storeId = defaultStore.Id;

        //admin user
        var adminUser = new Customer
        {
            CustomerGuid = Guid.NewGuid(),
            Email = _installationSettings.AdminEmail,
            Username = _installationSettings.AdminEmail,
            Active = true,
            CreatedOnUtc = DateTime.UtcNow,
            LastActivityDateUtc = DateTime.UtcNow,
            RegisteredInStoreId = storeId
        };

        var defaultAdminUserAddress = await _dataProvider.InsertEntityAsync(
            new Address
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "12345678",
                Email = _installationSettings.AdminEmail,
                FaxNumber = string.Empty,
                Company = "Nop Solutions Ltd",
                Address1 = "21 West 52nd Street",
                Address2 = string.Empty,
                City = "New York",
                StateProvinceId = await GetFirstEntityIdAsync<StateProvince>(sp => sp.Name == "New York"),
                CountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow
            });

        adminUser.BillingAddressId = defaultAdminUserAddress.Id;
        adminUser.ShippingAddressId = defaultAdminUserAddress.Id;
        adminUser.FirstName = defaultAdminUserAddress.FirstName;
        adminUser.LastName = defaultAdminUserAddress.LastName;

        await _dataProvider.InsertEntityAsync(adminUser);

        await _dataProvider.InsertEntityAsync(new CustomerAddressMapping { CustomerId = adminUser.Id, AddressId = defaultAdminUserAddress.Id });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new CustomerCustomerRoleMapping { CustomerId = adminUser.Id, CustomerRoleId = crAdministrators.Id },
            new CustomerCustomerRoleMapping { CustomerId = adminUser.Id, CustomerRoleId = crForumModerators.Id },
            new CustomerCustomerRoleMapping { CustomerId = adminUser.Id, CustomerRoleId = crRegistered.Id }});

        //set hashed admin password
        var customerPassword = new CustomerPassword
        {
            CustomerId = await GetDefaultCustomerIdAsync(),
            PasswordFormat = PasswordFormat.Hashed,
            CreatedOnUtc = DateTime.UtcNow
        };

        //generate a cryptographic random number
        using var provider = RandomNumberGenerator.Create();
        var buff = new byte[NopCustomerServicesDefaults.PasswordSaltKeySize];
        provider.GetBytes(buff);
        var saltKey = Convert.ToBase64String(buff);

        customerPassword.PasswordSalt = saltKey;
        customerPassword.Password = HashHelper.CreateHash(Encoding.UTF8.GetBytes(string.Concat(_installationSettings.AdminPassword, saltKey)), NopCustomerServicesDefaults.DefaultHashedPasswordFormat);

        await _dataProvider.InsertEntityAsync(customerPassword);

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

        await _dataProvider.InsertEntityAsync(searchEngineUser);

        await _dataProvider.InsertEntityAsync(new CustomerCustomerRoleMapping { CustomerRoleId = crGuests.Id, CustomerId = searchEngineUser.Id });

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

        await _dataProvider.InsertEntityAsync(backgroundTaskUser);

        await _dataProvider.InsertEntityAsync(new CustomerCustomerRoleMapping { CustomerId = backgroundTaskUser.Id, CustomerRoleId = crGuests.Id });
    }

    /// <summary>
    /// Installs a default topics
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallTopicsAsync()
    {
        var defaultTopicTemplate = await Table<TopicTemplate>().FirstOrDefaultAsync(tt => tt.Name == "Default template") ?? throw new Exception("Topic template cannot be loaded");

        var topics = new List<Topic>
            {
                new() {
                    SystemName = "AboutUs",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 20,
                    Published = true,
                    Title = "About us",
                    Body =
                        "<p>Put your &quot;About Us&quot; information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new() {
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
                new() {
                    SystemName = "ConditionsOfUse",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 15,
                    Published = true,
                    Title = "Conditions of Use",
                    Body = "<p>Put your conditions of use information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new() {
                    SystemName = "ContactUs",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body = "<p>Put your contact information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new() {
                    SystemName = "ForumWelcomeMessage",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "Forums",
                    Body = "<p>Put your welcome message here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new() {
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
                new() {
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
                new() {
                    SystemName = "PrivacyInfo",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 10,
                    Published = true,
                    Title = "Privacy notice",
                    Body = "<p>Put your privacy policy information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new() {
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
                new() {
                    SystemName = "ShippingInfo",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 5,
                    Published = true,
                    Title = "Shipping & returns",
                    Body =
                        "<p>Put your shipping &amp; returns information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new() {
                    SystemName = "ApplyVendor",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body = "<p>Put your apply vendor instructions here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new() {
                    SystemName = "VendorTermsOfService",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "Terms of services for vendors",
                    Body = "<p>Put your terms of service information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                }
            };

        await _dataProvider.BulkInsertEntitiesAsync(topics);

        //search engine names
        foreach (var topic in topics)
        {
            await _dataProvider.InsertEntityAsync(new UrlRecord
            {
                EntityId = topic.Id,
                EntityName = nameof(Topic),
                LanguageId = 0,
                IsActive = true,
                Slug = await ValidateSeNameAsync(topic, !string.IsNullOrEmpty(topic.Title) ? topic.Title : topic.SystemName)
            });
        }
    }

    /// <summary>
    /// Installs a default types of activity log
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallActivityLogTypesAsync()
    {
        var activityLogTypes = new List<ActivityLogType>
            {
                //admin area activities
                new() {
                    SystemKeyword = "AddNewAddressAttribute",
                    Enabled = true,
                    Name = "Add a new address attribute"
                },
                new() {
                    SystemKeyword = "AddNewAddressAttributeValue",
                    Enabled = true,
                    Name = "Add a new address attribute value"
                },
                new() {
                    SystemKeyword = "AddNewAffiliate",
                    Enabled = true,
                    Name = "Add a new affiliate"
                },
                new() {
                    SystemKeyword = "AddNewBlogPost",
                    Enabled = true,
                    Name = "Add a new blog post"
                },
                new() {
                    SystemKeyword = "AddNewCampaign",
                    Enabled = true,
                    Name = "Add a new campaign"
                },
                new() {
                    SystemKeyword = "AddNewCategory",
                    Enabled = true,
                    Name = "Add a new category"
                },
                new() {
                    SystemKeyword = "AddNewCheckoutAttribute",
                    Enabled = true,
                    Name = "Add a new checkout attribute"
                },
                new() {
                    SystemKeyword = "AddNewCountry",
                    Enabled = true,
                    Name = "Add a new country"
                },
                new() {
                    SystemKeyword = "AddNewCurrency",
                    Enabled = true,
                    Name = "Add a new currency"
                },
                new() {
                    SystemKeyword = "AddNewCustomer",
                    Enabled = true,
                    Name = "Add a new customer"
                },
                new() {
                    SystemKeyword = "AddNewCustomerAttribute",
                    Enabled = true,
                    Name = "Add a new customer attribute"
                },
                new() {
                    SystemKeyword = "AddNewCustomerAttributeValue",
                    Enabled = true,
                    Name = "Add a new customer attribute value"
                },
                new() {
                    SystemKeyword = "AddNewCustomerRole",
                    Enabled = true,
                    Name = "Add a new customer role"
                },
                new() {
                    SystemKeyword = "AddNewDiscount",
                    Enabled = true,
                    Name = "Add a new discount"
                },
                new() {
                    SystemKeyword = "AddNewEmailAccount",
                    Enabled = true,
                    Name = "Add a new email account"
                },
                new() {
                    SystemKeyword = "AddNewGiftCard",
                    Enabled = true,
                    Name = "Add a new gift card"
                },
                new() {
                    SystemKeyword = "AddNewFilterLevelValue",
                    Enabled = true,
                    Name = "Add a new filter level value"
                },
                new() {
                    SystemKeyword = "AddNewLanguage",
                    Enabled = true,
                    Name = "Add a new language"
                },
                new() {
                    SystemKeyword = "AddNewManufacturer",
                    Enabled = true,
                    Name = "Add a new manufacturer"
                },
                new() {
                    SystemKeyword = "AddNewMeasureDimension",
                    Enabled = true,
                    Name = "Add a new measure dimension"
                },
                new() {
                    SystemKeyword = "AddNewMeasureWeight",
                    Enabled = true,
                    Name = "Add a new measure weight"
                },
                new() {
                    SystemKeyword = "AddNewNews",
                    Enabled = true,
                    Name = "Add a new news"
                },
                new() {
                    SystemKeyword = "AddNewProduct",
                    Enabled = true,
                    Name = "Add a new product"
                },
                new() {
                    SystemKeyword = "AddNewProductAttribute",
                    Enabled = true,
                    Name = "Add a new product attribute"
                },
                new() {
                    SystemKeyword = "AddNewSetting",
                    Enabled = true,
                    Name = "Add a new setting"
                },
                new() {
                    SystemKeyword = "AddNewSpecAttribute",
                    Enabled = true,
                    Name = "Add a new specification attribute"
                },
                new() {
                    SystemKeyword = "AddNewSpecAttributeGroup",
                    Enabled = true,
                    Name = "Add a new specification attribute group"
                },
                new() {
                    SystemKeyword = "AddNewStateProvince",
                    Enabled = true,
                    Name = "Add a new state or province"
                },
                new() {
                    SystemKeyword = "AddNewStore",
                    Enabled = true,
                    Name = "Add a new store"
                },
                new() {
                    SystemKeyword = "AddNewMenu",
                    Enabled = true,
                    Name = "Add a new menu"
                },
                new() {
                    SystemKeyword = "AddNewMenuItem",
                    Enabled = true,
                    Name = "Add a new menu item"
                },
                new() {
                    SystemKeyword = "AddNewTopic",
                    Enabled = true,
                    Name = "Add a new topic"
                },
                new() {
                    SystemKeyword = "AddNewReviewType",
                    Enabled = true,
                    Name = "Add a new review type"
                },
                new() {
                    SystemKeyword = "AddNewVendor",
                    Enabled = true,
                    Name = "Add a new vendor"
                },
                new() {
                    SystemKeyword = "AddNewVendorAttribute",
                    Enabled = true,
                    Name = "Add a new vendor attribute"
                },
                new() {
                    SystemKeyword = "AddNewVendorAttributeValue",
                    Enabled = true,
                    Name = "Add a new vendor attribute value"
                },
                new() {
                    SystemKeyword = "AddNewWarehouse",
                    Enabled = true,
                    Name = "Add a new warehouse"
                },
                new() {
                    SystemKeyword = "AddNewWidget",
                    Enabled = true,
                    Name = "Add a new widget"
                },
                new() {
                    SystemKeyword = "AddSubscriptionType",
                    Enabled = true,
                    Name = "Add a new subscription type"
                },
                new() {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new() {
                    SystemKeyword = "DeleteAddressAttribute",
                    Enabled = true,
                    Name = "Delete an address attribute"
                },
                new() {
                    SystemKeyword = "DeleteAddressAttributeValue",
                    Enabled = true,
                    Name = "Delete an address attribute value"
                },
                new() {
                    SystemKeyword = "DeleteAffiliate",
                    Enabled = true,
                    Name = "Delete an affiliate"
                },
                new() {
                    SystemKeyword = "DeleteBlogPost",
                    Enabled = true,
                    Name = "Delete a blog post"
                },
                new() {
                    SystemKeyword = "DeleteBlogPostComment",
                    Enabled = true,
                    Name = "Delete a blog post comment"
                },
                new() {
                    SystemKeyword = "DeleteCampaign",
                    Enabled = true,
                    Name = "Delete a campaign"
                },
                new() {
                    SystemKeyword = "DeleteCategory",
                    Enabled = true,
                    Name = "Delete category"
                },
                new() {
                    SystemKeyword = "DeleteCheckoutAttribute",
                    Enabled = true,
                    Name = "Delete a checkout attribute"
                },
                new() {
                    SystemKeyword = "DeleteCountry",
                    Enabled = true,
                    Name = "Delete a country"
                },
                new() {
                    SystemKeyword = "DeleteCurrency",
                    Enabled = true,
                    Name = "Delete a currency"
                },
                new() {
                    SystemKeyword = "DeleteCustomer",
                    Enabled = true,
                    Name = "Delete a customer"
                },
                new() {
                    SystemKeyword = "DeleteCustomerAttribute",
                    Enabled = true,
                    Name = "Delete a customer attribute"
                },
                new() {
                    SystemKeyword = "DeleteCustomerAttributeValue",
                    Enabled = true,
                    Name = "Delete a customer attribute value"
                },
                new() {
                    SystemKeyword = "DeleteCustomerRole",
                    Enabled = true,
                    Name = "Delete a customer role"
                },
                new() {
                    SystemKeyword = "DeleteDiscount",
                    Enabled = true,
                    Name = "Delete a discount"
                },
                new() {
                    SystemKeyword = "DeleteEmailAccount",
                    Enabled = true,
                    Name = "Delete an email account"
                },
                new () {
                    SystemKeyword = "DeleteFilterLevelValue",
                    Enabled = true,
                    Name = "Delete a filter level value"
                },
                new() {
                    SystemKeyword = "DeleteGiftCard",
                    Enabled = true,
                    Name = "Delete a gift card"
                },
                new() {
                    SystemKeyword = "DeleteLanguage",
                    Enabled = true,
                    Name = "Delete a language"
                },
                new() {
                    SystemKeyword = "DeleteManufacturer",
                    Enabled = true,
                    Name = "Delete a manufacturer"
                },
                new() {
                    SystemKeyword = "DeleteMeasureDimension",
                    Enabled = true,
                    Name = "Delete a measure dimension"
                },
                new() {
                    SystemKeyword = "DeleteMeasureWeight",
                    Enabled = true,
                    Name = "Delete a measure weight"
                },
                new() {
                    SystemKeyword = "DeleteMessageTemplate",
                    Enabled = true,
                    Name = "Delete a message template"
                },
                new() {
                    SystemKeyword = "DeleteNews",
                    Enabled = true,
                    Name = "Delete a news"
                },
                 new() {
                    SystemKeyword = "DeleteNewsComment",
                    Enabled = true,
                    Name = "Delete a news comment"
                },
                new() {
                    SystemKeyword = "DeleteOrder",
                    Enabled = true,
                    Name = "Delete an order"
                },
                new() {
                    SystemKeyword = "DeletePlugin",
                    Enabled = true,
                    Name = "Delete a plugin"
                },
                new() {
                    SystemKeyword = "DeleteProduct",
                    Enabled = true,
                    Name = "Delete a product"
                },
                new() {
                    SystemKeyword = "DeleteProductAttribute",
                    Enabled = true,
                    Name = "Delete a product attribute"
                },
                new() {
                    SystemKeyword = "DeleteProductReview",
                    Enabled = true,
                    Name = "Delete a product review"
                },
                new() {
                    SystemKeyword = "DeleteReturnRequest",
                    Enabled = true,
                    Name = "Delete a return request"
                },
                new() {
                    SystemKeyword = "DeleteReviewType",
                    Enabled = true,
                    Name = "Delete a review type"
                },
                new() {
                    SystemKeyword = "DeleteSetting",
                    Enabled = true,
                    Name = "Delete a setting"
                },
                new() {
                    SystemKeyword = "DeleteSpecAttribute",
                    Enabled = true,
                    Name = "Delete a specification attribute"
                },
                new() {
                    SystemKeyword = "DeleteSpecAttributeGroup",
                    Enabled = true,
                    Name = "Delete a specification attribute group"
                },
                new() {
                    SystemKeyword = "DeleteStateProvince",
                    Enabled = true,
                    Name = "Delete a state or province"
                },
                new() {
                    SystemKeyword = "DeleteStore",
                    Enabled = true,
                    Name = "Delete a store"
                },
                new() {
                    SystemKeyword = "DeleteSubscriptionType",
                    Enabled = true,
                    Name = "Delete a subscription type"
                },
                new() {
                    SystemKeyword = "DeleteSystemLog",
                    Enabled = true,
                    Name = "Delete system log"
                },
                new() {
                    SystemKeyword = "DeleteTopic",
                    Enabled = true,
                    Name = "Delete a topic"
                },
                new() {
                    SystemKeyword = "DeleteMenu",
                    Enabled = true,
                    Name = "Delete a menu"
                },
                new() {
                    SystemKeyword = "DeleteMenuItem",
                    Enabled = true,
                    Name = "Delete a menu item"
                },
                new() {
                    SystemKeyword = "DeleteVendor",
                    Enabled = true,
                    Name = "Delete a vendor"
                },
                new() {
                    SystemKeyword = "DeleteVendorAttribute",
                    Enabled = true,
                    Name = "Delete a vendor attribute"
                },
                new() {
                    SystemKeyword = "DeleteVendorAttributeValue",
                    Enabled = true,
                    Name = "Delete a vendor attribute value"
                },
                new() {
                    SystemKeyword = "DeleteWarehouse",
                    Enabled = true,
                    Name = "Delete a warehouse"
                },
                new() {
                    SystemKeyword = "DeleteWidget",
                    Enabled = true,
                    Name = "Delete a widget"
                },
                new() {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new() {
                    SystemKeyword = "EditAddressAttribute",
                    Enabled = true,
                    Name = "Edit an address attribute"
                },
                 new() {
                    SystemKeyword = "EditAddressAttributeValue",
                    Enabled = true,
                    Name = "Edit an address attribute value"
                },
                new() {
                    SystemKeyword = "EditAffiliate",
                    Enabled = true,
                    Name = "Edit an affiliate"
                },
                new() {
                    SystemKeyword = "EditBlogPost",
                    Enabled = true,
                    Name = "Edit a blog post"
                },
                new() {
                    SystemKeyword = "EditCampaign",
                    Enabled = true,
                    Name = "Edit a campaign"
                },
                new() {
                    SystemKeyword = "EditCategory",
                    Enabled = true,
                    Name = "Edit category"
                },
                new() {
                    SystemKeyword = "EditCheckoutAttribute",
                    Enabled = true,
                    Name = "Edit a checkout attribute"
                },
                new() {
                    SystemKeyword = "EditCountry",
                    Enabled = true,
                    Name = "Edit a country"
                },
                new() {
                    SystemKeyword = "EditCurrency",
                    Enabled = true,
                    Name = "Edit a currency"
                },
                new() {
                    SystemKeyword = "EditCustomer",
                    Enabled = true,
                    Name = "Edit a customer"
                },
                new() {
                    SystemKeyword = "EditCustomerAttribute",
                    Enabled = true,
                    Name = "Edit a customer attribute"
                },
                new() {
                    SystemKeyword = "EditCustomerAttributeValue",
                    Enabled = true,
                    Name = "Edit a customer attribute value"
                },
                new() {
                    SystemKeyword = "EditCustomerRole",
                    Enabled = true,
                    Name = "Edit a customer role"
                },
                new() {
                    SystemKeyword = "EditDiscount",
                    Enabled = true,
                    Name = "Edit a discount"
                },
                new() {
                    SystemKeyword = "EditEmailAccount",
                    Enabled = true,
                    Name = "Edit an email account"
                },
                new() {
                    SystemKeyword = "EditFilterLevelValue",
                    Enabled = true,
                    Name = "Edit a filter level value"
                },
                new() {
                    SystemKeyword = "EditGiftCard",
                    Enabled = true,
                    Name = "Edit a gift card"
                },
                new() {
                    SystemKeyword = "EditLanguage",
                    Enabled = true,
                    Name = "Edit a language"
                },
                new() {
                    SystemKeyword = "EditManufacturer",
                    Enabled = true,
                    Name = "Edit a manufacturer"
                },
                new() {
                    SystemKeyword = "EditMeasureDimension",
                    Enabled = true,
                    Name = "Edit a measure dimension"
                },
                new() {
                    SystemKeyword = "EditMeasureWeight",
                    Enabled = true,
                    Name = "Edit a measure weight"
                },
                new() {
                    SystemKeyword = "EditMessageTemplate",
                    Enabled = true,
                    Name = "Edit a message template"
                },
                new() {
                    SystemKeyword = "EditNews",
                    Enabled = true,
                    Name = "Edit a news"
                },
                new() {
                    SystemKeyword = "EditOrder",
                    Enabled = true,
                    Name = "Edit an order"
                },
                new() {
                    SystemKeyword = "EditPlugin",
                    Enabled = true,
                    Name = "Edit a plugin"
                },
                new() {
                    SystemKeyword = "EditProduct",
                    Enabled = true,
                    Name = "Edit a product"
                },
                new() {
                    SystemKeyword = "EditProductAttribute",
                    Enabled = true,
                    Name = "Edit a product attribute"
                },
                new() {
                    SystemKeyword = "EditProductReview",
                    Enabled = true,
                    Name = "Edit a product review"
                },
                new() {
                    SystemKeyword = "EditPromotionProviders",
                    Enabled = true,
                    Name = "Edit promotion providers"
                },
                new() {
                    SystemKeyword = "EditReturnRequest",
                    Enabled = true,
                    Name = "Edit a return request"
                },
                new() {
                    SystemKeyword = "EditReviewType",
                    Enabled = true,
                    Name = "Edit a review type"
                },
                new() {
                    SystemKeyword = "EditSettings",
                    Enabled = true,
                    Name = "Edit setting(s)"
                },
                new() {
                    SystemKeyword = "EditStateProvince",
                    Enabled = true,
                    Name = "Edit a state or province"
                },
                new() {
                    SystemKeyword = "EditStore",
                    Enabled = true,
                    Name = "Edit a store"
                },
                new() {
                    SystemKeyword = "EditSubscriptionType",
                    Enabled = true,
                    Name = "Edit a subscription type"
                },
                new() {
                    SystemKeyword = "EditTask",
                    Enabled = true,
                    Name = "Edit a task"
                },
                new() {
                    SystemKeyword = "EditSpecAttribute",
                    Enabled = true,
                    Name = "Edit a specification attribute"
                },
                new() {
                    SystemKeyword = "EditSpecAttributeGroup",
                    Enabled = true,
                    Name = "Edit a specification attribute group"
                },
                new() {
                    SystemKeyword = "EditVendor",
                    Enabled = true,
                    Name = "Edit a vendor"
                },
                new() {
                    SystemKeyword = "EditVendorAttribute",
                    Enabled = true,
                    Name = "Edit a vendor attribute"
                },
                new() {
                    SystemKeyword = "EditVendorAttributeValue",
                    Enabled = true,
                    Name = "Edit a vendor attribute value"
                },
                new() {
                    SystemKeyword = "EditWarehouse",
                    Enabled = true,
                    Name = "Edit a warehouse"
                },
                new() {
                    SystemKeyword = "EditMenu",
                    Enabled = true,
                    Name = "Edit a menu"
                },
                new() {
                    SystemKeyword = "EditMenuItem",
                    Enabled = true,
                    Name = "Edit a menu item"
                },
                new() {
                    SystemKeyword = "EditTopic",
                    Enabled = true,
                    Name = "Edit a topic"
                },
                new() {
                    SystemKeyword = "EditWidget",
                    Enabled = true,
                    Name = "Edit a widget"
                },
                new() {
                    SystemKeyword = "Impersonation.Started",
                    Enabled = true,
                    Name = "Customer impersonation session. Started"
                },
                new() {
                    SystemKeyword = "Impersonation.Finished",
                    Enabled = true,
                    Name = "Customer impersonation session. Finished"
                },
                new() {
                    SystemKeyword = "ImportCategories",
                    Enabled = true,
                    Name = "Categories were imported"
                },
                new() {
                    SystemKeyword = "ImportFilterLevelValues",
                    Enabled = true,
                    Name = "Import filter level values"
                },
                new() {
                    SystemKeyword = "ImportManufacturers",
                    Enabled = true,
                    Name = "Manufacturers were imported"
                },
                new() {
                    SystemKeyword = "ImportProducts",
                    Enabled = true,
                    Name = "Products were imported"
                },
                new() {
                    SystemKeyword = "ImportCustomers",
                    Enabled = true,
                    Name = "Customers were imported"
                },
                new() {
                    SystemKeyword = "ImportNewsLetterSubscriptions",
                    Enabled = true,
                    Name = "Newsletter subscriptions were imported"
                },
                new() {
                    SystemKeyword = "ImportStates",
                    Enabled = true,
                    Name = "States were imported"
                },
                new() {
                    SystemKeyword = "ExportCustomers",
                    Enabled = true,
                    Name = "Customers were exported"
                },
                new() {
                    SystemKeyword = "ExportCategories",
                    Enabled = true,
                    Name = "Categories were exported"
                },
                new () {
                    SystemKeyword = "ExportFilterLevelValues",
                    Enabled = true,
                    Name = "Export filter level values"
                },
                new() {
                    SystemKeyword = "ExportManufacturers",
                    Enabled = true,
                    Name = "Manufacturers were exported"
                },
                new() {
                    SystemKeyword = "ExportProducts",
                    Enabled = true,
                    Name = "Products were exported"
                },
                new() {
                    SystemKeyword = "ExportOrders",
                    Enabled = true,
                    Name = "Orders were exported"
                },
                new() {
                    SystemKeyword = "ExportStates",
                    Enabled = true,
                    Name = "States were exported"
                },
                new() {
                    SystemKeyword = "ExportNewsLetterSubscriptions",
                    Enabled = true,
                    Name = "Newsletter subscriptions were exported"
                },
                new() {
                    SystemKeyword = "InstallNewPlugin",
                    Enabled = true,
                    Name = "Install a new plugin"
                },
                new() {
                    SystemKeyword = "UninstallPlugin",
                    Enabled = true,
                    Name = "Uninstall a plugin"
                },
                new() {
                    SystemKeyword = "UpdatePlugin",
                    Enabled = true,
                    Name = "Update a plugin"
                },
                //public store activities
                new() {
                    SystemKeyword = "PublicStore.ViewCategory",
                    Enabled = false,
                    Name = "Public store. View a category"
                },
                new() {
                    SystemKeyword = "PublicStore.ViewManufacturer",
                    Enabled = false,
                    Name = "Public store. View a manufacturer"
                },
                new() {
                    SystemKeyword = "PublicStore.ViewProduct",
                    Enabled = false,
                    Name = "Public store. View a product"
                },
                new() {
                    SystemKeyword = "PublicStore.PlaceOrder",
                    Enabled = false,
                    Name = "Public store. Place an order"
                },
                new() {
                    SystemKeyword = "PublicStore.SendPM",
                    Enabled = false,
                    Name = "Public store. Send PM"
                },
                new() {
                    SystemKeyword = "PublicStore.ContactUs",
                    Enabled = false,
                    Name = "Public store. Use contact us form"
                },
                new() {
                    SystemKeyword = "PublicStore.AddToCompareList",
                    Enabled = false,
                    Name = "Public store. Add to compare list"
                },
                new() {
                    SystemKeyword = "PublicStore.AddToShoppingCart",
                    Enabled = false,
                    Name = "Public store. Add to shopping cart"
                },
                new() {
                    SystemKeyword = "PublicStore.AddToWishlist",
                    Enabled = false,
                    Name = "Public store. Add to wishlist"
                },
                new() {
                    SystemKeyword = "PublicStore.SuccessfulLogin",
                    Enabled = false,
                    Name = "Public store. Successful login"
                },
                new() {
                    SystemKeyword = "PublicStore.FailedLogin",
                    Enabled = false,
                    Name = "Public store. Failed login"
                },
                new() {
                    SystemKeyword = "PublicStore.Logout",
                    Enabled = false,
                    Name = "Public store. Logout"
                },
                new() {
                    SystemKeyword = "PublicStore.AddProductReview",
                    Enabled = false,
                    Name = "Public store. Add product review"
                },
                new() {
                    SystemKeyword = "PublicStore.AddNewsComment",
                    Enabled = false,
                    Name = "Public store. Add news comment"
                },
                new() {
                    SystemKeyword = "PublicStore.AddBlogComment",
                    Enabled = false,
                    Name = "Public store. Add blog comment"
                },
                new() {
                    SystemKeyword = "PublicStore.AddForumTopic",
                    Enabled = false,
                    Name = "Public store. Add forum topic"
                },
                new() {
                    SystemKeyword = "PublicStore.EditForumTopic",
                    Enabled = false,
                    Name = "Public store. Edit forum topic"
                },
                new() {
                    SystemKeyword = "PublicStore.DeleteForumTopic",
                    Enabled = false,
                    Name = "Public store. Delete forum topic"
                },
                new() {
                    SystemKeyword = "PublicStore.AddForumPost",
                    Enabled = false,
                    Name = "Public store. Add forum post"
                },
                new() {
                    SystemKeyword = "PublicStore.EditForumPost",
                    Enabled = false,
                    Name = "Public store. Edit forum post"
                },
                new() {
                    SystemKeyword = "PublicStore.DeleteForumPost",
                    Enabled = false,
                    Name = "Public store. Delete forum post"
                },
                new() {
                    SystemKeyword = "UploadNewPlugin",
                    Enabled = true,
                    Name = "Upload a plugin"
                },
                new() {
                    SystemKeyword = "UploadNewTheme",
                    Enabled = true,
                    Name = "Upload a theme"
                },
                new() {
                    SystemKeyword = "UploadIcons",
                    Enabled = true,
                    Name = "Upload a favicon and app icons"
                }
            };

        await _dataProvider.BulkInsertEntitiesAsync(activityLogTypes);
    }

    /// <summary>
    /// Installs a default product templates
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallProductTemplatesAsync()
    {
        var productTemplates = new List<ProductTemplate>
        {
            new() {
                Name = "Simple product",
                ViewPath = "ProductTemplate.Simple",
                DisplayOrder = 10,
                IgnoredProductTypes = ((int)ProductType.GroupedProduct).ToString()
            },
            new() {
                Name = "Grouped product (with variants)",
                ViewPath = "ProductTemplate.Grouped",
                DisplayOrder = 100,
                IgnoredProductTypes = ((int)ProductType.SimpleProduct).ToString()
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(productTemplates);
    }

    /// <summary>
    /// Installs a default category templates
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallCategoryTemplatesAsync()
    {
        var categoryTemplates = new List<CategoryTemplate>
        {
            new() {
                Name = "Products in Grid or Lines",
                ViewPath = "CategoryTemplate.ProductsInGridOrLines",
                DisplayOrder = 1
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(categoryTemplates);
    }

    /// <summary>
    /// Installs a default manufacturer templates
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallManufacturerTemplatesAsync()
    {
        var manufacturerTemplates = new List<ManufacturerTemplate>
        {
            new() {
                Name = "Products in Grid or Lines",
                ViewPath = "ManufacturerTemplate.ProductsInGridOrLines",
                DisplayOrder = 1
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(manufacturerTemplates);
    }

    /// <summary>
    /// Installs a default schedule tasks
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallScheduleTasksAsync()
    {
        var lastEnabledUtc = DateTime.UtcNow;
        var tasks = new List<ScheduleTask>
            {
                new() {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "Nop.Services.Messages.QueuedMessagesSendTask, Nop.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new() {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "Nop.Services.Common.KeepAliveTask, Nop.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new() {
                    Name = nameof(ResetLicenseCheckTask),
                    Seconds = 2073600,
                    Type = "Nop.Services.Common.ResetLicenseCheckTask, Nop.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new() {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "Nop.Services.Customers.DeleteGuestsTask, Nop.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new() {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "Nop.Services.Caching.ClearCacheTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new() {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Nop.Services.Logging.ClearLogTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new() {
                    Name = "Update currency exchange rates",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Nop.Services.Directory.UpdateExchangeRateTask, Nop.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new() {
                    Name = "Delete inactive customers (GDPR)",
                    //24 hours
                    Seconds = 86400,
                    Type = "Nop.Services.Gdpr.DeleteInactiveCustomersTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false
                }
            };

        await _dataProvider.BulkInsertEntitiesAsync(tasks);
    }

    /// <summary>
    /// Installs a default reasons of return request
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallReturnRequestReasonsAsync()
    {
        var returnRequestReasons = new List<ReturnRequestReason>
        {
            new() {
                Name = "Received Wrong Product",
                DisplayOrder = 1
            },
            new() {
                Name = "Wrong Product Ordered",
                DisplayOrder = 2
            },
            new() {
                Name = "There Was A Problem With The Product",
                DisplayOrder = 3
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(returnRequestReasons);
    }

    /// <summary>
    /// Installs a default actions for return request
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallReturnRequestActionsAsync()
    {
        var returnRequestActions = new List<ReturnRequestAction>
        {
            new() {
                Name = "Repair",
                DisplayOrder = 1
            },
            new() {
                Name = "Replacement",
                DisplayOrder = 2
            },
            new() {
                Name = "Store Credit",
                DisplayOrder = 3
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(returnRequestActions);
    }

    /// <summary>
    /// Installs menus
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallMenusAsync()
    {
        await _dataProvider.InsertEntityAsync(new Menu
        {
            Name = "Categories",
            MenuType = MenuType.Main,
            DisplayAllCategories = true,
            Published = _installationSettings.InstallSampleData
        });

        var standardMainMenu = await _dataProvider.InsertEntityAsync(new Menu
        {
            Name = "Menu",
            MenuType = MenuType.Main,
            DisplayOrder = 0,
            DisplayAllCategories = false,
            Published = !_installationSettings.InstallSampleData
        });

        await _dataProvider.BulkInsertEntitiesAsync(
        [
            new MenuItem
            {
                MenuId = standardMainMenu.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.HOMEPAGE,
                Title = "Home page",
                Published = true
            },
            new MenuItem
            {
                MenuId = standardMainMenu.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.NEW_PRODUCTS,
                Title = "New products",
                Published = true
            },
            new MenuItem
            {
                MenuId = standardMainMenu.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.SEARCH,
                Title = "Search",
                Published = true
            },
            new MenuItem
            {
                MenuId = standardMainMenu.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.CUSTOMER_INFO,
                Title = "My account",
                Published = true
            },
            new MenuItem
            {
                MenuId = standardMainMenu.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.BLOG,
                Title = "Blog",
                Published = true
            },
            new MenuItem
            {
                MenuId = standardMainMenu.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.CONTACT_US,
                Title = "Contact us",
                Published = true
            }
        ]);

        var footerInformation = await _dataProvider.InsertEntityAsync(new Menu
        {
            Name = "Information",
            MenuType = MenuType.Footer,
            DisplayOrder = 0,
            Published = true
        });

        await _dataProvider.BulkInsertEntitiesAsync(
        [
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.SITEMAP,
                Title = "Sitemap",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = Table<Topic>().FirstOrDefault(t => t.SystemName == "ShippingInfo")?.Id,
                Published = true
            },
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = Table<Topic>().FirstOrDefault(t => t.SystemName == "PrivacyInfo")?.Id,
                Published = true
            },
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = Table<Topic>().FirstOrDefault(t => t.SystemName == "ConditionsOfUse")?.Id,
                Published = true
            },
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.TopicPage,
                EntityId = Table<Topic>().FirstOrDefault(t => t.SystemName == "AboutUs")?.Id,
                Published = true
            },
            new MenuItem
            {
                MenuId = footerInformation.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.CONTACT_US,
                Title = "Contact us",
                Published = true
            },
        ]);

        var footerCustomerService = await _dataProvider.InsertEntityAsync(new Menu
        {
            Name = "Customer service",
            MenuType = MenuType.Footer,
            DisplayOrder = 1,
            Published = true
        });

        await _dataProvider.BulkInsertEntitiesAsync(
        [
            new MenuItem
            {
                MenuId = footerCustomerService.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.SEARCH,
                Title = "Search",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerCustomerService.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.NEWS,
                Title = "News",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerCustomerService.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.BLOG,
                Title = "Blog",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerCustomerService.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.RECENTLY_VIEWED_PRODUCTS,
                Title = "Recently viewed products",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerCustomerService.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.COMPARE_PRODUCTS,
                Title = "Compare products list",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerCustomerService.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.NEW_PRODUCTS,
                Title = "New products",
                Published = true
            },
        ]);

        var footerMyAccount = await _dataProvider.InsertEntityAsync(new Menu
        {
            Name = "My account",
            MenuType = MenuType.Footer,
            DisplayOrder = 2,
            Published = true
        });

        await _dataProvider.BulkInsertEntitiesAsync(
        [
           new MenuItem
            {
                MenuId = footerMyAccount.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.CUSTOMER_INFO,
                Title = "My account",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerMyAccount.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.CUSTOMER_ORDERS,
                Title = "Orders",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerMyAccount.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.CUSTOMER_ADDRESSES,
                Title = "Addresses",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerMyAccount.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.CART,
                Title = "Shopping cart",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerMyAccount.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.WISHLIST,
                Title = "Wishlist",
                Published = true
            },
            new MenuItem
            {
                MenuId = footerMyAccount.Id,
                MenuItemType = MenuItemType.StandardPage,
                RouteName = NopRouteNames.General.APPLY_VENDOR_ACCOUNT,
                Title = "Apply for vendor account",
                Published = true
            },
        ]);
    }

    #endregion
}
