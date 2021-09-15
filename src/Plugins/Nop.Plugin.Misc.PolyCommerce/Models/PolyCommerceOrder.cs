using System.Collections.Generic;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class PolyCommerceOrder
    {
        public string Email { get; set; }
        public string OrderNumber { get; set; }
        public List<string> Notes { get; set; }
        public decimal OrderSubtotalInclTax { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }
        public decimal OrderTotal { get; set; }
        public string CurrencyCode { get; set; }
        public string PaymentMethodName { get; set; } // e.g. Payments.AfterPay
        public string ShippingMethod { get; set; }
        public int PaymentStatusId { get; set; }
        public int OrderStatusId { get; set; }
        public decimal OrderShippingTotalInclTax { get; set; }
        public decimal OrderShippingTotalExclTax { get; set; }
        public PolyCommerceAddress Address { get; set; }
        public List<PolyCommerceOrderItem> OrderItems { get; set; }
    }
}
