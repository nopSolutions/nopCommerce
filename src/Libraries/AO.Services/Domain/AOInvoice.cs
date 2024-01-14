using Nop.Core;
using System;

namespace AO.Services.Domain
{
    public class AOInvoice : BaseEntity
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime PaymentDate { get; set; }
                
        public decimal InvoiceTax { get; set; }

        public decimal InvoiceDiscount { get; set; }

        public decimal InvoiceTotal { get; set; }

        public decimal InvoiceRefundedAmount { get; set; }

        public decimal InvoiceShipping { get; set; }

        public decimal CurrencyRate { get; set; }

        public string TrackingNumber { get; set; }

        public string CustomerCurrencyCode { get; set; }

        public bool InvoiceIsPaid { get; set; }

        public bool InvoiceIsManual { get; set; }

        public DateTime? BookedDate { get; set; }

        public int? EconomicInvoiceNumber { get; set; }
    }
}