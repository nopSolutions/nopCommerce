using System;
using Nop.Plugin.Shipping.EasyPost.Domain.Batch;
using Nop.Plugin.Shipping.EasyPost.Models.Pickup;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.EasyPost.Models.Batch
{
    /// <summary>
    /// Represents a batch model
    /// </summary>
    public record BatchModel : BaseNopEntityModel
    {
        #region Ctor

        public BatchModel()
        {
            PickupModel = new PickupModel();
            BatchShipmentSearchModel = new BatchShipmentSearchModel();
        }

        #endregion

        #region Properties

        #region General

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Batch.Fields.Id")]
        public string BatchId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Batch.Fields.Status")]
        public string Status { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Batch.Fields.PickupStatus")]
        public string PickupStatus { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Batch.Fields.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Batch.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Common

        public BatchStatus BatchStatus { get; set; }

        public PickupModel PickupModel { get; set; }

        public BatchShipmentSearchModel BatchShipmentSearchModel { get; set; }

        public string LabelFormat { get; set; }

        public bool Purchased { get; set; }

        public bool ManifestGenerated { get; set; }

        #endregion

        #endregion
    }
}