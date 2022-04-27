using System;
using Nop.Core;

namespace Nop.Plugin.Shipping.EasyPost.Domain.Batch
{
    /// <summary>
    /// Represents a batch
    /// </summary>
    public class EasyPostBatch : BaseEntity
    {
        /// <summary>
        /// Gets or sets the batch identifier
        /// </summary>
        public string BatchId { get; set; }

        /// <summary>
        /// Gets or sets the batch GUID
        /// </summary>
        public Guid BatchGuid { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the batch label format
        /// </summary>
        public string LabelFormat { get; set; }

        /// <summary>
        /// Gets or sets a URL to download the manifest document
        /// </summary>
        public string ManifestUrl { get; set; }

        /// <summary>
        /// A comma-separated list of ids of associated shipments
        /// </summary>
        public string ShipmentIds { get; set; }

        /// <summary>
        /// Gets or sets the pickup identifier
        /// </summary>
        public string PickupId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was updated
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was created
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}