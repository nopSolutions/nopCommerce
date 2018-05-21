namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Order cancelled event
    /// </summary>
    public class OrderCancelledEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        public OrderCancelledEvent(Order order)
        {
            this.Order = order;
        }

        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; }
    }
}