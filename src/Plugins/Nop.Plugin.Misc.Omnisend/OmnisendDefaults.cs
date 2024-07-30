using Nop.Core;

namespace Nop.Plugin.Misc.Omnisend;

/// <summary>
/// Represents plugin constants
/// </summary>
public class OmnisendDefaults
{
    #region Tokens

    public static string ProductId => "{ProductId}";

    /// <summary>
    /// Gets a token to place brand identifier in the tracking script
    /// </summary>
    public static string BrandId => "{BrandId}";

    public static string Sku => "{SKU}";

    public static string Currency => "{Currency}";

    public static string Price => "{Price}";

    public static string Title => "{Title}";

    public static string ImageUrl => "{ImageUrl}";

    public static string ProductUrl => "{ProductUrl}";

    public static string Email => "{Email}";

    #endregion

    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Misc.Omnisend";

    /// <summary>
    /// Gets an identify contact attribute name
    /// </summary>
    public static string IdentifyContactAttribute => "Omnisend.IdentifyContact";

    /// <summary>
    /// Gets a customer email attribute name
    /// </summary>
    public static string CustomerEmailAttribute => "Omnisend.CustomerEmail";

    /// <summary>
    /// Gets a customer shopping cart id attribute name
    /// </summary>
    public static string CurrentCustomerShoppingCartIdAttribute => "Omnisend.ShoppingCartId";

    /// <summary>
    /// Gets a customer shopping cart id attribute name
    /// </summary>
    public static string StoredCustomerShoppingCartIdAttribute => "Omnisend.ShoppingCartId.Stored";

    /// <summary>
    /// Gets a user agent used to request third-party services
    /// </summary>
    public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

    /// <summary>
    /// Gets a header of the API key authorization: key
    /// </summary>
    public static string ApiKeyHeader => "X-API-KEY";

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.Misc.Omnisend.Configure";

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string AbandonedCheckoutRouteName => "Plugin.Misc.Omnisend.AbandonedCheckout";

    /// <summary>
    /// Gets a period (in seconds) before the request times out
    /// </summary>
    public static int RequestTimeout => 30;
        
    /// <summary>
    /// Gets the integration origin
    /// </summary>
    public static string IntegrationOrigin => "nopCommerce";
        
    /// <summary>
    /// Version of the integration
    /// </summary>
    public static string IntegrationVersion => "1.0";

    /// <summary>
    /// Default contact tags
    /// </summary>
    public static List<string> ContactTags => new() {$"source: nopCommerce {NopVersion.FULL_VERSION}" };

    /// <summary>
    /// ContactId query parameter name
    /// </summary>
    public static string ContactIdQueryParamName => "omnisendContactID";

    /// <summary>
    /// Gets an OrderCanceled attribute name
    /// </summary>
    public static string OrderCanceledAttribute => "Omnisend.OrderCanceledEvent.IsSent";

    /// <summary>
    /// Gets an OrderFulfilled attribute name
    /// </summary>
    public static string OrderFulfilledAttribute => "Omnisend.OrderFulfilledEvent.IsSent";

    #region Batch

    /// <summary>
    /// Minimal count of entities to use batch functionality
    /// </summary>
    public static int MinCountToUseBatch => 1;

    /// <summary>
    /// The batch page size
    ///
    /// The max size is 1000
    /// </summary>
    public static int PageSize => 800;

    /// <summary>
    /// Batch finished status
    /// </summary>
    public static string BatchFinishedStatus => "finished";

    #endregion
        
    #region Endpoints and URLs

    /// <summary>
    /// Gets a base API URL
    /// </summary>
    public static string BaseApiUrl => "https://api.omnisend.com/v3";

    /// <summary>
    /// Get a batches API URL
    /// </summary>
    public static string BatchesApiUrl => BaseApiUrl + "/batches";

    /// <summary>
    /// Get an accounts API URL
    /// </summary>
    public static string AccountsApiUrl => BaseApiUrl + "/accounts";

    /// <summary>
    /// Get a contacts endpoint
    /// </summary>
    public static string ContactsEndpoint => "contacts";

    /// <summary>
    /// Get a contacts API URL
    /// </summary>
    public static string ContactsApiUrl => BaseApiUrl + $"/{ContactsEndpoint}";
        
    /// <summary>
    /// Get an orders endpoint
    /// </summary>
    public static string OrdersEndpoint => "orders";

    /// <summary>
    /// Get an orders API URL
    /// </summary>
    public static string OrdersApiUrl => BaseApiUrl + $"/{OrdersEndpoint}";

    /// <summary>
    /// Get a carts endpoint
    /// </summary>
    public static string CartsEndpoint => "carts";

    /// <summary>
    /// Get a carts API URL
    /// </summary>
    public static string CartsApiUrl => BaseApiUrl + $"/{CartsEndpoint}";

    /// <summary>
    /// Get a products endpoint
    /// </summary>
    public static string ProductsEndpoint => "products";

    /// <summary>
    /// Get a products API URL
    /// </summary>
    public static string ProductsApiUrl => BaseApiUrl + $"/{ProductsEndpoint}";

    /// <summary>
    /// Get a categories endpoint
    /// </summary>
    public static string CategoriesEndpoint => "categories";

    /// <summary>
    /// Get a categories API URL
    /// </summary>
    public static string CategoriesApiUrl => BaseApiUrl + $"/{CategoriesEndpoint}";

    /// <summary>
    /// Get a customer-events endpoint
    /// </summary>
    public static string CustomerEventsEndpoint => "customer-events";

    /// <summary>
    /// Get a customer-events API URL
    /// </summary>
    public static string CustomerEventsApiUrl => BaseApiUrl + $"/{CustomerEventsEndpoint}";

    #endregion
}