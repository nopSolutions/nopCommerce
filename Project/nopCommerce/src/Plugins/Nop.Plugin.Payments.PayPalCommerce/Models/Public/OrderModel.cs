using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the order model
/// </summary>
public record OrderModel : BaseNopModel
{
    #region Properties

    public bool CheckoutIsEnabled { get; set; }

    public bool LoginIsRequired { get; set; }

    public string OrderId { get; set; }

    public string Status { get; set; }

    public string PayerActionUrl { get; set; }

    public string Error { get; set; }

    #endregion
}