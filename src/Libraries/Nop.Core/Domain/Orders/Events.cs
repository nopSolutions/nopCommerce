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

    /// <summary>
    /// Order refunded event
    /// </summary>
    public class OrderRefundedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amount">Amount</param>
        public OrderRefundedEvent(Order order, decimal amount)
        {
            this.Order = order;
            this.Amount = amount;
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