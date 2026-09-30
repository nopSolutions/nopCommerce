using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Rest.Models;

namespace Nop.Plugin.Api.Rest.Mappings
{
    public static class DtoMappings
    {
        public static OrderDto ToDto(this Order order)
        {
            if (order == null) return null!;
            return new OrderDto
            {
                Id = order.Id,
                OrderTotal = order.OrderTotal,
                CreatedOnUtc = order.CreatedOnUtc,
                OrderStatus = order.OrderStatus.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                ShippingStatus = order.ShippingStatus.ToString(),
                CustomerId = order.CustomerId
            };
        }

        public static CustomerDto ToDto(this Customer customer)
        {
            if (customer == null) return null!;
            return new CustomerDto
            {
                Id = customer.Id,
                Email = customer.Email,
                Username = customer.Username,
                CreatedOnUtc = customer.CreatedOnUtc
            };
        }
    }
}
