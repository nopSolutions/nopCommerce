namespace Nop.Plugin.Shipping.EasyPost.Domain.Batch
{
    /// <summary>
    /// Represents a batch shipment
    /// </summary>
    public class BatchShipment
    {
        /// <summary>
        /// Gets or sets the entry id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the batch identifier
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// Gets or sets the shipment identifier
        /// </summary>
        public string ShipmentId { get; set; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        public string CustomOrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the shipment total weight
        /// </summary>
        public string TotalWeight { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public BatchShipmentStatus Status { get; set; }
    }
}