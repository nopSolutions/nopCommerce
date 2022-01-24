using Nop.Web.Framework.Models;

namespace Nop.Plugin.Shipping.EasyPost.Models.Batch
{
    /// <summary>
    /// Represents a batch shipment search model
    /// </summary>
    public record BatchShipmentSearchModel : BaseSearchModel
    {
        #region Properties

        public int BatchId { get; set; }

        #endregion
    }
}