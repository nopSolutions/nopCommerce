namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Order paid event
    /// </summary>
    public class OrderPaidEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        public OrderPaidEvent(Order order)
        {
            this.Order = order;
        }

        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; }
    }
}