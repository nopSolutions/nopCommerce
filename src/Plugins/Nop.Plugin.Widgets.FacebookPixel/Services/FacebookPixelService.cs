using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Framework.Models.Cms;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.FacebookPixel.Services;

/// <summary>
/// Represents Facebook Pixel service
/// </summary>
public class FacebookPixelService
{
    #region Constants

    /// <summary>
    /// Get default tabs number to format scripts indentation
    /// </summary>
    protected const int TABS_NUMBER = 2;

    #endregion

    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly FacebookConversionsHttpClient _facebookConversionsHttpClient;
    protected readonly ICategoryService _categoryService;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;
    protected readonly IOrderService _orderService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IProductService _productService;
    protected readonly IRepository<FacebookPixelConfiguration> _facebookPixelConfigurationRepository;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly ITaxService _taxService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWidgetPluginManager _widgetPluginManager;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public FacebookPixelService(CurrencySettings currencySettings,
        FacebookConversionsHttpClient facebookConversionsHttpClient,
        ICategoryService categoryService,
        ICountryService countryService,
        ICurrencyService currencyService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPriceCalculationService priceCalculationService,
        IProductService productService,
        IRepository<FacebookPixelConfiguration> facebookPixelConfigurationRepository,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        ITaxService taxService,
        IWebHelper webHelper,
        IWidgetPluginManager widgetPluginManager,
        IWorkContext workContext)
    {
        _currencySettings = currencySettings;
        _facebookConversionsHttpClient = facebookConversionsHttpClient;
        _categoryService = categoryService;
        _countryService = countryService;
        _currencyService = currencyService;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _priceCalculationService = priceCalculationService;
        _productService = productService;
        _facebookPixelConfigurationRepository = facebookPixelConfigurationRepository;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _taxService = taxService;
        _webHelper = webHelper;
        _widgetPluginManager = widgetPluginManager;
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
    /// The task result contains the function result
    /// </returns>
    protected async Task<TResult> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
    {
        try
        {
            //check whether the plugin is active
            if (!await PluginActiveAsync())
                return default;

            //invoke function
            return await function();
        }
        catch (Exception exception)
        {
            if (!logErrors)
                return default;

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer.IsSearchEngineAccount() || customer.IsBackgroundTaskAccount())
                return default;

            //log errors
            var error = $"{FacebookPixelDefaults.SystemName} error: {Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(error, exception, customer);

            return default;
        }
    }

