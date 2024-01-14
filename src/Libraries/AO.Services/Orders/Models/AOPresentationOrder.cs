using System.Collections.Generic;

namespace AO.Services.Orders.Models
{
    public class AOPresentationOrder
    {
        public int OrderId { get; set; }

        public string TotalOrderAmountStr { get; set; }

        public decimal TotalOrderAmount { get; set; }

        public string OrderDateTime { get; set; }

        public string FirstName { get; set; }

        public string CustomerInfo { get; set; }

        public string CustomerEmail { get; set; }

        public int ShipmentId { get; set; }

        public string ShippingInfo { get; set; }

        public string ShippingInfoClassName { get; set; }

        public string CustomerComment { get; set; }

        public string InternalOrderNotes { get; set; }

        public List<AOOrderItem> PresentationOrderItems { get; set; }

        public string FormattedPaymentStatus { get; set; }

        public virtual string PaymentMethodSystemName { get; set; }

        public string WarningMessage { get; set; }
    }
}
