using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the payment token model
/// </summary>
public record PaymentTokenModel : BaseNopEntityModel
{
    #region Properties

    public string Type { get; set; }

    public string Title { get; set; }

    public string Expiration { get; set; }

    public bool IsPrimaryMethod { get; set; }

    #endregion
}