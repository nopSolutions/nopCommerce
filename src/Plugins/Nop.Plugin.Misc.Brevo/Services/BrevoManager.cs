using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.Brevo.Domain;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using static brevo_csharp.Model.GetAttributesAttributes;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents Brevo manager
/// </summary>
public partial class BrevoManager
{
    #region Fields

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILanguageService _languageService;
    protected readonly ILogger _logger;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly ISettingService _settingService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreService _storeService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public BrevoManager(IActionContextAccessor actionContextAccessor,
        ICountryService countryService,
        ICustomerService customerService,
        IEmailAccountService emailAccountService,
        IGenericAttributeService genericAttributeService,
        ILanguageService languageService,
        ILogger logger,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        ISettingService settingService,
        IStateProvinceService stateProvinceService,
        IStoreService storeService,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        _actionContextAccessor = actionContextAccessor;
        _countryService = countryService;
        _customerService = customerService;
        _emailAccountService = emailAccountService;
        _genericAttributeService = genericAttributeService;
        _languageService = languageService;
        _logger = logger;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _settingService = settingService;
        _stateProvinceService = stateProvinceService;
        _storeService = storeService;
        _urlHelperFactory = urlHelperFactory;
        _webHelper = webHelper;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Handle function and get result
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="function">Function</param>
    /// <param name="logErrors">Whether to log errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result; error if exists
    /// </returns>
    private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
    {
        try
        {
            //whether plugin is configured
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
            if (!IsConfigured(brevoSettings))
                throw new NopException("Plugin not configured");

            return (await function(), default);
        }
        catch (Exception exception)
        {
            var errorMessage = exception.Message;
            if (logErrors)
            {
                var logMessage = $"{BrevoDefaults.SystemName} error: {Environment.NewLine}{errorMessage}";
                await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());
            }

            return (default, errorMessage);
        }
    }

    /// <summary>
    /// Prepare API client
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the aPI client
    /// </returns>
    protected async Task<TClient> CreateApiClientAsync<TClient>(Func<Configuration, TClient> clientCtor) where TClient : IApiAccessor
    {
        //check whether plugin is configured to request services (validate API key)
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
        if (!IsConfigured(brevoSettings))
            throw new NopException("Plugin not configured");

        var apiConfiguration = new Configuration()
        {
            ApiKey = new Dictionary<string, string>
            {
                [BrevoDefaults.ApiKeyHeader] = brevoSettings.ApiKey,
                [BrevoDefaults.PartnerKeyHeader] = brevoSettings.ApiKey
            },
            ApiKeyPrefix = new Dictionary<string, string> { [BrevoDefaults.PartnerKeyHeader] = BrevoDefaults.PartnerName },
            UserAgent = BrevoDefaults.UserAgent
        };

        return clientCtor(apiConfiguration);
    }