    /// <summary>
    /// Check whether the plugin is active for the current user and the current store
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    protected async Task<bool> PluginActiveAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        return await _widgetPluginManager.IsPluginActiveAsync(FacebookPixelDefaults.SystemName, customer, store.Id);
    }

    /// <summary>
    /// Prepare scripts
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PrepareScriptsAsync(IList<FacebookPixelConfiguration> configurations)
    {
        return await PrepareInitScriptAsync(configurations) +
               await PrepareUserPropertiesScriptAsync(configurations) +
               await PreparePageViewScriptAsync(configurations) +
               await PrepareTrackedEventsScriptAsync(configurations);
    }

    /// <summary>
    /// Prepare user info (used with Advanced Matching feature)
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the user info
    /// </returns>
    protected async Task<string> GetUserObjectAsync()
    {
        //prepare user object
        var customer = await _workContext.GetCurrentCustomerAsync();
        var email = customer.Email;
        var firstName = customer.FirstName;
        var lastName = customer.LastName;
        var phone = customer.Phone;
        var gender = customer.Gender;
        var birthday = customer.DateOfBirth;
        var city = customer.City;
        var countryId = customer.CountryId;
        var countryName = (await _countryService.GetCountryByIdAsync(countryId))?.TwoLetterIsoCode;
        var stateId = customer.StateProvinceId;
        var stateName = (await _stateProvinceService.GetStateProvinceByIdAsync(stateId))?.Abbreviation;
        var zipcode = customer.ZipPostalCode;

        return FormatEventObject(
        [
            ("em", JavaScriptEncoder.Default.Encode(email?.ToLowerInvariant() ?? string.Empty)),
            ("fn", JavaScriptEncoder.Default.Encode(firstName?.ToLowerInvariant() ?? string.Empty)),
            ("ln", JavaScriptEncoder.Default.Encode(lastName?.ToLowerInvariant() ?? string.Empty)),
            ("ph", new string(phone?.Where(c => char.IsDigit(c)).ToArray()) ?? string.Empty),
            ("external_id", customer.CustomerGuid.ToString().ToLowerInvariant()),
            ("ge", gender?.FirstOrDefault().ToString().ToLowerInvariant()),
            ("db", birthday?.ToString("yyyyMMdd")),
            ("ct", JavaScriptEncoder.Default.Encode(city?.ToLowerInvariant() ?? string.Empty)),
            ("st", stateName?.ToLowerInvariant()),
            ("zp", JavaScriptEncoder.Default.Encode(zipcode?.ToLowerInvariant() ?? string.Empty)),
            ("cn", countryName?.ToLowerInvariant())
        ]);
    }

    /// <summary>
    /// Prepare script to init Facebook Pixel
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PrepareInitScriptAsync(IList<FacebookPixelConfiguration> configurations)
    {
        //prepare init script
        return await FormatScriptAsync(configurations, async configuration =>
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            var additionalParameter = configuration.PassUserProperties
                ? $", {{uid: '{customer.CustomerGuid}'}}"
                : (configuration.UseAdvancedMatching
                    ? $", {await GetUserObjectAsync()}"
                    : null);
            return $"fbq('init', '{configuration.PixelId}'{additionalParameter});";
        });
    }

    /// <summary>
    /// Prepare script to pass user properties
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PrepareUserPropertiesScriptAsync(IList<FacebookPixelConfiguration> configurations)
    {
        //filter active configurations
        var activeConfigurations = configurations.Where(configuration => configuration.PassUserProperties).ToList();
        if (!activeConfigurations.Any())
            return string.Empty;

        //prepare user object
        var customer = await _workContext.GetCurrentCustomerAsync();
        var createdOn = new DateTimeOffset(customer.CreatedOnUtc).ToUnixTimeSeconds().ToString();
        var city = customer.City;
        var countryId = customer.CountryId;
        var countryName = (await _countryService.GetCountryByIdAsync(countryId))?.TwoLetterIsoCode;
        var currency = (await _workContext.GetWorkingCurrencyAsync())?.CurrencyCode;
        var gender = customer.Gender;
        var language = (await _workContext.GetWorkingLanguageAsync())?.UniqueSeoCode;
        var stateId = customer.StateProvinceId;
        var stateName = (await _stateProvinceService.GetStateProvinceByIdAsync(stateId))?.Abbreviation;
        var zipcode = customer.ZipPostalCode;

        var userObject = FormatEventObject(
        [
            ("$account_created_time", createdOn),
            ("$city", JavaScriptEncoder.Default.Encode(city ?? string.Empty)),
            ("$country", countryName),
            ("$currency", currency),
            ("$gender", gender?.FirstOrDefault().ToString()),
            ("$language", language),
            ("$state", stateName),
            ("$zipcode", JavaScriptEncoder.Default.Encode(zipcode ?? string.Empty))
        ]);

        //prepare script
        return await FormatScriptAsync(activeConfigurations, configuration =>
            Task.FromResult($"fbq('setUserProperties', '{configuration.PixelId}', {userObject});"));
    }

    /// <summary>
    /// Prepare script to track "PageView" event
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PreparePageViewScriptAsync(IList<FacebookPixelConfiguration> configurations)
    {
        //a single active configuration is enough to track PageView event
        var activeConfigurations = configurations.Where(configuration => configuration.TrackPageView).Take(1).ToList();
        return await FormatScriptAsync(activeConfigurations, configuration =>
            Task.FromResult($"fbq('track', '{FacebookPixelDefaults.PAGE_VIEW}');"));
    }

    /// <summary>
    /// Prepare scripts to track events
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PrepareTrackedEventsScriptAsync(IList<FacebookPixelConfiguration> configurations)
    {
        //get previously stored events and remove them from the session data
        var events = (await _httpContextAccessor.HttpContext.Session
                         .GetAsync<IList<TrackedEvent>>(FacebookPixelDefaults.TrackedEventsSessionValue))
                     ?? new List<TrackedEvent>();
        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var activeEvents = events.Where(trackedEvent =>
                trackedEvent.CustomerId == customer.Id && trackedEvent.StoreId == store.Id)
            .ToList();
        await _httpContextAccessor.HttpContext.Session.SetAsync(
            FacebookPixelDefaults.TrackedEventsSessionValue,
            events.Except(activeEvents).ToList());

        if (!activeEvents.Any())
            return string.Empty;

        return await activeEvents.AggregateAwaitAsync(string.Empty, async (preparedScripts, trackedEvent) =>
        {
            //filter active configurations
            var activeConfigurations = trackedEvent.EventName switch
            {
                FacebookPixelDefaults.ADD_TO_CART => configurations.Where(configuration => configuration.TrackAddToCart).ToList(),
                FacebookPixelDefaults.PURCHASE => configurations.Where(configuration => configuration.TrackPurchase).ToList(),
                FacebookPixelDefaults.VIEW_CONTENT => configurations.Where(configuration => configuration.TrackViewContent).ToList(),
                FacebookPixelDefaults.ADD_TO_WISHLIST => configurations.Where(configuration => configuration.TrackAddToWishlist).ToList(),
                FacebookPixelDefaults.INITIATE_CHECKOUT => configurations.Where(configuration => configuration.TrackInitiateCheckout).ToList(),
                FacebookPixelDefaults.SEARCH => configurations.Where(configuration => configuration.TrackSearch).ToList(),
                FacebookPixelDefaults.CONTACT => configurations.Where(configuration => configuration.TrackContact).ToList(),
                FacebookPixelDefaults.COMPLETE_REGISTRATION => configurations.Where(configuration => configuration.TrackCompleteRegistration).ToList(),
                _ => new List<FacebookPixelConfiguration>()
            };
            if (trackedEvent.IsCustomEvent)
            {
                activeConfigurations = await configurations.WhereAwait(async configuration =>
                    (await GetCustomEventsAsync(configuration.Id)).Any(customEvent => customEvent.EventName == trackedEvent.EventName)).ToListAsync();
            }

            //prepare event scripts
            return preparedScripts + await trackedEvent.EventObjects.AggregateAwaitAsync(string.Empty, async (preparedEventScripts, eventObject) =>
            {
                return preparedEventScripts + await FormatScriptAsync(activeConfigurations, configuration =>
                {
                    //used for accurate event tracking with multiple Facebook Pixels
                    var actionName = configurations.Count > 1
                        ? (trackedEvent.IsCustomEvent ? "trackSingleCustom" : "trackSingle")
                        : (trackedEvent.IsCustomEvent ? "trackCustom" : "track");
                    var additionalParameter = configurations.Count > 1 ? $", '{configuration.PixelId}'" : null;

                    //prepare event script
                    var eventObjectParameter = !string.IsNullOrEmpty(eventObject) ? $", {eventObject}" : null;
                    return Task.FromResult($"fbq('{actionName}'{additionalParameter}, '{trackedEvent.EventName}'{eventObjectParameter});");
                });
            });
        });
    }

    /// <summary>
    /// Prepare script to track event and store it for the further using
    /// </summary>
    /// <param name="eventName">Event name</param>
    /// <param name="eventObject">Event object</param>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="isCustomEvent">Whether the event is a custom one</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task PrepareTrackedEventScriptAsync(string eventName, string eventObject,
        int? customerId = null, int? storeId = null, bool isCustomEvent = false)
    {
        //prepare script and store it into the session data, we use this later
        var customer = await _workContext.GetCurrentCustomerAsync();
        customerId ??= customer.Id;
        var store = await _storeContext.GetCurrentStoreAsync();
        storeId ??= store.Id;
        var events = await _httpContextAccessor.HttpContext.Session
                         .GetAsync<IList<TrackedEvent>>(FacebookPixelDefaults.TrackedEventsSessionValue)
                     ?? new List<TrackedEvent>();
        var activeEvent = events.FirstOrDefault(trackedEvent =>
            trackedEvent.EventName == eventName && trackedEvent.CustomerId == customerId && trackedEvent.StoreId == storeId);
        if (activeEvent == null)
        {
            activeEvent = new TrackedEvent
            {
                EventName = eventName,
                CustomerId = customerId.Value,
                StoreId = storeId.Value,
                IsCustomEvent = isCustomEvent
            };
            events.Add(activeEvent);
        }
        activeEvent.EventObjects.Add(eventObject);
        await _httpContextAccessor.HttpContext.Session.SetAsync(FacebookPixelDefaults.TrackedEventsSessionValue, events);
    }

    /// <summary>
    /// Format event object to look pretty
    /// </summary>
    /// <param name="properties">Event object properties</param>
    /// <param name="tabsNumber">Tabs number for indentation script</param>
    /// <returns>Script code</returns>
    protected string FormatEventObject(List<(string Name, object Value)> properties, int? tabsNumber = null)
    {
        //local function to format list of objects
        string formatObjectList(List<List<(string Name, object Value)>> objectList)
        {
            var formattedList = objectList.Aggregate(string.Empty, (preparedObjects, propertiesList) =>
            {
                if (propertiesList != null)
                {
                    var value = FormatEventObject(propertiesList, (tabsNumber ?? TABS_NUMBER) + 1);
                    preparedObjects += $"{Environment.NewLine}{new string('\t', (tabsNumber ?? TABS_NUMBER) + 1)}{value},";
                }

                return preparedObjects;
            }).TrimEnd(',');
            return $"[{formattedList}]";
        }

        //format single object
        var formattedObject = properties.Aggregate(string.Empty, (preparedObject, property) =>
        {
            if (!string.IsNullOrEmpty(property.Value?.ToString()))
            {
                //format property value
                var value = property.Value is string valueString
                    ? $"'{valueString.Replace("'", "\\'")}'"
                    : (property.Value is List<List<(string Name, object Value)>> valueList
                        ? formatObjectList(valueList)
                        : (property.Value is decimal valueDecimal
                            ? valueDecimal.ToString("F", CultureInfo.InvariantCulture)
                            : property.Value.ToString().ToLowerInvariant()));

                //format object property
                preparedObject += $"{Environment.NewLine}{new string('\t', (tabsNumber ?? TABS_NUMBER) + 1)}{property.Name}: {value},";
            }

            return preparedObject;
        }).TrimEnd(',');

        return $"{{{formattedObject}{Environment.NewLine}{new string('\t', tabsNumber ?? TABS_NUMBER)}}}";
    }

    /// <summary>
    /// Format script to look pretty
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <param name="getScript">Function to get script for the passed configuration</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> FormatScriptAsync(IList<FacebookPixelConfiguration> configurations, Func<FacebookPixelConfiguration, Task<string>> getScript)
    {
        if (!configurations.Any())
            return string.Empty;

        //format script
        var formattedScript = await configurations.AggregateAwaitAsync(string.Empty, async (preparedScripts, configuration) =>
            preparedScripts + Environment.NewLine + new string('\t', TABS_NUMBER) + await getScript(configuration));
        formattedScript += Environment.NewLine;

        return formattedScript;
    }

    /// <summary>
    /// Format custom event data to look pretty
    /// </summary>
    /// <param name="customData">Custom data</param>
    /// <returns>Script code</returns>
    protected string FormatCustomData(ConversionsEventCustomData customData)
    {
        List<(string Name, object Value)> getProperties(JObject jObject)
        {
            var result = jObject.ToObject<Dictionary<string, object>>();
            foreach (var pair in result)
            {
                if (pair.Value is JObject nestedObject)
                    result[pair.Key] = getProperties(nestedObject);
                if (pair.Value is JArray nestedArray && nestedArray.OfType<JObject>().Any())
                    result[pair.Key] = nestedArray.OfType<JObject>().Select(obj => getProperties(obj)).ToList();
            }

            return result.Select(pair => (pair.Key, pair.Value)).ToList();
        }

        try
        {
            var customDataObject = JObject.FromObject(customData, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

            return FormatEventObject(getProperties(customDataObject));

        }
        catch
        {
            //if something went wrong, just serialize the data without format
            return JsonConvert.SerializeObject(customData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }

    /// <summary>
    /// Get configurations
    /// </summary>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of configurations
    /// </returns>
    protected async Task<IList<FacebookPixelConfiguration>> GetConfigurationsAsync(int storeId = 0)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(FacebookPixelDefaults.ConfigurationsCacheKey, storeId);

        var query = _facebookPixelConfigurationRepository.Table;

        //filter by the store
        if (storeId > 0)
            query = query.Where(configuration => configuration.StoreId == storeId);

        query = query.OrderBy(configuration => configuration.Id);

        return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
    }

    /// <summary>
    /// Prepare Pixel script and send requests to Conversions API for the passed event
    /// </summary>
    /// <param name="prepareModel">Function to prepare model</param>
    /// <param name="eventName">Event name</param>
    /// <param name="storeId">Store identifier; pass null to load records for the current store</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value whether handling was successful
    /// </returns>
    protected async Task<bool> HandleEventAsync(Func<Task<ConversionsEvent>> prepareModel, string eventName, int? storeId = null)
    {
        storeId ??= (await _storeContext.GetCurrentStoreAsync()).Id;
        var configurations = (await GetConfigurationsAsync(storeId ?? 0)).Where(configuration => eventName switch
        {
            FacebookPixelDefaults.ADD_TO_CART => configuration.TrackAddToCart,
            FacebookPixelDefaults.ADD_TO_WISHLIST => configuration.TrackAddToWishlist,
            FacebookPixelDefaults.PURCHASE => configuration.TrackPurchase,
            FacebookPixelDefaults.VIEW_CONTENT => configuration.TrackViewContent,
            FacebookPixelDefaults.INITIATE_CHECKOUT => configuration.TrackInitiateCheckout,
            FacebookPixelDefaults.PAGE_VIEW => configuration.TrackPageView,
            FacebookPixelDefaults.SEARCH => configuration.TrackSearch,
            FacebookPixelDefaults.CONTACT => configuration.TrackContact,
            FacebookPixelDefaults.COMPLETE_REGISTRATION => configuration.TrackCompleteRegistration,
            _ => false
        }).ToList();

        var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
        var pixelConfigurations = configurations.Where(configuration => configuration.PixelScriptEnabled).ToList();
        if (!conversionsApiConfigurations.Any() && !pixelConfigurations.Any())
            return false;

        var model = await prepareModel();

        if (pixelConfigurations.Any())
            await PrepareEventScriptAsync(model);

        var logErrors = true; //set it to false to ignore Conversions API errors 
        foreach (var configuration in conversionsApiConfigurations)
        {
            await HandleFunctionAsync(async () =>
            {
                var response = await _facebookConversionsHttpClient.SendEventAsync(configuration, model);
                var error = JsonConvert.DeserializeAnonymousType(response, new { Error = new ApiError() })?.Error;
                if (!string.IsNullOrEmpty(error?.Message))
                    throw new NopException($"{error.Code} - {error.Message}{Environment.NewLine}Debug ID: {error.DebugId}");

                return true;
            }, logErrors);
        }

        return true;
    }

    /// <summary>
    /// Prepare user data for conversions api
    /// </summary>
    /// <returns>
    /// <param name="customer">Customer</param>
    /// A task that represents the asynchronous operation
    /// The task result contains the user data
    /// </returns>
    protected async Task<ConversionsEventUserData> PrepareUserDataAsync(Customer customer = null)
    {
        //prepare user object
        customer ??= await _workContext.GetCurrentCustomerAsync();
        var twoLetterCountryIsoCode = (await _countryService.GetCountryByIdAsync(customer.CountryId))?.TwoLetterIsoCode;
        var stateName = (await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId))?.Abbreviation;
        var ipAddress = _webHelper.GetCurrentIpAddress();
        var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.UserAgent].ToString();

        return new ConversionsEventUserData
        {
            EmailAddress = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.Email?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            FirstName = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.FirstName?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            LastName = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.LastName?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            PhoneNumber = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.Phone?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            ExternalId = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer?.CustomerGuid.ToString()?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            Gender = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.Gender?.FirstOrDefault().ToString() ?? string.Empty), "SHA256")],
            DateOfBirth = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.DateOfBirth?.ToString("yyyyMMdd") ?? string.Empty), "SHA256")],
            City = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.City?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            State = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(stateName?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            Zip = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.ZipPostalCode?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            Country = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(twoLetterCountryIsoCode?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            ClientIpAddress = ipAddress?.ToLowerInvariant(),
            ClientUserAgent = userAgent?.ToLowerInvariant(),
            Id = customer.Id
        };
    }

    /// <summary>
    /// Prepare add to cart event model
    /// </summary>
    /// <param name="item">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareAddToCartEventModelAsync(ShoppingCartItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        //check whether the shopping was initiated by the customer
        var customer = await _workContext.GetCurrentCustomerAsync();

        var store = await _storeContext.GetCurrentStoreAsync();

        if (item.CustomerId != customer.Id)
            throw new NopException("Shopping was not initiated by customer");

        var eventName = item.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart
            ? FacebookPixelDefaults.ADD_TO_CART
            : FacebookPixelDefaults.ADD_TO_WISHLIST;

        var product = await _productService.GetProductByIdAsync(item.ProductId);
        var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(product?.Id ?? 0)).FirstOrDefault();
        var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
        var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
        var quantity = product != null ? (int?)item.Quantity : null;
        var (productPrice, _, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, includeDiscounts: false);
        var (price, _) = await _taxService.GetProductPriceAsync(product, productPrice);
        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
        var priceValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price, currentCurrency);
        var currency = currentCurrency?.CurrencyCode;

        var eventObject = new ConversionsEventCustomData
                {
                    ContentCategory = categoryName,
                    ContentIds = [sku],
                    ContentName = product?.Name,
                    ContentType = "product",
                    Contents =
                    [
                    new
                    {
                    id = sku,
                    quantity = quantity,
                    item_price = priceValue
                }
                ],
            Currency = currency,
            Value = priceValue
        };

        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = eventName,
                EventTime = new DateTimeOffset(item.CreatedOnUtc).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(customer),
                CustomData = eventObject,
                StoreId = item.StoreId
            }
            ]
        };
    }

    /// <summary>
    /// Prepare purchase event model
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PreparePurchaseModelAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        //check whether the purchase was initiated by the customer
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (order.CustomerId != customer.Id)
            throw new NopException("Purchase was not initiated by customer");

        //prepare event object
        var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        var contentsProperties = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async item =>
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
            var quantity = product != null ? (int?)item.Quantity : null;
            return new { id = sku, quantity = quantity };
        }).Cast<object>().ToListAsync();
        var eventObject = new ConversionsEventCustomData
        {
            ContentType = "product",
            Contents = contentsProperties,
            Currency = currency?.CurrencyCode,
            Value = order.OrderTotal
        };

        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.PURCHASE,
                EventTime = new DateTimeOffset(order.CreatedOnUtc).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(customer),
                CustomData = eventObject,
                StoreId = order.StoreId
            }
            ]
        };
    }

    /// <summary>
    /// Prepare view content event model
    /// </summary>
    /// <param name="productDetails">Product details model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareViewContentModelAsync(ProductDetailsModel productDetails)
    {
        ArgumentNullException.ThrowIfNull(productDetails);

        //prepare event object
        var product = await _productService.GetProductByIdAsync(productDetails.Id);
        var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(product?.Id ?? 0)).FirstOrDefault();
        var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
        var sku = productDetails.Sku;
        var priceValue = productDetails.ProductPrice.PriceValue;
        var currency = (await _workContext.GetWorkingCurrencyAsync())?.CurrencyCode;

        var eventObject = new ConversionsEventCustomData
        {
            ContentCategory = categoryName,
            ContentIds = [sku],
            ContentName = product?.Name,
            ContentType = "product",
            Currency = currency,
            Value = priceValue
        };

        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.VIEW_CONTENT,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = eventObject
            }
            ]
        };
    }

    /// <summary>
    /// Prepare initiate checkout event model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareInitiateCheckoutModelAsync()
    {
        //prepare event object
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
        var (price, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false, false);
        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
        var priceValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price ?? 0, currentCurrency);
        var currency = currentCurrency?.CurrencyCode;

        var contentsProperties = await cart.SelectAwait(async item =>
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
            var quantity = product != null ? (int?)item.Quantity : null;
            return new { id = sku, quantity = quantity };
        }).Cast<object>().ToListAsync();

        var eventObject = new ConversionsEventCustomData
        {
            ContentType = "product",
            Contents = contentsProperties,
            Currency = currency,
            Value = priceValue
        };

        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.INITIATE_CHECKOUT,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(customer),
                CustomData = eventObject
            }
            ]
        };
    }

    /// <summary>
    /// Prepare page view event model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PreparePageViewModelAsync()
    {
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.PAGE_VIEW,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = new ConversionsEventCustomData()
            }
            ]
        };
    }

    /// <summary>
    /// Prepare search event model
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareSearchModelAsync(string searchTerm)
    {
        //prepare event object
        var eventObject = new ConversionsEventCustomData
        {
            SearchString = JavaScriptEncoder.Default.Encode(searchTerm)
        };

        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.SEARCH,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = eventObject
            }
            ]
        };
    }

    /// <summary>
    /// Prepare contact event model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareContactModelAsync()
    {
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.CONTACT,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = new ConversionsEventCustomData()
            }
            ]
        };
    }

    /// <summary>
    /// Prepare complete registration event model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareCompleteRegistrationModelAsync()
    {
        //prepare event object
        var eventObject = new ConversionsEventCustomData
        {
            Status = true.ToString()
        };

        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.COMPLETE_REGISTRATION,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = eventObject
            }
            ]
        };
    }

    #endregion

    #region Methods

    #region Scripts

    /// <summary>
    /// Prepare script to track events and store it into the session value
    /// </summary>
    /// <param name="conversionsEvent">Conversions event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task PrepareEventScriptAsync(ConversionsEvent conversionsEvent)
    {
        await HandleFunctionAsync(async () =>
        {
            //get current stored events 
            var events = await _httpContextAccessor.HttpContext.Session
                             .GetAsync<IList<TrackedEvent>>(FacebookPixelDefaults.TrackedEventsSessionValue)
                         ?? new List<TrackedEvent>();

            var store = await _storeContext.GetCurrentStoreAsync();
            foreach (var conversionsEventData in conversionsEvent.Data)
            {
                conversionsEventData.StoreId ??= store.Id;
                var activeEvent = events.FirstOrDefault(trackedEvent =>
                    trackedEvent.EventName == conversionsEventData.EventName &&
                    trackedEvent.CustomerId == conversionsEventData.UserData?.Id &&
                    trackedEvent.StoreId == conversionsEventData.StoreId);
                if (activeEvent is null)
                {
                    activeEvent = new TrackedEvent
                    {
                        EventName = conversionsEventData.EventName,
                        CustomerId = conversionsEventData.UserData?.Id ?? 0,
                        StoreId = conversionsEventData.StoreId ?? 0,
                        IsCustomEvent = conversionsEventData.IsCustomEvent
                    };
                    events.Add(activeEvent);
                }

                activeEvent.EventObjects.Add(FormatCustomData(conversionsEventData.CustomData));
            }

            //update events in the session value
            await _httpContextAccessor.HttpContext.Session.SetAsync(FacebookPixelDefaults.TrackedEventsSessionValue, events);

            return true;
        });
    }

    /// <summary>
    /// Prepare Facebook Pixel script
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    public async Task<string> PrepareScriptAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            //get the enabled configurations
            var store = await _storeContext.GetCurrentStoreAsync();
            var configurations = await (await GetConfigurationsAsync(store.Id)).WhereAwait(async configuration =>
            {
                if (!configuration.PixelScriptEnabled)
                    return false;

                if (!configuration.DisableForUsersNotAcceptingCookieConsent)
                    return true;

                //don't display Pixel for users who not accepted Cookie Consent
                var cookieConsentAccepted = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(),
                    NopCustomerDefaults.EuCookieLawAcceptedAttribute, store.Id);
                return cookieConsentAccepted;
            }).ToListAsync();
            if (!configurations.Any())
                return string.Empty;

            //base script
            return $@"
    <!-- Facebook Pixel Code -->
    <script>

        !function (f, b, e, v, n, t, s) {{
            if (f.fbq) return;
            n = f.fbq = function () {{
                n.callMethod ? n.callMethod.apply(n, arguments) : n.queue.push(arguments)
            }};
            if (!f._fbq) f._fbq = n;
            n.push = n;
            n.loaded = !0;
            n.version = '2.0';
            n.agent = '{FacebookPixelDefaults.AgentId}';
            n.queue = [];
            t = b.createElement(e);
            t.async = !0;
            t.src = v;
            s = b.getElementsByTagName(e)[0];
            s.parentNode.insertBefore(t, s)
        }}(window, document, 'script', 'https://connect.facebook.net/en_US/fbevents.js');
        {await PrepareScriptsAsync(configurations)}
    </script>
    <!-- End Facebook Pixel Code -->";
        });
    }

    /// <summary>
    /// Prepare Facebook Pixel script
    /// </summary>
    /// <param name="widgetZone">Widget zone to place script</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    public async Task<string> PrepareCustomEventsScriptAsync(string widgetZone)
    {
        return await HandleFunctionAsync(async () =>
        {
            var customEvents = await (await GetConfigurationsAsync()).SelectManyAwait(async configuration => await GetCustomEventsAsync(configuration.Id, widgetZone)).ToListAsync();
            foreach (var customEvent in customEvents)
                await PrepareTrackedEventScriptAsync(customEvent.EventName, string.Empty, isCustomEvent: true);

            return string.Empty;
        });
    }

    #endregion

    #region Conversions API

    /// <summary>
    /// Send add to cart events
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendAddToCartEventAsync(ShoppingCartItem shoppingCartItem)
    {
        await HandleFunctionAsync(async () =>
        {
            var eventName = shoppingCartItem.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart
                ? FacebookPixelDefaults.ADD_TO_CART
                : FacebookPixelDefaults.ADD_TO_WISHLIST;

            return await HandleEventAsync(() => PrepareAddToCartEventModelAsync(shoppingCartItem), eventName, shoppingCartItem.StoreId);
        });
    }

    /// <summary>
    /// Send purchase events
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendPurchaseEventAsync(Order order)
    {
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PreparePurchaseModelAsync(order), FacebookPixelDefaults.PURCHASE, order.StoreId));
    }

    /// <summary>
    /// Send view content events
    /// </summary>
    /// <param name="productDetailsModel">Product details model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendViewContentEventAsync(ProductDetailsModel productDetailsModel)
    {
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareViewContentModelAsync(productDetailsModel), FacebookPixelDefaults.VIEW_CONTENT));
    }

    /// <summary>
    /// Send initiate checkout events
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendInitiateCheckoutEventAsync()
    {
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareInitiateCheckoutModelAsync(), FacebookPixelDefaults.INITIATE_CHECKOUT));
    }

    /// <summary>
    /// Send page view events
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendPageViewEventAsync()
    {
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PreparePageViewModelAsync(), FacebookPixelDefaults.PAGE_VIEW));
    }

    /// <summary>
    /// Send search events
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendSearchEventAsync(string searchTerm)
    {
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareSearchModelAsync(searchTerm), FacebookPixelDefaults.SEARCH));
    }

    /// <summary>
    /// Send contact events
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendContactEventAsync()
    {
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareContactModelAsync(), FacebookPixelDefaults.CONTACT));
    }

    /// <summary>
    /// Send complete registration events
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendCompleteRegistrationEventAsync()
    {
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareCompleteRegistrationModelAsync(), FacebookPixelDefaults.COMPLETE_REGISTRATION));
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Get configurations
    /// </summary>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of configurations
    /// </returns>
    public async Task<IPagedList<FacebookPixelConfiguration>> GetPagedConfigurationsAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _facebookPixelConfigurationRepository.Table;

        //filter by the store
        if (storeId > 0)
            query = query.Where(configuration => configuration.StoreId == storeId);

        query = query.OrderBy(configuration => configuration.Id);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Get a configuration by the identifier
    /// </summary>
    /// <param name="configurationId">Configuration identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the configuration
    /// </returns>
    public async Task<FacebookPixelConfiguration> GetConfigurationByIdAsync(int configurationId)
    {
        if (configurationId == 0)
            return null;

        return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(FacebookPixelDefaults.ConfigurationCacheKey, configurationId), async () =>
            await _facebookPixelConfigurationRepository.GetByIdAsync(configurationId));
    }

    /// <summary>
    /// Insert the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertConfigurationAsync(FacebookPixelConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        await _facebookPixelConfigurationRepository.InsertAsync(configuration, false);
        await _staticCacheManager.RemoveByPrefixAsync(FacebookPixelDefaults.PrefixCacheKey);
    }

    /// <summary>
    /// Update the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateConfigurationAsync(FacebookPixelConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        await _facebookPixelConfigurationRepository.UpdateAsync(configuration, false);
        await _staticCacheManager.RemoveByPrefixAsync(FacebookPixelDefaults.PrefixCacheKey);
    }

    /// <summary>
    /// Delete the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteConfigurationAsync(FacebookPixelConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        await _facebookPixelConfigurationRepository.DeleteAsync(configuration, false);
        await _staticCacheManager.RemoveByPrefixAsync(FacebookPixelDefaults.PrefixCacheKey);
    }

    /// <summary>
    /// Get configuration custom events
    /// </summary>
    /// <param name="configurationId">Configuration identifier</param>
    /// <param name="widgetZone">Widget zone name; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of custom events
    /// </returns>
    public async Task<IList<CustomEvent>> GetCustomEventsAsync(int configurationId, string widgetZone = null)
    {
        var cachedCustomEvents = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(FacebookPixelDefaults.CustomEventsCacheKey, configurationId), async () =>
        {
            //load configuration custom events
            var configuration = await GetConfigurationByIdAsync(configurationId);
            var customEventsValue = configuration?.CustomEvents ?? string.Empty;
            var customEvents = JsonConvert.DeserializeObject<List<CustomEvent>>(customEventsValue) ?? new List<CustomEvent>();
            return customEvents;
        });

        //filter by the widget zone
        if (!string.IsNullOrEmpty(widgetZone))
            cachedCustomEvents = cachedCustomEvents.Where(customEvent => customEvent.WidgetZones?.Contains(widgetZone) ?? false).ToList();

        return cachedCustomEvents;
    }

    /// <summary>
    /// Save configuration custom events
    /// </summary>
    /// <param name="configurationId">Configuration identifier</param>
    /// <param name="eventName">Event name</param>
    /// <param name="widgetZones">Widget zones names</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SaveCustomEventsAsync(int configurationId, string eventName, IList<string> widgetZones)
    {
        if (string.IsNullOrEmpty(eventName))
            return;

        var configuration = await GetConfigurationByIdAsync(configurationId);
        if (configuration == null)
            return;

        //load configuration custom events
        var customEventsValue = configuration.CustomEvents ?? string.Empty;
        var customEvents = JsonConvert.DeserializeObject<List<CustomEvent>>(customEventsValue) ?? new List<CustomEvent>();

        //try to get an event by the passed name
        var customEvent = customEvents
            .FirstOrDefault(customEvent => eventName.Equals(customEvent.EventName, StringComparison.InvariantCultureIgnoreCase));
        if (customEvent == null)
        {
            //create new one if not exist
            customEvent = new CustomEvent { EventName = eventName };
            customEvents.Add(customEvent);
        }

        //update widget zones of this event
        customEvent.WidgetZones = widgetZones ?? new List<string>();

        //or delete an event
        if (!customEvent.WidgetZones.Any())
            customEvents.Remove(customEvent);

        //update configuration 
        configuration.CustomEvents = JsonConvert.SerializeObject(customEvents);
        await UpdateConfigurationAsync(configuration);
        await _staticCacheManager.RemoveByPrefixAsync(FacebookPixelDefaults.PrefixCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(WidgetModelDefaults.WidgetPrefixCacheKey);
    }

    /// <summary>
    /// Get used widget zones for all custom events
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of widget zones names
    /// </returns>
    public async Task<IList<string>> GetCustomEventsWidgetZonesAsync()
    {
        return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(FacebookPixelDefaults.WidgetZonesCacheKey), async () =>
        {
            //load custom events and their widget zones
            var configurations = await GetConfigurationsAsync();
            var customEvents = await configurations.SelectManyAwait(async configuration => await GetCustomEventsAsync(configuration.Id)).ToListAsync();
            var widgetZones = await customEvents.SelectMany(customEvent => customEvent.WidgetZones).Distinct().ToListAsync();

            return widgetZones;
        });
    }

    #endregion

    #endregion
}