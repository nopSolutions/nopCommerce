namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Order placed event
    /// </summary>
    public class OrderPlacedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        public OrderPlacedEvent(Order order)
        {
            this.Order = order;
        }

        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; }
    }
}