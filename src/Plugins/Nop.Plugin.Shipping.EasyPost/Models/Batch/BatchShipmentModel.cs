using Nop.Web.Framework.Models;

namespace Nop.Plugin.Shipping.EasyPost.Models.Batch
{
    /// <summary>
    /// Represents a batch shipment model
    /// </summary>
    public record BatchShipmentModel : BaseNopEntityModel
    {
        #region Properties

        public int BatchId { get; set; }

        public string ShipmentId { get; set; }

        public string CustomOrderNumber { get; set; }

        public string TotalWeight { get; set; }

        #endregion
    }
}