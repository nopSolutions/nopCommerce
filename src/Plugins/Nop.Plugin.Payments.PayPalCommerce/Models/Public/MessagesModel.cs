using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the Pay Later messages model
/// </summary>
public record MessagesModel : BaseNopModel
{
    #region Properties

    public ButtonPlacement Placement { get; set; }

    public string CurrencyCode { get; set; }

    public string Amount { get; set; }

    public string Country { get; set; }

    public string Config { get; set; }

    public bool LoadScript { get; set; }

    #endregion
}