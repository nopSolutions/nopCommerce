using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the payment info model
/// </summary>
public record PaymentInfoModel : BaseNopModel
{
    #region Properties

    public ButtonPlacement Placement { get; set; }

    public int? ProductId { get; set; }

    public (string Url, string ClientToken, string UserToken) Script { get; set; } = new();

    public (string Email, string FullName) Customer { get; set; } = new();

    public MessagesModel MessagesModel { get; set; } = new();

    public (bool? IsRecurring, bool IsShippable) Cart { get; set; } = new();

    #endregion
}