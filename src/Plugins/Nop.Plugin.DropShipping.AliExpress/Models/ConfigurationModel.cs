using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DropShipping.AliExpress.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.AppKey")]
    public string? AppKey { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.AppSecret")]
    public string? AppSecret { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.AccessToken")]
    public string? AccessToken { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.RefreshToken")]
    public string? RefreshToken { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.AccessTokenExpiresOn")]
    public DateTime? AccessTokenExpiresOnUtc { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.RefreshTokenExpiresOn")]
    public DateTime? RefreshTokenExpiresOnUtc { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.DefaultMarginPercentage")]
    public decimal DefaultMarginPercentage { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.VatPercentage")]
    public decimal VatPercentage { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.CustomsDutyPercentage")]
    public decimal CustomsDutyPercentage { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.DefaultShippingCountry")]
    public string? DefaultShippingCountry { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.DefaultCurrency")]
    public string? DefaultCurrency { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.EnableDailySync")]
    public bool EnableDailySync { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.DailySyncHour")]
    public int DailySyncHour { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.AutoCreateOrders")]
    public bool AutoCreateOrders { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.AutoCreateLocalShipments")]
    public bool AutoCreateLocalShipments { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.TokenRefreshDaysBeforeExpiry")]
    public int TokenRefreshDaysBeforeExpiry { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.UseSandbox")]
    public bool UseSandbox { get; set; }

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.AuthorizationUrl")]
    public string AuthorizationUrl { get; set; } = "https://api-sg.aliexpress.com/oauth/authorize";

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.RedirectUri")]
    public string RedirectUri { get; set; } = "ali-express/callback";

    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.AuthorizationLaunchUrl")]
    public string? AuthorizationLaunchUrl { get; set; }

    public bool IsTokenValid { get; set; }
    
    [NopResourceDisplayName("Plugins.DropShipping.AliExpress.Fields.RedirectUriHost")]
    public string RedirectUriHost { get; set; }
}
