using System;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Api.Factories
{
    public class OrderFactory : IFactory<Order>
    {
        public Task<Order> InitializeAsync()
        {
            var order = new Order();

            order.CreatedOnUtc = DateTime.UtcNow;
            order.OrderGuid = new Guid();
            order.PaymentStatus = PaymentStatus.Pending;
            order.ShippingStatus = ShippingStatus.NotYetShipped;
            order.OrderStatus = OrderStatus.Pending;

            return Task.FromResult(order);
        }
    }
}
