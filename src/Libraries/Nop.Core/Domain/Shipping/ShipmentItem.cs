<<<<<<< HEAD
﻿namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipment item
    /// </summary>
    public partial class ShipmentItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the shipment identifier
        /// </summary>
        public int ShipmentId { get; set; }

        /// <summary>
        /// Gets or sets the order item identifier
        /// </summary>
        public int OrderItemId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the warehouse identifier
        /// </summary>
        public int WarehouseId { get; set; }
    }
=======
﻿namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipment item
    /// </summary>
    public partial class ShipmentItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the shipment identifier
        /// </summary>
        public int ShipmentId { get; set; }

        /// <summary>
        /// Gets or sets the order item identifier
        /// </summary>
        public int OrderItemId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the warehouse identifier
        /// </summary>
        public int WarehouseId { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}