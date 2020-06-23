using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
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
        private readonly ICacheKeyService _cacheKeyService;
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
            ICacheKeyService cacheKeyService,
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
            _cacheKeyService = cacheKeyService;
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
        /// <returns>Result</returns>
        private TResult HandleFunction<TResult>(Func<TResult> function)
        {
            try
            {
                //check whether the plugin is active
                if (!PluginActive())
                    return default;

                //invoke function
                return function();
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
                _logger.Error(error, exception, _workContext.CurrentCustomer);

                return default;
            }
        }

        /// <summary>
        /// Check whether the plugin is active for the current user and the current store
        /// </summary>
        /// <returns>Result</returns>
        private bool PluginActive()
        {
            return _widgetPluginManager
                .IsPluginActive(FacebookPixelDefaults.SystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Prepare scripts
        /// </summary>
        /// <param name="configurations">Enabled configurations</param>
        /// <returns>Script code</returns>
        private string PrepareScripts(IList<FacebookPixelConfiguration> configurations)
        {
            return PrepareInitScript(configurations) +
                PrepareUserPropertiesScript(configurations) +
                PreparePageViewScript(configurations) +
                PrepareTrackedEventsScript(configurations);
        }

        /// <summary>
        /// Prepare script to init Facebook Pixel
        /// </summary>
        /// <param name="configurations">Enabled configurations</param>
        /// <returns>Script code</returns>
        private string PrepareInitScript(IList<FacebookPixelConfiguration> configurations)
        {
            //local function to prepare user info (used with Advanced Matching feature)
            string getUserObject()
            {
                //prepare user object
                var email = _workContext.CurrentCustomer.Email;
                var firstName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.FirstNameAttribute);
                var lastName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.LastNameAttribute);
                var phone = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.PhoneAttribute);
                var gender = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.GenderAttribute);
                var birthday = _genericAttributeService.GetAttribute<DateTime?>(_workContext.CurrentCustomer, NopCustomerDefaults.DateOfBirthAttribute);
                var city = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.CityAttribute);
                var countryId = _genericAttributeService.GetAttribute<int>(_workContext.CurrentCustomer, NopCustomerDefaults.CountryIdAttribute);
                var countryName = _countryService.GetCountryById(countryId)?.TwoLetterIsoCode;
                var stateId = _genericAttributeService.GetAttribute<int>(_workContext.CurrentCustomer, NopCustomerDefaults.StateProvinceIdAttribute);
                var stateName = _stateProvinceService.GetStateProvinceById(stateId)?.Abbreviation;
                var zipcode = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.ZipPostalCodeAttribute);

                var userObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("em", email?.ToLower()),
                    ("fn", firstName?.ToLower()),
                    ("ln", lastName?.ToLower()),
                    ("ph", new string(phone?.Where(c => char.IsDigit(c)).ToArray()) ?? string.Empty),
                    ("external_id", _workContext.CurrentCustomer.CustomerGuid.ToString().ToLower()),
                    ("ge", gender?.FirstOrDefault().ToString().ToLower()),
                    ("db", birthday?.ToString("yyyyMMdd")),
                    ("ct", city?.ToLower()),
                    ("st", stateName?.ToLower()),
                    ("zp", zipcode?.ToLower()),
                    ("cn", countryName?.ToLower())
                });

                return userObject;
            }

            //prepare init script
            return FormatScript(configurations, configuration =>
            {
                var additionalParameter = configuration.PassUserProperties
                    ? $", {{uid: '{_workContext.CurrentCustomer.CustomerGuid}'}}"
                    : (configuration.UseAdvancedMatching
                    ? $", {getUserObject()}"
                    : null);
                return $"fbq('init', '{configuration.PixelId}'{additionalParameter});";
            });
        }

        /// <summary>
        /// Prepare script to pass user properties
        /// </summary>
        /// <param name="configurations">Enabled configurations</param>
        /// <returns>Script code</returns>
        private string PrepareUserPropertiesScript(IList<FacebookPixelConfiguration> configurations)
        {
            //filter active configurations
            var activeConfigurations = configurations.Where(configuration => configuration.PassUserProperties).ToList();
            if (!activeConfigurations.Any())
                return string.Empty;

            //prepare user object
            var createdOn = new DateTimeOffset(_workContext.CurrentCustomer.CreatedOnUtc).ToUnixTimeSeconds().ToString();
            var city = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.CityAttribute);
            var countryId = _genericAttributeService.GetAttribute<int>(_workContext.CurrentCustomer, NopCustomerDefaults.CountryIdAttribute);
            var countryName = _countryService.GetCountryById(countryId)?.TwoLetterIsoCode;
            var currency = _workContext.WorkingCurrency?.CurrencyCode;
            var gender = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.GenderAttribute);
            var language = _workContext.WorkingLanguage?.UniqueSeoCode;
            var stateId = _genericAttributeService.GetAttribute<int>(_workContext.CurrentCustomer, NopCustomerDefaults.StateProvinceIdAttribute);
            var stateName = _stateProvinceService.GetStateProvinceById(stateId)?.Abbreviation;
            var zipcode = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.ZipPostalCodeAttribute);

            var userObject = FormatEventObject(new List<(string Name, object Value)>
            {
                ("$account_created_time", createdOn),
                ("$city", city),
                ("$country", countryName),
                ("$currency", currency),
                ("$gender", gender?.FirstOrDefault().ToString()),
                ("$language", language),
                ("$state", stateName),
                ("$zipcode", zipcode)
            });

            //prepare script
            return FormatScript(activeConfigurations, configuration =>
                $"fbq('setUserProperties', '{configuration.PixelId}', {userObject});");
        }

        /// <summary>
        /// Prepare script to track "PageView" event
        /// </summary>
        /// <param name="configurations">Enabled configurations</param>
        /// <returns>Script code</returns>
        private string PreparePageViewScript(IList<FacebookPixelConfiguration> configurations)
        {
            //a single active configuration is enough to track PageView event
            var activeConfigurations = configurations.Where(configuration => configuration.TrackPageView).Take(1).ToList();
            return FormatScript(activeConfigurations, configuration =>
                $"fbq('track', '{FacebookPixelDefaults.PAGE_VIEW}');");
        }

        /// <summary>
        /// Prepare scripts to track events
        /// </summary>
        /// <param name="configurations">Enabled configurations</param>
        /// <returns>Script code</returns>
        private string PrepareTrackedEventsScript(IList<FacebookPixelConfiguration> configurations)
        {
            //get previously stored events and remove them from the session data
            var events = _httpContextAccessor.HttpContext.Session
                .Get<IList<TrackedEvent>>(FacebookPixelDefaults.TrackedEventsSessionValue) ?? new List<TrackedEvent>();
            var activeEvents = events.Where(trackedEvent =>
                trackedEvent.CustomerId == _workContext.CurrentCustomer.Id && trackedEvent.StoreId == _storeContext.CurrentStore.Id)
                .ToList();
            _httpContextAccessor.HttpContext.Session.Set(FacebookPixelDefaults.TrackedEventsSessionValue, events.Except(activeEvents).ToList());

            if (!activeEvents.Any())
                return string.Empty;

            return activeEvents.Aggregate(string.Empty, (preparedScripts, trackedEvent) =>
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
                    activeConfigurations = configurations.Where(configuration =>
                        GetCustomEvents(configuration.Id).Any(customEvent => customEvent.EventName == trackedEvent.EventName)).ToList();
                }

                //prepare event scripts
                return preparedScripts + trackedEvent.EventObjects.Aggregate(string.Empty, (preparedEventScripts, eventObject) =>
                {
                    return preparedEventScripts + FormatScript(activeConfigurations, configuration =>
                    {
                        //used for accurate event tracking with multiple Facebook Pixels
                        var actionName = configurations.Count > 1
                            ? (trackedEvent.IsCustomEvent ? "trackSingleCustom" : "trackSingle")
                            : (trackedEvent.IsCustomEvent ? "trackCustom" : "track");
                        var additionalParameter = configurations.Count > 1 ? $", '{configuration.PixelId}'" : null;

                        //prepare event script
                        var eventObjectParameter = !string.IsNullOrEmpty(eventObject) ? $", {eventObject}" : null;
                        return $"fbq('{actionName}'{additionalParameter}, '{trackedEvent.EventName}'{eventObjectParameter});";
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
        private void PrepareTrackedEventScript(string eventName, string eventObject,
            int? customerId = null, int? storeId = null, bool isCustomEvent = false)
        {
            //prepare script and store it into the session data, we use this later
            customerId ??= _workContext.CurrentCustomer.Id;
            storeId ??= _storeContext.CurrentStore.Id;
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
                        : property.Value.ToString().ToLower()));

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
        /// <returns>Script code</returns>
        private string FormatScript(IList<FacebookPixelConfiguration> configurations, Func<FacebookPixelConfiguration, string> getScript)
        {
            if (!configurations.Any())
                return string.Empty;

            //format script
            var formattedScript = configurations.Aggregate(string.Empty, (preparedScripts, configuration) =>
                preparedScripts + Environment.NewLine + new string('\t', TABS_NUMBER) + getScript(configuration));
            formattedScript += Environment.NewLine;

            return formattedScript;
        }

        /// <summary>
        /// Get configurations
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <returns>List of configurations</returns>
        private IList<FacebookPixelConfiguration> GetConfigurations(int storeId = 0)
        {
            var key = _cacheKeyService.PrepareKeyForDefaultCache(FacebookPixelDefaults.ConfigurationsCacheKey, storeId);

            var query = _facebookPixelConfigurationRepository.Table;

            //filter by the store
            if (storeId > 0)
                query = query.Where(configuration => configuration.StoreId == storeId);

            query = query.OrderBy(configuration => configuration.Id);

            return query.ToCachedList(key);
        }

        #endregion

        #region Methods

        #region Scripts

        /// <summary>
        /// Prepare Facebook Pixel script
        /// </summary>
        /// <returns>Script code</returns>
        public string PrepareScript()
        {
            return HandleFunction(() =>
            {
                //get the enabled configurations
                var configurations = GetConfigurations(_storeContext.CurrentStore.Id).Where(configuration =>
                {
                    if (!configuration.Enabled)
                        return false;

                    if (!configuration.DisableForUsersNotAcceptingCookieConsent)
                        return true;

                    //don't display Pixel for users who not accepted Cookie Consent
                    var cookieConsentAccepted = _genericAttributeService.GetAttribute<bool>(_workContext.CurrentCustomer,
                            NopCustomerDefaults.EuCookieLawAcceptedAttribute, _storeContext.CurrentStore.Id);
                    return cookieConsentAccepted;
                }).ToList();
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
        {PrepareScripts(configurations)}
    </script>
    <!-- End Facebook Pixel Code -->
    ";
            });
        }

        /// <summary>
        /// Prepare Facebook Pixel script
        /// </summary>
        /// <param name="widgetZone">Widget zone to place script</param>
        /// <returns>Script code</returns>
        public string PrepareCustomEventsScript(string widgetZone)
        {
            return HandleFunction(() =>
            {
                var customEvents = GetConfigurations().SelectMany(configuration => GetCustomEvents(configuration.Id, widgetZone)).ToList();
                foreach (var customEvent in customEvents)
                {
                    PrepareTrackedEventScript(customEvent.EventName, string.Empty, isCustomEvent: true);
                }

                return string.Empty;
            });
        }

        /// <summary>
        /// Prepare script to track "AddToCart" and "AddToWishlist" events
        /// </summary>
        /// <param name="item">Shopping cart item</param>
        public void PrepareAddToCartScript(ShoppingCartItem item)
        {
            HandleFunction(() =>
            {
                //check whether the adding was initiated by the customer
                if (item.CustomerId != _workContext.CurrentCustomer.Id)
                    return false;

                //prepare event object
                var product = _productService.GetProductById(item.ProductId);
                var categoryMapping = _categoryService.GetProductCategoriesByProductId(product?.Id ?? 0).FirstOrDefault();
                var categoryName = _categoryService.GetCategoryById(categoryMapping?.CategoryId ?? 0)?.Name;
                var sku = product != null ? _productService.FormatSku(product, item.AttributesXml) : string.Empty;
                var quantity = product != null ? (int?)item.Quantity : null;
                var productPrice = _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false);
                var price = _taxService.GetProductPrice(product, productPrice, out _);
                var priceValue = _currencyService.ConvertFromPrimaryStoreCurrency(price, _workContext.WorkingCurrency);
                var currency = _workContext.WorkingCurrency?.CurrencyCode;

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
                PrepareTrackedEventScript(eventName, eventObject, item.CustomerId, item.StoreId);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "Purchase" event
        /// </summary>
        /// <param name="order">Order</param>
        public void PreparePurchaseScript(Order order)
        {
            HandleFunction(() =>
            {
                //check whether the purchase was initiated by the customer
                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                    return false;

                //prepare event object
                var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                var contentsProperties = _orderService.GetOrderItems(order.Id).Select(item =>
                {
                    var product = _productService.GetProductById(item.ProductId);
                    var sku = product != null ? _productService.FormatSku(product, item.AttributesXml) : string.Empty;
                    var quantity = product != null ? (int?)item.Quantity : null;
                    return new List<(string Name, object Value)> { ("id", sku), ("quantity", quantity) };
                }).ToList();
                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("content_type", "product"),
                    ("contents", contentsProperties),
                    ("currency", currency?.CurrencyCode),
                    ("value", order.OrderTotal)
                });

                //prepare event script
                PrepareTrackedEventScript(FacebookPixelDefaults.PURCHASE, eventObject, order.CustomerId, order.StoreId);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "ViewContent" event
        /// </summary>
        /// <param name="model">Product details model</param>
        public void PrepareViewContentScript(ProductDetailsModel model)
        {
            HandleFunction(() =>
            {
                //prepare event object
                var product = _productService.GetProductById(model.Id);
                var categoryMapping = _categoryService.GetProductCategoriesByProductId(product?.Id ?? 0).FirstOrDefault();
                var categoryName = _categoryService.GetCategoryById(categoryMapping?.CategoryId ?? 0)?.Name;
                var sku = model.Sku;
                var priceValue = model.ProductPrice.PriceValue;
                var currency = _workContext.WorkingCurrency?.CurrencyCode;

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
                PrepareTrackedEventScript(FacebookPixelDefaults.VIEW_CONTENT, eventObject);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "InitiateCheckout" event
        /// </summary>
        public void PrepareInitiateCheckoutScript()
        {
            HandleFunction(() =>
            {
                //prepare event object
                var cart = _shoppingCartService
                    .GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);
                var price = _orderTotalCalculationService.GetShoppingCartTotal(cart, false, false);
                var priceValue = _currencyService.ConvertFromPrimaryStoreCurrency(price ?? 0, _workContext.WorkingCurrency);
                var currency = _workContext.WorkingCurrency?.CurrencyCode;

                var contentsProperties = cart.Select(item =>
                {
                    var product = _productService.GetProductById(item.ProductId);
                    var sku = product != null ? _productService.FormatSku(product, item.AttributesXml) : string.Empty;
                    var quantity = product != null ? (int?)item.Quantity : null;
                    return new List<(string Name, object Value)> { ("id", sku), ("quantity", quantity) };
                }).ToList();
                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("content_type", "product"),
                    ("contents", contentsProperties),
                    ("currency", currency),
                    ("value", priceValue)
                });

                //prepare event script
                PrepareTrackedEventScript(FacebookPixelDefaults.INITIATE_CHECKOUT, eventObject);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "Search" event
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        public void PrepareSearchScript(string searchTerm)
        {
            HandleFunction(() =>
            {
                //prepare event object
                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("search_string", searchTerm)
                });

                //prepare event script
                PrepareTrackedEventScript(FacebookPixelDefaults.SEARCH, eventObject);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "Contact" event
        /// </summary>
        public void PrepareContactScript()
        {
            HandleFunction(() =>
            {
                //prepare event script
                PrepareTrackedEventScript(FacebookPixelDefaults.CONTACT, string.Empty);

                return true;
            });
        }

        /// <summary>
        /// Prepare script to track "CompleteRegistration" event
        /// </summary>
        public void PrepareCompleteRegistrationScript()
        {
            HandleFunction(() =>
            {
                //prepare event object
                var eventObject = FormatEventObject(new List<(string Name, object Value)>
                {
                    ("status", true)
                });

                //prepare event script
                PrepareTrackedEventScript(FacebookPixelDefaults.COMPLETE_REGISTRATION, eventObject);

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
        /// <returns>Paged list of configurations</returns>
        public IPagedList<FacebookPixelConfiguration> GetPagedConfigurations(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _facebookPixelConfigurationRepository.Table;

            //filter by the store
            if (storeId > 0)
                query = query.Where(configuration => configuration.StoreId == storeId);

            query = query.OrderBy(configuration => configuration.Id);

            return new PagedList<FacebookPixelConfiguration>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Get a configuration by the identifier
        /// </summary>
        /// <param name="configurationId">Configuration identifier</param>
        /// <returns>Configuration</returns>
        public FacebookPixelConfiguration GetConfigurationById(int configurationId)
        {
            if (configurationId == 0)
                return null;

            return _staticCacheManager.Get(_cacheKeyService.PrepareKeyForDefaultCache(FacebookPixelDefaults.ConfigurationCacheKey, configurationId), () =>
                _facebookPixelConfigurationRepository.GetById(configurationId));
        }

        /// <summary>
        /// Insert the configuration
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public void InsertConfiguration(FacebookPixelConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _facebookPixelConfigurationRepository.Insert(configuration);
            _staticCacheManager.RemoveByPrefix(FacebookPixelDefaults.PrefixCacheKey);
        }

        /// <summary>
        /// Update the configuration
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public void UpdateConfiguration(FacebookPixelConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _facebookPixelConfigurationRepository.Update(configuration);
            _staticCacheManager.RemoveByPrefix(FacebookPixelDefaults.PrefixCacheKey);
        }

        /// <summary>
        /// Delete the configuration
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public void DeleteConfiguration(FacebookPixelConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _facebookPixelConfigurationRepository.Delete(configuration);
            _staticCacheManager.RemoveByPrefix(FacebookPixelDefaults.PrefixCacheKey);
        }

        /// <summary>
        /// Get configuration custom events
        /// </summary>
        /// <param name="configurationId">Configuration identifier</param>
        /// <param name="widgetZone">Widget zone name; pass null to load all records</param>
        /// <returns>List of custom events</returns>
        public IList<CustomEvent> GetCustomEvents(int configurationId, string widgetZone = null)
        {
            var cachedCustomEvents = _staticCacheManager.Get(_cacheKeyService.PrepareKeyForDefaultCache(FacebookPixelDefaults.CustomEventsCacheKey, configurationId), () =>
            {
                //load configuration custom events
                var configuration = GetConfigurationById(configurationId);
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
        public void SaveCustomEvents(int configurationId, string eventName, IList<string> widgetZones)
        {
            if (string.IsNullOrEmpty(eventName))
                return;

            var configuration = GetConfigurationById(configurationId);
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
            UpdateConfiguration(configuration);
            _staticCacheManager.RemoveByPrefix(FacebookPixelDefaults.PrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.WidgetPrefixCacheKey);
        }

        /// <summary>
        /// Get used widget zones for all custom events
        /// </summary>
        /// <returns>List of widget zones names</returns>
        public IList<string> GetCustomEventsWidgetZones()
        {
            return _staticCacheManager.Get(_cacheKeyService.PrepareKeyForDefaultCache(FacebookPixelDefaults.WidgetZonesCacheKey), () =>
            {
                //load custom events and their widget zones
                var configurations = GetConfigurations();
                var customEvents = configurations.SelectMany(configuration => GetCustomEvents(configuration.Id));
                var widgetZones = customEvents.SelectMany(customEvent => customEvent.WidgetZones).Distinct().ToList();

                return widgetZones;
            });
        }

        #endregion

        #endregion
    }
}