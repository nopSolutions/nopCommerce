namespace Nop.Plugin.Payments.PayPalCommerce.Models.Public;

/// <summary>
/// Represents the order approved model
/// </summary>
public record OrderApprovedModel : OrderModel
{
    #region Properties

    public bool PayNow { get; set; }

    #endregion
}