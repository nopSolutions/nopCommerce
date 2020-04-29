using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class PolyCommerceOrder
    {
        public string Email { get; set; }
        public string OrderNumber { get; set; }
        public string Notes { get; set; }
        public decimal OrderSubtotalInclTax { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }
        public decimal OrderTotal { get; set; }
        public string CurrencyCode { get; set; }
        public string PaymentMethodName { get; set; } // e.g. Payments.AfterPay
        public string ShippingMethod { get; set; }
        public int PaymentStatusId { get; set; }
        public PolyCommerceAddress Address { get; set; }
        public List<PolyCommerceOrderItem> OrderItems { get; set; } = new List<PolyCommerceOrderItem>();
    }

    public class PolyCommerceOrderItem
    {
        public int ExternalProductId { get; set; }
        public decimal UnitPriceInclTax { get; set; }
        public decimal UnitPriceExclTax { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public int Quantity { get; set; }
        public decimal? ItemWeight { get; set;  }
    }

    public class PolyCommerceAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string TwoLetterCountryCode { get; set; }
        public string ZipPostalCode { get; set; }
    }
}
