using Nop.Web.Framework.Models;

namespace Nop.Plugin.Shipping.EasyPost.Models.Batch
{
    /// <summary>
    /// Represents a batch shipment list model
    /// </summary>
    public record BatchShipmentListModel : BasePagedListModel<BatchShipmentModel>
    {
    }
}