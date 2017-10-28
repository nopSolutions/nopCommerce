namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Order paid event
    /// </summary>
    public class OrderPaidEvent
    {
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