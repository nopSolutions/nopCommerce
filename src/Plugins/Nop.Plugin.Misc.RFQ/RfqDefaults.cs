namespace Nop.Plugin.Misc.RFQ;

/// <summary>
/// Represents plugin constants
/// </summary>
public class RfqDefaults
{

    public const string CUSTOMER_SENT_NEW_REQUEST_QUOTE = "RequestQuote.CustomerSentNewRequest.StoreOwnerNotification";
    public const string ADMIN_SENT_NEW_QUOTE = "Quote.AdminSentNewQuote.CustomerNotification";

    /// <summary>
    /// Gets an algorithm used to create the hash value
    /// </summary>
    public const string HASH_ALGORITHM = "SHA1";

    /// <summary>
    /// Gets an quantity form key prefix
    /// </summary>
    public const string QUANTITY_FORM_KEY = "quantity_";

    /// <summary>
    /// Gets an unit price form key prefix
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
    public static string CustomerRequestRouteName => "Nop.Plugin.Misc.RFQ.CustomerRequest";

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