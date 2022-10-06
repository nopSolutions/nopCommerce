namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Order refunded event
    /// </summary>
    public partial class OrderRefundedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amount">Amount</param>
        public OrderRefundedEvent(Order order, decimal amount)
        {
            Order = order;
            Amount = amount;
        }

        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; }
    }
}