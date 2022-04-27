using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.FacebookPixel.Services
{
    /// <summary>
    /// Represents Facebook Pixel service
    /// </summary>
    public class FacebookPixelService
    {
        #region Constants

        /// <summary>
        /// Get default tabs number to format scripts indentation
        /// </summary>
        private const int TABS_NUMBER = 2;

        #endregion

        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductService _productService;
        private readonly IRepository<FacebookPixelConfiguration> _facebookPixelConfigurationRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public FacebookPixelService(CurrencySettings currencySettings,
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
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            _currencySettings = currencySettings;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        private async Task<TResult> HandleFunctionAsync<TResult>(Func<Task<TResult>> function)
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
                //get a short error message
                var detailedException = exception;
                do
                {
                    detailedException = detailedException.InnerException;
                } while (detailedException?.InnerException != null);


                //log errors
                var error = $"{FacebookPixelDefaults.SystemName} error: {Environment.NewLine}{exception.Message}";
                await _logger.ErrorAsync(error, exception, await _workContext.GetCurrentCustomerAsync());

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
        private async Task<bool> PluginActiveAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            return await _widgetPluginManager
                .IsPluginActiveAsync(FacebookPixelDefaults.SystemName, await _workContext.GetCurrentCustomerAsync(), store.Id);
        }

        /// <summary>
        /// Prepare scripts
        /// </summary>
        /// <param name="configurations">Enabled configurations</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the script code
        /// </returns>
        private async Task<string> PrepareScriptsAsync(IList<FacebookPixelConfiguration> configurations)
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
        private async Task<string> GetUserObjectAsync()
        {
            //prepare user object
            var customer = await _workContext.GetCurrentCustomerAsync();
            var email = customer.Email;
            var firstName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            var lastName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute);
            var phone = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
            var gender = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.GenderAttribute);
            var birthday = await _genericAttributeService.GetAttributeAsync<DateTime?>(customer, NopCustomerDefaults.DateOfBirthAttribute);
            var city = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CityAttribute);
            var countryId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.CountryIdAttribute);
            var countryName = (await _countryService.GetCountryByIdAsync(countryId))?.TwoLetterIsoCode;
            var stateId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute);
            var stateName = (await _stateProvinceService.GetStateProvinceByIdAsync(stateId))?.Abbreviation;
            var zipcode = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);

            var userObject = FormatEventObject(new List<(string Name, object Value)>
                {
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
                });

            return userObject;
        }

        /// <summary>
        /// Prepare script to init Facebook Pixel
        /// </summary>
        /// <param name="configurations">Enabled configurations</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the script code
        /// </returns>
        private async Task<string> PrepareInitScriptAsync(IList<FacebookPixelConfiguration> configurations)
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
        private async Task<string> PrepareUserPropertiesScriptAsync(IList<FacebookPixelConfiguration> configurations)
        {
            //filter active configurations
            var activeConfigurations = configurations.Where(configuration => configuration.PassUserProperties).ToList();
            if (!activeConfigurations.Any())
                return string.Empty;

            //prepare user object
            var customer = await _workContext.GetCurrentCustomerAsync();
            var createdOn = new DateTimeOffset(customer.CreatedOnUtc).ToUnixTimeSeconds().ToString();
            var city = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CityAttribute);
            var countryId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.CountryIdAttribute);
            var countryName = (await _countryService.GetCountryByIdAsync(countryId))?.TwoLetterIsoCode;
            var currency = (await _workContext.GetWorkingCurrencyAsync())?.CurrencyCode;
            var gender = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.GenderAttribute);
            var language = (await _workContext.GetWorkingLanguageAsync())?.UniqueSeoCode;
            var stateId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute);
            var stateName = (await _stateProvinceService.GetStateProvinceByIdAsync(stateId))?.Abbreviation;
            var zipcode = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);

            var userObject = FormatEventObject(new List<(string Name, object Value)>
            {
                ("$account_created_time", createdOn),
                ("$city", JavaScriptEncoder.Default.Encode(city ?? string.Empty)),
                ("$country", countryName),
                ("$currency", currency),
                ("$gender", gender?.FirstOrDefault().ToString()),
                ("$language", language),
                ("$state", stateName),
                ("$zipcode", JavaScriptEncoder.Default.Encode(zipcode ?? string.Empty))
            });

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
        private async Task<string> PreparePageViewScriptAsync(IList<FacebookPixelConfiguration> configurations)
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
        private async Task<string> PrepareTrackedEventsScriptAsync(IList<FacebookPixelConfiguration> configurations)
        {
            //get previously stored events and remove them from the session data
            var events = _httpContextAccessor.HttpContext.Session
                .Get<IList<TrackedEvent>>(FacebookPixelDefaults.TrackedEventsSessionValue) ?? new List<TrackedEvent>();
            var store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var activeEvents = events.Where(trackedEvent =>
                trackedEvent.CustomerId == customer.Id && trackedEvent.StoreId == store.Id)
                .ToList();
            _httpContextAccessor.HttpContext.Session.Set(FacebookPixelDefaults.TrackedEventsSessionValue, events.Except(activeEvents).ToList());

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
        private async Task PrepareTrackedEventScriptAsync(string eventName, string eventObject,
            int? customerId = null, int? storeId = null, bool isCustomEvent = false)
        {
            //prepare script and store it into the session data, we use this later
            var customer = await _workContext.GetCurrentCustomerAsync();
            customerId ??= customer.Id;
            var store = await _storeContext.GetCurrentStoreAsync();
            storeId ??= store.Id;
            var events = _httpContextAccessor.HttpContext.Session
                .Get<IList<TrackedEvent>>(FacebookPixelDefaults.TrackedEventsSessionValue) ?? new List<TrackedEvent>();
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
            _httpContextAccessor.HttpContext.Session.Set(FacebookPixelDefaults.TrackedEventsSessionValue, events);
        }

        /// <summary>
        /// Format event object to look pretty
        /// </summary>
        /// <param name="properties">Event object properties</param>
        /// <param name="tabsNumber">Tabs number for indentation script</param>
        /// <returns>Script code</returns>
        private string FormatEventObject(IList<(string Name, object Value)> properties, int? tabsNumber = null)
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
        private async Task<string> FormatScriptAsync(IList<FacebookPixelConfiguration> configurations, Func<FacebookPixelConfiguration, Task<string>> getScript)
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
        /// Get configurations
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of configurations
        /// </returns>
        private async Task<IList<FacebookPixelConfiguration>> GetConfigurationsAsync(int storeId = 0)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(FacebookPixelDefaults.ConfigurationsCacheKey, storeId);

            var query = _facebookPixelConfigurationRepository.Table;

            //filter by the store
            if (storeId > 0)
                query = query.Where(configuration => configuration.StoreId == storeId);

            query = query.OrderBy(configuration => configuration.Id);

            return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
        }

        #endregion

        #region Methods

        #region Scripts

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
                    if (!configuration.Enabled)
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
    <!-- End Facebook Pixel Code -->
    ";
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

        /// <summary>
        /// Prepare script to track "AddToCart" and "AddToWishlist" events
        /// </summary>
        /// <param name="item">Shopping cart item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PrepareAddToCartScriptAsync(ShoppingCartItem item)
        {
            await HandleFunctionAsync(async () =>
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                //check whether the adding was initiated by the customer
                if (item.CustomerId != customer.Id)
                    return false;

                //prepare event object
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(product?.Id ?? 0)).FirstOrDefault();
                var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
                var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
                var quantity = product != null ? (int?)item.Quantity : null;
                var (productPrice, _, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, includeDiscounts: false);
                var (price, _) = await _taxService.GetProductPriceAsync(product, productPrice);
                var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                var priceValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price, currentCurrency);
                var currency = currentCurrency?.CurrencyCode;

                var contentsProperties = new List<(string Name, object Value)> { ("id", sku), ("quantity", quantity) };
                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("content_category", categoryName),
                    ("content_name", product?.Name),
                    ("content_type", "product"),
                    ("contents", new[] { contentsProperties }.ToList()),
                    ("currency", currency),
                    ("value", priceValue)
                });

                //prepare event script
                var eventName = item.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart
                    ? FacebookPixelDefaults.ADD_TO_CART
                    : FacebookPixelDefaults.ADD_TO_WISHLIST;
                await PrepareTrackedEventScriptAsync(eventName, eventObject, item.CustomerId, item.StoreId);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "Purchase" event
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PreparePurchaseScriptAsync(Order order)
        {
            await HandleFunctionAsync(async () =>
            {
                //check whether the purchase was initiated by the customer
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (order.CustomerId != customer.Id)
                    return false;

                //prepare event object
                var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
                var contentsProperties = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async item =>
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
                    var quantity = product != null ? (int?)item.Quantity : null;
                    return new List<(string Name, object Value)> { ("id", sku), ("quantity", quantity) };
                }).ToListAsync();
                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("content_type", "product"),
                    ("contents", contentsProperties),
                    ("currency", currency?.CurrencyCode),
                    ("value", order.OrderTotal)
                });

                //prepare event script
                await PrepareTrackedEventScriptAsync(FacebookPixelDefaults.PURCHASE, eventObject, order.CustomerId, order.StoreId);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "ViewContent" event
        /// </summary>
        /// <param name="model">Product details model</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PrepareViewContentScriptAsync(ProductDetailsModel model)
        {
            await HandleFunctionAsync(async () =>
            {
                //prepare event object
                var product = await _productService.GetProductByIdAsync(model.Id);
                var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(product?.Id ?? 0)).FirstOrDefault();
                var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
                var sku = model.Sku;
                var priceValue = model.ProductPrice.PriceValue;
                var currency = (await _workContext.GetWorkingCurrencyAsync())?.CurrencyCode;

                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("content_category", categoryName),
                    ("content_ids", sku),
                    ("content_name", product?.Name),
                    ("content_type", "product"),
                    ("currency", currency),
                    ("value", priceValue)
                });

                //prepare event script
                await PrepareTrackedEventScriptAsync(FacebookPixelDefaults.VIEW_CONTENT, eventObject);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "InitiateCheckout" event
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PrepareInitiateCheckoutScriptAsync()
        {
            await HandleFunctionAsync(async () =>
            {
                //prepare event object
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService
                    .GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
                var (price, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false, false);
                var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                var priceValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price ?? 0, currentCurrency);
                var currency = currentCurrency?.CurrencyCode;

                var contentsProperties = await cart.SelectAwait(async item =>
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
                    var quantity = product != null ? (int?)item.Quantity : null;
                    return new List<(string Name, object Value)> { ("id", sku), ("quantity", quantity) };
                }).ToListAsync();
                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("content_type", "product"),
                    ("contents", contentsProperties),
                    ("currency", currency),
                    ("value", priceValue)
                });

                //prepare event script
                await PrepareTrackedEventScriptAsync(FacebookPixelDefaults.INITIATE_CHECKOUT, eventObject);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "Search" event
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PrepareSearchScriptAsync(string searchTerm)
        {
            await HandleFunctionAsync(async () =>
            {
                //prepare event object
                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("search_string", JavaScriptEncoder.Default.Encode(searchTerm ?? string.Empty))
                });

                //prepare event script
                await PrepareTrackedEventScriptAsync(FacebookPixelDefaults.SEARCH, eventObject);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "Contact" event
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PrepareContactScriptAsync()
        {
            await HandleFunctionAsync(async () =>
            {
                //prepare event script
                await PrepareTrackedEventScriptAsync(FacebookPixelDefaults.CONTACT, string.Empty);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "CompleteRegistration" event
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PrepareCompleteRegistrationScriptAsync()
        {
            await HandleFunctionAsync(async () =>
            {
                //prepare event object
                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("status", true)
                });

                //prepare event script
                await PrepareTrackedEventScriptAsync(FacebookPixelDefaults.COMPLETE_REGISTRATION, eventObject);

                return true;
            });
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
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

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
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

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
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

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
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.WidgetPrefixCacheKey);
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
}