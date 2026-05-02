using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the payment token list model
/// </summary>
public record PaymentTokenListModel : BaseNopModel
{
    #region Properties

    public bool VaultIsEnabled { get; set; }

    public string Error { get; set; }

    public List<PaymentTokenModel> PaymentTokens { get; set; } = new();

    #endregion
}