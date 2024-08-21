using System.ComponentModel;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the data source for the packaging slip
/// </summary>
public partial class ShipmentSource : DocumentSource
{
    #region Ctor

    public ShipmentSource()
    {
        Products = new();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the shipping address
    /// </summary>
    [DisplayName("Pdf.ShippingInformation")]
    public AddressItem Address { get; set; }

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    [DisplayName("Pdf.Order")]
    public string OrderNumberText { get; set; }

    /// <summary>
    /// Gets or sets a collection of shipping items
    /// </summary>
    public List<ProductItem> Products { get; set; }

    /// <summary>
    /// Gets or sets the shipment number
    /// </summary>
    [DisplayName("Pdf.Shipment")]
    public string ShipmentNumberText { get; set; }

    #endregion
}