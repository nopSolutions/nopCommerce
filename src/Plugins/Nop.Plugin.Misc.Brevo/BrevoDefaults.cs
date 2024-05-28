using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Plugin.Misc.Brevo;

/// <summary>
/// Represents plugin constants
/// </summary>
public static class BrevoDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Misc.Brevo";

    /// <summary>
    /// Gets a plugin partner name
    /// </summary>
    public static string PartnerName => "NOPCOMMERCE";

    /// <summary>
    /// Gets a user agent used to request Brevo services
    /// </summary>
    public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

    /// <summary>
    /// Gets a URL to edit message template on Brevo account
    /// </summary>
    public static string EditMessageTemplateUrl => "https://my.brevo.com/camp/template/{0}/message-setup?tap_a=30591-fb13f0&tap_s=840216-5153c7";

    /// <summary>
    /// Gets a name of the route to the import contacts callback
    /// </summary>
    public static string ImportContactsRoute => "Plugin.Misc.Brevo.ImportContacts";

    /// <summary>
    /// Gets a name of the route to the unsubscribe contact callback
    /// </summary>
    public static string UnsubscribeContactRoute => "Plugin.Misc.Brevo.Unsubscribe";

    /// <summary>
    /// Gets a name of the synchronization schedule task
    /// </summary>
    public static string SynchronizationTaskName => "Synchronization (Brevo plugin)";

    /// <summary>
    /// Gets a type of the synchronization schedule task
    /// </summary>
    public static string SynchronizationTask => "Nop.Plugin.Misc.Brevo.Services.SynchronizationTask";

    /// <summary>
    /// Gets a default synchronization period in hours
    /// </summary>
    public static int DefaultSynchronizationPeriod => 12;

    /// <summary>
    /// Gets a default synchronization limit of Lists
    /// </summary>
    public static int DefaultSynchronizationListsLimit => 50;

    /// <summary>
    /// Gets a header of the API key authorization: api-key
    /// </summary>
    public static string ApiKeyHeader => "api-key";

    /// <summary>
    /// Gets a header of the API key authorization: partner-key
    /// </summary>
    public static string PartnerKeyHeader => "partner-key";

    /// <summary>
    /// Gets a name of attribute to store an email
    /// </summary>
    public static string EmailServiceAttribute => "EMAIL";

    /// <summary>
    /// Gets a name of attribute to store a first name
    /// </summary>
    public static string FirstNameServiceAttribute => "FIRSTNAME";

    /// <summary>
    /// Gets a name of attribute to store a first name for French
    /// </summary>
    public static string FirstNameFrenchServiceAttribute => "PRENOM";

    /// <summary>
    /// Gets a name of attribute to store a first name for Italian
    /// </summary>
    public static string FirstNameItalianServiceAttribute => "NOME";

    /// <summary>
    /// Gets a name of attribute to store a first name for Spanish
    /// </summary>
    public static string FirstNameSpanishServiceAttribute => "NOMBRE";

    /// <summary>
    /// Gets a name of attribute to store a first name for German
    /// </summary>
    public static string FirstNameGermanServiceAttribute => "VORNAME";

    /// <summary>
    /// Gets a name of attribute to store a first name for Portuguese
    /// </summary>
    public static string FirstNamePortugueseServiceAttribute => "NOME";

    /// <summary>
    /// Gets a name of attribute to store a last name
    /// </summary>
    public static string LastNameServiceAttribute => "LASTNAME";

    /// <summary>
    /// Gets a name of attribute to store a last name for French
    /// </summary>
    public static string LastNameFrenchServiceAttribute => "NOM";

    /// <summary>
    /// Gets a name of attribute to store a last name for Italian
    /// </summary>
    public static string LastNameItalianServiceAttribute => "COGNOME";

    /// <summary>
    /// Gets a name of attribute to store a last name for Spanish
    /// </summary>
    public static string LastNameSpanishServiceAttribute => "APELLIDO";

    /// <summary>
    /// Gets a name of attribute to store a last name for German
    /// </summary>
    public static string LastNameGermanServiceAttribute => "NAME";

    /// <summary>
    /// Gets a name of attribute to store a last name for Portuguese
    /// </summary>
    public static string LastNamePortugueseServiceAttribute => "SOBRENOME";

    /// <summary>
    /// Gets a name of attribute to store a username
    /// </summary>
    public static string UsernameServiceAttribute => "USERNAME";

    /// <summary>
    /// Gets a name of attribute to store a phone
    /// </summary>
    public static string PhoneServiceAttribute => "PHONE";

    /// <summary>
    /// Gets a name of attribute to store a SMS
    /// </summary>
    public static string SMSServiceAttribute => "SMS";

    /// <summary>
    /// Gets a name of attribute to store a country
    /// </summary>
    public static string CountryServiceAttribute => "COUNTRY";

    /// <summary>
    /// Gets a name of attribute to store a gender
    /// </summary>
    public static string GenderServiceAttribute => "GENDER";

    /// <summary>
    /// Gets a name of attribute to store a date of birth
    /// </summary>
    public static string DateOfBirthServiceAttribute => "DATE_OF_BIRTH";

    /// <summary>
    /// Gets a name of attribute to store a company
    /// </summary>
    public static string CompanyServiceAttribute => "COMPANY";

    /// <summary>
    /// Gets a name of attribute to store a address 1
    /// </summary>
    public static string Address1ServiceAttribute => "ADDRESS_1";

    /// <summary>
    /// Gets a name of attribute to store a address 2
    /// </summary>
    public static string Address2ServiceAttribute => "ADDRESS_2";

    /// <summary>
    /// Gets a name of attribute to store a zip code
    /// </summary>
    public static string ZipCodeServiceAttribute => "ZIP_CODE";

    /// <summary>
    /// Gets a name of attribute to store a city
    /// </summary>
    public static string CityServiceAttribute => "CITY";

    /// <summary>
    /// Gets a name of attribute to store a zip code
    /// </summary>
    public static string CountyServiceAttribute => "COUNTY";

    /// <summary>
    /// Gets a name of attribute to store a zip code
    /// </summary>
    public static string StateServiceAttribute => "STATE";

    /// <summary>
    /// Gets a name of attribute to store a zip code
    /// </summary>
    public static string FaxServiceAttribute => "FAX";

    /// <summary>
    /// Gets a name of attribute to store a language
    /// </summary>
    public static string LanguageAttribute => "LANGUAGE";

    /// <summary>
    /// Gets a name of attribute to store a store identifier
    /// </summary>
    public static string StoreIdServiceAttribute => "STORE_ID";

    /// <summary>
    /// Gets a name of attribute to store an identifier
    /// </summary>
    public static string IdServiceAttribute => "ID";

    /// <summary>
    /// Gets a name of attribute to store an order identifier
    /// </summary>
    public static string OrderIdServiceAttribute => "ORDER_ID";

    /// <summary>
    /// Gets a name of attribute to store an order date
    /// </summary>
    public static string OrderDateServiceAttribute => "ORDER_DATE";

    /// <summary>
    /// Gets a name of attribute to store an order total
    /// </summary>
    public static string OrderTotalServiceAttribute => "ORDER_PRICE";

    /// <summary>
    /// Gets a name of attribute to store an order total sum
    /// </summary>
    public static string OrderTotalSumServiceAttribute => "NOPCOMMERCE_CA_USER";

    /// <summary>
    /// Gets a name of attribute to store an order total sum of month
    /// </summary>
    public static string OrderTotalMonthSumServiceAttribute => "NOPCOMMERCE_LAST_30_DAYS_CA";

    /// <summary>
    /// Gets a name of attribute to store an order count
    /// </summary>
    public static string OrderCountServiceAttribute => "NOPCOMMERCE_ORDER_TOTAL";

    /// <summary>
    /// Gets a name of attribute to store all orders total sum
    /// </summary>
    public static string AllOrderTotalSumServiceAttribute => "NOPCOMMERCE_CA_TOTAL";

    /// <summary>
    /// Gets a name of attribute to store all orders total sum of month
    /// </summary>
    public static string AllOrderTotalMonthSumServiceAttribute => "NOPCOMMERCE_CA_LAST_30DAYS";

    /// <summary>
    /// Gets a name of attribute to store all orders count
    /// </summary>
    public static string AllOrderCountServiceAttribute => "NOPCOMMERCE_ORDERS_COUNT";

    /// <summary>
    /// Gets a key of the attribute to store shopping cart identifier
    /// </summary>
    public static string ShoppingCartGuidAttribute => "ShoppingCartGuid";

    /// <summary>
    /// Gets a header of the marketing automation authentication key
    /// </summary>
    public static string MarketingAutomationKeyHeader => "ma-key";

    /// <summary>
    /// Gets the marketing automation services URL
    /// </summary>
    public static string MarketingAutomationUrl => "https://in-automate.brevo.com/api/v2/";

    /// <summary>
    /// Gets a key of the attribute to store template identifier
    /// </summary>
    public static string TemplateIdAttribute => "TemplateId";

    /// <summary>
    /// Gets a key of the attribute to store a value indicating whether to use SMS notification
    /// </summary>
    public static string UseSmsAttribute => "UseSmsNotifications";

    /// <summary>
    /// Gets a key of the attribute to store SMS text
    /// </summary>
    public static string SmsTextAttribute => "SMSText";

    /// <summary>
    /// Gets a key of the attribute to store phone type
    /// </summary>
    public static string PhoneTypeAttribute => "PhoneTypeId";

    /// <summary>
    /// Gets a name of custom email header 
    /// </summary>
    public static string EmailCustomHeader => "X-Mailin-Tag";

    /// <summary>
    /// Gets a name of the cart updated event
    /// </summary>
    public static string CartUpdatedEventName => "cart_updated";

    /// <summary>
    /// Gets a name of the cart deleted event
    /// </summary>
    public static string CartDeletedEventName => "cart_deleted";

    /// <summary>
    /// Gets a name of the order completed event
    /// </summary>
    public static string OrderCompletedEventName => "order_completed";

    /// <summary>
    /// Gets a key of cache synchronization
    /// </summary>
    public static CacheKey SyncKeyCache => new("PLUGINS_MISC_BREVO_SYNCINFO");

    /// <summary>
    /// Gets a key of notification message
    /// </summary>
    public static string NotificationMessage => "Unfortunately, there’s been an error, feel free to reach our support team.";

    /// <summary>
    /// Gets a token to place tracking identifier in the tracking script
    /// </summary>
    public static string TrackingScriptId => "{TRACKING_ID}";

    /// <summary>
    /// Gets a token to place customer email in the tracking script
    /// </summary>
    public static string TrackingScriptCustomerEmail => "{CUSTOMER_EMAIL}";

    /// <summary>
    /// Generic attribute name to hide general settings block on the plugin configuration page
    /// </summary>
    public static string HideGeneralBlock => "BrevoPage.HideGeneralBlock";

    /// <summary>
    /// Generic attribute name to hide synchronization block on the plugin configuration page
    /// </summary>
    public static string HideSynchronizationBlock => "BrevoPage.HideSynchronizationBlock";

    /// <summary>
    /// Generic attribute name to hide transactional block on the plugin configuration page
    /// </summary>
    public static string HideTransactionalBlock => "BrevoPage.HideTransactionalBlock";

    /// <summary>
    /// Generic attribute name to hide SMS block on the plugin configuration page
    /// </summary>
    public static string HideSmsBlock => "BrevoPage.HideSmsBlock";

    /// <summary>
    /// Generic attribute name to hide marketing automation block on the plugin configuration page
    /// </summary>
    public static string HideMarketingAutomationBlock => "BrevoPage.HideMarketingAutomationBlock";
}