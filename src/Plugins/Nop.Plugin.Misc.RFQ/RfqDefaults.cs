using Nop.Core.Caching;

namespace Nop.Plugin.Misc.RFQ;

/// <summary>
/// Represents plugin constants
/// </summary>
public class RfqDefaults
{
    #region Caching defaults

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : shopping cart item ID
    /// </remarks>
    public static CacheKey QuoteItemByShoppingCartItemCacheKey => new("Misc.RFQ.QuoteItemByShoppingCartItem.{0}");

    #endregion

    /// <summary>
    /// Gets a system name of store owner notification about new request quote
    /// </summary>
    public const string CUSTOMER_SENT_NEW_REQUEST_QUOTE = "Misc.RFQ.CustomerSentNewRequest.StoreOwnerNotification";

    /// <summary>
    /// Gets a system name of customer notification about new quote
    /// </summary>
    public const string ADMIN_SENT_NEW_QUOTE = "Misc.RFQ.AdminSentNewQuote.CustomerNotification";

    /// <summary>
    /// Gets a quantity form key prefix
    /// </summary>
    public const string QUANTITY_FORM_KEY = "quantity_";

    /// <summary>
    /// Gets a unit price form key prefix
    /// </summary>
    public const string UNIT_PRICE_FORM_KEY = "unit_price_";

    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Misc.RFQ";

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Nop.Plugin.Misc.RFQ.Configure";

    /// <summary>
    /// Gets the customer request route name
    /// </summary>
    public static string CreateCustomerRequestRouteName => "Nop.Plugin.Misc.RFQ.CreateCustomerRequest";

    /// <summary>
    /// Gets the customer request route name
    /// </summary>
    public static string ClearCustomerRequestRouteName => "Nop.Plugin.Misc.RFQ.ClearCustomerRequest";

    /// <summary>
    /// Gets the customer requests route name
    /// </summary>
    public static string CustomerRequestsRouteName => "Nop.Plugin.Misc.RFQ.CustomerRequests";

    /// <summary>
    /// Gets the customer quotes route name
    /// </summary>
    public static string CustomerQuotesRouteName => "Nop.Plugin.Misc.RFQ.CustomerQuotes";

    /// <summary>
    /// Gets the customer quote route name
    /// </summary>
    public static string CustomerQuoteRouteName => "Nop.Plugin.Misc.RFQ.CustomerQuote";

    /// <summary>
    /// Gets the requests administration menu system name
    /// </summary>
    public static string RequestsAdminMenuSystemName => "Nop.Plugin.Misc.RFQ.RequestsAdminMenu";

    /// <summary>
    /// Gets the quotes administration menu system name
    /// </summary>
    public static string QuotesAdminMenuSystemName => "Nop.Plugin.Misc.RFQ.QuotesAdminMenu";

    /// <summary>
    /// Requests tab identifier for customer navigation
    /// </summary>
    public static int RequestsTabId => 101;

    /// <summary>
    /// Quotes tab identifier for customer navigation
    /// </summary>
    public static int QuotesTabId => 102;

    /// <summary>
    /// Gets the date-time string format
    /// </summary>
    public static string DateTimeStringFormat => "yyyy-MM-dd HH:mm:ss";
}