    /// <summary>
    /// Import contacts from passed stores to account
    /// </summary>
    /// <param name="storeIds">List of store identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of messages
    /// </returns>
    protected async Task<IList<(NotifyType Type, string Message)>> ImportContactsAsync(IList<int> storeIds)
    {
        var messages = new List<(NotifyType, string)>();

        //import contacts to account
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            foreach (var storeId in storeIds)
            {
                //get list identifier from the settings
                var key = $"{nameof(BrevoSettings)}.{nameof(BrevoSettings.ListId)}";
                var listId = await _settingService.GetSettingByKeyAsync<int>(key, storeId: storeId);
                if (listId == 0)
                {
                    await _logger.WarningAsync($"Brevo synchronization warning: List ID is empty for store #{storeId}");
                    messages.Add((NotifyType.Warning, $"List ID is empty for store #{storeId}"));
                    continue;
                }

                //try to get store subscriptions
                var subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(storeId: storeId, isActive: true);
                if (!subscriptions.Any())
                {
                    await _logger.WarningAsync($"Brevo synchronization warning: There are no subscriptions for store #{storeId}");
                    messages.Add((NotifyType.Warning, $"There are no subscriptions for store #{storeId}"));
                    continue;
                }

                //get notification URL
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var notificationUrl = urlHelper.RouteUrl(BrevoDefaults.ImportContactsRoute, null, _webHelper.GetCurrentRequestProtocol());

                var name = string.Empty;

                switch (await GetAccountLanguageAsync())
                {
                    case BrevoAccountLanguage.French:
                        name =
                            $"{BrevoDefaults.FirstNameFrenchServiceAttribute};" +
                            $"{BrevoDefaults.LastNameFrenchServiceAttribute};";
                        break;
                    case BrevoAccountLanguage.German:
                        name =
                            $"{BrevoDefaults.FirstNameGermanServiceAttribute};" +
                            $"{BrevoDefaults.LastNameGermanServiceAttribute};";
                        break;
                    case BrevoAccountLanguage.Italian:
                        name =
                            $"{BrevoDefaults.FirstNameItalianServiceAttribute};" +
                            $"{BrevoDefaults.LastNameItalianServiceAttribute};";
                        break;
                    case BrevoAccountLanguage.Portuguese:
                        name =
                            $"{BrevoDefaults.FirstNamePortugueseServiceAttribute};" +
                            $"{BrevoDefaults.LastNamePortugueseServiceAttribute};";
                        break;
                    case BrevoAccountLanguage.Spanish:
                        name =
                            $"{BrevoDefaults.FirstNameSpanishServiceAttribute};" +
                            $"{BrevoDefaults.LastNameSpanishServiceAttribute};";
                        break;

                    case BrevoAccountLanguage.English:
                        name =
                            $"{BrevoDefaults.FirstNameServiceAttribute};" +
                            $"{BrevoDefaults.LastNameServiceAttribute};";
                        break;
                }

                var languages = await _languageService.GetAllLanguagesAsync(storeId: storeId);

                //prepare CSV 
                var title =
                    $"{BrevoDefaults.EmailServiceAttribute};" +
                    name +
                    $"{BrevoDefaults.UsernameServiceAttribute};" +
                    $"{BrevoDefaults.SMSServiceAttribute};" +
                    $"{BrevoDefaults.PhoneServiceAttribute};" +
                    $"{BrevoDefaults.CountryServiceAttribute};" +
                    $"{BrevoDefaults.StoreIdServiceAttribute};" +
                    $"{BrevoDefaults.GenderServiceAttribute};" +
                    $"{BrevoDefaults.DateOfBirthServiceAttribute};" +
                    $"{BrevoDefaults.CompanyServiceAttribute};" +
                    $"{BrevoDefaults.Address1ServiceAttribute};" +
                    $"{BrevoDefaults.Address2ServiceAttribute};" +
                    $"{BrevoDefaults.ZipCodeServiceAttribute};" +
                    $"{BrevoDefaults.CityServiceAttribute};" +
                    $"{BrevoDefaults.CountyServiceAttribute};" +
                    $"{BrevoDefaults.StateServiceAttribute};" +
                    $"{BrevoDefaults.FaxServiceAttribute};" +
                    $"{BrevoDefaults.LanguageAttribute};";
                var csv = await subscriptions.AggregateAwaitAsync(title, async (all, subscription) =>
                {
                    var firstName = string.Empty;
                    var lastName = string.Empty;
                    var phone = string.Empty;
                    var countryName = string.Empty;
                    var sms = string.Empty;
                    var gender = string.Empty;
                    var dateOfBirth = string.Empty;
                    var company = string.Empty;
                    var address1 = string.Empty;
                    var address2 = string.Empty;
                    var zipCode = string.Empty;
                    var city = string.Empty;
                    var county = string.Empty;
                    var state = string.Empty;
                    var fax = string.Empty;
                    Language language = null;

                    var customer = await _customerService.GetCustomerByEmailAsync(subscription.Email);
                    if (customer != null)
                    {
                        firstName = customer.FirstName;
                        lastName = customer.LastName;
                        phone = customer.Phone;
                        var countryId = customer.CountryId;
                        var country = await _countryService.GetCountryByIdAsync(countryId);
                        countryName = country?.Name;
                        var countryIsoCode = country?.NumericIsoCode ?? 0;
                        if (countryIsoCode > 0 && !string.IsNullOrEmpty(phone))
                        {
                            //use the first phone code only
                            var phoneCode = ISO3166.FromISOCode(countryIsoCode)
                                ?.DialCodes?.FirstOrDefault()?.Replace(" ", string.Empty) ?? string.Empty;
                            sms = phone.Replace($"+{phoneCode}", string.Empty);
                        }
                        gender = customer.Gender;
                        dateOfBirth = customer.DateOfBirth?.ToString("yyyy-MM-dd");
                        company = customer.Company;
                        address1 = customer.StreetAddress;
                        address2 = customer.StreetAddress2;
                        zipCode = customer.ZipPostalCode;
                        city = customer.City;
                        county = customer.County;
                        state = (await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId))?.Name;
                        fax = customer.Fax;
                    }

                    language = languages.FirstOrDefault(lang => lang.Id == (customer?.LanguageId ?? subscription.LanguageId))
                        ?? languages.FirstOrDefault();

                    return $"{all}\n" +
                           $"{subscription.Email};" +
                           $"{firstName};" +
                           $"{lastName};" +
                           $"{customer?.Username};" +
                           $"{sms};" +
                           $"{phone};" +
                           $"{countryName};" +
                           $"{subscription.StoreId};" +
                           $"{gender};" +
                           $"{dateOfBirth};" +
                           $"{company};" +
                           $"{address1};" +
                           $"{address2};" +
                           $"{zipCode};" +
                           $"{city};" +
                           $"{county};" +
                           $"{state};" +
                           $"{fax};" +
                           $"{language?.LanguageCulture};";
                });

                //prepare data to import
                var requestContactImport = new RequestContactImport
                {
                    NotifyUrl = notificationUrl,
                    FileBody = csv,
                    ListIds = [listId]
                };

                //start import
                await client.ImportContactsAsync(requestContactImport);
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo synchronization error: {exception.Message}", exception, await _workContext.GetCurrentCustomerAsync());
            messages.Add((NotifyType.Error, $"Brevo synchronization error: {exception.Message}"));
        }

