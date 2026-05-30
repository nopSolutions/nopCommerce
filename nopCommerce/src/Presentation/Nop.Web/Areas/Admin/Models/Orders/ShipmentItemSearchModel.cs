using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a shipment item search model
/// </summary>
public partial record ShipmentItemSearchModel : BaseSearchModel
{
    #region Properties

    public int ShipmentId { get; set; }

    #endregion
}