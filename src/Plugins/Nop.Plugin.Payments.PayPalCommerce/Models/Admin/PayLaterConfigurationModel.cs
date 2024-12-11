using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Admin;

/// <summary>
/// Represents the Pay Later configuration model
/// </summary>
public record PayLaterConfigurationModel : BaseNopModel
{
    public string ClientId { get; set; }

    public bool UseSandbox { get; set; }

    public string Locale { get; set; }

    public string Config { get; set; }
}