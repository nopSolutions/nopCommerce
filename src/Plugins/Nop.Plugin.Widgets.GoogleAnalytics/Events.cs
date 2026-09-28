using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Widgets.GoogleAnalytics.Api;
using Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;
using Newtonsoft.Json.Linq;
using Nop.Web.Framework.Events;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.GoogleAnalytics;

public class EventConsumer :
    IConsumer<OrderPlacedEvent>,
    IConsumer<OrderPaidEvent>,
    IConsumer<OrderRefundedEvent>,
    IConsumer<PageRenderingEvent>
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly GoogleAnalyticsHttpClient _googleAnalyticsHttpClient;
    protected readonly ICategoryService _categoryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IWidgetPluginManager _widgetPluginManager;

    #endregion

    #region Ctor

    public EventConsumer(CurrencySettings currencySettings,
        GoogleAnalyticsHttpClient googleAnalyticsHttpClient,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        IOrderService orderService,
        IProductService productService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService,
        IWidgetPluginManager widgetPluginManager)
    {
        _currencySettings = currencySettings;
        _googleAnalyticsHttpClient = googleAnalyticsHttpClient;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _orderService = orderService;
        _productService = productService;
        _settingService = settingService;
        _storeContext = storeContext;
        _storeService = storeService;
        _widgetPluginManager = widgetPluginManager;
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task<bool> IsPluginEnabledAsync()
    {
        return await _widgetPluginManager.IsPluginActiveAsync(GoogleAnalyticsDefaults.SystemName);
    }

    /// <summary>
    /// Extracts the numeric session ID from the GA4 session cookie value.
    /// Cookie format: GS2.1.s{sessionId}$o{...}$g{...}$t{...}$j{...}$l{...}$h{...}
    /// </summary>
    private string ExtractSessionId(string sessionCookieValue)
    {
        if (string.IsNullOrEmpty(sessionCookieValue))
            return string.Empty;

        try
        {
            // Cookie format: GS2.1.s1772611011$o198$g0$...
            // Split by '$' to get segments, first segment contains session ID
            var segments = sessionCookieValue.Split('$');

            // First segment is like "GS2.1.s1772611011"
            // Find the 's' prefix and extract the number after it
            var firstSegment = segments[0]; // "GS2.1.s1772611011"
            var sIndex = firstSegment.LastIndexOf('s');

            if (sIndex >= 0 && sIndex < firstSegment.Length - 1)
            {
                var extracted = firstSegment[(sIndex + 1)..];
                // Verify it's a valid number
                if (long.TryParse(extracted, out _))
                    return extracted;
            }
        }
        catch
        {
            // Fall through to return empty
        }

        return string.Empty;
    }

    private string ExtractClientId(string gaCookieValue)
    {
        if (string.IsNullOrEmpty(gaCookieValue))
            return Guid.NewGuid().ToString(); // fallback

        // Cookie format: GA1.1.1511006404.1753253791
        // client_id = "1511006404.1753253791" (everything after second dot)
        var parts = gaCookieValue.Split('.');
        if (parts.Length >= 4)
            return $"{parts[2]}.{parts[3]}";

        return gaCookieValue; // return as-is if unexpected format
    }

    // Helper to safely read from session with default fallback
    private string ReadSession(ISession session, string key, string defaultValue)
    {
        try
        {
            var value = session.GetString(key);
            // ✅ Guard against null — never send null to GA4
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
        catch
        {
            return defaultValue;
        }
    }

    private (string source, string medium) DetectSourceMedium(string referrer)
    {
        if (string.IsNullOrEmpty(referrer))
            return ("(direct)", "(none)");

        try
        {
            var uri = new Uri(referrer);
            var host = uri.Host.ToLower().Replace("www.", "");

            var organicEngines = new Dictionary<string, string>
            {
                { "google",     "google"     },
                { "bing",       "bing"       },
                { "yahoo",      "yahoo"      },
                { "duckduckgo", "duckduckgo" },
                { "yandex",     "yandex"     },
                { "baidu",      "baidu"      }
            };

            foreach (var engine in organicEngines)
                if (host.Contains(engine.Key))
                    return (engine.Value, "organic");

            var socialNetworks = new Dictionary<string, string>
            {
                { "facebook",  "facebook"  },
                { "instagram", "instagram" },
                { "twitter",   "twitter"   },
                { "linkedin",  "linkedin"  },
                { "pinterest", "pinterest" },
                { "tiktok",    "tiktok"    }
            };

            foreach (var social in socialNetworks)
                if (host.Contains(social.Key))
                    return (social.Value, "social");

            return (host, "referral");
        }
        catch
        {
            return ("(direct)", "(none)");
        }
    }

    #endregion

    #region Methods

    protected async Task SaveCookiesAsync(Order order, GoogleAnalyticsSettings googleAnalyticsSettings, Store store)
    {
        //try to get cookie
        var httpContext = _httpContextAccessor.HttpContext;

        //client_id
        httpContext.Request.Cookies.TryGetValue(GoogleAnalyticsDefaults.ClientIdCookiesName, out var rawClientId);
        var clientId = ExtractClientId(rawClientId); // "1511006404.1753253791"
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.ClientIdAttribute, clientId, store.Id);

        //session_id
        var measurementId = googleAnalyticsSettings.GoogleId.Split('-')[1];
        var sessionCookieKey = $"{GoogleAnalyticsDefaults.SessionIdCookiesName}{measurementId}";
        httpContext.Request.Cookies.TryGetValue(sessionCookieKey, out var rawSessionId);
        var sessionId = ExtractSessionId(rawSessionId); // "1772611011"
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.SessionIdAttribute, sessionId, store.Id);

        var session = httpContext.Session;
        var utmSource = ReadSession(session, GoogleAnalyticsDefaults.UtmSourceKey, "(direct)");
        var utmMedium = ReadSession(session, GoogleAnalyticsDefaults.UtmMediumKey, "(none)");
        var utmCampaign = ReadSession(session, GoogleAnalyticsDefaults.UtmCampaignKey, "(not set)");
        var utmTerm = ReadSession(session, GoogleAnalyticsDefaults.UtmTermKey, "(not set)");
        var utmContent = ReadSession(session, GoogleAnalyticsDefaults.UtmContentKey, "(not set)");

        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.UtmSourceKey, utmSource, store.Id);
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.UtmMediumKey, utmMedium, store.Id);
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.UtmCampaignKey, utmCampaign, store.Id);
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.UtmTermKey, utmTerm, store.Id);
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.UtmContentKey, utmContent, store.Id);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ProcessOrderEventAsync(Order order, GoogleAnalyticsSettings googleAnalyticsSettings, string eventName)
    {
        try
        {
            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
            var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            var orderId = order.CustomOrderNumber;
            var orderShipping = googleAnalyticsSettings.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax;
            var orderTax = order.OrderTax;
            var orderTotal = order.OrderTotal;

            var gaRequest = new EventRequest
            {
                ClientId = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.ClientIdAttribute, store.Id),
                UserId = order.CustomerId.ToString(),
                TimestampMicros = (DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).Ticks / 10
            };

            var events = new List<Event>();

            var utmSource = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.UtmSourceKey, store.Id);
            var utmMedium = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.UtmMediumKey, store.Id);
            var utmCampaign = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.UtmCampaignKey, store.Id);
            var utmTerm = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.UtmTermKey, store.Id);
            var utmContent = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.UtmContentKey, store.Id);
            var sessionId = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.SessionIdAttribute, store.Id);

            // Create campaign_details event with campaign parameters
            var campaignParams = new JObject
            {
                ["campaign_id"] = "",
                ["campaign"] = utmCampaign,
                ["source"] = utmSource,
                ["medium"] = utmMedium,
                ["term"] = utmTerm,
                ["content"] = utmContent,
                ["session_id"] = sessionId,
                ["engagement_time_msec"] = 100
            };
            var campaignEvent = new Event
            {
                Name = "campaign_details",
                Params = campaignParams
            };
            events.Add(campaignEvent);

            // Create purchase/refund event with ecommerce parameters
            var gaParams = new JObject
            {
                ["currency"] = currency,
                ["transaction_id"] = orderId,
                ["value"] = orderTotal,
                ["tax"] = orderTax,
                ["shipping"] = orderShipping,
                ["session_id"] = sessionId,
                ["engagement_time_msec"] = 100
            };

            var items = new JArray();
            foreach (var item in await _orderService.GetOrderItemsAsync(order.Id))
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                var sku = await _productService.FormatSkuAsync(product, item.AttributesXml);
                if (string.IsNullOrEmpty(sku))
                    sku = product.Id.ToString();
                //get category
                var category = (await _categoryService.GetCategoryByIdAsync((await _categoryService.GetProductCategoriesByProductIdAsync(product.Id)).FirstOrDefault()?.CategoryId ?? 0))?.Name;
                if (string.IsNullOrEmpty(category))
                    category = "No category";
                var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;

                var gaItem = new JObject
                {
                    ["item_id"] = sku,
                    ["item_name"] = product.Name,
                    ["affiliation"] = store.Name,
                    ["item_category"] = category,
                    ["price"] = unitPrice,
                    ["quantity"] = item.Quantity
                };

                items.Add(gaItem);
            }
            gaParams["items"] = items;

            var gaEvent = new Event
            {
                Name = eventName,
                Params = gaParams
            };
            events.Add(gaEvent);
            gaRequest.Events = events;

            await _googleAnalyticsHttpClient.RequestAsync(gaRequest, googleAnalyticsSettings);
        }
        catch (Exception ex)
        {
            await _logger.InsertLogAsync(LogLevel.Error, "Google Analytics. Error canceling transaction from server side", ex.ToString());
        }
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var order = eventMessage.Order;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        //ecommerce is disabled
        if (!googleAnalyticsSettings.EnableEcommerce)
            return;

        //if we use HTTP requests to notify GA about new orders (only when they are paid), then we should notify GA about deleted AND paid orders
        var sendRequest = order.PaymentStatus == PaymentStatus.Paid;

        if (sendRequest)
            await ProcessOrderEventAsync(order, googleAnalyticsSettings, GoogleAnalyticsDefaults.OrderRefundedEventName);
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var order = eventMessage.Order;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        //ecommerce is disabled
        if (!googleAnalyticsSettings.EnableEcommerce)
            return;

        //we use HTTP requests to notify GA about new orders (only when they are paid)
        await ProcessOrderEventAsync(order, googleAnalyticsSettings, GoogleAnalyticsDefaults.OrderPaidEventName);
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var order = eventMessage.Order;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        //ecommerce is disabled
        if (!googleAnalyticsSettings.EnableEcommerce)
            return;

        await SaveCookiesAsync(order, googleAnalyticsSettings, store);
    }

    public async Task HandleEventAsync(PageRenderingEvent eventMessage)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Session == null)
            return;

        var query = httpContext.Request.Query;
        var session = httpContext.Session;

        try
        {
            if (query.ContainsKey("utm_source"))
            {
                // ✅ Paid / UTM traffic
                session.SetString(GoogleAnalyticsDefaults.UtmSourceKey, query["utm_source"].ToString() ?? "(direct)");
                session.SetString(GoogleAnalyticsDefaults.UtmMediumKey, query["utm_medium"].ToString() ?? "(none)");
                session.SetString(GoogleAnalyticsDefaults.UtmCampaignKey, query["utm_campaign"].ToString() ?? "(not set)");
                session.SetString(GoogleAnalyticsDefaults.UtmTermKey, query["utm_term"].ToString() ?? "(not set)");
                session.SetString(GoogleAnalyticsDefaults.UtmContentKey, query["utm_content"].ToString() ?? "(not set)");
            }
            else if (!session.Keys.Contains(GoogleAnalyticsDefaults.UtmSourceKey))
            {
                // ✅ Detect organic / referral from Referer header
                var referrer = httpContext.Request.Headers["Referer"].ToString();
                var (source, medium) = DetectSourceMedium(referrer);

                session.SetString(GoogleAnalyticsDefaults.UtmSourceKey, source);
                session.SetString(GoogleAnalyticsDefaults.UtmMediumKey, medium);
                session.SetString(GoogleAnalyticsDefaults.UtmCampaignKey, "(not set)");
                session.SetString(GoogleAnalyticsDefaults.UtmTermKey, "(not set)");
                session.SetString(GoogleAnalyticsDefaults.UtmContentKey, "(not set)");
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Error capturing UTM in PageRenderingEvent", ex);
        }

        return;
    }
    #endregion
}