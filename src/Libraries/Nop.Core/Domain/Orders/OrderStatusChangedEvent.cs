namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Order status changed event
    /// </summary>
    public partial class OrderStatusChangedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="previousOrderStatus">Previous order status</param>
        public OrderStatusChangedEvent(Order order, OrderStatus previousOrderStatus)
        {
            Order = order;
            PreviousOrderStatus = previousOrderStatus;
        }

        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Previous order status
        /// </summary>
        public OrderStatus PreviousOrderStatus { get; set; }
    }
}