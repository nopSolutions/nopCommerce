namespace Nop.Core.Domain.Orders
{
    public class OrderPaidEvent
    {
        private readonly Order _order;

        public OrderPaidEvent(Order order)
        {
            this._order = order;
        }

        public Order Order
        {
            get { return _order; }
        }
    }

    public class OrderPlacedEvent
    {
        private readonly Order _order;

        public OrderPlacedEvent(Order order)
        {
            this._order = order;
        }

        public Order Order
        {
            get { return _order; }
        }
    }

    public class OrderCancelledEvent
    {
        private readonly Order _order;

        public OrderCancelledEvent(Order order)
        {
            this._order = order;
        }

        public Order Order
        {
            get { return _order; }
        }
    }
}