using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.Avalara.Models.Customer;

/// <summary>
/// Represents a tax exemption certificate model
/// </summary>
public record ExemptionCertificateModel : BaseNopEntityModel
{
    #region Properties

    public string Status { get; set; }

    public string SignedDate { get; set; }

    public string ExpirationDate { get; set; }

    public string ExposureZone { get; set; }

    #endregion
}