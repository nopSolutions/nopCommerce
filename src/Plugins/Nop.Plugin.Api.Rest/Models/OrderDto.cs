using System;

namespace Nop.Plugin.Api.Rest.Models
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? ShippingStatus { get; set; }
        public int CustomerId { get; set; }
    }
}