        return messages;
    }

    /// <summary>
    /// Export contacts from account to passed stores
    /// </summary>
    /// <param name="storeIds">List of store identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of messages
    /// </returns>
    protected async Task<IList<(NotifyType Type, string Message)>> ExportContactsAsync(IList<int> storeIds)
    {
        var messages = new List<(NotifyType, string)>();

        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            foreach (var storeId in storeIds)
            {
                //get list identifier from the settings
                var key = $"{nameof(BrevoSettings)}.{nameof(BrevoSettings.ListId)}";
                var listId = await _settingService.GetSettingByKeyAsync<int>(key, storeId: storeId, loadSharedValueIfNotFound: true);
                if (listId == 0)
                {
                    await _logger.WarningAsync($"Brevo synchronization warning: List ID is empty for store #{storeId}");
                    messages.Add((NotifyType.Warning, $"List ID is empty for store #{storeId}"));
                    continue;
                }

                //check whether there are contacts in the list
                var contacts = await client.GetContactsFromListAsync(listId);
                var template = new { contacts = new[] { new { email = string.Empty, emailBlacklisted = false } } };
                var contactObjects = JsonConvert.DeserializeAnonymousType(contacts.ToJson(), template);
                var blackListedEmails = contactObjects?.contacts?.Where(contact => contact.emailBlacklisted)
                    .Select(contact => contact.email).ToList() ?? new List<string>();

                foreach (var email in blackListedEmails)
                {
                    //email in black list, so unsubscribe contact from all stores
                    foreach (var id in (await _storeService.GetAllStoresAsync()).Select(store => store.Id))
                    {
                        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, id);
                        if (subscription != null)
                        {
                            subscription.Active = false;
                            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription, false);
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo synchronization error: {exception.Message}", exception, await _workContext.GetCurrentCustomerAsync());
            messages.Add((NotifyType.Error, $"Brevo synchronization error: {exception.Message}"));
        }

        return messages;
    }

    /// <summary>
    /// Add new service attribute in account
    /// </summary>
    /// <param name="attributes">Collection of attributes</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the errors if exist
    /// </returns>
    protected async Task<string> CreateAttributesAsync(IList<(CategoryEnum Category, string Name, string Value, CreateAttribute.TypeEnum? Type)> attributes)
    {
        if (!attributes.Any())
            return string.Empty;

        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            foreach (var attribute in attributes)
            {
                //prepare data
                var createAttribute = new CreateAttribute(attribute.Value, type: attribute.Type);

                //create attribute
                await client.CreateAttributeAsync(attribute.Category.ToString().ToLowerInvariant(), attribute.Name, createAttribute);
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return exception.Message;
        }

        return string.Empty;
    }

    [GeneratedRegex("(%[^\\%]*.%)")]
    private static partial Regex SpecialCharRegex();

    [GeneratedRegex("({{\\s*params\\..*?\\s*}})")]
    private static partial Regex ParamsRegex();

    #endregion

    #region Methods

    #region Synchronization

    /// <summary>
    /// Synchronize contacts 
    /// </summary>
    /// <param name="synchronizationTask">Whether it's a scheduled synchronization</param>
    /// <param name="storeId">Store identifier; pass 0 to synchronize contacts for all stores</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of messages
    /// </returns>
    public async Task<IList<(NotifyType Type, string Message)>> SynchronizeAsync(bool synchronizationTask = true, int storeId = 0)
    {
        var messages = new List<(NotifyType, string)>();
        try
        {
            //whether plugin is configured
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
            if (!string.IsNullOrEmpty(brevoSettings.ApiKey))
            {
                //use only passed store identifier for the manual synchronization
                //use all store ids for the synchronization task
                var storeIds = !synchronizationTask
                        ? [storeId]
                    : new List<int> { 0 }.Union((await _storeService.GetAllStoresAsync()).Select(store => store.Id)).ToList();

                var importMessages = await ImportContactsAsync(storeIds);
                messages.AddRange(importMessages);

                var exportMessages = await ExportContactsAsync(storeIds);
                messages.AddRange(exportMessages);
            }

        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo synchronization error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            messages.Add((NotifyType.Error, $"Brevo synchronization error: {exception.Message}"));
        }

        return messages;
    }

    /// <summary>
    /// Subscribe new contact
    /// </summary>
    /// <param name="subscription">Subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task SubscribeAsync(NewsLetterSubscription subscription)
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            //try to get list identifier
            var key = $"{nameof(BrevoSettings)}.{nameof(BrevoSettings.ListId)}";
            var listId = await _settingService.GetSettingByKeyAsync<int>(key, storeId: subscription.StoreId);
            if (listId == 0)
                listId = await _settingService.GetSettingByKeyAsync<int>(key);
            if (listId == 0)
            {
                await _logger.WarningAsync($"Brevo synchronization warning: List ID is empty for store #{subscription.StoreId}");
                return;
            }

            GetExtendedContactDetails contactObject = null;
            try
            {
                contactObject = await client.GetContactInfoAsync(subscription.Email);
            }
            catch (ApiException apiException)
            {
                if (apiException.ErrorCode != 404)
                {
                    await _logger.ErrorAsync($"Brevo error: {apiException.Message}.", apiException, await _workContext.GetCurrentCustomerAsync());
                    return;
                }
            }

            //prepare attributes
            var firstName = string.Empty;
            var lastName = string.Empty;
            var phone = string.Empty;
            var sms = string.Empty;
            var countryName = string.Empty;
            var gender = string.Empty;
            var dateOfBirth = string.Empty;
            var company = string.Empty;
            var address1 = string.Empty;
            var address2 = string.Empty;
            var zipCode = string.Empty;
            var city = string.Empty;
            var county = string.Empty;
            var state = string.Empty;
            var fax = string.Empty;
            Language language = null;

            var customer = await _customerService.GetCustomerByEmailAsync(subscription.Email);
            if (customer != null)
            {
                firstName = customer.FirstName;
                lastName = customer.LastName;
                phone = customer.Phone;
                var countryId = customer.CountryId;
                var country = await _countryService.GetCountryByIdAsync(countryId);
                countryName = country?.Name;
                var countryIsoCode = country?.NumericIsoCode ?? 0;
                if (countryIsoCode > 0 && !string.IsNullOrEmpty(phone))
                {
                    //use the first phone code only
                    var phoneCode = ISO3166.FromISOCode(countryIsoCode)
                        ?.DialCodes?.FirstOrDefault()?.Replace(" ", string.Empty) ?? string.Empty;
                    sms = phone.Replace($"+{phoneCode}", string.Empty);
                }
                gender = customer.Gender;
                dateOfBirth = customer.DateOfBirth?.ToString("yyyy-MM-dd");
                company = customer.Company;
                address1 = customer.StreetAddress;
                address2 = customer.StreetAddress2;
                zipCode = customer.ZipPostalCode;
                city = customer.City;
                county = customer.County;
                state = (await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId))?.Name;
                fax = customer.Fax;
            }

            language = await _languageService.GetLanguageByIdAsync(customer?.LanguageId ?? subscription.LanguageId)
                ?? (await _languageService.GetAllLanguagesAsync(storeId: subscription.StoreId)).FirstOrDefault();

            var attributes = new Dictionary<string, string>
            {
                [BrevoDefaults.UsernameServiceAttribute] = customer?.Username,
                [BrevoDefaults.SMSServiceAttribute] = sms,
                [BrevoDefaults.PhoneServiceAttribute] = phone,
                [BrevoDefaults.CountryServiceAttribute] = countryName,
                [BrevoDefaults.StoreIdServiceAttribute] = subscription.StoreId.ToString(),
                [BrevoDefaults.GenderServiceAttribute] = gender,
                [BrevoDefaults.DateOfBirthServiceAttribute] = dateOfBirth,
                [BrevoDefaults.CompanyServiceAttribute] = company,
                [BrevoDefaults.Address1ServiceAttribute] = address1,
                [BrevoDefaults.Address2ServiceAttribute] = address2,
                [BrevoDefaults.ZipCodeServiceAttribute] = zipCode,
                [BrevoDefaults.CityServiceAttribute] = city,
                [BrevoDefaults.CountyServiceAttribute] = county,
                [BrevoDefaults.StateServiceAttribute] = state,
                [BrevoDefaults.FaxServiceAttribute] = fax,
                [BrevoDefaults.LanguageAttribute] = language?.LanguageCulture
            };

            switch (await GetAccountLanguageAsync())
            {
                case BrevoAccountLanguage.French:
                    attributes.Add(BrevoDefaults.FirstNameFrenchServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameFrenchServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.German:
                    attributes.Add(BrevoDefaults.FirstNameGermanServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameGermanServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.Italian:
                    attributes.Add(BrevoDefaults.FirstNameItalianServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameItalianServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.Portuguese:
                    attributes.Add(BrevoDefaults.FirstNamePortugueseServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNamePortugueseServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.Spanish:
                    attributes.Add(BrevoDefaults.FirstNameSpanishServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameSpanishServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.English:
                    attributes.Add(BrevoDefaults.FirstNameServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameServiceAttribute, lastName);
                    break;
            }

            //Add new contact
            if (contactObject == null)
            {
                var createContact = new CreateContact
                {
                    Email = subscription.Email,
                    Attributes = attributes,
                    ListIds = [listId],
                    UpdateEnabled = true
                };
                await client.CreateContactAsync(createContact);
            }
            else
            {
                //update contact
                var updateContact = new UpdateContact
                {
                    Attributes = attributes,
                    ListIds = [listId],
                    EmailBlacklisted = false
                };
                await client.UpdateContactAsync(subscription.Email, updateContact);
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
        }
    }

    /// <summary>
    /// Unsubscribe contact
    /// </summary>
    /// <param name="subscription">Subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task UnsubscribeAsync(NewsLetterSubscription subscription)
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            //try to get list identifier
            var key = $"{nameof(BrevoSettings)}.{nameof(BrevoSettings.ListId)}";
            var listId = await _settingService.GetSettingByKeyAsync<int>(key, storeId: subscription.StoreId);
            if (listId == 0)
                listId = await _settingService.GetSettingByKeyAsync<int>(key);
            if (listId == 0)
            {
                await _logger.WarningAsync($"Brevo synchronization warning: List ID is empty for store #{subscription.StoreId}");
                return;
            }

            //update contact
            var updateContact = new UpdateContact
            {
                UnlinkListIds = [listId]
            };
            await client.UpdateContactAsync(subscription.Email, updateContact);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
        }
    }

    /// <summary>
    /// Unsubscribe contact
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task HandleWebhookAsync(Microsoft.AspNetCore.Http.HttpRequest request)
    {
        await HandleFunctionAsync(async () =>
        {
            using var streamReader = new StreamReader(request.Body);
            var requestContent = await streamReader.ReadToEndAsync();

            //parse string to JSON object
            var unsubscriber = JsonConvert.DeserializeAnonymousType(requestContent,
                new { tag = (int?)0, email = string.Empty, date_event = string.Empty });

            //we pass the store identifier in the X-Mailin-Tag at sending emails, now get it here
            var storeId = unsubscriber?.tag;
            if (!storeId.HasValue)
                return true;

            //get subscription by email and store identifier
            var email = unsubscriber?.email;
            var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, storeId.Value);
            if (subscription == null)
                return true;

            //update subscription
            subscription.Active = false;
            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
            await _logger.InformationAsync($"{BrevoDefaults.SystemName} unsubscription: email {email}, store #{storeId}, date {unsubscriber?.date_event}");

            return true;
        });
    }

    /// <summary>
    /// Create webhook to get notification about unsubscribed contacts
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the webhook id
    /// </returns>
    public async Task<int> GetUnsubscribeWebHookIdAsync()
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new WebhooksApi(config));

            //check whether webhook already exist
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
            if (brevoSettings.UnsubscribeWebhookId != 0)
            {
                await client.GetWebhookAsync(brevoSettings.UnsubscribeWebhookId);
                return brevoSettings.UnsubscribeWebhookId;
            }

            //or create new one
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var notificationUrl = urlHelper.RouteUrl(BrevoDefaults.UnsubscribeContactRoute, null, _webHelper.GetCurrentRequestProtocol());
            var webhook = new CreateWebhook(notificationUrl, "Unsubscribe event webhook",
                [CreateWebhook.EventsEnum.Unsubscribed], CreateWebhook.TypeEnum.Transactional);
            var result = await client.CreateWebhookAsync(webhook);

            return (int)result.Id;
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return 0;
        }
    }

    /// <summary>
    /// Update contact after completing order
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task UpdateContactAfterCompletingOrderAsync(Core.Domain.Orders.Order order)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(order);

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer.Email is null)
                return;

            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            try
            {
                var contactInfo = await client.GetContactInfoAsync(customer.Email);
            }
            catch (ApiException apiException)
            {
                if (apiException.ErrorCode == 404)
                {
                    return;
                }
                else
                {
                    await _logger.ErrorAsync($"Brevo error: {apiException.Message}.", apiException, await _workContext.GetCurrentCustomerAsync());
                    return;
                }
            }

            //update contact
            var attributes = new Dictionary<string, string>
            {
                [BrevoDefaults.IdServiceAttribute] = order.Id.ToString(),
                [BrevoDefaults.OrderIdServiceAttribute] = order.Id.ToString(),
                [BrevoDefaults.OrderDateServiceAttribute] = order.PaidDateUtc.ToString(),
                [BrevoDefaults.OrderTotalServiceAttribute] = order.OrderTotal.ToString()
            };
            var updateContact = new UpdateContact { Attributes = attributes };
            await client.UpdateContactAsync(customer.Email, updateContact);

        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
        }
    }

    #endregion

    #region Common

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Result</returns>
    public static bool IsConfigured(BrevoSettings settings)
    {
        //API key is required to request remote services
        return !string.IsNullOrEmpty(settings?.ApiKey);
    }

    /// <summary>
    /// Get account information
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the account info; whether marketing automation is enabled, errors if exist
    /// </returns>
    public async Task<(string Info, bool MarketingAutomationEnabled, string MAkey, string Errors)> GetAccountInfoAsync()
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new AccountApi(config));

            //get account
            var account = await client.GetAccountAsync();

            //prepare info
            var info = string.Format("First name: {1}{0}Last name: {2}{0}Email: {3}{0}Email credits: {4}{0}SMS credits: {5}{0}",
                Environment.NewLine,
                WebUtility.HtmlEncode(account.FirstName),
                WebUtility.HtmlEncode(account.LastName),
                WebUtility.HtmlEncode(account.Email),
                account.Plan.Where(plan => plan.Type != GetAccountPlan.TypeEnum.Sms).Sum(plan => plan.Credits),
                account.Plan.Where(plan => plan.Type == GetAccountPlan.TypeEnum.Sms).Sum(plan => plan.Credits));

            //get marketing automation tacker ID
            var key = account.MarketingAutomation?.Key ?? string.Empty;

            return (info, account.MarketingAutomation?.Enabled ?? false, key, null);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (null, false, null, exception.Message);
        }
    }

    /// <summary>
    /// Set partner value
    /// </summary>
    /// <returns>True if partner successfully set; otherwise false</returns>
    public async Task<bool> SetPartnerAsync()
    {
        try
        {
            var stores = (await _storeService.GetAllStoresAsync()).ToList();
            var storeCredentials = new Dictionary<string, string>();
            foreach (var store in stores)
            {
                var bSettings = await _settingService.LoadSettingAsync<BrevoSettings>(store.Id);
                var apiKey = bSettings.ApiKey;
                if (!string.IsNullOrEmpty(apiKey) && !storeCredentials.Where(s => s.Value == apiKey).Any())
                    storeCredentials.Add(store.Url, apiKey);
            }

            //whether plugin is configured
            if (!storeCredentials.Any())
                return false;

            foreach (var storeCredential in storeCredentials)
            {
                await HttpBrevoClientAsync(storeCredential.Key, storeCredential.Value);
            }
        }
        catch (Exception exception)
        {
            //log full error
            _logger.Error($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return false;
        }

        return true;

        async System.Threading.Tasks.Task HttpBrevoClientAsync(string storeUrl, string apiKey)
        {
            //create API client
            var httpClient = new HttpClient
            {
                //configure client
                BaseAddress = new Uri(BrevoDefaults.AccountApiUrl),
                Timeout = TimeSpan.FromSeconds(10),
            };

            //Default Request Headers needed to be added in the HttpClient Object
            httpClient.DefaultRequestHeaders.Add(BrevoDefaults.ApiKeyHeader, apiKey);
            httpClient.DefaultRequestHeaders.Add(BrevoDefaults.SibPluginHeader, BrevoDefaults.PluginVersion);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, BrevoDefaults.UserAgentAccountAPI);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

            var requestObject = new JObject
            {
                { "partnerName", BrevoDefaults.PartnerName },
                { "active", true },
                { "plugin_version", "1.0.0" },
                { "shop_version", NopVersion.FULL_VERSION },
                { "shop_url", storeUrl },
                { "created_at", DateTime.UtcNow },
                { "activated_at", DateTime.UtcNow },
                { "type", "sib" }
            };

            var requestString = JsonConvert.SerializeObject(requestObject);
            var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "partner/information") { Content = requestContent };

            var httpResponse = await httpClient.SendAsync(requestMessage);
            httpResponse.EnsureSuccessStatusCode();
        }
    }

    /// <summary>
    /// Get available lists to synchronize contacts
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of id-name pairs of lists; errors if exist
    /// </returns>
    public async Task<(IList<(string Id, string Name)> Lists, string Errors)> GetListsAsync()
    {
        var availableLists = new List<(string Id, string Name)>();

        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            //get available lists
            var lists = await client.GetListsAsync(BrevoDefaults.DefaultSynchronizationListsLimit);

            //prepare id-name pairs
            var template = new { lists = new[] { new { id = string.Empty, name = string.Empty } } };
            var listObjects = JsonConvert.DeserializeAnonymousType(lists.ToJson(), template);
            if (listObjects?.lists != null)
            {
                foreach (var list in listObjects.lists)
                {
                    if (list != null)
                        availableLists.Add((list.id, list.name));
                }
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (availableLists, exception.Message);
        }

        return (availableLists, null);
    }

    /// <summary>
    /// Get available senders of transactional emails
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of id-name pairs of senders; errors if exist
    /// </returns>
    public async Task<(IList<(string Id, string Name)> Lists, string Errors)> GetSendersAsync()
    {
        var availableSenders = new List<(string Id, string Name)>();

        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new SendersApi(config));

            //get available senderes
            var senders = await client.GetSendersAsync();

            //prepare id-name pairs
            foreach (var sender in senders.Senders)
            {
                availableSenders.Add((sender.Id.ToString(), $"{sender.Name} ({sender.Email})"));
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (availableSenders, exception.Message);
        }

        return (availableSenders, null);
    }

    /// <summary>
    /// Get account language
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the BrevoAccountLanguage
    /// </returns>
    public async Task<BrevoAccountLanguage> GetAccountLanguageAsync()
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            var attributes = await client.GetAttributesAsync();
            var allAttribytes = attributes.Attributes.Select(s => s.Name).ToList();

            var defaultNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameServiceAttribute,
                BrevoDefaults.LastNameServiceAttribute
            };
            if (defaultNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.English;

            var frenchNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameFrenchServiceAttribute,
                BrevoDefaults.LastNameFrenchServiceAttribute
            };
            if (frenchNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.French;

            var italianNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameItalianServiceAttribute,
                BrevoDefaults.LastNameItalianServiceAttribute
            };
            if (italianNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.Italian;

            var spanishNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameSpanishServiceAttribute,
                BrevoDefaults.LastNameSpanishServiceAttribute
            };
            if (spanishNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.Spanish;

            var germanNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameGermanServiceAttribute,
                BrevoDefaults.LastNameGermanServiceAttribute
            };
            if (germanNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.German;

            var portugueseNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNamePortugueseServiceAttribute,
                BrevoDefaults.LastNamePortugueseServiceAttribute
            };
            if (portugueseNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.Portuguese;

            //Create default customer names attribytes
            var initialAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>
            {
                (CategoryEnum.Normal, BrevoDefaults.FirstNameServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.LastNameServiceAttribute, null, CreateAttribute.TypeEnum.Text)
            };
            //create attributes that are not already on account
            var newAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>();
            foreach (var attribute in initialAttributes)
            {
                if (!allAttribytes.Contains(attribute.Name))
                    newAttributes.Add(attribute);
            }

            await CreateAttributesAsync(newAttributes);

            return BrevoAccountLanguage.English;
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return BrevoAccountLanguage.English;
        }
    }

    /// <summary>
    /// Check and create missing attributes in account
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the errors if exist
    /// </returns>
    public async Task<string> PrepareAttributesAsync()
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            var attributes = await client.GetAttributesAsync();
            var attributeNames = attributes.Attributes.Select(s => s.Name).ToList();

            //prepare attributes to create
            var initialAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>
            {
                (CategoryEnum.Normal, BrevoDefaults.UsernameServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.PhoneServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.CountryServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.StoreIdServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.GenderServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.DateOfBirthServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.CompanyServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.Address1ServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.Address2ServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.ZipCodeServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.CityServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.CountyServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.StateServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.FaxServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.LanguageAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Transactional, BrevoDefaults.OrderIdServiceAttribute, null, CreateAttribute.TypeEnum.Id),
                (CategoryEnum.Transactional, BrevoDefaults.OrderDateServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Transactional, BrevoDefaults.OrderTotalServiceAttribute, null, CreateAttribute.TypeEnum.Float),
                (CategoryEnum.Calculated, BrevoDefaults.OrderTotalSumServiceAttribute, $"SUM[{BrevoDefaults.OrderTotalServiceAttribute}]", null),
                (CategoryEnum.Calculated, BrevoDefaults.OrderTotalMonthSumServiceAttribute, $"SUM[{BrevoDefaults.OrderTotalServiceAttribute},{BrevoDefaults.OrderDateServiceAttribute},>,NOW(-30)]", null),
                (CategoryEnum.Calculated, BrevoDefaults.OrderCountServiceAttribute, $"COUNT[{BrevoDefaults.OrderIdServiceAttribute}]", null),
                (CategoryEnum.Global, BrevoDefaults.AllOrderTotalSumServiceAttribute, $"SUM[{BrevoDefaults.OrderTotalSumServiceAttribute}]", null),
                (CategoryEnum.Global, BrevoDefaults.AllOrderTotalMonthSumServiceAttribute, $"SUM[{BrevoDefaults.OrderTotalMonthSumServiceAttribute}]", null),
                (CategoryEnum.Global, BrevoDefaults.AllOrderCountServiceAttribute, $"SUM[{BrevoDefaults.OrderCountServiceAttribute}]", null)
            };

            //create attributes that are not already on account
            var newAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>();
            foreach (var attribute in initialAttributes)
            {
                if (!attributeNames.Contains(attribute.Name))
                    newAttributes.Add(attribute);
            }

            return await CreateAttributesAsync(newAttributes);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return exception.Message;
        }
    }

    /// <summary>
    /// Move message template tokens to transactional attributes
    /// </summary>
    /// <param name="tokens">List of available message templates tokens</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the errors if exist
    /// </returns>
    public async Task<string> PrepareTransactionalAttributesAsync(IList<string> tokens)
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            //get already existing transactional attributes
            var attributes = await client.GetAttributesAsync();
            var transactionalAttributes = attributes.Attributes
                .Where(attribute => attribute.Category == CategoryEnum.Transactional).ToList();

            //bring tokens to attributes format
            tokens = tokens.Select(token => token.Replace("%", "").Replace(".", "_").Replace("(s)", "-s-").ToUpperInvariant()).ToList();

            //get attributes that are not already on account
            tokens = tokens.Except(transactionalAttributes.Select(attribute => attribute.Name)).ToList();
            if (!tokens.Any())
                return string.Empty;

            //prepare attributes to create
            var newAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>();
            foreach (var token in tokens)
            {
                newAttributes.Add((CategoryEnum.Transactional, token, null, CreateAttribute.TypeEnum.Text));
            }

            return await CreateAttributesAsync(newAttributes);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return exception.Message;
        }
    }

    #endregion

    #region SMTP

    /// <summary>
    /// Check whether SMTP is enabled on account
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of check; errors if exist
    /// </returns>
    public async Task<(bool Enabled, string Errors)> SmtpIsEnabledAsync()
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new AccountApi(config));

            //get account
            var account = await client.GetAccountAsync();
            return (account.Relay?.Enabled ?? false, null);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (false, exception.Message);
        }
    }

    /// <summary>
    /// Get email account identifier
    /// </summary>
    /// <param name="senderId">Sender identifier</param>
    /// <param name="smtpKey">SMTP key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email account identifier; errors if exist
    /// </returns>
    public async Task<(int Id, string Errors)> GetEmailAccountIdAsync(string senderId, string smtpKey)
    {
        try
        {
            //create API clients
            var sendersClient = await CreateApiClientAsync(config => new SendersApi(config));
            var accountClient = await CreateApiClientAsync(config => new AccountApi(config));

            //get all available senders
            var senders = await sendersClient.GetSendersAsync();
            if (!senders.Senders.Any())
                return (0, "There are no senders");

            var currentSender = senders.Senders.FirstOrDefault(sender => sender.Id.ToString() == senderId);
            if (currentSender != null)
            {
                //try to find existing email account by name and email
                var emailAccount = (await _emailAccountService.GetAllEmailAccountsAsync())
                    .FirstOrDefault(account => account.DisplayName == currentSender.Name && account.Email == currentSender.Email);
                if (emailAccount != null)
                    return (emailAccount.Id, null);
            }

            //or create new one
            currentSender ??= senders.Senders.FirstOrDefault();
            var relay = (await accountClient.GetAccountAsync()).Relay;
            var newEmailAccount = new EmailAccount
            {
                Host = relay?.Data?.Relay,
                Port = relay?.Data?.Port ?? 0,
                Username = relay?.Data?.UserName,
                Password = smtpKey,
                EnableSsl = true,
                Email = currentSender.Email,
                DisplayName = currentSender.Name
            };
            await _emailAccountService.InsertEmailAccountAsync(newEmailAccount);

            return (newEmailAccount.Id, null);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (0, exception.Message);
        }
    }

    /// <summary>
    /// Get email template identifier
    /// </summary>
    /// <param name="templateId">Current email template id</param>
    /// <param name="message">Message template</param>
    /// <param name="emailAccount">Email account</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email template identifier
    /// </returns>
    public async Task<int?> GetTemplateIdAsync(int? templateId, MessageTemplate message, EmailAccount emailAccount)
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new TransactionalEmailsApi(config));

            //check whether email template already exists
            if (templateId > 0)
            {
                await client.GetSmtpTemplateAsync(templateId);
                return templateId;
            }

            //or create new one
            if (emailAccount == null)
                throw new NopException("Email account not configured");

            //the original body and subject of the email template are the same as that of the message template
            var body = message.Body.Replace("%if", "\"if\"").Replace("endif%", "\"endif\"");
            body = SpecialCharRegex().Replace(body, x => $"{{{{ params.{x.ToString().Replace("%", "").Replace(".", "_").ToUpperInvariant()} }}}}");
            var subject = message.Subject.Replace("%if", "\"if\"").Replace("endif%", "\"endif\"");
            subject = SpecialCharRegex().Replace(subject, x => $"{{{{ params.{x.ToString().Replace("%", "").Replace(".", "_").ToUpperInvariant()} }}}}");

            //create email template
            var createSmtpTemplate = new CreateSmtpTemplate(sender: new CreateSmtpTemplateSender(emailAccount.DisplayName, emailAccount.Email),
                templateName: message.Name, htmlContent: body, subject: subject, isActive: true);
            var emailTemplate = await client.CreateSmtpTemplateAsync(createSmtpTemplate);

            return (int?)emailTemplate.Id;
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return null;
        }
    }

    /// <summary>
    /// Convert Brevo email template to queued email
    /// </summary>
    /// <param name="templateId">Email template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email
    /// </returns>
    public async Task<QueuedEmail> GetQueuedEmailFromTemplateAsync(int templateId)
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new TransactionalEmailsApi(config));

            if (templateId == 0)
                throw new NopException("Message template is empty");

            //get template
            var template = await client.GetSmtpTemplateAsync(templateId);

            //bring attributes to tokens format
            var subject = ParamsRegex().Replace(template.Subject, x => $"%{x.ToString().Replace("{", "").Replace("}", "").Replace("params.", "").Replace("_", ".").Trim()}%");
            subject = subject.Replace("\"if\"", "%if").Replace("\"endif\"", "endif%");
            var body = ParamsRegex().Replace(template.HtmlContent, x => $"%{x.ToString().Replace("{", "").Replace("}", "").Replace("params.", "").Replace("_", ".").Trim()}%");
            body = body.Replace("\"if\"", "%if").Replace("\"endif\"", "endif%");

            //map template to queued email
            return new QueuedEmail
            {
                Subject = subject,
                Body = body,
                FromName = template.Sender?.Name,
                From = template.Sender?.Email
            };
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo email sending error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return null;
        }
    }

    #endregion

    #region SMS

    /// <summary>
    /// Send SMS 
    /// </summary>
    /// <param name="to">Phone number of the receiver</param>
    /// <param name="from">Name of sender</param>
    /// <param name="text">Text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task SendSMSAsync(string to, string from, string text)
    {
        //whether SMS notifications enabled
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
        if (!brevoSettings.UseSmsNotifications)
            return;

        try
        {
            //check number and text
            if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(text))
                throw new NopException("Phone number or SMS text is empty");

            //create API client
            var client = await CreateApiClientAsync(config => new TransactionalSMSApi(config));

            //create SMS data
            var transactionalSms = new SendTransacSms(sender: from, recipient: to, content: text, type: SendTransacSms.TypeEnum.Transactional);

            //send SMS
            var sms = await client.SendTransacSmsAsync(transactionalSms);
            await _logger.InformationAsync($"Brevo SMS sent: {sms?.Reference ?? $"credits remaining {sms?.RemainingCredits?.ToString()}"}");
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo SMS sending error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
        }
    }

    /// <summary>
    /// Send SMS campaign
    /// </summary>
    /// <param name="listId">Contact list identifier</param>
    /// <param name="from">Name of sender</param>
    /// <param name="text">Text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<string> SendSMSCampaignAsync(int listId, string from, string text)
    {
        try
        {
            //check list and text
            if (listId == 0 || string.IsNullOrEmpty(text) || string.IsNullOrEmpty(from))
                throw new NopException("List or SMS text or sender name is empty");

            //create API client
            var client = await CreateApiClientAsync(config => new SMSCampaignsApi(config));

            //create SMS campaign
            var campaign = await client.CreateSmsCampaignAsync(new CreateSmsCampaign(name: CommonHelper.EnsureMaximumLength(text, 20),
                sender: from, content: text, recipients: new CreateSmsCampaignRecipients([listId])));

            //send campaign
            await client.SendSmsCampaignNowAsync(campaign.Id);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo SMS sending error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return exception.Message;
        }

        return string.Empty;
    }

    #endregion

    #endregion
}