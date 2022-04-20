namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Order status changed event
    /// </summary>
    public class OrderStatusChangedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        public OrderStatusChangedEvent(Order order)
        {
            Order = order;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="prevOrderStatus">Previous order status</param>
        public OrderStatusChangedEvent(Order order, OrderStatus prevOrderStatus)
        {
            Order = order;
            PrevOrderStatus = prevOrderStatus;
        }

        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Previous order status
        /// </summary>
        public OrderStatus PrevOrderStatus { get; set; }
    }
}