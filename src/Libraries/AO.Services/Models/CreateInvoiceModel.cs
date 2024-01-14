using System;

namespace AO.Services.Models
{
    public class CreateInvoiceModel
    {
        public string OrderItemText { get; set; }

        public decimal InvoiceTotal { get; set; }

        public string InvoiceCurrencyCode { get; set; }

        public decimal InvoiceTaxRate { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime PaymentDate { get; set; }

        public int CustomerId { get; set; }

        public bool InvoiceIsPaid { get; set; }

        public bool InvoiceIsManual { get; set; }
    }
}
