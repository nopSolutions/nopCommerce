using Nop.Core.Configuration;

namespace Nop.Plugin.DropShipping.AliExpress;

/// <summary>
/// Represents settings for AliExpress drop shipping plugin
/// </summary>
public class AliExpressSettings : ISettings
{
    /// <summary>
    /// Gets or sets the AliExpress App Key
    /// </summary>
    public string? AppKey { get; set; }

    /// <summary>
    /// Gets or sets the AliExpress App Secret
    /// </summary>
    public string? AppSecret { get; set; }

    /// <summary>
    /// Gets or sets the current access token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets when the access token expires (UTC)
    /// </summary>
    public DateTime? AccessTokenExpiresOnUtc { get; set; }

    /// <summary>
    /// Gets or sets when the refresh token expires (UTC)
    /// </summary>
    public DateTime? RefreshTokenExpiresOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the default margin percentage to apply to products
    /// </summary>
    public decimal DefaultMarginPercentage { get; set; } = 25m;

    /// <summary>
    /// Gets or sets the VAT percentage
    /// </summary>
    public decimal VatPercentage { get; set; } = 15m;

    /// <summary>
    /// Gets or sets the customs duty percentage
    /// </summary>
    public decimal CustomsDutyPercentage { get; set; } = 10m;

    /// <summary>
    /// Gets or sets the default shipping country code
    /// </summary>
    public string? DefaultShippingCountry { get; set; } = "ZA";

    /// <summary>
    /// Gets or sets the default currency code
    /// </summary>
    public string? DefaultCurrency { get; set; } = "ZAR";

    /// <summary>
    /// Gets or sets whether to automatically sync prices daily
    /// </summary>
    public bool EnableDailySync { get; set; } = true;

    /// <summary>
    /// Gets or sets the hour (0-23) when daily sync should run
    /// </summary>
    public int DailySyncHour { get; set; } = 2;

    /// <summary>
    /// Gets or sets whether to automatically create orders on AliExpress
    /// </summary>
    public bool AutoCreateOrders { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to automatically create local shipments with Courier Guy
    /// </summary>
    public bool AutoCreateLocalShipments { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of days before token expiry to refresh
    /// </summary>
    public int TokenRefreshDaysBeforeExpiry { get; set; } = 7;

    /// <summary>
    /// Gets or sets whether the plugin is in sandbox/test mode
    /// </summary>
    public bool UseSandbox { get; set; } = false;

    /// <summary>
    /// Gets or sets the default language for product descriptions
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";

    public int DefaultPageSize { get; set; } = 10;
    public int DefaultPageIndex { get; set; } = 1;
    /*
     *     "RedirectUri": "https://shop.kung-fu.co.za/ali-express/callback",
       "AuthorizationUrl": "https://api-sg.aliexpress.com/oauth/authorize",

     */
    public string AuthorizationUrl { get; set; } = "https://api-sg.aliexpress.com/oauth/authorize";
    public string RedirectUri { get; set; } = "ali-express/callback";
    
    public string RedirectUriHost { get; set; } = "https://shop.kung-fu.co.za";
    public string AuthorizationLaunchUrl { get; set; }
}
