namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Order authorized event
    /// </summary>
    public class OrderAuthorizedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        public OrderAuthorizedEvent(Order order)
        {
            Order = order;
        }

        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; }
    }
}
