using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Admin;

/// <summary>
/// Represents the authentication model
/// </summary>
public record AuthenticationModel : BaseNopModel
{
    #region Properties

    public int StoreId { get; set; }

    public string SharedId { get; set; }

    public string AuthCode { get; set; }

    #endregion
}