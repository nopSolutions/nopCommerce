namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Order voided event
    /// </summary>
    public class OrderVoidedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="order">Order</param>
        public OrderVoidedEvent(Order order)
        {
            this.Order = order;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Voided order
        /// </summary>
        public Order Order { get; }

        #endregion
    }
}